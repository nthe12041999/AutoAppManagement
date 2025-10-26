# Migration Guide: ChangePasswordWithOtp - Đơn giản hóa từ 3 bước xuống 2 bước

## 🔄 Thay đổi

### Trước đây (3 bước - PHỨC TẠP):
```
1. POST /api/Verification/SendOtp → Gửi OTP
2. POST /api/Verification/VerifyOtp → Verify OTP, nhận token
3. POST /api/Account/ChangePasswordWithOtp (token) → Đổi password với token
```

### Bây giờ (2 bước - ĐƠN GIẢN):
```
1. POST /api/Verification/SendOtp → Gửi OTP
2. POST /api/Account/ChangePasswordWithOtp (otp) → Verify OTP + Đổi password luôn
```

---

## 📝 Chi tiết thay đổi

### Request DTO thay đổi

**Trước:**
```csharp
public class ChangePasswordWithOtpRequest
{
    public long AccountId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string Token { get; set; }  // ❌ Dùng token
}
```

**Sau:**
```csharp
public class ChangePasswordWithOtpRequest
{
    public long AccountId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string Otp { get; set; }  // ✅ Dùng OTP trực tiếp
}
```

---

## 🔌 API Usage

### Bước 1: Gửi OTP (KHÔNG ĐỔI)

**Endpoint:** `POST /api/Verification/SendOtp`

**Request:**
```json
{
  "email": "user@example.com",
  "type": 3
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

---

### Bước 2: Đổi mật khẩu với OTP (ĐÃ THAY ĐỔI)

**Endpoint:** `POST /api/Account/ChangePasswordWithOtp`

**Request MỚI:**
```json
{
  "accountId": 1,
  "oldPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "otp": "123456"
}
```

**Request CŨ (KHÔNG DÙNG NỮA):**
```json
{
  "accountId": 1,
  "oldPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Đổi mật khẩu thành công"
}
```

---

## 💻 Frontend Migration

### React/Vue/Angular - TRƯỚC ĐÂY

```javascript
// ❌ Flow cũ - 3 bước
async function changePasswordWithOtp(accountId, oldPassword, newPassword, email) {
  // 1. Gửi OTP
  await fetch('/api/Verification/SendOtp', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, type: 3 })
  });

  // 2. User nhập OTP, verify để lấy token
  const verifyRes = await fetch('/api/Verification/VerifyOtp', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, code: userInputOtp, type: 3 })
  });
  const { data } = await verifyRes.json();
  const token = data.token;

  // 3. Đổi mật khẩu với token
  const changeRes = await fetch('/api/Account/ChangePasswordWithOtp', {
    method: 'POST',
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${jwtToken}`
    },
    body: JSON.stringify({ 
      accountId, 
      oldPassword, 
      newPassword, 
      token 
    })
  });
  
  return await changeRes.json();
}
```

### React/Vue/Angular - BÂY GIỜ

```javascript
// ✅ Flow mới - 2 bước đơn giản
async function changePasswordWithOtp(accountId, oldPassword, newPassword, email) {
  // 1. Gửi OTP
  await fetch('/api/Verification/SendOtp', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, type: 3 })
  });

  // 2. User nhập OTP, đổi mật khẩu luôn (không cần verify riêng)
  const changeRes = await fetch('/api/Account/ChangePasswordWithOtp', {
    method: 'POST',
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${jwtToken}`
    },
    body: JSON.stringify({ 
      accountId, 
      oldPassword, 
      newPassword, 
      otp: userInputOtp  // ✅ Truyền OTP trực tiếp
    })
  });
  
  return await changeRes.json();
}
```

---

## 🎨 UI/UX Flow

### Màn 1: Nhập mật khẩu cũ và mới
```
┌─────────────────────────────────┐
│     Đổi Mật Khẩu                │
│                                 │
│  Mật khẩu cũ: [______________]  │
│  Mật khẩu mới: [______________] │
│  Xác nhận MK:  [______________] │
│                                 │
│         [Gửi OTP]               │
└─────────────────────────────────┘
```

### Màn 2: Nhập OTP và xác nhận
```
┌─────────────────────────────────────┐
│   Xác Thực OTP                      │
│                                     │
│  Email: user@example.com            │
│  OTP:   [______]                    │
│                                     │
│  Mã có hiệu lực 10 phút (9:45)     │
│  Không nhận được mã?                 │
│  [Gửi lại OTP]                       │
│                                     │
│            [Đổi mật khẩu]           │ ← ✅ Verify + Đổi password luôn
└─────────────────────────────────────┘
```

---

## 🔍 So sánh với flow Quên Mật Khẩu

Bây giờ **cả 2 flow đều đồng nhất** - verify OTP và action trong cùng 1 API:

### Flow Quên Mật Khẩu (ForgotPassword)
```
1. POST /api/Account/ForgotPassword → Gửi OTP
2. POST /api/Account/ConfirmOtpResetPassword (email, otp) → Verify + Reset password
```

### Flow Đổi Mật Khẩu (ChangePassword)
```
1. POST /api/Verification/SendOtp → Gửi OTP
2. POST /api/Account/ChangePasswordWithOtp (accountId, oldPassword, newPassword, otp) → Verify + Change password
```

**Đều là 2 bước, đều verify OTP trực tiếp trong API cuối!** ✅

---

## ⚠️ Breaking Changes

### Backend Changes
- `ChangePasswordWithOtpRequest.Token` → `ChangePasswordWithOtpRequest.Otp`
- Service không còn gọi `ValidateVerificationToken`, thay bằng `VerifyOtpAsync`

### Frontend Changes
- Không cần gọi `/api/Verification/VerifyOtp` trước khi đổi password
- Truyền `otp` thay vì `token` vào request

### Testing
- Update tất cả test cases liên quan đến `ChangePasswordWithOtp`
- Update Postman/Insomnia collections

---

## 🧪 Testing Checklist

### Test Cases Cần Chạy Lại

- [ ] Đổi mật khẩu thành công với OTP hợp lệ
- [ ] Đổi mật khẩu thất bại với OTP sai
- [ ] Đổi mật khẩu thất bại với OTP đã hết hạn (>10 phút)
- [ ] Đổi mật khẩu thất bại với OTP đã sử dụng
- [ ] Đổi mật khẩu thất bại với mật khẩu cũ sai
- [ ] Gửi lại OTP khi chưa nhận được
- [ ] Rate limiting khi gửi OTP nhiều lần

### Postman Request Example

```json
POST /api/Account/ChangePasswordWithOtp
Authorization: Bearer {{jwt_token}}
Content-Type: application/json

{
  "accountId": 1,
  "oldPassword": "OldPassword123!",
  "newPassword": "NewPassword456!",
  "otp": "123456"
}
```

---

## ✅ Lợi ích

1. ✅ **UX tốt hơn** - Giảm từ 3 bước xuống 2 bước
2. ✅ **Code đơn giản hơn** - Ít API calls hơn
3. ✅ **Bảo mật vẫn cao** - Vẫn verify OTP đầy đủ
4. ✅ **Đồng nhất với ForgotPassword flow**
5. ✅ **Ít lỗi hơn** - Ít state management hơn

---

## 🚀 Deployment Steps

1. [ ] Update backend code (DTO + Service)
2. [ ] Test API với Postman
3. [ ] Update frontend code
4. [ ] Update API documentation
5. [ ] Update Postman collections
6. [ ] Test end-to-end flow
7. [ ] Deploy to staging
8. [ ] Test on staging
9. [ ] Deploy to production
10. [ ] Notify frontend team

---

**Created:** 2024  
**Version:** 2.0 (Simplified)
