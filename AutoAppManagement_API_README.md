# AutoAppManagement API - Postman Collection

Bộ sưu tập Postman API đầy đủ cho dự án AutoAppManagement.API bao gồm tất cả các endpoints và chức năng.

## 📁 Các tệp được tạo

1. **AutoAppManagement_API_Postman_Collection.json** - Bộ sưu tập Postman chính
2. **AutoAppManagement_API_Environment.postman_environment.json** - File môi trường với các biến cấu hình
3. **AutoAppManagement_API_README.md** - Tài liệu hướng dẫn sử dụng (file này)

## 🚀 Cách import vào Postman

### Bước 1: Import Collection
1. Mở Postman
2. Click **Import** button
3. Chọn file `AutoAppManagement_API_Postman_Collection.json`
4. Click **Import**

### Bước 2: Import Environment
1. Click vào biểu tượng **Environment** (góc trên bên phải)
2. Click **Import**
3. Chọn file `AutoAppManagement_API_Environment.postman_environment.json`
4. Click **Import**
5. Chọn environment "AutoAppManagement API Environment" từ dropdown

## 🔧 Cấu hình môi trường

### Biến môi trường chính:
- `base_url`: https://localhost:7000/api (URL cơ sở của API)
- `auth_token`: Token xác thực (sẽ được tự động set sau khi login)
- `admin_username`: admin (username mặc định cho admin)
- `admin_password`: admin123 (password mặc định cho admin)

### Biến test:
- `test_account_id`: 1
- `test_username`: testuser
- `test_password`: testpass123
- `test_email`: test@example.com
- `test_phone`: +84123456789
- `test_device_id`: device123
- `test_license_key`: LICENSE-KEY-123

## 📋 Cấu trúc Collection

### 1. **Authentication** 🔐
- **Account Login**: Đăng nhập cho user thường
- **Admin Login**: Đăng nhập cho admin

### 2. **Account Management** 👤
#### CRUD Operations
- Get All Accounts
- Get Account by ID
- Get Accounts with Paging
- Create/Update Account
- Delete Account

#### Account Operations
- Get Account by Username
- Change Password
- Lock/Unlock Account
- Activate/Deactivate Account
- Extend Account
- Update Account Info
- Upload Avatar

#### Account Queries
- Get Accounts by Level
- Get Expired Accounts
- Get Expiring Accounts

### 3. **Account Devices** 📱
- Get All Account Devices
- Get Account Devices by Account ID
- Register/Update/Delete Device
- Activate/Deactivate Device
- Get Active Devices
- Get Devices by Type
- Check Device Registration

### 4. **License Management** 🎫
#### CRUD Operations
- Get All Licenses
- Get License by ID
- Create/Update License
- Delete License

#### License Operations
- Get Licenses by Account ID
- Get License by Key
- Renew/Suspend/Activate License
- Extend License

#### License Queries
- Get Expired Licenses
- Get Expiring Licenses

### 5. **Role Management** 👥
#### CRUD Operations
- Get All Roles
- Get Role by ID
- Create/Update Role
- Delete Role

#### Role Operations
- Get Roles by Account ID
- Assign Role to Account

### 6. **Permission Management** 🔑
#### CRUD Operations
- Get All Permissions
- Get Permission by ID
- Create/Update Permission
- Delete Permission

#### Permission Operations
- Get Role Accounts by Account/Role ID
- Assign/Remove Role from Account
- Bulk Assign/Remove Roles
- Get Accounts with Roles
- Get Roles with Accounts
- Check Account Permissions
- Sync Account Roles

### 7. **Notification Management** 🔔
#### CRUD Operations
- Get All Notifications
- Get Notification by ID
- Create/Update Notification
- Delete Notification

#### Notification Operations (Chưa implement)
- Get Unread Notification Count
- Mark as Read
- Get Notifications by Range

### 8. **Admin Account Management** 👑
#### CRUD Operations
- Get All Admin Accounts
- Get Admin Account by ID
- Create/Update Admin Account
- Delete Admin Account

### 9. **File Management** 📁
- Get Image
- Download File

## 🔄 Quy trình sử dụng

### 1. Xác thực (Authentication)
```
1. Sử dụng "Admin Login" hoặc "Account Login"
2. Token sẽ được tự động lưu vào biến `auth_token`
3. Tất cả các request sau sẽ tự động sử dụng token này
```

### 2. Test cơ bản
```
1. Login → Admin Login
2. Tạo account → Account Management → CRUD Operations → Create/Update Account
3. Gán role → Role Management → Role Operations → Assign Role to Account
4. Tạo license → License Management → CRUD Operations → Create/Update License
```

## ⚙️ Base Controller Features

Tất cả các controller kế thừa từ `BaseBusinessController` đều có các endpoint cơ bản:
- `GetAll()` - Lấy tất cả records
- `GetById(id)` - Lấy record theo ID
- `GetPaging(request)` - Lấy records với phân trang
- `SubmitData(dto)` - Tạo mới/cập nhật record
- `Delete(id)` - Xóa record

## 🔒 Xác thực và Phân quyền

### Headers tự động:
- `Authorization: Bearer {{auth_token}}`

### Roles được sử dụng:
- `Admin`: Toàn quyền truy cập
- `Customer`: Quyền truy cập hạn chế

## 📝 Lưu ý quan trọng

1. **Environment**: Đảm bảo đã chọn đúng environment "AutoAppManagement API Environment"
2. **HTTPS**: API chạy trên HTTPS port 7000
3. **Authentication**: Phải login trước khi sử dụng các endpoint khác
4. **Token**: Token sẽ tự động được set sau khi login thành công
5. **Validation**: Tất cả request đều có validation, đảm bảo gửi đúng format dữ liệu

## 🐛 Troubleshooting

### Lỗi 401 Unauthorized:
- Kiểm tra token có được set trong environment không
- Thử login lại để refresh token

### Lỗi 403 Forbidden:
- Kiểm tra role của account có đủ quyền không
- Thử login với account admin

### Lỗi 404 Not Found:
- Kiểm tra URL có đúng không
- Đảm bảo API server đang chạy

### Lỗi 500 Internal Server Error:
- Kiểm tra database connection
- Xem logs trong Console của API server

## 📊 Test Data Examples

### Account Data:
```json
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "password123",
  "phoneNumber": "+84123456789",
  "fullName": "Test User",
  "level": 1,
  "expiryDate": "2024-12-31T23:59:59",
  "status": 1
}
```

### License Data:
```json
{
  "accountId": 1,
  "licenseKey": "LICENSE-KEY-123",
  "licenseType": "Premium",
  "expiryDate": "2024-12-31T23:59:59",
  "maxDevices": 5,
  "status": 1
}
```

### Device Data:
```json
{
  "accountId": 1,
  "deviceId": "device123",
  "deviceType": "mobile",
  "deviceName": "iPhone 14",
  "fingerprint": "abc123def456"
}
```

## 🔄 Update History

- **v1.0** (2024-09-04): Tạo collection đầy đủ với tất cả endpoints
- Hỗ trợ auto-authentication với JWT
- Bao gồm tất cả CRUD operations cho mọi entity
- Test cases và example data đầy đủ

---

**Tác giả**: Auto-generated cho AutoAppManagement.API  
**Ngày tạo**: 04/09/2024  
**Version**: 1.0
