# Giải thích về Refresh Token Strategy

## Vấn đề hiện tại:
RefreshTokenAsync tạo token mới mỗi lần refresh → 1 device có thể có nhiều refresh token

## 2 Giải pháp:

### Cách 1: Update token cũ (ĐÃ IMPLEMENT)
```csharp
// Thay vì tạo token mới, update token cũ
refreshToken.Token = newTokens.RefreshToken!;
refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
refreshToken.IsUsed = false; // Reset
```

**Ưu điểm:**
- 1 device = 1 refresh token record
- Không tạo nhiều record trong DB
- Dễ quản lý

**Nhược điểm:**
- Kém bảo mật hơn (không có rotation)
- Khó track history

### Cách 2: Xóa token cũ trước khi tạo mới
```csharp
// Trước khi tạo token mới, xóa tất cả token cũ của device này
await RefreshTokenRepository.RevokeTokensByAccountAndDeviceAsync(
    account.ID, 
    refreshToken.DeviceInfo, 
    ipAddress);

// Sau đó tạo token mới
var newRefreshToken = await CreateRefreshTokenAsync(...);
```

**Ưu điểm:**
- Vẫn có rotation (bảo mật tốt)
- 1 device = 1 active token
- Có lịch sử (token cũ bị revoke)

**Nhược điểm:**
- Phức tạp hơn
- Nhiều record trong DB

## Khuyến nghị:
- **Production**: Dùng Cách 1 (đã implement)
- **High Security**: Dùng Cách 2

## Test:
```bash
# Login
curl -X POST "/api/Account/Login" -d '{"emailOrPhone":"test","password":"123","deviceId":"iPhone123"}'

# Refresh lần 1
curl -X POST "/api/Account/RefreshToken" -d '{"refreshToken":"token_here"}'

# Refresh lần 2 
curl -X POST "/api/Account/RefreshToken" -d '{"refreshToken":"new_token_here"}'

# Kiểm tra DB - chỉ có 1 record active cho device "iPhone123"
SELECT COUNT(*) FROM RefreshToken WHERE DeviceInfo = 'iPhone123' AND IsRevoked = 0 AND IsUsed = 0;
```