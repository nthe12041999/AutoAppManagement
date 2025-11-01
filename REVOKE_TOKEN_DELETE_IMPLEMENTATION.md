# RevokeToken - Xóa Token Implementation

## Tóm tắt thay đổi

Thay vì chỉ đánh dấu `IsRevoked = true`, bây giờ RevokeToken API sẽ **XÓA LUÔN** token khỏi database.

## Thay đổi trong Repository

### RefreshTokenRepository.cs

**Thêm interface method:**
```csharp
Task<bool> DeleteTokensByAccountAndDeviceAsync(long accountId, string? ipAddress = null, string? userAgent = null);
```

**Cập nhật RevokeTokensByAccountAndDeviceAsync:**
- Thay đổi từ `Update` token (đánh dấu IsRevoked) 
- Thành `Remove` token (xóa khỏi database)

**Thêm DeleteTokensByAccountAndDeviceAsync:**
- Tìm token dựa trên accountId, IP và UserAgent
- Xóa token bằng `_dbset.Remove(token)`

## Thay đổi trong Service

### RefreshTokenService.cs

**RevokeTokenAsync method:**
```csharp
// Cũ: Đánh dấu IsRevoked = true
token.IsRevoked = true;
token.RevokedDate = DateTime.UtcNow;
_repository.Update(token);

// Mới: Xóa luôn token
var refreshToken = await RefreshTokenRepository.GetByTokenAsync(request.Token);
RefreshTokenRepository.Delete(refreshToken);
```

### AccountService.cs

**RevokeCurrentDeviceToken method:**
- Thay đổi từ tìm token theo deviceId
- Thành tìm token theo IP + UserAgent từ HttpContext
- Sử dụng `DeleteTokensByAccountAndDeviceAsync` để xóa token

## Ưu điểm của việc xóa token

1. **Bảo mật cao hơn:** Token bị xóa hoàn toàn, không thể khôi phục
2. **Tiết kiệm dung lượng database:** Không lưu trữ token đã thu hồi
3. **Đơn giản hóa logic:** Không cần kiểm tra trạng thái IsRevoked
4. **Performance tốt hơn:** Ít record trong database

## API Usage

**Endpoint:** `POST /api/Account/RevokeToken`

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json
```

**Body:** Không cần body, API sẽ tự động lấy thông tin từ JWT token

**Response thành công:**
```json
{
    "success": true,
    "message": "Đã xóa token của thiết bị hiện tại",
    "data": true
}
```

**Response lỗi:**
```json
{
    "success": false,
    "message": "Không tìm thấy token nào để xóa",
    "data": null
}
```

## Test với Postman

1. **Login** để lấy access token
2. **Copy access token** vào Authorization header
3. **Call RevokeToken API** - token sẽ bị xóa khỏi database
4. **Thử gọi API khác** - sẽ bị lỗi Unauthorized vì token đã không tồn tại

## Lưu ý quan trọng

- Token bị XÓA hoàn toàn, không thể khôi phục
- Người dùng sẽ phải đăng nhập lại để có token mới
- Việc xóa token áp dụng cho thiết bị hiện tại (dựa trên IP + UserAgent)
- Có thể mở rộng để xóa tất cả token của user bằng `RevokeAllUserTokensAsync`

## Database Impact

Trước đây:
```sql
UPDATE RefreshTokens SET IsRevoked = 1, RevokedDate = GETDATE() WHERE Token = @token
```

Bây giờ:
```sql
DELETE FROM RefreshTokens WHERE Token = @token
```