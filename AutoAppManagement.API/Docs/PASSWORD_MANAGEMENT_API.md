# Password Management API Documentation

## Tổng quan
Tài liệu này mô tả các API để quản lý mật khẩu trong hệ thống AutoApp Management, bao gồm:
- API quên mật khẩu (gửi mật khẩu mới qua email)
- API đổi mật khẩu (user tự đổi)
- API admin đổi mật khẩu (admin đổi cho user khác)

## 1. API Quên Mật Khẩu

### Endpoint
```
POST /api/Account/ForgotPassword
```

### Mô tả
API này cho phép user yêu cầu đặt lại mật khẩu khi quên. Hệ thống sẽ:
1. Tạo mật khẩu mới mạnh (12 ký tự)
2. Cập nhật mật khẩu trong database
3. Gửi mật khẩu mới qua email

### Request Body
```json
{
  "emailOrPhone": "user@example.com"
}
```

### Response Success
```json
{
  "isSuccess": true,
  "message": "Mật khẩu mới đã được gửi đến email của bạn.",
  "data": {
    "isSuccess": true,
    "message": "Mật khẩu mới đã được gửi đến email của bạn.",
    "maskedEmail": "u***r@example.com"
  }
}
```

### Response Error
```json
{
  "isSuccess": false,
  "message": "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên."
}
```

### Curl Example
```bash
curl -X POST "https://localhost:44395/api/Account/ForgotPassword" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "test@example.com"
  }'
```

## 2. API Đổi Mật Khẩu (User)

### Endpoint
```
POST /api/Account/ChangePassword
```

### Mô tả
API này cho phép user đổi mật khẩu của chính mình. Yêu cầu:
- User phải đăng nhập (có JWT token)
- Phải cung cấp mật khẩu hiện tại
- Mật khẩu mới phải đủ mạnh (tối thiểu 60% độ mạnh)

### Headers
```
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json
```

### Request Body
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "NewStrongPass123!",
  "confirmPassword": "NewStrongPass123!"
}
```

### Response Success
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

### Response Error
```json
{
  "isSuccess": false,
  "message": "Mật khẩu hiện tại không chính xác"
}
```

### Curl Example
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

## 3. API Admin Đổi Mật Khẩu

### Endpoint
```
POST /api/Account/AdminChangePassword
```

### Mô tả
API này cho phép admin đổi mật khẩu cho user khác. Đặc điểm:
- Chỉ admin mới có quyền sử dụng
- Không cần mật khẩu cũ
- Có thể gửi email thông báo cho user

### Headers
```
Authorization: Bearer <ADMIN_JWT_TOKEN>
Content-Type: application/json
```

### Request Body
```json
{
  "accountId": 123,
  "newPassword": "NewStrongPass123!",
  "confirmPassword": "NewStrongPass123!",
  "sendEmailNotification": true
}
```

### Response Success
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

### Curl Example
```bash
curl -X POST "https://localhost:44395/api/Account/AdminChangePassword" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "accountId": 123,
    "newPassword": "NewStrongPass123!",
    "confirmPassword": "NewStrongPass123!",
    "sendEmailNotification": true
  }'
```

## Cấu hình Email

### Cập nhật appsettings.json
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "AutoApp Management System"
  }
}
```

### Lưu ý bảo mật
1. **Gmail App Password**: Nếu sử dụng Gmail, cần tạo App Password thay vì dùng mật khẩu thường
2. **Environment Variables**: Trong production, nên sử dụng environment variables thay vì hardcode trong appsettings
3. **SMTP SSL**: Luôn sử dụng SSL/TLS cho SMTP connection

## Quy tắc mật khẩu mạnh

Hệ thống áp dụng các quy tắc sau để đánh giá độ mạnh mật khẩu:

### Điểm số tối thiểu: 60/100

### Tiêu chí chấm điểm:
- **Độ dài >= 8 ký tự**: +25 điểm
- **Độ dài >= 12 ký tự**: +10 điểm thêm
- **Độ dài >= 16 ký tự**: +10 điểm thêm
- **Có chữ thường**: +10 điểm
- **Có chữ hoa**: +10 điểm
- **Có số**: +10 điểm
- **Có ký tự đặc biệt**: +15 điểm
- **Không có pattern lặp**: +10 điểm

### Ví dụ mật khẩu mạnh:
- `MyStr0ng!Pass` (85 điểm)
- `SecureP@ssw0rd2024` (100 điểm)
- `Tr0ub4dor&3` (90 điểm)

## Lỗi thường gặp

### 1. Email không được gửi
**Nguyên nhân**: Cấu hình SMTP sai hoặc Gmail App Password không đúng
**Giải pháp**: Kiểm tra lại cấu hình EmailSettings và tạo App Password mới

### 2. Mật khẩu không đủ mạnh
**Nguyên nhân**: Mật khẩu mới có điểm số < 60
**Giải pháp**: Sử dụng mật khẩu có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt

### 3. Tài khoản không tồn tại
**Nguyên nhân**: Email/phone không có trong hệ thống hoặc tài khoản đã bị vô hiệu hóa
**Giải pháp**: Kiểm tra lại thông tin hoặc liên hệ admin

### 4. JWT Token không hợp lệ
**Nguyên nhân**: Token hết hạn hoặc không đúng format
**Giải pháp**: Đăng nhập lại để lấy token mới

## Test Cases

### Test Case 1: Quên mật khẩu thành công
```bash
# Request
POST /api/Account/ForgotPassword
{
  "emailOrPhone": "existing@example.com"
}

# Expected: 200 OK, email được gửi
```

### Test Case 2: Đổi mật khẩu thành công
```bash
# Request (với valid JWT)
POST /api/Account/ChangePassword
{
  "currentPassword": "correctOldPass",
  "newPassword": "NewStr0ng!Pass",
  "confirmPassword": "NewStr0ng!Pass"
}

# Expected: 200 OK, mật khẩu được cập nhật
```

### Test Case 3: Admin đổi mật khẩu thành công
```bash
# Request (với admin JWT)
POST /api/Account/AdminChangePassword
{
  "accountId": 123,
  "newPassword": "AdminSet!Pass123",
  "confirmPassword": "AdminSet!Pass123",
  "sendEmailNotification": true
}

# Expected: 200 OK, mật khẩu được cập nhật, email được gửi
```
