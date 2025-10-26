# API Curl Commands - Quick Reference

## Base URL
```
https://localhost:44395
```

---

## 1. Đổi Mật Khẩu (Change Password)

**Endpoint:** `POST /api/Account/ChangePassword`

**Yêu cầu:** Phải có JWT token (đã đăng nhập)

**Request:**
```bash
curl -X POST "https://localhost:44395/api/Account/ChangePassword" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "oldPassword123",
    "newPassword": "NewStrongPass123!",
    "confirmPassword": "NewStrongPass123!"
  }'
```

**Response Success:**
```json
{
  "isSuccess": true,
  "message": "Đổi mật khẩu thành công",
  "data": {
    "isSuccess": true,
    "message": "Đổi mật khẩu thành công"
  }
}
```

**Response Error:**
```json
{
  "isSuccess": false,
  "message": "Mật khẩu hiện tại không chính xác"
}
```

---

## 2. Quên Mật Khẩu (Forgot Password)

**Endpoint:** `POST /api/Account/ForgotPassword`

**Yêu cầu:** Không cần token (public endpoint)

**Request:**
```bash
curl -X POST "https://localhost:44395/api/Account/ForgotPassword" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "test@example.com"
  }'
```

**Response Success:**
```json
{
  "isSuccess": true,
  "message": "Mật khẩu mới đã được gửi đến email của bạn.",
  "data": {
    "isSuccess": true,
    "message": "Mật khẩu mới đã được gửi đến email của bạn.",
    "maskedEmail": "t***t@example.com"
  }
}
```

**Response Error:**
```json
{
  "isSuccess": false,
  "message": "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên."
}
```

---

## 3. Đăng Nhập (Login)

**Endpoint:** `POST /api/Account/Login`

**Yêu cầu:** Không cần token

**Request:**
```bash
curl -X POST "https://localhost:44395/api/Account/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "user@example.com",
    "password": "password123"
  }'
```

**Response Success:**
```json
{
  "isSuccess": true,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck...",
    "accessTokenExpired": "2024-10-25T10:30:00Z",
    "refreshTokenExpired": "2024-10-31T10:30:00Z",
    "loginTime": "2024-10-24T10:30:00Z",
    "licenseInfo": {
      "licenseId": 1,
      "licenseName": "Premium License",
      "licenseType": "Premium",
      "status": 1,
      "daysRemaining": 365
    }
  }
}
```

---

## 4. Làm Mới Token (Refresh Token)

**Endpoint:** `POST /api/Account/RefreshToken`

**Yêu cầu:** Refresh token từ API Login

**Request:**
```bash
curl -X POST "https://localhost:44395/api/Account/RefreshToken" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck..."
  }'
```

**Response Success:**
```json
{
  "isSuccess": true,
  "message": "Refresh token thành công",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "accessTokenExpired": "2024-10-25T11:30:00Z",
    "refreshToken": "CfDJ8N3Xx6CkOruOvBjBFdOo7dl...",
    "refreshTokenExpired": "2024-11-01T11:30:00Z"
  }
}
```

**Response Error:**
```json
{
  "isSuccess": false,
  "message": "Refresh token không hợp lệ"
}
```

---

## 5. Thu Hồi Token (Revoke Token)

**Endpoint:** `POST /api/Account/RevokeToken`

**Yêu cầu:** Access token (JWT)

**Request:**
```bash
curl -X POST "https://localhost:44395/api/Account/RevokeToken" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "token": "CfDJ8M2Ww5BjNqtNuAiAEcNm6ck..."
  }'
```

**Response Success:**
```json
{
  "isSuccess": true,
  "message": "Thu hồi token thành công"
}
```

---

## 6. Thu Hồi Tất Cả Token (Revoke All Tokens)

**Endpoint:** `POST /api/Account/RevokeAllTokens`

**Yêu cầu:** Access token (JWT)

**Request:**
```bash
curl -X POST "https://localhost:44395/api/Account/RevokeAllTokens" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json"
```

**Response Success:**
```json
{
  "isSuccess": true,
  "message": "Thu hồi tất cả token thành công"
}
```

---

## 7. Admin Đổi Mật Khẩu (Admin Change Password)

**Endpoint:** `POST /api/Account/AdminChangePassword`

**Yêu cầu:** Admin JWT token

**Request:**
```bash
curl -X POST "https://localhost:44395/api/Account/AdminChangePassword" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "accountId": 123,
    "newPassword": "AdminSet!Pass123",
    "confirmPassword": "AdminSet!Pass123",
    "sendEmailNotification": true
  }'
```

**Response Success:**
```json
{
  "isSuccess": true,
  "message": "Đổi mật khẩu thành công",
  "data": {
    "isSuccess": true,
    "message": "Đổi mật khẩu thành công"
  }
}
```

---

## Token Lifecycle

```
1. Đăng nhập → Nhận Access Token (24h) + Refresh Token (7 ngày)
2. Access Token hết hạn → Dùng Refresh Token để lấy token mới
3. Refresh Token hết hạn → Phải đăng nhập lại
4. Logout → Thu hồi tất cả token
```

---

## Lưu Ý Quan Trọng

### 1. Thay thế giá trị
- `YOUR_JWT_TOKEN`: Thay bằng access token từ API Login
- `YOUR_ACCESS_TOKEN`: Thay bằng access token hiện tại
- `CfDJ8M2Ww5BjNqtNuAiAEcNm6ck...`: Thay bằng refresh token thực tế

### 2. Quy tắc mật khẩu mạnh
- Tối thiểu 8 ký tự
- Phải có chữ hoa, chữ thường, số và ký tự đặc biệt
- Điểm số tối thiểu: 60/100

### 3. Token expiry
- **Access Token**: 24 giờ
- **Refresh Token**: 7 ngày

### 4. Base URL trong Production
Thay `https://localhost:44395` bằng domain production thực tế.

---

## Flow Sử Dụng Thông Thường

### Khi User đăng nhập lần đầu:
```bash
# 1. Login
curl -X POST ".../api/Account/Login" -d '{"emailOrPhone":"user@example.com","password":"pass"}'

# Nhận: accessToken + refreshToken
```

### Khi Access Token hết hạn:
```bash
# 2. Refresh Token
curl -X POST ".../api/Account/RefreshToken" -d '{"refreshToken":"..."}'

# Nhận: accessToken mới + refreshToken mới
```

### Khi User logout:
```bash
# 3. Revoke All Tokens
curl -X POST ".../api/Account/RevokeAllTokens" -H "Authorization: Bearer ..."
```

### Khi User quên mật khẩu:
```bash
# 4. Forgot Password
curl -X POST ".../api/Account/ForgotPassword" -d '{"emailOrPhone":"user@example.com"}'

# Nhận mật khẩu mới qua email
```
