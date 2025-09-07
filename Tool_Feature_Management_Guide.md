# Tool Feature Management System

## Tổng quan

Hệ thống quản lý tính năng tool cho phép bạn:

1. **Quản lý tính năng**: Định nghĩa các tính năng của tool và phân loại chúng
2. **Kiểm soát license**: Gán tính năng cho từng license với giới hạn riêng biệt  
3. **Giới hạn tài nguyên**: Thiết lập quota sử dụng (daily, monthly, total)
4. **Tracking sử dụng**: Theo dõi việc sử dụng tính năng và tài nguyên
5. **Kiểm tra quyền truy cập**: Tự động kiểm tra quyền khi user sử dụng tính năng

## Các thành phần chính

### 1. ToolFeature (Tính năng)
```json
{
  "featureCode": "EXPORT_PDF",
  "featureName": "Xuất PDF", 
  "description": "Tính năng xuất báo cáo dưới dạng PDF",
  "category": "Export",
  "featureType": "Feature", // Feature, Resource, API
  "requiresLicense": true,
  "defaultLimits": "{\"daily\": 100, \"monthly\": 2000}"
}
```

### 2. LicenseFeature (Gán tính năng cho license)
```json
{
  "licenseId": 1,
  "toolFeatureId": 1,
  "isEnabled": true,
  "resourceLimits": "{\"maxFileSize\": \"10MB\", \"formats\": [\"PDF\", \"DOC\"]}",
  "usageQuota": "{\"daily\": 50, \"monthly\": 1000}",
  "effectiveFrom": "2025-01-01",
  "effectiveTo": "2025-12-31"
}
```

### 3. FeatureUsage (Tracking sử dụng)
```json
{
  "accountId": 1,
  "licenseId": 1, 
  "toolFeatureId": 1,
  "usageType": "Export", // Access, Resource, API_Call
  "usageCount": 1,
  "resourceConsumed": 1.5,
  "usageDate": "2025-09-07T10:30:00Z",
  "usageData": "{\"fileSize\": \"1.5MB\", \"format\": \"PDF\"}"
}
```

## Cách sử dụng

### 1. Tạo và quản lý tính năng

```csharp
// Tạo tính năng mới
var createRequest = new CreateToolFeatureRequest
{
    FeatureCode = "ADVANCED_REPORT",
    FeatureName = "Báo cáo nâng cao", 
    Description = "Tạo báo cáo với biểu đồ và phân tích",
    Category = "Reporting",
    FeatureType = "Feature",
    RequiresLicense = true,
    DefaultLimits = "{\"daily\": 20, \"monthly\": 500}"
};

var result = await toolFeatureService.CreateToolFeatureAsync(createRequest);
```

### 2. Gán tính năng cho license

```csharp
// Gán tính năng cho license với giới hạn riêng
var assignRequest = new AssignFeatureToLicenseRequest
{
    LicenseId = 1,
    ToolFeatureId = 1,
    IsEnabled = true,
    UsageQuota = "{\"daily\": 10, \"monthly\": 200}", // Giới hạn thấp hơn default
    ResourceLimits = "{\"maxComplexity\": \"medium\"}"
};

var result = await licenseFeatureService.AssignFeatureToLicenseAsync(assignRequest);
```

### 3. Sử dụng Attribute để kiểm tra quyền tự động

```csharp
[HttpPost("GenerateAdvancedReport")]
[Roles(RoleConstant.Customer)]
[FeatureAccess("ADVANCED_REPORT", "Generation", 1)]
public async Task<IActionResult> GenerateReport()
{
    // Logic tạo báo cáo
    // Quyền truy cập đã được kiểm tra tự động
    return Ok(new { report = "generated" });
}

[HttpPost("UploadFile")]  
[ResourceQuotaCheck("CLOUD_STORAGE", "Upload", "fileSize")]
public async Task<IActionResult> UploadFile(decimal fileSize)
{
    // Upload file với kiểm tra quota storage
    return Ok();
}
```

### 4. Kiểm tra quyền thủ công

```csharp
// Kiểm tra quyền truy cập
var checkRequest = new CheckFeatureAccessRequest
{
    AccountId = userId,
    FeatureCode = "EXPORT_PDF", 
    LicenseKey = "license-key-here",
    UsageType = "Export",
    ResourceAmount = 1
};

var accessResult = await featureAccessService.CheckFeatureAccessAsync(checkRequest);

if (accessResult.HasAccess)
{
    // Cho phép sử dụng tính năng
    // accessResult.LimitInfo chứa thông tin về giới hạn hiện tại
}
else
{
    // Từ chối: accessResult.Reason chứa lý do
}
```

### 5. Ghi nhận sử dụng tính năng

```csharp
// Ghi nhận khi user sử dụng tính năng
await featureAccessService.RecordFeatureUsageAsync(
    accountId: userId,
    licenseKey: "license-key",
    featureCode: "EXPORT_PDF",
    usageType: "Export", 
    resourceAmount: 1,
    usageData: "{\"fileName\": \"report.pdf\", \"size\": \"2MB\"}"
);
```

## Các loại giới hạn

### 1. Usage Quota (Giới hạn sử dụng)
```json
{
  "daily": 100,     // Tối đa 100 lần/ngày
  "monthly": 2000,  // Tối đa 2000 lần/tháng  
  "total": 50000    // Tối đa 50000 lần trong suốt thời gian license
}
```

### 2. Resource Limits (Giới hạn tài nguyên)
```json
{
  "maxFileSize": "10MB",
  "maxConcurrent": 5,
  "allowedFormats": ["PDF", "Excel", "Word"],
  "maxComplexity": "high"
}
```

## API Endpoints

### Tool Feature Management
- `POST /api/ToolFeature/Create` - Tạo tính năng mới
- `PUT /api/ToolFeature/Update` - Cập nhật tính năng
- `GET /api/ToolFeature/GetByCode/{code}` - Lấy tính năng theo mã
- `GET /api/ToolFeature/GetByCategory/{category}` - Lấy theo danh mục

### License Feature Management  
- `POST /api/LicenseFeature/AssignFeature` - Gán tính năng cho license
- `DELETE /api/LicenseFeature/RemoveFeature` - Xóa tính năng khỏi license
- `GET /api/LicenseFeature/GetFeaturesByLicense/{id}` - Lấy tính năng của license

### Feature Access Control
- `POST /api/FeatureAccess/CheckAccess` - Kiểm tra quyền truy cập
- `POST /api/FeatureAccess/RecordUsage` - Ghi nhận sử dụng
- `POST /api/FeatureAccess/GetUsageReport` - Báo cáo sử dụng

## Ví dụ cấu hình License Features

### License Basic (Gói cơ bản)
```json
[
  {
    "featureCode": "EXPORT_PDF",
    "usageQuota": "{\"daily\": 10, \"monthly\": 200}"
  },
  {
    "featureCode": "CLOUD_STORAGE", 
    "resourceLimits": "{\"total\": \"1GB\"}"
  }
]
```

### License Premium (Gói cao cấp)
```json
[
  {
    "featureCode": "EXPORT_PDF",
    "usageQuota": "{\"daily\": 100, \"monthly\": 2000}"
  },
  {
    "featureCode": "ADVANCED_ANALYTICS",
    "usageQuota": "{\"daily\": 50, \"monthly\": 1000}"
  },
  {
    "featureCode": "CLOUD_STORAGE",
    "resourceLimits": "{\"total\": \"10GB\"}"
  },
  {
    "featureCode": "API_ACCESS",
    "usageQuota": "{\"daily\": 5000, \"monthly\": 100000}"
  }
]
```

## Best Practices

### 1. Thiết kế Feature Code
- Sử dụng naming convention: `CATEGORY_ACTION` (VD: `EXPORT_PDF`, `ANALYTICS_ADVANCED`)
- Đặt tên dễ hiểu và không trùng lặp
- Phân loại theo category để quản lý dễ dàng

### 2. Thiết lập Quota
- Đặt giới hạn hợp lý theo từng gói license
- Có thể override default limits cho từng license riêng biệt
- Cân nhắc cả daily, monthly và total limits

### 3. Error Handling
- Luôn kiểm tra quyền trước khi thực hiện tính năng tốn tài nguyên
- Thông báo lỗi rõ ràng khi vượt quota
- Implement fail-open policy khi có lỗi hệ thống

### 4. Performance
- Cache thông tin license feature để giảm database queries
- Sử dụng background job để ghi usage tracking
- Partition FeatureUsage table theo thời gian

### 5. Monitoring
- Track usage patterns để tối ưu quota
- Alert khi có user vượt quota bất thường
- Báo cáo định kỳ về tình hình sử dụng features

## Troubleshooting

### 1. User không thể sử dụng tính năng
- Kiểm tra license có active không
- Kiểm tra feature có được gán cho license không
- Kiểm tra quota có bị vượt không
- Kiểm tra effective date của license feature

### 2. Quota không chính xác
- Kiểm tra timezone trong usage tracking
- Kiểm tra logic tính toán daily/monthly reset
- Verify usage data có bị duplicate không

### 3. Performance chậm
- Optimize database indexes
- Implement caching cho feature access checks
- Sử dụng async processing cho usage recording
