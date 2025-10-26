# Hướng dẫn sử dụng OTP Verification

## Tổng quan
Hệ thống đã được tích hợp xác thực OTP qua email cho các tính năng:
1. **Đăng ký tài khoản** (Register)
2. **Quên mật khẩu** (Forgot Password)
3. **Đổi mật khẩu** (Change Password)

## Cấu hình Email Settings

Thêm vào `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "AutoApp Management"
  }
}
```

**Lưu ý:** 
- Với Gmail, cần sử dụng App Password thay vì password thông thường
- Bật 2FA và tạo App Password tại: https://myaccount.google.com/apppasswords

## Database Migration

Thêm entity `VerificationCode` vào DbContext và chạy migration:

```bash
dotnet ef migrations add AddVerificationCode
dotnet ef database update
```

## Đăng ký Dependency Injection

Thêm vào `Program.cs`:

```csharp
builder.Services.AddScoped<IVerificationService, VerificationService>();
```

## API Endpoints

### 1. Gửi OTP

**Endpoint:** `POST /api/Verification/SendOtp`

**Request:**
```json
{
  "email": "user@example.com",
  "type": 1  // 1=Register, 2=ForgotPassword, 3=ChangePassword
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Mã OTP đã được gửi đến email của bạn",
  "data": {
    "email": "user@example.com",
    "expiryMinutes": 10
  }
}
```

### 2. Xác thực OTP

**Endpoint:** `POST /api/Verification/VerifyOtp`

**Request:**
```json
{
  "email": "user@example.com",
  "code": "123456",
  "type": 2  // ForgotPassword
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Xác thực OTP thành công",
  "data": {
    "isValid": true,
    "message": "Xác thực thành công",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

### 3. Gửi lại OTP

**Endpoint:** `POST /api/Verification/ResendOtp?email=user@example.com&type=2`

**Response:** Tương tự SendOtp

---

## Flow 1: Quên mật khẩu (Forgot Password)

### Bước 1: Gửi OTP
```http
POST /api/Verification/SendOtp
Content-Type: application/json

{
  "email": "user@example.com",
  "type": 2
}
```

### Bước 2: Xác thực OTP
```http
POST /api/Verification/VerifyOtp
Content-Type: application/json

{
  "email": "user@example.com",
  "code": "123456",
  "type": 2
}
```

**Lưu token từ response!**

### Bước 3: Đặt lại mật khẩu
```http
POST /api/Account/ResetPasswordWithToken
Content-Type: application/json

{
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "newPassword": "NewPassword123!"
}
```

---

## Flow 2: Đổi mật khẩu (Change Password)

### Bước 1: Gửi OTP
```http
POST /api/Verification/SendOtp
Content-Type: application/json
Authorization: Bearer {jwt-token}

{
  "email": "user@example.com",
  "type": 3
}
```

### Bước 2: Xác thực OTP
```http
POST /api/Verification/VerifyOtp
Content-Type: application/json

{
  "email": "user@example.com",
  "code": "123456",
  "type": 3
}
```

**Lưu token từ response!**

### Bước 3: Đổi mật khẩu với OTP
```http
POST /api/Account/ChangePasswordWithOtp
Content-Type: application/json
Authorization: Bearer {jwt-token}

{
  "accountId": 1,
  "oldPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

## Flow 3: Đăng ký (Register) - Tùy chọn

### Bước 1: Tạo tài khoản (chưa active)
```http
POST /api/Account/Register
Content-Type: application/json

{
  "email": "newuser@example.com",
  "username": "newuser",
  "password": "Password123!"
}
```

### Bước 2: Gửi OTP xác thực email
```http
POST /api/Verification/SendOtp
Content-Type: application/json

{
  "email": "newuser@example.com",
  "type": 1
}
```

### Bước 3: Xác thực OTP và kích hoạt tài khoản
```http
POST /api/Verification/VerifyOtp
Content-Type: application/json

{
  "email": "newuser@example.com",
  "code": "123456",
  "type": 1
}
```

**Sau khi verify thành công, tự động kích hoạt tài khoản**

---

## Lưu ý quan trọng

### Thời gian hiệu lực
- **OTP:** 10 phút
- **Verification Token:** 30 phút

### Bảo mật
- OTP chỉ sử dụng được 1 lần
- Token được mã hóa bằng JWT
- Mỗi lần gửi OTP mới sẽ vô hiệu hóa OTP cũ
- Không cho phép gửi lại OTP nếu OTP hiện tại vẫn còn hiệu lực

### Error Handling
- **Mã OTP không hợp lệ:** Mã sai hoặc đã được sử dụng
- **Mã OTP đã hết hạn:** Quá 10 phút kể từ khi gửi
- **Token không hợp lệ:** Token sai, hết hạn, hoặc không đúng loại
- **Email không tồn tại:** Tài khoản không tồn tại hoặc đã bị vô hiệu hóa

### UI/UX Suggestions
1. Hiển thị countdown timer 10 phút cho OTP
2. Disable nút "Gửi lại OTP" trong thời gian OTP còn hiệu lực
3. Hiển thị số lần nhập sai còn lại (tùy chọn)
4. Clear form sau khi verify thành công

---

## Ví dụ Frontend Flow (React/Vue/Angular)

```javascript
// 1. Quên mật khẩu - Gửi OTP
async function forgotPassword(email) {
  const response = await fetch('/api/Verification/SendOtp', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, type: 2 })
  });
  return await response.json();
}

// 2. Verify OTP
async function verifyOtp(email, code) {
  const response = await fetch('/api/Verification/VerifyOtp', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, code, type: 2 })
  });
  const result = await response.json();
  // Lưu token vào state/localStorage
  return result.data.token;
}

// 3. Reset password
async function resetPassword(email, token, newPassword) {
  const response = await fetch('/api/Account/ResetPasswordWithToken', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, token, newPassword })
  });
  return await response.json();
}
```

---

## Testing

### Test với Postman/Insomnia
1. Import collection từ `postman_collection.json` (nếu có)
2. Hoặc sử dụng các request trên theo thứ tự

### Test Email
- Kiểm tra spam folder nếu không nhận được email
- Đảm bảo cấu hình SMTP đúng
- Test với email thật trước khi deploy production

---

## Troubleshooting

### Không nhận được email
1. Kiểm tra cấu hình SMTP trong appsettings.json
2. Kiểm tra App Password (Gmail)
3. Check spam folder
4. Kiểm tra logs trong console/file

### OTP không hợp lệ
1. Đảm bảo OTP chưa hết hạn (10 phút)
2. Đảm bảo OTP chưa được sử dụng
3. Đảm bảo email và type khớp

### Token expired
1. Token có hiệu lực 30 phút
2. Nếu hết hạn, cần gửi lại OTP và verify lại

---

## Migration từ hệ thống cũ

Nếu đang dùng hệ thống gửi mật khẩu tạm:

1. **Giữ nguyên API cũ** cho backward compatibility
2. **Thêm API mới** với OTP
3. **Frontend** sử dụng API mới
4. **Sau 1-2 tháng** deprecate API cũ

---

## Tương lai

Có thể mở rộng:
- [ ] SMS OTP (thêm phone number vào VerificationCode)
- [ ] Email template customization
- [ ] Rate limiting (giới hạn số lần gửi OTP)
- [ ] Captcha protection
- [ ] Multi-factor authentication (2FA)

---

**Created by:** AutoApp Management Team  
**Last Updated:** 2024
