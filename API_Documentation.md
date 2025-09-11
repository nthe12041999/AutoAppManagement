# AutoAppManagement API Documentation

## 📋 Tổng quan
Bộ sưu tập API cho hệ thống AutoAppManagement bao gồm tất cả các endpoints cho việc quản lý Admin Accounts, User Accounts, Licenses, Features và Roles.

## 🚀 Cách sử dụng

### 1. Import Postman Collection
```bash
# Import file sau vào Postman:
AutoAppManagement_API_Collection.postman_collection.json
```

### 2. Sử dụng cURL Commands
```bash
# Chạy các lệnh từ file:
AutoAppManagement_cURL_Commands.sh
```

### 3. Setup Environment Variables
```
base_url: https://localhost:7000
access_token: [sẽ được tự động set sau khi login]
```

## 🔐 Authentication Flow

### Bước 1: Login để lấy token
```bash
curl --location 'https://localhost:7000/api/Auth/Login' \
--header 'Content-Type: application/json' \
--data '{
    "username": "admin",
    "password": "123456"
}'
```

### Bước 2: Sử dụng token trong các request tiếp theo
```bash
curl --location 'https://localhost:7000/api/AdminAccount/GetAll' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'
```

## 📚 API Categories

### 🔐 Authentication APIs
- **POST** `/api/Auth/Login` - Admin login
- **POST** `/api/Account/Login` - User login

### 👥 Admin Account Management
- **GET** `/api/AdminAccount/GetAll` - Lấy tất cả admin accounts
- **GET** `/api/AdminAccount/GetById/{id}` - Lấy admin theo ID
- **POST** `/api/AdminAccount/SubmitData` - Tạo/Cập nhật admin
- **DELETE** `/api/AdminAccount/Delete/{id}` - Xóa admin
- **POST** `/api/AdminAccount/ChangePassword` - Đổi mật khẩu
- **POST** `/api/AdminAccount/LockAccount` - Khóa tài khoản
- **POST** `/api/AdminAccount/UnlockAccount/{id}` - Mở khóa tài khoản
- **GET** `/api/AdminAccount/GetAccountsByRole/{role}` - Lấy admin theo role

### 👤 Account Management  
- **GET** `/api/Account/GetPaging` - Lấy accounts có phân trang
- **GET** `/api/Account/GetAccountByUsername` - Lấy account theo username
- **POST** `/api/Account/ChangePassword` - Đổi mật khẩu
- **POST** `/api/Account/LockAccount` - Khóa account
- **POST** `/api/Account/UnlockAccount` - Mở khóa account
- **POST** `/api/Account/ActivateAccount` - Kích hoạt account
- **POST** `/api/Account/DeactivateAccount` - Vô hiệu hóa account
- **GET** `/api/Account/GetAccountsByLevel` - Lấy accounts theo level
- **GET** `/api/Account/GetExpiredAccounts` - Lấy accounts đã hết hạn
- **GET** `/api/Account/GetExpiringAccounts` - Lấy accounts sắp hết hạn
- **POST** `/api/Account/ExtendAccount` - Gia hạn account

### 📄 License Management
- **GET** `/api/License/GetPaging` - Lấy licenses có phân trang
- **GET** `/api/License/GetById/{id}` - Lấy license theo ID
- **GET** `/api/License/GetLicensesByAccountId` - Lấy licenses theo Account ID
- **GET** `/api/License/GetLicenseByKey` - Lấy license theo key
- **POST** `/api/License/SubmitData` - Tạo/Cập nhật license
- **POST** `/api/License/AssignLicenseToAccount` - Gán license cho account (1-1)
- **POST** `/api/License/AssignLicenseToUser` - Gán license cho user (Many-Many)
- **POST** `/api/License/UnassignLicenseFromAccount/{id}` - Hủy gán license khỏi account
- **DELETE** `/api/License/UnassignLicenseFromUser/{id}` - Hủy gán license khỏi user
- **GET** `/api/License/GetUsersAssignedToLicense/{id}` - Lấy users được gán license
- **POST** `/api/License/RenewLicense` - Gia hạn license
- **POST** `/api/License/SuspendLicense` - Tạm dừng license
- **POST** `/api/License/ActivateLicense` - Kích hoạt license
- **GET** `/api/License/GetExpiredLicenses` - Lấy licenses đã hết hạn
- **GET** `/api/License/GetExpiringLicenses` - Lấy licenses sắp hết hạn
- **POST** `/api/License/ExtendLicense` - Gia hạn license đến ngày cụ thể

### ⚙️ Feature Management
- **GET** `/api/FeatureManagement/GetPaging` - Lấy features có phân trang
- **GET** `/api/FeatureManagement/GetById/{id}` - Lấy feature theo ID
- **GET** `/api/FeatureManagement/GetFeatureByCode` - Lấy feature theo code
- **GET** `/api/FeatureManagement/GetFeaturesByCategory` - Lấy features theo category
- **GET** `/api/FeatureManagement/GetActiveFeatures` - Lấy features đang hoạt động
- **POST** `/api/FeatureManagement/SubmitData` - Tạo/Cập nhật feature
- **DELETE** `/api/FeatureManagement/Delete/{id}` - Xóa feature
- **POST** `/api/FeatureManagement/ToggleFeatureStatus/{id}` - Bật/tắt feature
- **GET** `/api/FeatureManagement/GetFeatureUsage` - Lấy thống kê sử dụng feature
- **POST** `/api/FeatureManagement/RecordUsage` - Ghi nhận việc sử dụng feature

### 🔒 Role Management
- **GET** `/api/Role/GetPaging` - Lấy roles có phân trang
- **GET** `/api/Role/GetById/{id}` - Lấy role theo ID
- **POST** `/api/Role/SubmitData` - Tạo/Cập nhật role
- **DELETE** `/api/Role/Delete/{id}` - Xóa role

## 🎯 Workflow Examples

### Example 1: Tạo và gán license cho user
```bash
# 1. Login
TOKEN=$(curl -s 'https://localhost:7000/api/Auth/Login' \
  -H 'Content-Type: application/json' \
  -d '{"username":"admin","password":"123456"}' | jq -r '.data.token')

# 2. Tạo license mới
curl -X POST 'https://localhost:7000/api/License/SubmitData' \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "licenseKey": "LIC-2024-TEST",
    "licenseName": "Test License",
    "licenseType": 1,
    "maxDevices": 5,
    "maxUsers": 10,
    "startDate": "2024-01-01T00:00:00.000Z",
    "expiryDate": "2024-12-31T23:59:59.000Z",
    "price": 199.99,
    "state": 1
  }'

# 3. Gán license cho user
curl -X POST 'https://localhost:7000/api/License/AssignLicenseToUser' \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "licenseId": 1,
    "accountId": 1,
    "notes": "Test assignment"
  }'
```

### Example 2: Quản lý features
```bash
# 1. Lấy danh sách features
curl -X GET 'https://localhost:7000/api/FeatureManagement/GetActiveFeatures' \
  -H "Authorization: Bearer $TOKEN"

# 2. Tạo feature mới
curl -X POST 'https://localhost:7000/api/FeatureManagement/SubmitData' \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "code": "NEW_FEATURE",
    "name": "Tính năng mới",
    "category": "Advanced",
    "isActive": true,
    "state": 1
  }'

# 3. Ghi nhận việc sử dụng feature
curl -X POST 'https://localhost:7000/api/FeatureManagement/RecordUsage' \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "userId": 1,
    "featureId": 1,
    "usageType": "API_Call",
    "resourceConsumed": 1
  }'
```

## 📊 Entity State Values
Khi tạo/cập nhật entities, sử dụng `state` field:
- **1** = Add (Tạo mới)
- **2** = Edit (Cập nhật)
- **3** = Remove (Xóa)

## 🔧 Response Format
Tất cả API responses đều follow format:
```json
{
  "success": true,
  "message": "Thông điệp",
  "data": {
    // Dữ liệu response
  }
}
```

## ⚠️ Common Error Codes
- **400** - Bad Request (Dữ liệu không hợp lệ)
- **401** - Unauthorized (Chưa đăng nhập hoặc token hết hạn)
- **403** - Forbidden (Không có quyền truy cập)
- **404** - Not Found (Không tìm thấy resource)
- **500** - Internal Server Error (Lỗi server)

## 🛠️ Development Setup

### Prerequisites
- .NET 8 SDK
- SQL Server
- Visual Studio hoặc Visual Studio Code

### Chạy project
```bash
# Clone repository
git clone https://github.com/nthe12041999/AutoAppManagement

# Restore packages
dotnet restore

# Update database
dotnet ef database update --project AutoAppManagement.API

# Run API
dotnet run --project AutoAppManagement.API

# Run Web App  
dotnet run --project AutoAppManagement
```

### Database Seeding
```bash
# Chạy script để tạo dữ liệu features
sqlcmd -S "your_server" -d "AutoAppManagement" -i "insert_features_data.sql"
```

## 📝 Notes
- Tất cả timestamps đều sử dụng UTC format
- License keys phải unique
- Feature codes phải unique
- Mặc định port: **7000** cho API, **5000** cho WebApp
- Sử dụng HTTPS cho production

## 🤝 Contributing
1. Fork repository
2. Tạo feature branch
3. Commit changes
4. Push to branch
5. Tạo Pull Request

## 📄 License
This project is licensed under the MIT License.