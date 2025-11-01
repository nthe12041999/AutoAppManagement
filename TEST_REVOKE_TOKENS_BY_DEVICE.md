# Test RevokeTokensByDevice API

## Test Login với DeviceId

```bash
# Test Login API với DeviceId
curl -X POST "https://localhost:7001/api/Account/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "test@example.com",
    "password": "password123",
    "deviceId": "iPhone-15-Pro-TEST123",
    "fingerprint": "browser_fingerprint_123"
  }'
```

Response mẫu:
```json
{
  "isSuccess": true,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "loginTime": "2025-11-01T10:30:00Z",
    "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck...",
    "refreshTokenExpired": "2025-11-08T10:30:00Z",
    "licenseInfo": {...}
  }
}
```

## Test RevokeTokensByDevice API

```bash
# Test RevokeTokensByDevice API
curl -X POST "https://localhost:7001/api/Account/RevokeTokensByDevice" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "iPhone-15-Pro-TEST123"
  }'
```

Response mẫu thành công:
```json
{
  "isSuccess": true,
  "message": "Đã thu hồi 1 token(s) của device iPhone-15-Pro-TEST123",
  "data": true
}
```

## Flow hoạt động:

1. **Login**: `deviceId` từ request được lưu vào `RefreshToken.DeviceInfo`
2. **RevokeTokensByDevice**: Tìm tất cả RefreshToken có:
   - `AccountId` = accountId từ JWT token hiện tại
   - `DeviceInfo` CONTAINS `deviceId` OR `UserAgent` CONTAINS `deviceId`
   - `IsRevoked` = false, `IsUsed` = false, `ExpiryDate` > now
3. **Mark revoked**: Set `IsRevoked = true`, `RevokedDate = now`, `RevokedByIp`

## Kiểm tra database:

```sql
-- Xem RefreshToken trước khi revoke
SELECT AccountId, DeviceInfo, UserAgent, IsRevoked, CreatedDate, ExpiryDate
FROM RefreshToken 
WHERE AccountId = @AccountId 
  AND (DeviceInfo LIKE '%iPhone-15-Pro-TEST123%' OR UserAgent LIKE '%iPhone-15-Pro-TEST123%')
  AND IsRevoked = 0;

-- Xem RefreshToken sau khi revoke
SELECT AccountId, DeviceInfo, UserAgent, IsRevoked, RevokedDate, RevokedByIp
FROM RefreshToken 
WHERE AccountId = @AccountId 
  AND (DeviceInfo LIKE '%iPhone-15-Pro-TEST123%' OR UserAgent LIKE '%iPhone-15-Pro-TEST123%')
  AND IsRevoked = 1;
```

## Test với RefreshToken đã bị revoke:

```bash
# Thử sử dụng refresh token đã bị revoke
curl -X POST "https://localhost:7001/api/Account/RefreshToken" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck..."
  }'
```

Response mong đợi:
```json
{
  "isSuccess": false,
  "message": "Refresh token đã hết hạn hoặc bị thu hồi",
  "data": null
}
```