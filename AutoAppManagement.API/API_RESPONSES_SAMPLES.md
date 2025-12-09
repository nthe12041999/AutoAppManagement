# API Response Samples - AutoAppManagement API

## Response Format

Tất cả API đều trả về format chuẩn:

```json
{
    "IsSuccess": true,
    "Message": "Thông báo",
    "Data": { /* Dữ liệu trả về */ }
}
```

---

## Account Controller

### 1. Login
**POST** `/api/Account/Login`

**Request:**
```json
{
    "EmailOrPhone": "customer@example.com",
    "Password": "password123",
    "DeviceId": "device-123",
    "Fingerprint": "fp-123"
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Đăng nhập thành công",
    "Data": {
        "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "LoginTime": "2024-01-15T10:30:00",
        "TokenExpiry": "2024-01-16T10:30:00",
        "LicenseInfo": {
            "LicenseId": 1,
            "LicenseKey": "LICENSE-KEY-123",
            "LicenseName": "Premium",
            "LicenseType": "Premium",
            "StartDate": "2024-01-01T00:00:00",
            "EndDate": "2024-12-31T23:59:59",
            "Status": 1,
            "DaysRemaining": 350,
            "WarningMessage": ""
        },
        "AvailableResources": [
            {
                "FeatureId": 1,
                "FeatureName": "AI Chat",
                "FeatureCode": "AI_CHAT",
                "ToolName": "AI Assistant",
                "Description": "Chat với AI",
                "IsEnabled": true,
                "UsageLimit": 100,
                "UsedCount": 25,
                "RemainingCount": 75,
                "PeriodStart": "2024-01-01T00:00:00",
                "PeriodEnd": "2024-12-31T23:59:59",
                "LimitType": "monthly",
                "Status": "available",
                "WarningMessage": ""
            }
        ],
        "AllowedFeatures": ["AI_CHAT", "EXPORT_DATA"],
        "RefreshToken": "refresh-token-string",
        "RefreshTokenExpired": "2024-02-15T10:30:00"
    }
}
```

### 2. GetPaging
**POST** `/api/Account/GetPaging`

**Request:**
```json
{
    "PageIndex": 1,
    "PageSize": 10,
    "Filter": "",
    "Sort": "Id",
    "RequestedColumns": ["Name", "Email", "Phone", "LicenseName", "Status", "CreatedDate"]
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "Data": [
            {
                "ID": 1,
                "Name": "Nguyễn Văn A",
                "Email": "customer@example.com",
                "Phone": "0123456789",
                "LicenseName": "Premium",
                "Status": 1,
                "StatusName": "Hoạt động",
                "CreatedDate": "2024-01-01T00:00:00",
                "State": 0
            }
        ],
        "PageIndex": 1,
        "PageSize": 10,
        "TotalItems": 1247,
        "TotalPages": 125
    }
}
```

### 3. GetById
**GET** `/api/Account/GetById/1`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "ID": 1,
        "Name": "Nguyễn Văn A",
        "Email": "customer@example.com",
        "Phone": "0123456789",
        "LicenseId": 1,
        "LicenseName": "Premium",
        "Status": 1,
        "StatusName": "Hoạt động",
        "CreatedDate": "2024-01-01T00:00:00",
        "ExpiryDate": "2024-12-31T23:59:59",
        "State": 0
    }
}
```

### 4. RefreshToken
**POST** `/api/Account/RefreshToken`

**Request:**
```json
"refresh-token-string-here"
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Token đã được làm mới",
    "Data": {
        "AccessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "AccessTokenExpired": "2024-01-16T10:30:00",
        "RefreshToken": "new-refresh-token",
        "RefreshTokenExpired": "2024-02-15T10:30:00"
    }
}
```

### 5. GetCustomerAccountStatistics
**GET** `/api/Account/GetCustomerAccountStatistics`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "TotalCustomers": 1247,
        "ActiveCustomers": 986,
        "PremiumCustomers": 450,
        "LockedCustomers": 12,
        "ExpiredCustomers": 89,
        "NewCustomersThisMonth": 45
    }
}
```

### 6. ChangePasswordWithOtp
**POST** `/api/Account/ChangePasswordWithOtp`

**Request:**
```json
{
    "AccountId": 1,
    "OldPassword": "oldPassword123",
    "NewPassword": "newPassword123",
    "Otp": "123456"
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Đổi mật khẩu thành công",
    "Data": true
}
```

### 7. ForgotPassword
**POST** `/api/Account/ForgotPassword`

**Request:**
```json
{
    "EmailOrPhone": "customer@example.com"
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Mã OTP đã được gửi đến email của bạn",
    "Data": true
}
```

---

## AdminAccount Controller

### 1. Login
**POST** `/api/AdminAccount/Login`

**Request:**
```json
{
    "Username": "admin",
    "Password": "admin123"
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Đăng nhập thành công",
    "Data": {
        "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "LoginTime": "2024-01-15T10:30:00",
        "TokenExpiry": "2024-01-16T10:30:00",
        "RefreshToken": "refresh-token-string",
        "RefreshTokenExpired": "2024-02-15T10:30:00"
    }
}
```

### 2. GetPaging
**POST** `/api/AdminAccount/GetPaging`

**Request:**
```json
{
    "PageIndex": 1,
    "PageSize": 10,
    "Filter": "",
    "Sort": "Id",
    "RequestedColumns": ["FullName", "Email", "Phone", "Role", "Status"]
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "Data": [
            {
                "ID": 1,
                "FullName": "Admin User",
                "Email": "admin@example.com",
                "Phone": "0123456789",
                "Role": "admin",
                "Status": 1,
                "CreatedDate": "2024-01-01T00:00:00"
            }
        ],
        "PageIndex": 1,
        "PageSize": 10,
        "TotalItems": 45,
        "TotalPages": 5
    }
}
```

### 3. GetProfile
**GET** `/api/AdminAccount/profile`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Lấy thông tin thành công",
    "Data": {
        "ID": 1,
        "FullName": "Admin User",
        "Email": "admin@example.com",
        "Phone": "0123456789",
        "Role": "admin",
        "Status": 1
    }
}
```

---

## License Controller

### 1. GetPaging
**POST** `/api/License/GetPaging`

**Request:**
```json
{
    "PageIndex": 1,
    "PageSize": 10,
    "Filter": "",
    "Sort": "Id",
    "RequestedColumns": ["LicenseKey", "LicenseName", "LicenseType", "StartDate", "EndDate", "Status"]
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "Data": [
            {
                "ID": 1,
                "LicenseKey": "LICENSE-KEY-123",
                "LicenseName": "Premium",
                "LicenseType": "Premium",
                "StartDate": "2024-01-01T00:00:00",
                "EndDate": "2024-12-31T23:59:59",
                "Status": 1,
                "State": 0
            }
        ],
        "PageIndex": 1,
        "PageSize": 10,
        "TotalItems": 50,
        "TotalPages": 5
    }
}
```

### 2. GetStatistics
**GET** `/api/License/statistics`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "TotalLicenses": 50,
        "ActiveLicenses": 45,
        "ExpiredLicenses": 3,
        "SuspendedLicenses": 2,
        "PremiumLicenses": 20,
        "BasicLicenses": 25,
        "EnterpriseLicenses": 5
    }
}
```

### 3. GetLicensesByAccountId
**GET** `/api/License/GetLicensesByAccountId?accountId=1`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": [
        {
            "ID": 1,
            "LicenseKey": "LICENSE-KEY-123",
            "LicenseName": "Premium",
            "LicenseType": "Premium",
            "StartDate": "2024-01-01T00:00:00",
            "EndDate": "2024-12-31T23:59:59",
            "Status": 1
        }
    ]
}
```

---

## Role Controller

### 1. GetPaging
**POST** `/api/Role/GetPaging`

**Request:**
```json
{
    "PageIndex": 1,
    "PageSize": 10,
    "Filter": "",
    "Sort": "Id",
    "RequestedColumns": ["RoleName", "Description", "Status"]
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "Data": [
            {
                "ID": 1,
                "RoleName": "Customer",
                "Description": "Khách hàng",
                "Status": 1,
                "State": 0
            }
        ],
        "PageIndex": 1,
        "PageSize": 10,
        "TotalItems": 10,
        "TotalPages": 1
    }
}
```

### 2. GetRolesByAccountId
**GET** `/api/Role/GetRolesByAccountId/1`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": [
        {
            "ID": 1,
            "RoleName": "Customer",
            "Description": "Khách hàng",
            "Status": 1
        }
    ]
}
```

### 3. SubmitData (Create/Update)
**POST** `/api/Role/SubmitData`

**Request (Create):**
```json
{
    "Id": 0,
    "State": 1,
    "RoleName": "Customer",
    "Description": "Khách hàng",
    "Status": 1
}
```

**Request (Update):**
```json
{
    "Id": 1,
    "State": 2,
    "RoleName": "Customer Updated",
    "Description": "Khách hàng đã cập nhật",
    "Status": 1
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Lưu thành công",
    "Data": {
        "ID": 1,
        "RoleName": "Customer",
        "Description": "Khách hàng",
        "Status": 1
    }
}
```

---

## Permission Controller

### 1. GetAllPermissions
**GET** `/api/Permission/GetAllPermissions`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": [
        {
            "ID": 1,
            "Resource": "Account",
            "Action": "View",
            "Description": "Xem tài khoản",
            "Category": "Account"
        },
        {
            "ID": 2,
            "Resource": "Account",
            "Action": "Edit",
            "Description": "Chỉnh sửa tài khoản",
            "Category": "Account"
        }
    ]
}
```

### 2. GetRolePermissions
**GET** `/api/Permission/GetRolePermissions/1`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": [
        {
            "PermissionId": 1,
            "Resource": "Account",
            "Action": "View",
            "Scope": "own",
            "Priority": 1
        }
    ]
}
```

### 3. AssignPermissionToRole
**POST** `/api/Permission/AssignPermissionToRole`

**Request:**
```json
{
    "RoleId": 1,
    "PermissionId": 1,
    "ScopeDefault": "own",
    "Priority": 1
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Gán permission thành công",
    "Data": true
}
```

---

## FeatureManagement Controller

### 1. GetMyFeatures
**GET** `/api/FeatureManagement/my-features`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "userId": 1,
        "allowedFeatures": [
            {
                "FeatureId": 1,
                "FeatureCode": "AI_CHAT",
                "FeatureName": "AI Chat",
                "IsEnabled": true,
                "UsageLimit": 100,
                "UsedCount": 25,
                "RemainingCount": 75
            }
        ],
        "totalFeatures": 1
    }
}
```

### 2. CheckFeature
**GET** `/api/FeatureManagement/check-feature/1`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "userId": 1,
        "featureId": 1,
        "isAllowed": true,
        "timestamp": "2024-01-15T10:30:00"
    }
}
```

### 3. RecordFeatureUsage
**POST** `/api/FeatureManagement/record-usage`

**Request:**
```json
{
    "FeatureId": 1,
    "ResourceAmount": 1,
    "UsageType": "Access"
}
```

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "Ghi nhận sử dụng tính năng thành công",
    "Data": true
}
```

---

## ToolVersion Controller

### 1. GetCurrentVersion
**GET** `/api/ToolVersion/current/1`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": {
        "ID": 1,
        "ToolCode": 1,
        "Version": "1.0.0",
        "ReleaseDate": "2024-01-01T00:00:00",
        "IsActive": true,
        "DownloadUrl": "https://example.com/download/tool-v1.0.0.exe",
        "ReleaseNotes": "Initial release"
    }
}
```

### 2. GetVersionHistory
**GET** `/api/ToolVersion/history/1?limit=10`

**Response (200 OK):**
```json
{
    "IsSuccess": true,
    "Message": "",
    "Data": [
        {
            "ID": 1,
            "ToolCode": 1,
            "Version": "1.0.0",
            "ReleaseDate": "2024-01-01T00:00:00",
            "IsActive": true
        },
        {
            "ID": 2,
            "ToolCode": 1,
            "Version": "0.9.0",
            "ReleaseDate": "2023-12-01T00:00:00",
            "IsActive": false
        }
    ]
}
```

---

## Error Responses

### 400 Bad Request
```json
{
    "IsSuccess": false,
    "Message": "Dữ liệu không hợp lệ",
    "Data": null
}
```

### 401 Unauthorized
```json
{
    "IsSuccess": false,
    "Message": "Token không hợp lệ hoặc đã hết hạn",
    "Data": null
}
```

### 403 Forbidden
```json
{
    "IsSuccess": false,
    "Message": "Bạn không có quyền thực hiện thao tác này",
    "Data": null
}
```

### 404 Not Found
```json
{
    "IsSuccess": false,
    "Message": "Không tìm thấy dữ liệu",
    "Data": null
}
```

### 500 Internal Server Error
```json
{
    "IsSuccess": false,
    "Message": "Đã có lỗi xảy ra",
    "Data": null
}
```

---

## Authentication

Tất cả API (trừ Login và các API có `[AllowAnonymous]`) đều yêu cầu Bearer Token:

**Header:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Notes

1. **Base URL**: `https://localhost:5001/api` (Development) hoặc production URL
2. **Content-Type**: `application/json` cho POST/PUT requests
3. **Paging**: Tất cả GetPaging endpoints đều dùng POST với body chứa PagingRequestDTO
4. **Filter**: Có thể là JSON string của FilterCondition array hoặc simple search string
5. **RequestedColumns**: Danh sách các columns cần hiển thị, backend sẽ join bảng tương ứng











