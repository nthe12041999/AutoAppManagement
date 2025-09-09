# AutoAppManagement API - Updated Postman Collection

## Thông tin Collection mới nhất 🚀

Collection này đã được cập nhật với những thay đổi mới nhất của dự án AutoAppManagement:

### ✨ Những thay đổi chính:

1. **🔐 Enhanced Login API**: 
   - API Login giờ đây trả về đầy đủ thông tin tool permissions
   - Bao gồm thông tin tài khoản, license và các resource có sẵn
   - Tự động lưu token vào collection variables

2. **🔧 Unified Tool Management**:
   - Đã merge ToolSimpleController vào ToolController
   - Tất cả API tool management giờ được tập trung tại `/api/Tools`
   - Bổ sung các endpoint cho Tool Versions và Tool Categories

3. **🎯 Complete Tool Feature Management**:
   - Quản lý Tool Features với đầy đủ CRUD operations
   - License Feature assignment và tracking
   - Feature Access Control với usage monitoring

## 📋 Files trong package:

1. **AutoAppManagement_API_Updated_Postman_Collection.json** - Collection chính
2. **AutoAppManagement_Updated_Environment.postman_environment.json** - Environment variables
3. **README.md** - Hướng dẫn sử dụng (file này)

## 🚀 Cách sử dụng:

### Bước 1: Import vào Postman
1. Mở Postman
2. Click **Import**
3. Chọn file `AutoAppManagement_API_Updated_Postman_Collection.json`
4. Import environment từ file `AutoAppManagement_Updated_Environment.postman_environment.json`

### Bước 2: Cấu hình Environment
1. Chọn environment "AutoAppManagement Updated Environment"
2. Cập nhật các giá trị cần thiết:
   ```
   baseUrl: https://localhost:7000
   testEmail: customer@email.com (thay bằng email test của bạn)
   testPassword: password123 (thay bằng password test của bạn)
   ```

### Bước 3: Authenticate
1. Vào folder **🔐 Authentication**
2. Chạy request **Customer Login (Updated with Tool Permissions)**
3. Token sẽ tự động được lưu vào collection variables

### Bước 4: Test các API
Các folder chính để test:

- **👤 Account Management** - Quản lý tài khoản
- **🔧 Tool Management (Updated)** - Quản lý tools, versions, categories
- **🎯 Tool Features & Access** - Quản lý features và access control
- **🎫 License Management** - Quản lý license
- **📊 Admin Management** - Quản lý admin accounts

## 🔑 Authentication

Collection hỗ trợ 2 loại authentication:

### 1. Customer Authentication
```
POST /api/Account/Login
```
**Response mới** (đã enhanced):
```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJ...",
    "account": {
      "id": 1,
      "userName": "customer",
      "email": "customer@email.com"
    },
    "licenseInfo": {
      "licenseKey": "LIC-2024-001",
      "expiryDate": "2024-12-31T23:59:59Z"
    },
    "availableResources": [
      {
        "featureCode": "AI_TEXT_GEN",
        "featureName": "AI Text Generation",
        "isEnabled": true,
        "usageQuota": {...}
      }
    ]
  }
}
```

### 2. Admin Authentication
```
POST /api/AdminAccount/Login
```

## 🔧 Tool Management APIs

### Tools Controller (Updated)
Tất cả API tools đã được consolidate vào `/api/Tools`:

#### Basic Operations:
- `GET /api/Tools` - Lấy tất cả tools
- `GET /api/Tools/{id}` - Lấy tool theo ID
- `GET /api/Tools/by-code/{code}` - Lấy tool theo code
- `POST /api/Tools` - Tạo tool mới
- `PUT /api/Tools/{id}` - Cập nhật tool
- `DELETE /api/Tools/{id}` - Xóa tool

#### Advanced Operations:
- `GET /api/Tools/list?pageIndex=0&pageSize=10` - Paging tools
- `GET /api/Tools/by-category/{category}` - Tools theo category
- `GET /api/Tools/by-type/{type}` - Tools theo type
- `GET /api/Tools/public` - Public tools
- `POST /api/Tools/search` - Tìm kiếm tools
- `GET /api/Tools/statistics` - Thống kê tools
- `GET /api/Tools/{id}/versions` - Lấy versions của tool

### Tool Versions Management:
- `GET /api/ToolVersions` - Tất cả versions
- `GET /api/ToolVersions/tool/{toolId}` - Versions theo tool
- `GET /api/ToolVersions/tool/{toolId}/latest` - Version mới nhất
- `GET /api/ToolVersions/tool/{toolId}/stable` - Stable versions
- `GET /api/ToolVersions/compare/{v1}/{v2}` - So sánh versions

### Tool Categories Management:
- `GET /api/ToolCategories` - Tất cả categories
- `GET /api/ToolCategories/root` - Root categories
- `GET /api/ToolCategories/{id}/sub-categories` - Sub categories
- `GET /api/ToolCategories/active` - Active categories
- `GET /api/ToolCategories/statistics` - Thống kê categories

## 🎯 Feature Access Control

### Tool Features:
- `GET /api/ToolFeature/GetAll` - Tất cả features
- `GET /api/ToolFeature/GetByCode/{code}` - Feature theo code
- `GET /api/ToolFeature/GetByCategory/{category}` - Features theo category
- `POST /api/ToolFeature/Create` - Tạo feature mới

### License Features:
- `POST /api/LicenseFeature/AssignFeature` - Assign feature cho license
- `GET /api/LicenseFeature/GetFeaturesByLicense/{licenseId}` - Features của license
- `GET /api/LicenseFeature/CheckFeatureEnabled` - Kiểm tra feature enabled

### Feature Access Control:
- `POST /api/FeatureAccess/CheckAccess` - Kiểm tra quyền truy cập
- `POST /api/FeatureAccess/RecordUsage` - Ghi nhận usage
- `POST /api/FeatureAccess/GetUsageReport` - Báo cáo usage
- `GET /api/FeatureAccess/CheckUsageLimits` - Kiểm tra limits

## 🧪 Testing Workflow

### 1. Authentication Flow:
```
1. Login → Get token
2. Token tự động được set cho các requests tiếp theo
3. Test các protected endpoints
```

### 2. Tool Management Flow:
```
1. Get all tools → List existing tools
2. Create new tool → Add tool
3. Get tool by ID → Verify creation
4. Update tool → Modify tool
5. Get tool versions → Check versions
6. Search tools → Test search functionality
```

### 3. Feature Access Flow:
```
1. Get account features → List available features
2. Check feature access → Verify permissions
3. Record usage → Track usage
4. Get usage report → Monitor usage
5. Check usage limits → Validate limits
```

## 🔒 Security Features

### Auto Authorization:
- Collection tự động thêm `Authorization: Bearer <token>` header
- Token được lưu và sử dụng cho tất cả requests

### Environment Variables:
- Sensitive data được đánh dấu là `secret`
- Dễ dàng switch giữa các environments (dev, staging, prod)

### Request Validation:
- Pre-request scripts validate token
- Post-response scripts check for errors
- Automatic error logging trong Console

## 📊 Monitoring & Logging

### Console Logging:
- ✅ Successful requests với green checkmark
- ❌ Failed requests với red X
- 📋 Response data logging cho important endpoints
- 🔧 Available resources count logging

### Response Validation:
- Automatic status code checking
- Response structure validation
- Error message extraction và display

## 🛠️ Troubleshooting

### Common Issues:

1. **401 Unauthorized**:
   - Kiểm tra token có hợp lệ không
   - Chạy lại Login request để get token mới

2. **404 Not Found**:
   - Kiểm tra baseUrl trong environment
   - Verify API endpoint path

3. **500 Internal Server Error**:
   - Check server logs
   - Verify request payload format

### Debug Tips:
- Sử dụng Postman Console để xem logs chi tiết
- Check Environment variables trước khi chạy
- Verify server đang chạy tại đúng port (7000)

## 📈 Performance Testing

Collection bao gồm các endpoints để test performance:

- Paging endpoints với different page sizes
- Bulk operations (create/update multiple items)
- Search functionality với different criteria
- Statistics endpoints cho monitoring

## 🔄 Continuous Updates

Collection này sẽ được cập nhật định kỳ khi có:
- API endpoints mới
- Response format changes
- Authentication method updates
- New feature additions

---

**Happy Testing! 🎉**

*Nếu có vấn đề gì, vui lòng check console logs hoặc contact dev team.*
