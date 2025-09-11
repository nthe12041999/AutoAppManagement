# AutoAppManagement API - cURL Commands
# Tập hợp các lệnh cURL để test tất cả API endpoints

# =================================
# 🔐 AUTHENTICATION APIs  
# =================================

# Admin Login - Lấy access token
curl --location 'https://localhost:7000/api/Auth/Login' \
--header 'Content-Type: application/json' \
--data '{
    "username": "admin",
    "password": "123456"
}'

# User Login
curl --location 'https://localhost:7000/api/Account/Login' \
--header 'Content-Type: application/json' \
--data '{
    "emailOrPhone": "user@example.com",
    "password": "123456"
}'

# =================================
# 👥 ADMIN ACCOUNT MANAGEMENT APIs
# =================================

# Get All Admin Accounts
curl --location 'https://localhost:7000/api/AdminAccount/GetAll' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Admin Account By ID
curl --location 'https://localhost:7000/api/AdminAccount/GetById/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Create Admin Account
curl --location 'https://localhost:7000/api/AdminAccount/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "fullName": "Nguyễn Admin",
    "email": "admin@example.com",
    "phoneNumber": "0123456789",
    "userName": "admin_new",
    "passwordHash": "hashed_password",
    "role": "admin",
    "isActive": true,
    "state": 1
}'

# Update Admin Account
curl --location 'https://localhost:7000/api/AdminAccount/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "fullName": "Nguyễn Admin Updated",
    "email": "admin_updated@example.com",
    "phoneNumber": "0987654321",
    "userName": "admin_updated",
    "role": "admin",
    "isActive": true,
    "state": 2
}'

# Delete Admin Account
curl --location --request DELETE 'https://localhost:7000/api/AdminAccount/Delete/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Change Admin Password
curl --location 'https://localhost:7000/api/AdminAccount/ChangePassword' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "newPassword": "new_password_123"
}'

# Lock Admin Account
curl --location 'https://localhost:7000/api/AdminAccount/LockAccount' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "minutes": 30,
    "reason": "Vi phạm quy định"
}'

# Unlock Admin Account
curl --location --request POST 'https://localhost:7000/api/AdminAccount/UnlockAccount/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Admin Login
curl --location 'https://localhost:7000/api/AdminAccount/Login' \
--header 'Content-Type: application/json' \
--data '{
    "username": "admin",
    "password": "123456"
}'

# Get Accounts By Role
curl --location 'https://localhost:7000/api/AdminAccount/GetAccountsByRole/admin' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# =================================
# 👤 ACCOUNT MANAGEMENT APIs
# =================================

# Get All Accounts (Paging)
curl --location 'https://localhost:7000/api/Account/GetPaging' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "pageIndex": 1,
    "pageSize": 10,
    "filter": "",
    "sort": "Id"
}'

# Get Account By Username
curl --location 'https://localhost:7000/api/Account/GetAccountByUsername?username=testuser' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Change Account Password
curl --location 'https://localhost:7000/api/Account/ChangePassword' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "newPassword": "new_password_123"
}'

# Lock Account
curl --location 'https://localhost:7000/api/Account/LockAccount' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "reason": "Vi phạm điều khoản sử dụng"
}'

# Unlock Account
curl --location --request POST 'https://localhost:7000/api/Account/UnlockAccount?id=1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Activate Account
curl --location --request POST 'https://localhost:7000/api/Account/ActivateAccount?id=1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Deactivate Account
curl --location --request POST 'https://localhost:7000/api/Account/DeactivateAccount?id=1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Accounts By Level
curl --location 'https://localhost:7000/api/Account/GetAccountsByLevel?level=1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Expired Accounts
curl --location 'https://localhost:7000/api/Account/GetExpiredAccounts' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Expiring Accounts
curl --location 'https://localhost:7000/api/Account/GetExpiringAccounts?days=30' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Extend Account
curl --location 'https://localhost:7000/api/Account/ExtendAccount' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "newExpiryDate": "2025-12-31T23:59:59.000Z"
}'

# =================================
# 📄 LICENSE MANAGEMENT APIs
# =================================

# Get All Licenses (Paging)
curl --location 'https://localhost:7000/api/License/GetPaging' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "pageIndex": 1,
    "pageSize": 10,
    "filter": "",
    "sort": "Id"
}'

# Get License By ID
curl --location 'https://localhost:7000/api/License/GetById/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Licenses By Account ID
curl --location 'https://localhost:7000/api/License/GetLicensesByAccountId?accountId=1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get License By Key
curl --location 'https://localhost:7000/api/License/GetLicenseByKey?licenseKey=LIC-2024-BASIC' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Create License
curl --location 'https://localhost:7000/api/License/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "licenseKey": "LIC-2024-NEW",
    "licenseName": "Pro License",
    "licenseType": 1,
    "description": "Professional license với tất cả tính năng",
    "maxDevices": 5,
    "maxUsers": 10,
    "startDate": "2024-01-01T00:00:00.000Z",
    "expiryDate": "2024-12-31T23:59:59.000Z",
    "price": 299.99,
    "currency": "VND",
    "features": "[\"SEND_MESSAGE\",\"BULK_SEND_MESSAGE\",\"AI_MESSAGE\"]",
    "featureLimits": "{\"AI_MESSAGE\": {\"daily\": 100, \"monthly\": 3000}}",
    "state": 1
}'

# Update License
curl --location 'https://localhost:7000/api/License/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "licenseKey": "LIC-2024-UPDATED",
    "licenseName": "Pro License Updated",
    "licenseType": 2,
    "description": "Professional license updated",
    "maxDevices": 10,
    "maxUsers": 20,
    "startDate": "2024-01-01T00:00:00.000Z",
    "expiryDate": "2025-12-31T23:59:59.000Z",
    "price": 399.99,
    "currency": "VND",
    "state": 2
}'

# 🔗 Assign License To Account (1-1 relationship)
curl --location 'https://localhost:7000/api/License/AssignLicenseToAccount' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "licenseId": 1,
    "accountId": 2,
    "notes": "Gán license cho account test"
}'

# 🔗 Assign License To User (Many-Many relationship)
curl --location 'https://localhost:7000/api/License/AssignLicenseToUser' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "licenseId": 1,
    "accountId": 2,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T23:59:59Z",
    "notes": "Gán license cho user test"
}'

# ❌ Unassign License From Account
curl --location --request POST 'https://localhost:7000/api/License/UnassignLicenseFromAccount/2' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# ❌ Unassign License From User
curl --location --request DELETE 'https://localhost:7000/api/License/UnassignLicenseFromUser/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# 👥 Get Users Assigned To License
curl --location 'https://localhost:7000/api/License/GetUsersAssignedToLicense/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Renew License
curl --location 'https://localhost:7000/api/License/RenewLicense' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "licenseId": 1,
    "newExpiryDate": "2025-12-31T23:59:59.000Z",
    "reason": "Gia hạn license theo yêu cầu"
}'

# Suspend License
curl --location --request POST 'https://localhost:7000/api/License/SuspendLicense?id=1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Activate License
curl --location --request POST 'https://localhost:7000/api/License/ActivateLicense?id=1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Expired Licenses
curl --location 'https://localhost:7000/api/License/GetExpiredLicenses' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Expiring Licenses
curl --location 'https://localhost:7000/api/License/GetExpiringLicenses?days=30' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Extend License
curl --location --request POST 'https://localhost:7000/api/License/ExtendLicense?id=1&newExpiryDate=2025-12-31T23:59:59.000Z' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# =================================
# ⚙️ FEATURE MANAGEMENT APIs
# =================================

# Get All Features (Paging)
curl --location 'https://localhost:7000/api/FeatureManagement/GetPaging' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "pageIndex": 1,
    "pageSize": 10,
    "filter": "",
    "sort": "PriorityOrder"
}'

# Get Feature By ID
curl --location 'https://localhost:7000/api/FeatureManagement/GetById/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Feature By Code
curl --location 'https://localhost:7000/api/FeatureManagement/GetFeatureByCode?code=SEND_MESSAGE' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Features By Category
curl --location 'https://localhost:7000/api/FeatureManagement/GetFeaturesByCategory?category=Basic' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Active Features
curl --location 'https://localhost:7000/api/FeatureManagement/GetActiveFeatures' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Create Feature
curl --location 'https://localhost:7000/api/FeatureManagement/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "code": "NEW_FEATURE",
    "name": "Tính năng mới",
    "description": "Mô tả tính năng mới",
    "category": "Advanced",
    "icon": "star",
    "isActive": true,
    "isBeta": true,
    "priorityOrder": 50,
    "resourceType": "Count",
    "defaultLimit": 100,
    "state": 1
}'

# Update Feature
curl --location 'https://localhost:7000/api/FeatureManagement/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "code": "UPDATED_FEATURE",
    "name": "Tính năng đã cập nhật",
    "description": "Mô tả đã được cập nhật",
    "category": "Premium",
    "icon": "diamond",
    "isActive": true,
    "isBeta": false,
    "priorityOrder": 60,
    "resourceType": "Token",
    "defaultLimit": 200,
    "state": 2
}'

# Delete Feature
curl --location --request DELETE 'https://localhost:7000/api/FeatureManagement/Delete/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Toggle Feature Status
curl --location --request POST 'https://localhost:7000/api/FeatureManagement/ToggleFeatureStatus/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Get Feature Usage
curl --location 'https://localhost:7000/api/FeatureManagement/GetFeatureUsage?featureId=1&userId=1&fromDate=2024-01-01&toDate=2024-12-31' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Record Feature Usage
curl --location 'https://localhost:7000/api/FeatureManagement/RecordUsage' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "userId": 1,
    "featureId": 1,
    "usageType": "API_Call",
    "resourceConsumed": 1,
    "metadata": "{\"ipAddress\": \"192.168.1.1\", \"userAgent\": \"Test\"}"
}'

# =================================
# 🔒 ROLE MANAGEMENT APIs
# =================================

# Get All Roles (Paging)
curl --location 'https://localhost:7000/api/Role/GetPaging' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "pageIndex": 1,
    "pageSize": 10,
    "filter": "",
    "sort": "Id"
}'

# Get Role By ID
curl --location 'https://localhost:7000/api/Role/GetById/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# Create Role
curl --location 'https://localhost:7000/api/Role/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "roleName": "Manager",
    "roleDescription": "Manager role với quyền quản lý",
    "isActive": true,
    "state": 1
}'

# Update Role
curl --location 'https://localhost:7000/api/Role/SubmitData' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE' \
--data '{
    "id": 1,
    "roleName": "Senior Manager",
    "roleDescription": "Senior Manager với quyền cao hơn",
    "isActive": true,
    "state": 2
}'

# Delete Role
curl --location --request DELETE 'https://localhost:7000/api/Role/Delete/1' \
--header 'Authorization: Bearer YOUR_TOKEN_HERE'

# =================================
# 📝 USAGE EXAMPLES
# =================================

# Example 1: Complete workflow - Login -> Create License -> Assign to User
echo "=== COMPLETE WORKFLOW EXAMPLE ==="

# Step 1: Login to get token
echo "Step 1: Login..."
TOKEN=$(curl -s --location 'https://localhost:7000/api/Auth/Login' \
--header 'Content-Type: application/json' \
--data '{
    "username": "admin",
    "password": "123456"
}' | jq -r '.data.token')

echo "Token: $TOKEN"

# Step 2: Create a new license
echo "Step 2: Creating license..."
curl --location 'https://localhost:7000/api/License/SubmitData' \
--header 'Content-Type: application/json' \
--header "Authorization: Bearer $TOKEN" \
--data '{
    "licenseKey": "LIC-2024-EXAMPLE",
    "licenseName": "Example License",
    "licenseType": 1,
    "description": "Example license for testing",
    "maxDevices": 3,
    "maxUsers": 5,
    "startDate": "2024-01-01T00:00:00.000Z",
    "expiryDate": "2024-12-31T23:59:59.000Z",
    "price": 199.99,
    "currency": "VND",
    "features": "[\"SEND_MESSAGE\",\"ADD_FRIEND\"]",
    "state": 1
}'

# Step 3: Assign license to user
echo "Step 3: Assigning license to user..."
curl --location 'https://localhost:7000/api/License/AssignLicenseToUser' \
--header 'Content-Type: application/json' \
--header "Authorization: Bearer $TOKEN" \
--data '{
    "licenseId": 1,
    "accountId": 1,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T23:59:59Z",
    "notes": "License assignment for testing"
}'

echo "=== WORKFLOW COMPLETED ==="

# =================================
# 🚨 TESTING NOTES
# =================================

# 1. Thay thế YOUR_TOKEN_HERE bằng token thực từ API login
# 2. Đảm bảo server đang chạy trên https://localhost:7000
# 3. Một số API cần quyền Admin hoặc Customer
# 4. Kiểm tra database có dữ liệu test chưa
# 5. Sử dụng jq để parse JSON response (cài đặt: apt install jq hoặc brew install jq)

# Example: Lấy token và lưu vào variable
# TOKEN=$(curl -s 'https://localhost:7000/api/Auth/Login' -H 'Content-Type: application/json' -d '{"username":"admin","password":"123456"}' | jq -r '.data.token')

# Example: Sử dụng token trong request tiếp theo
# curl -H "Authorization: Bearer $TOKEN" 'https://localhost:7000/api/License/GetAll'