# VerificationCode Cleanup Service

## 📋 Tổng quan

Background service tự động dọn dẹp các mã OTP đã hết hạn trong bảng `VerificationCode`.

## ⚙️ Cấu hình

### Quy tắc xóa (mặc định):

1. **OTP đã sử dụng**: Xóa sau 1 giờ
2. **OTP chưa dùng**: Xóa sau 24 giờ
3. **OTP hết hạn**: Xóa sau 24 giờ

### Thời gian chạy:
- **Interval**: Mỗi 1 giờ
- **First run**: 10 giây sau khi app khởi động
- **Retry on error**: 5 phút

## 🚀 Đã được cài đặt

Service đã được đăng ký trong `Program.cs`:

```csharp
// Background Service - OTP Cleanup
services.AddHostedService<VerificationCodeCleanupService>();
```

✅ **Tự động chạy khi app start**  
✅ **Không cần config thêm**

## 📊 Logs

Service sẽ ghi log các hoạt động:

### Khởi động:
```
VerificationCodeCleanupService is starting.
```

### Cleanup thành công:
```
Found 15 expired verification codes to clean up
Successfully cleaned up 15 verification codes at 2024-01-01 10:00:00
  - Register: 5 codes
  - ForgotPassword: 8 codes
  - ChangePassword: 2 codes
Next cleanup scheduled at 2024-01-01 11:00:00
```

### Không có gì để xóa:
```
No expired verification codes to clean up.
Next cleanup scheduled at 2024-01-01 11:00:00
```

### Lỗi:
```
Error occurred while cleaning up verification codes
Failed to cleanup verification codes: [chi tiết lỗi]
```

## 🔧 Tùy chỉnh

Nếu muốn thay đổi thời gian, sửa trong `VerificationCodeCleanupService.cs`:

```csharp
// Chạy mỗi 1 giờ (mặc định)
private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);

// Giữ lại 24 giờ (mặc định)
private readonly TimeSpan _retentionPeriod = TimeSpan.FromHours(24);
```

### Ví dụ thay đổi:

**Chạy mỗi 30 phút:**
```csharp
private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(30);
```

**Giữ lại 7 ngày:**
```csharp
private readonly TimeSpan _retentionPeriod = TimeSpan.FromDays(7);
```

**Chạy mỗi ngày 1 lần:**
```csharp
private readonly TimeSpan _cleanupInterval = TimeSpan.FromDays(1);
```

## 🧪 Test thủ công

Nếu muốn test ngay lập tức, có thể thêm endpoint tạm:

```csharp
// VerificationController.cs
[HttpPost("ForceCleanup")]
[Roles(RoleConstant.Admin)]
public async Task<IActionResult> ForceCleanup()
{
    var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
    var repository = unitOfWork.GetRepository<VerificationCode>();
    
    var cutoffDate = DateTime.UtcNow.AddHours(-24);
    var expiredCodes = await repository.GetByCondition(v =>
        (v.IsUsed && v.UsedDate < DateTime.UtcNow.AddHours(-1)) ||
        (v.CreatedDate < cutoffDate));
    
    foreach (var code in expiredCodes)
    {
        await repository.Delete(code);
    }
    
    await unitOfWork.SaveAsync();
    
    return Ok($"Cleaned up {expiredCodes.Count()} codes");
}
```

## 📈 Monitoring

### Kiểm tra service đang chạy:

**Check logs khi app start:**
```
VerificationCodeCleanupService is starting.
```

### Kiểm tra số lượng OTP trong DB:

```sql
-- Tổng số OTP
SELECT COUNT(*) FROM VerificationCodes;

-- OTP theo trạng thái
SELECT 
    Type,
    IsUsed,
    COUNT(*) as Count,
    MIN(CreatedDate) as Oldest,
    MAX(CreatedDate) as Newest
FROM VerificationCodes
GROUP BY Type, IsUsed;

-- OTP cần xóa
SELECT COUNT(*) 
FROM VerificationCodes 
WHERE (IsUsed = 1 AND UsedDate < DATEADD(HOUR, -1, GETUTCDATE()))
   OR CreatedDate < DATEADD(HOUR, -24, GETUTCDATE())
   OR ExpiryDate < DATEADD(HOUR, -24, GETUTCDATE());
```

## ⚠️ Lưu ý

### Performance:
- Service chạy trong background, không ảnh hưởng API requests
- Sử dụng scoped service để tránh memory leak
- Batch delete, không xóa từng record một

### Bảo mật:
- Chỉ xóa OTP cũ, không ảnh hưởng OTP đang hoạt động
- Giữ lại log 24 giờ để audit
- Tự động retry khi lỗi

### Production:
- ✅ Nên giữ retention period = 24-48 giờ
- ✅ Cleanup interval = 1-6 giờ là hợp lý
- ⚠️ Không nên cleanup quá thường xuyên (< 30 phút)
- ⚠️ Không nên giữ quá lâu (> 7 ngày)

## 🔍 Troubleshooting

### Service không chạy?
1. Check logs khi app start
2. Verify `AddHostedService` đã được đăng ký
3. Check database connection

### Cleanup không xóa gì?
1. Check có OTP cũ trong DB không (SQL query trên)
2. Verify thời gian hệ thống (UTC)
3. Check logs xem có lỗi không

### Lỗi database?
1. Service sẽ tự retry sau 5 phút
2. Check connection string
3. Verify permissions của DB user

## 📝 Best Practices

1. **Monitor logs** - Setup log aggregation (Seq, ELK, etc)
2. **Alert on errors** - Setup alerts nếu cleanup liên tục fail
3. **Database indexes** - Thêm index cho `CreatedDate`, `UsedDate`, `IsUsed`
4. **Testing** - Test trong staging trước khi deploy production

### Recommended indexes:

```sql
CREATE NONCLUSTERED INDEX IX_VerificationCodes_Cleanup 
ON VerificationCodes(IsUsed, CreatedDate, UsedDate, ExpiryDate)
INCLUDE (ID);
```

---

**Status:** ✅ Active  
**Last Updated:** 2024  
**Owner:** AutoApp Management Team
