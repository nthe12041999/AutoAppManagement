# Flow Quên Mật Khẩu (Forgot Password)

## 🎯 Tổng quan

Flow đơn giản với 2 bước:
1. **ForgotPassword** - Gửi OTP qua email
2. **ConfirmOtpResetPassword** - Verify OTP và reset password tự động

## 📝 Flow chi tiết

```
User                    API                     Email
  |                      |                        |
  |--1. ForgotPassword-->|                        |
  |    (email)           |                        |
  |                      |--Send OTP Email------->|
  |                      |--Save OTP to DB------->|
  |<----Success----------|                        |
  |                      |                        |
  |                      |                        |
  |--2. ConfirmOTP------>|                        |
  |    (email, otp)      |                        |
  |                      |--Verify OTP----------->|
  |                      |--Generate Random Pass->|
  |                      |--Reset Password------->|
  |                      |--Send New Password---->|--Email Password-->
  |<----Success----------|                        |
```

## 🔌 API Endpoints

### 1. ForgotPassword - Gửi OTP

**Endpoint:** `POST /api/Account/ForgotPassword`

**Request:**
```json
{
  "emailOrPhone": "user@example.com"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Mã OTP đã được gửi đến email của bạn",
  "data": null
}
```

**Chức năng:**
- ✅ Kiểm tra email tồn tại
- ✅ Tạo mã OTP 6 số
- ✅ Lưu OTP vào DB (hiệu lực 10 phút)
- ✅ Gửi OTP qua email

---

### 2. ResendOtp - Gửi lại mã OTP

**Endpoint:** `POST /api/Account/ResendOtp`

**Request:**
```json
{
  "emailOrPhone": "user@example.com"
}
```

**Response (Thành công):**
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

**Response (OTP cũ vẫn còn hiệu lực):**
```json
{
  "isSuccess": false,
  "message": "Vui lòng đợi 5 phút trước khi gửi lại OTP"
}
```

**Chức năng:**
- ✅ Kiểm tra email tồn tại
- ✅ Kiểm tra OTP cũ còn hiệu lực không (rate limiting)
- ✅ Nếu OTP cũ hết hạn: Tạo OTP mới và gửi
- ✅ Nếu OTP cũ còn: Thông báo đợi

---

### 3. ConfirmOtpResetPassword - Verify OTP và Reset

**Endpoint:** `POST /api/Account/ConfirmOtpResetPassword`

**Request:**
```json
{
  "email": "user@example.com",
  "otp": "123456"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Đặt lại mật khẩu thành công. Mật khẩu mới đã được gửi đến email của bạn.",
  "data": null
}
```

**Chức năng:**
- ✅ Verify OTP (kiểm tra mã, hết hạn, đã dùng chưa)
- ✅ Đánh dấu OTP đã sử dụng
- ✅ Tạo mật khẩu random 8 ký tự
- ✅ Reset password trong DB
- ✅ Gửi mật khẩu mới qua email

---

## 💻 Ví dụ code Frontend

### React/Vue/Angular

```javascript
// Bước 1: Gửi OTP
async function forgotPassword(email) {
  const response = await fetch('/api/Account/ForgotPassword', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ emailOrPhone: email })
  });
  
  const result = await response.json();
  
  if (result.isSuccess) {
    alert('OTP đã được gửi đến email của bạn!');
    // Chuyển sang form nhập OTP
  } else {
    alert(result.message);
  }
}

// Bước 2: Gửi lại OTP (nếu cần)
async function resendOtp(email) {
  const response = await fetch('/api/Account/ResendOtp', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ emailOrPhone: email })
  });
  
  const result = await response.json();
  
  if (result.isSuccess) {
    alert('Mã OTP mới đã được gửi!');
  } else {
    alert(result.message); // "Vui lòng đợi 5 phút..."
  }
}

// Bước 3: Confirm OTP và reset password
async function confirmOtp(email, otp) {
  const response = await fetch('/api/Account/ConfirmOtpResetPassword', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ 
      email: email,
      otp: otp 
    })
  });
  
  const result = await response.json();
  
  if (result.isSuccess) {
    alert('Mật khẩu mới đã được gửi đến email!');
    // Chuyển về trang login
  } else {
    alert(result.message);
  }
}
```

---

## 🎨 UI/UX Flow

### Màn 1: Nhập Email
```
┌─────────────────────────────────┐
│     Quên Mật Khẩu               │
│                                 │
│  Email: [___________________]   │
│                                 │
│         [Gửi OTP]               │
└─────────────────────────────────┘
```

### Màn 2: Nhập OTP
```
┌─────────────────────────────────────┐
│   Nhập Mã OTP                       │
│                                     │
│  Email: user@example.com            │
│  OTP:   [______]                    │
│                                     │
│  Mã có hiệu lực 10 phút (9:45)     │
│  Không nhận được mã?                 │
│  [Gửi lại OTP]                       │
│                                     │
│            [Xác nhận]                │
└─────────────────────────────────────┘

Lưu ý:
- Nút "Gửi lại OTP" disable trong 1-2 phút sau khi gửi
- Hiển thị countdown timer cho OTP (10:00 → 9:59 → ...)
- Nếu OTP hết hạn, tự động enable nút "Gửi lại"
```

### Màn 3: Thành công
```
┌─────────────────────────────────┐
│   ✓ Thành công!                 │
│                                 │
│  Mật khẩu mới đã được gửi       │
│  đến email của bạn.             │
│                                 │
│  Vui lòng kiểm tra email và     │
│  đổi mật khẩu sau khi đăng nhập │
│                                 │
│       [Đăng nhập ngay]          │
└─────────────────────────────────┘
```

---

## ⚙️ Cấu hình

### Email Settings (appsettings.json)

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

### OTP Settings

- **Độ dài OTP:** 6 chữ số
- **Hiệu lực:** 10 phút
- **Sử dụng:** 1 lần duy nhất
- **Lưu trữ:** Database (bảng VerificationCodes)

### Random Password

- **Độ dài:** 8 ký tự
- **Bao gồm:** A-Z, a-z, 0-9, @#$%
- **Ví dụ:** `aB3@xY7$`

---

## 🔒 Bảo mật

### OTP Protection
✅ Chỉ sử dụng 1 lần  
✅ Hết hạn sau 10 phút  
✅ Lưu trong DB, không gửi qua URL  
✅ Rate limiting (tránh spam)  

### Password Generation
✅ Random 8 ký tự  
✅ Bao gồm chữ hoa, thường, số, ký tự đặc biệt  
✅ Hash SHA256 trước khi lưu DB  

### Email Security
✅ SSL/TLS encryption  
✅ App Password (không dùng password thật)  
✅ Template HTML (tránh phishing)  

---

## 📧 Email Templates

### OTP Email (đã có sẵn)
```html
Subject: Mã xác thực khôi phục mật khẩu - AutoApp Management

Bạn đã yêu cầu khôi phục mật khẩu.
Mã OTP của bạn là: 123456
Mã có hiệu lực trong 10 phút.
```

### Password Reset Email (đã có sẵn)
```html
Subject: Đặt lại mật khẩu - AutoApp Management

Mật khẩu mới của bạn: aB3@xY7$
Vui lòng đổi mật khẩu sau khi đăng nhập.
```

---

## ⚠️ Error Handling

### Lỗi thường gặp:

**Email không tồn tại:**
```json
{
  "isSuccess": false,
  "message": "Email không tồn tại trong hệ thống"
}
```

**OTP sai:**
```json
{
  "isSuccess": false,
  "message": "Mã OTP không hợp lệ"
}
```

**OTP hết hạn:**
```json
{
  "isSuccess": false,
  "message": "Mã OTP đã hết hạn"
}
```

**OTP đã dùng:**
```json
{
  "isSuccess": false,
  "message": "Mã OTP không hợp lệ"
}
```

---

## 🧪 Testing

### Test Flow thủ công:

1. Gọi `ForgotPassword` với email hợp lệ
2. Check email, copy OTP
3. Gọi `ConfirmOtpResetPassword` với email + OTP
4. Check email, nhận password mới
5. Login với password mới

### Test Cases:

- ✅ Email hợp lệ
- ❌ Email không tồn tại
- ❌ Email không đúng định dạng
- ❌ OTP sai
- ❌ OTP hết hạn (đợi > 10 phút)
- ❌ OTP đã dùng (dùng lại OTP cũ)
- ❌ Gửi OTP quá nhiều lần (rate limit)

---

## 🔄 So sánh với flow cũ

| Feature | Flow cũ (3 steps) | Flow mới (2 steps) |
|---------|-------------------|-------------------|
| **Số bước** | 3 | 2 |
| **Token trung gian** | ✅ Có | ❌ Không |
| **User nhập password** | ✅ Có | ❌ Không (auto) |
| **Đơn giản** | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| **UX** | OK | Excellent |
| **Security** | Cao | Cao |

---

## 📚 Best Practices

1. ✅ **Always use HTTPS** trong production
2. ✅ **Rate limiting** - Giới hạn 3 OTP/5 phút
3. ✅ **Log everything** - Audit trail
4. ✅ **Clear messaging** - User biết chính xác phải làm gì
5. ✅ **Cleanup old OTPs** - Background job tự động xóa

---

## 🚀 Deployment Checklist

- [ ] Config SMTP settings trong appsettings.json
- [ ] Test gửi email thành công
- [ ] Verify database có bảng VerificationCodes
- [ ] Test flow end-to-end
- [ ] Setup rate limiting
- [ ] Setup monitoring/logging
- [ ] Test trên staging trước
- [ ] Document cho team

---

**Created by:** AutoApp Management Team  
**Last Updated:** 2024  
**Version:** 2.0 (Simplified)
