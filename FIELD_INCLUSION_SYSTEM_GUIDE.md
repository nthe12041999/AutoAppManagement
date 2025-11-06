# Field Inclusion System - Hướng dẫn sử dụng

## 📋 Tổng quan

Field Inclusion System cho phép bạn specify các field/column cần được join từ các bảng khác trong GetPaging request, giúp optimize performance và linh hoạt trong việc lấy dữ liệu.

## 🔧 Cấu trúc Components

### 1. **PagingRequestDTO** - Enhanced
```csharp
public class PagingRequestDTO
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string Filter { get; set; } = "";
    public string Sort { get; set; } = "Id";
    
    // New fields for field inclusion
    public List<string> IncludeFields { get; set; } = new List<string>();
    public Dictionary<string, object> FieldMetadata { get; set; } = new Dictionary<string, object>();
}
```

### 2. **FieldInclusionConfig** - Configuration
```csharp
public class FieldInclusionConfig
{
    public string FieldName { get; set; } = string.Empty;
    public string JoinType { get; set; } = "Left";
    public string JoinTable { get; set; } = string.Empty;
    public string JoinCondition { get; set; } = string.Empty;
    public string SourceField { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public bool EnableCache { get; set; } = true;
    public int CacheMinutes { get; set; } = 5;
}
```

### 3. **BaseBusinessService** - Enhanced GetPaging
```csharp
public virtual async Task<object> GetPaging(PagingRequestDTO pagingRequestDTO)
{
    // Standard paging logic...
    
    // Custom processing with field inclusion
    var dtos = await CustomDataAfterGetPaging(pagingRequestDTO, entities);
    if (dtos == null)
    {
        dtos = Mapper.Map<List<TDto>>(entities);
    }
    
    return new { Data = dtos, TotalCount, TotalPages, CurrentPage, PageSize };
}

public virtual async Task<List<TDto>> CustomDataAfterGetPaging(PagingRequestDTO pagingRequestDTO, List<TEntity> entities)
{
    return null; // Override trong derived service
}
```

## 🚀 Cách sử dụng

### 1. **Backend - Service Layer**

#### Override CustomDataAfterGetPaging trong AccountService:
```csharp
public override async Task<List<AccountDTO>> CustomDataAfterGetPaging(PagingRequestDTO pagingRequestDTO, List<Account> entities)
{
    var accountDtos = Mapper.Map<List<AccountDTO>>(entities);
    
    // Kiểm tra có yêu cầu LicenseName không
    if (pagingRequestDTO.RequiresField("LicenseName"))
    {
        var licenses = await LicenseRepository.GetAll();
        foreach (var item in accountDtos)
        {
            var license = licenses.FirstOrDefault(l => l.ID == item.LicenseId);
            if (license != null)
            {
                item.LicenseName = license.LicenseName;
            }
        }
    }
    
    // Thêm các field khác nếu cần
    if (pagingRequestDTO.RequiresField("RoleName"))
    {
        // Join với Role table
    }
    
    return accountDtos;
}
```

### 2. **Backend - Controller Layer**

#### Tạo endpoint hỗ trợ field inclusion:
```csharp
[HttpPost]
public virtual async Task<IActionResult> GetPagingWithFields([FromBody] PagingRequestDTO request)
{
    try
    {
        // Auto-include common fields
        if (!request.IncludeFields.Contains("LicenseName"))
        {
            request = request.WithLicenseName();
        }

        var result = await Service.GetPaging(request.PageIndex, request.PageSize, request.Filter);
        ResOutput.SuccessEventHandler(result);
        return Ok(ResOutput);
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Error getting paging data with fields");
        ResOutput.ErrorEventHandler(ex.Message);
        return BadRequest(ResOutput);
    }
}
```

### 3. **Frontend - JavaScript**

#### Sử dụng PagingRequestBuilder:
```javascript
// Basic usage với license name
const request = new PagingRequestBuilder()
    .setPaging(1, 10)
    .setFilter("search term")
    .setSort("Name")
    .includeLicenseName()
    .build();

// Advanced usage với multiple fields
const advancedRequest = new PagingRequestBuilder()
    .setPaging(1, 20)
    .setFilter("")
    .includeLicenseName()
    .includeRoleName()
    .includeStatusName()
    .build();

// Custom field inclusion
const customRequest = new PagingRequestBuilder()
    .setPaging(1, 10)
    .includeCustomField("DepartmentName", {
        FieldName: "DepartmentName",
        JoinTable: "Department",
        JoinCondition: "DepartmentId = ID",
        SourceField: "DepartmentName",
        TargetField: "DepartmentName",
        EnableCache: true,
        CacheMinutes: 5
    })
    .build();
```

#### Grid integration:
```javascript
function customDataLoader(pageIndex, pageSize, filter, sortField) {
    return new Promise(async (resolve, reject) => {
        const request = new PagingRequestBuilder()
            .setPaging(pageIndex, pageSize)
            .setFilter(filter)
            .setSort(sortField)
            .includeLicenseName()
            .build();

        const response = await fetch('/Account/GetPagingWithFields', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(request)
        });

        const result = await response.json();
        if (result.IsSuccess) {
            resolve(result.Data);
        } else {
            reject(new Error(result.Message));
        }
    });
}
```

## 📈 Lợi ích

### 1. **Performance Optimization**
- Chỉ join các bảng khi thực sự cần thiết
- Cache config để tránh query trùng lặp
- Lazy loading cho các related data

### 2. **Flexibility**
- Frontend có thể specify fields cần thiết
- Easy configuration cho các join mới
- Reusable across different entities

### 3. **Maintainability**
- Clean separation of concerns
- Consistent pattern across all services
- Easy to extend và modify

## 🔍 Examples thực tế

### Example 1: Account Grid với License Name
```javascript
// Frontend request
const accountGrid = new AccountGridWithFieldInclusion();
accountGrid.loadData(1, 10, "").then(data => {
    // data sẽ có LicenseName field
    console.log('Accounts with license:', data);
});
```

### Example 2: Minimal data loading
```javascript
// Chỉ lấy data cơ bản, không join
const request = new PagingRequestBuilder()
    .setPaging(1, 100)
    .build(); // Không include field nào

// Hoặc explicit empty fields
request.IncludeFields = [];
```

### Example 3: Multiple joins
```javascript
const request = new PagingRequestBuilder()
    .setPaging(1, 10)
    .includeLicenseName()
    .includeRoleName()
    .includeCustomField("CreatedByName", {
        JoinTable: "AdminAccount",
        JoinCondition: "CreatedBy = ID",
        SourceField: "UserName",
        TargetField: "CreatedByName"
    })
    .build();
```

## ⚠️ Notes

1. **Cache Strategy**: Default cache 5-15 minutes for lookup data
2. **Performance**: Include chỉ những field thực sự cần thiết
3. **Naming**: Field names should be consistent với DTO properties
4. **Error Handling**: Always handle join failures gracefully
5. **Documentation**: Comment rõ ràng về các field dependencies

## 🔄 Extension Points

Để extend system cho entities khác:

1. Override `CustomDataAfterGetPaging` trong service
2. Thêm extension methods trong `PagingRequestExtensions`
3. Tạo custom data loader trong frontend
4. Update grid configuration để sử dụng custom loader

System này đã được implement cho **AccountService** và có thể easily extend cho các entities khác như License, Role, Department, etc.