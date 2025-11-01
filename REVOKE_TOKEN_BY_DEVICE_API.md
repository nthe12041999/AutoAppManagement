# Revoke Token By Device API Documentation

## Tổng quan

API này cho phép thu hồi refresh token theo device ID cụ thể. Token authentication hiện tại sẽ được sử dụng để xác định account, sau đó tất cả token của account đó trên device được chỉ định sẽ bị thu hồi.

## Endpoint

```
POST /api/Account/RevokeTokensByDevice
```

## Authentication

Yêu cầu token JWT hợp lệ trong header Authorization.

```
Authorization: Bearer <ACCESS_TOKEN>
```

## Permissions

- **Customer**: Có thể thu hồi token của device của chính mình
- **Admin**: Có thể thu hồi token của device của bất kỳ account nào

## Request Body

```json
{
  "deviceId": "device_identifier_string"
}
```

### Parameters

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| deviceId | string | Yes | ID của device cần thu hồi token. Có thể là device ID, IMEI, MAC address, hoặc bất kỳ identifier nào |

## Response

### Success Response

```json
{
  "isSuccess": true,
  "message": "Đã thu hồi 2 token(s) của device DEVICE123",
  "data": true
}
```

### Error Response

```json
{
  "isSuccess": false,
  "message": "Không tìm thấy thông tin tài khoản từ token",
  "data": null
}
```

## Cách hoạt động

1. **Xác thực token**: Kiểm tra token JWT trong header Authorization
2. **Lấy accountId**: Trích xuất accountId từ token JWT
3. **Tìm tokens**: Tìm tất cả refresh token của account trên device được chỉ định
4. **Thu hồi tokens**: Đánh dấu các token đó là đã bị thu hồi (IsRevoked = true)
5. **Lưu changes**: Lưu thay đổi vào database

## Logic tìm kiếm token

Hệ thống sẽ tìm các refresh token thỏa mãn:
- `AccountId` = accountId từ token hiện tại
- `IsRevoked` = false
- `IsUsed` = false  
- `ExpiryDate` > DateTime.UtcNow
- `Status` = Active
- `DeviceInfo` chứa deviceId HOẶC `UserAgent` chứa deviceId

## Use Cases

### 1. Đăng xuất khỏi device cụ thể
```bash
curl -X POST "https://localhost:7001/api/Account/RevokeTokensByDevice" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "iPhone-12-Pro-ABCD1234"
  }'
```

### 2. Thu hồi token khi mất thiết bị
```bash
curl -X POST "https://localhost:7001/api/Account/RevokeTokensByDevice" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "IMEI:123456789012345"
  }'
```

### 3. Admin thu hồi token device của user khác
```bash
curl -X POST "https://localhost:7001/api/Account/RevokeTokensByDevice" \
  -H "Authorization: Bearer admin_token_here..." \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "user_suspicious_device_id"
  }'
```

## Error Codes

| HTTP Status | Error Message | Description |
|-------------|---------------|-------------|
| 400 | "Dữ liệu không hợp lệ" | Request body không hợp lệ hoặc thiếu deviceId |
| 401 | "Unauthorized" | Token không hợp lệ hoặc đã hết hạn |
| 403 | "Forbidden" | Không có quyền thực hiện action này |
| 500 | "Lỗi khi thu hồi token theo device: {error}" | Lỗi server khi xử lý |

## Security Considerations

1. **Token Validation**: Luôn validate token JWT trước khi xử lý
2. **Account Isolation**: Chỉ thu hồi token của chính account hiện tại (trừ admin)
3. **Device Matching**: Sử dụng pattern matching để tìm device trong DeviceInfo/UserAgent
4. **Audit Trail**: Ghi log IP address khi thu hồi token
5. **Rate Limiting**: Áp dụng rate limiting để tránh abuse

## Testing

### Test Case 1: Thu hồi token device thành công
```javascript
// Đăng nhập và lấy token
const loginResponse = await fetch('/api/Account/Login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'testuser',
    password: 'password123',
    deviceId: 'TestDevice123'
  })
});

const { token } = await loginResponse.json();

// Thu hồi token của device
const revokeResponse = await fetch('/api/Account/RevokeTokensByDevice', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    deviceId: 'TestDevice123'
  })
});

const result = await revokeResponse.json();
console.log(result); // { isSuccess: true, message: "Đã thu hồi 1 token(s) của device TestDevice123" }
```

### Test Case 2: Thử sử dụng token đã bị thu hồi
```javascript
// Thử refresh token đã bị thu hồi
const refreshResponse = await fetch('/api/Account/RefreshToken', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    refreshToken: 'revoked_token_here'
  })
});

const result = await refreshResponse.json();
console.log(result); // { isSuccess: false, message: "Refresh token đã hết hạn hoặc bị thu hồi" }
```

## Related APIs

- `POST /api/Account/Login` - Đăng nhập và tạo token
- `POST /api/Account/RefreshToken` - Refresh access token
- `POST /api/Account/RevokeAllRefreshTokens` - Thu hồi tất cả token của account
- `GET /api/Account/GetAccountDevicesByAccountId` - Lấy danh sách device của account

## Database Impact

### RefreshToken Table
```sql
UPDATE RefreshToken 
SET IsRevoked = 1, 
    RevokedDate = GETUTCDATE(), 
    RevokedByIp = @IpAddress,
    UpdatedBy = @AccountId,
    UpdatedDate = GETUTCDATE()
WHERE AccountId = @AccountId 
  AND IsRevoked = 0 
  AND IsUsed = 0 
  AND ExpiryDate > GETUTCDATE()
  AND Status = 1
  AND (DeviceInfo LIKE '%' + @DeviceId + '%' OR UserAgent LIKE '%' + @DeviceId + '%')
```

## Performance Notes

- Index trên `(AccountId, IsRevoked, IsUsed, ExpiryDate, Status)` để tối ưu query
- Sử dụng LIKE pattern có thể chậm với dữ liệu lớn, cân nhắc full-text search
- Batch update để cải thiện performance khi có nhiều token

## Changelog

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2025-11-01 | Initial implementation |