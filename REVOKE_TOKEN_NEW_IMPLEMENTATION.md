# Test RevokeToken API với DeviceId từ JWT

## Mô tả

API `RevokeToken` bây giờ đã được cập nhật để:
1. **Login**: DeviceId được thêm vào JWT token
2. **RevokeToken**: Lấy deviceId trực tiếp từ JWT token để thu hồi

## Flow hoạt động mới

### 1. Login với DeviceId
```bash
curl -X POST "https://localhost:7001/api/Account/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "test@example.com",
    "password": "password123",
    "deviceId": "iPhone-15-Pro-ABC123"
  }'
```

**Response**:
```json
{
  "isSuccess": true,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkZXZpY2VJZCI6ImlQaG9uZS0xNS1Qcm8tQUJDMTIzIn0...",
    "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck...",
    ...
  }
}
```

**JWT Token giờ chứa**:
```json
{
  "NameIdentifier": "123",
  "Name": "testuser",
  "UserId": "123",
  "deviceId": "iPhone-15-Pro-ABC123",   // ← THÊM MỚI
  "loginTime": "2025-11-01 10:30:00"
}
```

### 2. RevokeToken (KHÔNG cần truyền deviceId)
```bash
curl -X POST "https://localhost:7001/api/Account/RevokeToken" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  # KHÔNG cần body!
```

**Response**:
```json
{
  "isSuccess": true,
  "message": "Đã thu hồi 1 token(s) của device iPhone-15-Pro-ABC123",
  "data": true
}
```

## So sánh API cũ vs mới

| API | Cũ | Mới |
|-----|----|----|
| **RevokeToken** | Dựa trên IP + User-Agent | Dựa trên deviceId từ JWT |
| **Request Body** | Không cần | Không cần |
| **Độ chính xác** | Thấp (có thể nhầm device) | Cao (chính xác device) |
| **Security** | Trung bình | Cao hơn |

## Cách hoạt động

### RevokeToken API:
1. **Extract JWT**: Lấy claims từ JWT token hiện tại
2. **Get DeviceId**: `var deviceId = userContext?.FindFirst("deviceId")?.Value`
3. **Query Database**: Tìm RefreshToken có `DeviceInfo == deviceId`
4. **Revoke**: Set `IsRevoked = true` cho các token tìm được

### Database Query:
```sql
UPDATE RefreshToken 
SET IsRevoked = 1, 
    RevokedDate = GETUTCDATE(), 
    RevokedByIp = @IpAddress
WHERE AccountId = @AccountId 
  AND DeviceInfo = @DeviceId
  AND IsRevoked = 0 
  AND IsUsed = 0 
  AND ExpiryDate > GETUTCDATE()
  AND Status = 1
```

## Test Cases

### Test 1: Đăng nhập và thu hồi token thành công
```bash
# 1. Login
LOGIN_RESPONSE=$(curl -s -X POST "https://localhost:7001/api/Account/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "test@example.com",
    "password": "password123",
    "deviceId": "TestDevice123"
  }')

# 2. Extract token
TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.data.token')

# 3. RevokeToken
curl -X POST "https://localhost:7001/api/Account/RevokeToken" \
  -H "Authorization: Bearer $TOKEN"

# Expected: Success với message "Đã thu hồi X token(s) của device TestDevice123"
```

### Test 2: Token không có deviceId
```bash
# Nếu login không có deviceId
curl -X POST "https://localhost:7001/api/Account/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "test@example.com", 
    "password": "password123"
  }'

# JWT không chứa deviceId claim
# RevokeToken sẽ trả lỗi: "Không tìm thấy deviceId trong token hiện tại"
```

### Test 3: Sử dụng token đã bị revoke
```bash
# Thử refresh token đã bị revoke
curl -X POST "https://localhost:7001/api/Account/RefreshToken" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "revoked_token_here"
  }'

# Expected: Error "Refresh token đã hết hạn hoặc bị thu hồi"
```

## Ưu điểm của cách mới

1. **Chính xác**: DeviceId được lưu trực tiếp trong JWT
2. **Đơn giản**: Không cần truyền tham số
3. **An toàn**: Không thể giả mạo deviceId
4. **Nhất quán**: DeviceId từ login = DeviceId khi revoke

## Migration Notes

- **Backward Compatible**: API cũ `RevokeTokensByDevice` vẫn hoạt động
- **JWT Size**: Tăng nhẹ do thêm deviceId claim
- **Database**: Không cần thay đổi schema