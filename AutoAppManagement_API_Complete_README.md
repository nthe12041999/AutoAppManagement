# AutoAppManagement API - Postman Collection Hướng Dẫn

## Tổng Quan

Collection này bao gồm tất cả API endpoints của AutoAppManagement.API với cấu trúc đầy đủ và chi tiết.

## Files Bao Gồm

- `AutoAppManagement_API_Complete_Postman_Collection.json` - Collection chính với tất cả endpoints
- `AutoAppManagement_API_Complete_Environment.postman_environment.json` - Environment variables

## Cấu Trúc Collection

### 🔐 Authentication

- **Account Login** - Đăng nhập customer với email/phone + password
- **Admin Login** - Đăng nhập admin với username + password

### 👤 Account Management (20+ endpoints)

- CRUD operations cho tài khoản
- Quản lý trạng thái (lock/unlock, activate/deactivate)
- Quản lý thông tin cá nhân, avatar
- Xem tài khoản theo level, expired, expiring
- Gia hạn tài khoản, đổi mật khẩu

### 📱 Device Management (11 endpoints)

- Đăng ký, cập nhật, xóa thiết bị
- Quản lý trạng thái thiết bị
- Xem thiết bị theo tài khoản, loại thiết bị
- Kiểm tra thiết bị đã đăng ký

### 🎫 License Management (13 endpoints)

- CRUD operations cho license
- Gia hạn, suspend, activate license
- Xem license expired, expiring
- Quản lý license theo tài khoản

### 👥 Role Management (7 endpoints)

- CRUD operations cho roles
- Gán role cho tài khoản
- Xem roles theo tài khoản

### 🔧 Admin Account Management (5 endpoints)

- CRUD operations cho admin accounts

## Hướng Dẫn Sử Dụng

### 1. Import vào Postman

1. Mở Postman
2. Click **Import**
3. Chọn file `AutoAppManagement_API_Complete_Postman_Collection.json`
4. Import environment file `AutoAppManagement_API_Complete_Environment.postman_environment.json`

### 2. Cấu Hình Environment

1. Chọn environment "AutoAppManagement API Environment - Complete"
2. Cập nhật các variables:
   - `baseUrl`: URL của API (mặc định: https://localhost:7000)
   - `testEmail`: Email test cho customer login
   - `testPassword`: Password test cho customer login
   - `adminUsername`: Username admin
   - `adminPassword`: Password admin

### 3. Authentication Flow

#### Customer Authentication:

1. Chạy request **"Account Login"** trong folder Authentication
2. Token sẽ được lưu tự động vào environment variable `token`
3. Tất cả các request khác sẽ sử dụng token này

#### Admin Authentication:

1. Chạy request **"Admin Login"** trong folder Authentication
2. Token admin sẽ được lưu vào environment variable `adminToken`

### 4. Test Scenarios

#### Scenario 1: Quản lý tài khoản customer

```
1. Account Login
2. Get Account by ID
3. Update Account Info
4. Get Account by Username (Admin required)
```

#### Scenario 2: Quản lý thiết bị

```
1. Account Login
2. Register Device
3. Get Devices by Account ID
4. Update Device
5. Activate/Deactivate Device
```

#### Scenario 3: Quản lý license

```
1. Admin Login
2. Get All Licenses
3. Create/Update License
4. Assign License to Account
5. Check License Status
```

#### Scenario 4: Quản lý roles

```
1. Admin Login
2. Get All Roles
3. Create Role
4. Assign Role to Account
5. Get Roles by Account ID
```

## Environment Variables

| Variable        | Mô Tả                  | Giá Trị Mặc Định            |
| --------------- | ---------------------- | --------------------------- |
| `baseUrl`       | Base URL của API       | https://localhost:7000      |
| `token`         | JWT token cho customer | (auto-set from login)       |
| `adminToken`    | JWT token cho admin    | (auto-set from admin login) |
| `testAccountId` | Account ID để test     | 1                           |
| `testEmail`     | Email test             | customer@email.com          |
| `testPassword`  | Password test          | password123                 |
| `testDeviceId`  | Device ID test         | device-001                  |
| `adminUsername` | Admin username         | admin                       |
| `adminPassword` | Admin password         | admin123                    |

## Request Headers

Tất cả requests (trừ login) đều cần header:

```
Authorization: Bearer {{token}}
Content-Type: application/json
```

## Response Formats

Tất cả API responses đều có format:

```json
{
  "success": true/false,
  "message": "Success/Error message",
  "data": {...},
  "errors": [...]
}
```

## Authorization Levels

### Public Endpoints

- Account Login
- Admin Login

### Customer Endpoints (Require Authentication)

- Xem thông tin tài khoản của mình
- Cập nhật thông tin cá nhân
- Quản lý thiết bị của mình
- Xem license của mình

### Admin Endpoints (Require Admin Role)

- Quản lý tất cả tài khoản
- Quản lý tất cả thiết bị
- Quản lý license
- Quản lý roles
- Quản lý admin accounts

## Tips & Best Practices

1. **Luôn chạy login trước** khi test các endpoints khác
2. **Kiểm tra token expiry** - login lại nếu token hết hạn
3. **Sử dụng correct environment** - đảm bảo chọn đúng environment
4. **Test với data thực** - thay đổi test data trong environment variables
5. **Check response status** - 200 OK, 401 Unauthorized, 403 Forbidden, etc.

## Debugging

### Lỗi 401 Unauthorized

- Kiểm tra token trong Authorization header
- Login lại để lấy token mới

### Lỗi 403 Forbidden

- Endpoint yêu cầu admin role
- Sử dụng admin login thay vì customer login

### Lỗi 404 Not Found

- Kiểm tra URL endpoint
- Kiểm tra baseUrl trong environment

### Lỗi 500 Internal Server Error

- Kiểm tra API server có chạy không
- Kiểm tra database connection
- Xem logs server để debug

## Contact & Support

- Developer: AutoAppManagement Team
- API Documentation: Xem trong source code controllers
- Database: SQL Server tại 125.253.121.206

---

_Generated for AutoAppManagement.API - Complete Testing Suite_
