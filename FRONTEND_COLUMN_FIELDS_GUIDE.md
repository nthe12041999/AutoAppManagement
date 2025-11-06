# Frontend Column Fields System - Updated Guide

## 📋 Tổng quan

Hệ thống đã được cập nhật để **Frontend phải gửi danh sách column fields xuống Backend** trong mỗi GetPaging request. Backend sẽ dựa vào danh sách này để quyết định join với bảng nào và lấy field nào.

## 🔄 Flow hoạt động

```
Frontend Grid Config → Extract Column Fields → Send to Backend → Backend Auto-detect Joins → Return Data
```

## 🚀 Cách thức hoạt động

### 1. **Frontend gửi RequestedColumns**

#### Grid Configuration:
```javascript
function customGridColumnConfig() {
    return [
        { field: 'Name', title: 'Họ và tên' },
        { field: 'Email', title: 'Email' },
        { field: 'LicenseName', title: 'License' },    // ← Trigger join License
        { field: 'StatusName', title: 'Trạng thái' },  // ← Trigger convert Status
        { field: 'CreatedDate', title: 'Ngày tạo' }
    ];
}
```

#### Request được gửi:
```javascript
const request = {
    PageIndex: 1,
    PageSize: 10,
    Filter: "",
    Sort: "Id",
    RequestedColumns: [                    // ← FE gửi danh sách columns
        'Name', 'Email', 'LicenseName', 
        'StatusName', 'CreatedDate'
    ],
    IncludeFields: [],                     // Auto-detect từ RequestedColumns
    FieldMetadata: {}
};
```

### 2. **Backend Auto-detect Joins**

#### PagingRequestDTO Enhancement:
```csharp
public class PagingRequestDTO
{
    // Standard paging properties...
    
    public List<string> RequestedColumns { get; set; } = new List<string>();  // ← NEW
    public List<string> IncludeFields { get; set; } = new List<string>();
    
    public void AutoDetectIncludeFields()
    {
        // Auto-map column → include field
        var columnMapping = new Dictionary<string, string>
        {
            ["LicenseName"] = "LicenseName",
            ["StatusName"] = "StatusName",
            ["RoleName"] = "RoleName"
        };
        
        foreach (var column in RequestedColumns)
        {
            if (columnMapping.ContainsKey(column))
            {
                IncludeFields.Add(columnMapping[column]);
            }
        }
    }
}
```

### 3. **Service Layer Processing**

#### AccountService:
```csharp
public override async Task<List<AccountDTO>> CustomDataAfterGetPaging(
    PagingRequestDTO request, List<Account> entities)
{
    var accountDtos = Mapper.Map<List<AccountDTO>>(entities);
    
    // Check nếu FE request LicenseName column
    if (request.RequiresField("LicenseName") || request.HasColumn("LicenseName"))
    {
        // Join với License table
        var licenses = await LicenseRepository.GetAll();
        foreach (var item in accountDtos)
        {
            var license = licenses.FirstOrDefault(l => l.ID == item.LicenseId);
            item.LicenseName = license?.LicenseName ?? "Chưa có gói cước";
        }
    }
    
    // Check nếu FE request StatusName column  
    if (request.RequiresField("StatusName") || request.HasColumn("StatusName"))
    {
        // Convert Status enum to readable text
        foreach (var item in accountDtos)
        {
            item.StatusName = item.Status switch
            {
                StatusEnum.Active => "Hoạt động",
                StatusEnum.Inactive => "Không hoạt động",
                StatusEnum.Locked => "Đã khóa",
                _ => "Không xác định"
            };
        }
    }
    
    return accountDtos;
}
```

## 💻 Implementation Examples

### **Cách 1: Auto-extract từ Grid Config**

```javascript
function customDataLoader(pageIndex, pageSize, filter, sortField) {
    // Lấy grid config
    const gridConfig = window.currentGridConfig || getCustomGridConfig();
    
    // Auto-extract columns từ grid config
    const request = new PagingRequestBuilder()
        .setPaging(pageIndex, pageSize)
        .setFilter(filter)
        .extractColumnsFromGridConfig(gridConfig)  // ← Auto extract
        .build();
        
    console.log('Request with auto-extracted columns:', request);
    // RequestedColumns: ['Name', 'Email', 'LicenseName', 'StatusName', ...]
}
```

### **Cách 2: Explicit specify columns**

```javascript
function customDataLoaderWithExplicitColumns(pageIndex, pageSize, filter, sortField) {
    // Explicit specify columns cần thiết
    const requestedColumns = [
        'Name', 'Email', 'Phone',
        'LicenseName',    // ← Sẽ trigger join với License
        'StatusName',     // ← Sẽ trigger convert Status enum  
        'CreatedDate', 'IsVerified'
    ];
    
    const request = new PagingRequestBuilder()
        .setPaging(pageIndex, pageSize)
        .setRequestedColumns(requestedColumns)  // ← Explicit set
        .build();
}
```

### **Cách 3: Conditional columns based on user role**

```javascript
function getDynamicColumns(userRole) {
    const baseColumns = ['Name', 'Email', 'Phone'];
    
    if (userRole === 'admin') {
        return [...baseColumns, 'LicenseName', 'StatusName', 'CreatedDate'];
    } else if (userRole === 'manager') {
        return [...baseColumns, 'StatusName'];
    } else {
        return baseColumns;  // Basic columns only
    }
}

function roleBasedDataLoader(pageIndex, pageSize, filter, sortField) {
    const userRole = getCurrentUserRole();
    const columns = getDynamicColumns(userRole);
    
    const request = new PagingRequestBuilder()
        .setPaging(pageIndex, pageSize)
        .setRequestedColumns(columns)
        .build();
}
```

## 🎯 Benefits của approach này

### 1. **Performance Optimization**
- Backend chỉ join khi FE thực sự cần
- Tránh load data không cần thiết
- Conditional processing based on UI requirements

### 2. **Flexibility**
- FE control hoàn toàn việc lấy data nào
- Dynamic columns based on user roles/permissions
- Easy to add/remove columns

### 3. **Maintainability**
- Clear contract giữa FE và BE
- Self-documenting code (columns trong request)
- Easy debugging (log requested columns)

## 📊 Example Requests

### **Basic Grid (chỉ hiển thị thông tin cơ bản):**
```json
{
    "PageIndex": 1,
    "PageSize": 10,
    "RequestedColumns": ["Name", "Email", "Phone"],
    "IncludeFields": []  // Empty → Không join gì cả
}
```

### **Full Grid (hiển thị đầy đủ thông tin):**
```json
{
    "PageIndex": 1,
    "PageSize": 10,
    "RequestedColumns": [
        "Name", "Email", "Phone", 
        "LicenseName", "StatusName", 
        "CreatedDate", "IsVerified"
    ],
    "IncludeFields": ["LicenseName", "StatusName"]  // Auto-detected
}
```

### **Admin Grid (thêm thông tin quản trị):**
```json
{
    "PageIndex": 1,
    "PageSize": 10,
    "RequestedColumns": [
        "Name", "Email", "LicenseName", "StatusName",
        "CreatedByName", "UpdatedByName", "LastLogin"
    ],
    "IncludeFields": ["LicenseName", "StatusName", "CreatedByName", "UpdatedByName"]
}
```

## 🔧 Extension Points

### Để thêm field mới:

1. **Thêm vào DTO:**
```csharp
public class AccountDTO 
{
    public string LicenseName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;      // ← NEW
    public string DepartmentName { get; set; } = string.Empty; // ← NEW
}
```

2. **Update AutoDetectIncludeFields:**
```csharp
var columnMapping = new Dictionary<string, string>
{
    ["LicenseName"] = "LicenseName",
    ["StatusName"] = "StatusName", 
    ["RoleName"] = "RoleName",          // ← NEW
    ["DepartmentName"] = "DepartmentName" // ← NEW
};
```

3. **Update Service processing:**
```csharp
if (request.RequiresField("RoleName"))
{
    // Join với Role table
}

if (request.RequiresField("DepartmentName"))
{
    // Join với Department table
}
```

4. **Update Grid config:**
```javascript
{
    field: 'RoleName',
    title: 'Vai trò',
    type: ColumnTypes.TEXT,
    sortable: true
}
```

## ⚠️ Important Notes

1. **RequestedColumns là required**: FE phải gửi danh sách columns
2. **Backend validation**: Check columns có valid không
3. **Performance**: Chỉ include fields thực sự cần thiết
4. **Caching**: Cache lookup data (License, Role, etc.) để optimize
5. **Logging**: Log requested columns để debug

Hệ thống này đảm bảo FE có full control over data requirements và BE chỉ xử lý những gì thực sự cần thiết! 🚀