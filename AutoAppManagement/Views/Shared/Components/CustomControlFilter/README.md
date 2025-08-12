# CustomControlFilter Component

Một View Component có thể tái sử dụng để tạo các bộ lọc tùy chỉnh cho bảng dữ liệu và danh sách.

## Tính năng

- ✅ Tìm kiếm với debouncing
- ✅ Dropdown filters (select)
- ✅ Input filters (text, number, email, etc.)
- ✅ Date range filters
- ✅ Custom buttons
- ✅ Auto-apply filters hoặc manual
- ✅ API integration
- ✅ Client-side table filtering
- ✅ Event system
- ✅ Responsive design

## Cách sử dụng cơ bản

### 1. Sử dụng trong View

```csharp
@{
    var filterModel = AutoAppManagement.ViewComponents.CustomControlFilterViewComponent.CreateBasicSearchFilter();
    filterModel.ContainerId = "myFilter";
    filterModel.Config.OnFilterChange = "handleMyFilter";
}
@await Component.InvokeAsync("CustomControlFilter", filterModel)
```

### 2. Tạo callback function trong JavaScript

```javascript
window.handleMyFilter = function(filters, containerId) {
    console.log('Filters changed:', filters);
    // Xử lý logic filter của bạn ở đây
};
```

## Các template có sẵn

### Basic Search Filter
```csharp
var model = CustomControlFilterViewComponent.CreateBasicSearchFilter(
    searchLabel: "Tìm kiếm...",
    searchPlaceholder: "Nhập từ khóa..."
);
```

### Search + Status Filter
```csharp
var model = CustomControlFilterViewComponent.CreateSearchWithStatusFilter(
    searchLabel: "Tìm kiếm theo tên, email...",
    statusOptions: new List<FilterOption>
    {
        new FilterOption { Value = "active", Text = "Hoạt động" },
        new FilterOption { Value = "inactive", Text = "Không hoạt động" }
    }
);
```

### License Filter (với date range)
```csharp
var model = CustomControlFilterViewComponent.CreateLicenseFilter();
```

### Customer Account Filter
```csharp
var model = CustomControlFilterViewComponent.CreateCustomerAccountFilter();
```

## Tùy chỉnh nâng cao

### Tạo filter tùy chỉnh hoàn toàn

```csharp
var model = new CustomControlFilterModel
{
    ContainerId = "customFilter",
    SearchLabel = "Tìm kiếm sản phẩm...",
    SearchInputColSize = 3,
    ShowSearchInput = true,
    ShowClearButton = true
};

// Thêm dropdown filter
model.FilterControls.Add(new FilterControl
{
    Id = "categoryFilter",
    Name = "category",
    Type = "select",
    DefaultOption = "Tất cả danh mục",
    ColSize = 2,
    Options = new List<FilterOption>
    {
        new FilterOption { Value = "electronics", Text = "Điện tử" },
        new FilterOption { Value = "clothing", Text = "Thời trang" }
    }
});

// Thêm input filter
model.FilterControls.Add(new FilterControl
{
    Id = "priceFilter",
    Name = "price",
    Type = "input",
    InputType = "number",
    Label = "Giá tối đa",
    Placeholder = "Nhập giá...",
    ColSize = 2
});

// Thêm date range filter
model.FilterControls.Add(new FilterControl
{
    Id = "dateRange",
    Name = "dateRange",
    Type = "daterange",
    ColSize = 3
});

// Thêm custom button
model.CustomButtons.Add(new CustomButton
{
    Id = "exportBtn",
    Text = "Xuất Excel",
    Icon = "bi bi-file-earmark-excel",
    CssClass = "btn-success",
    ColSize = 2
});

// Cấu hình JavaScript
model.Config.TargetTableId = "productTable";
model.Config.ApiEndpoint = "/Product/GetFilteredData";
model.Config.OnFilterChange = "handleProductFilter";
```

## JavaScript API

### Khởi tạo thủ công
```javascript
customControlFilter.init('myFilterId', {
    targetTableId: 'myTable',
    apiEndpoint: '/api/data',
    searchDebounceMs: 300,
    autoApplyFilters: true,
    onFilterChange: 'myCallbackFunction'
});
```

### Lấy giá trị filter hiện tại
```javascript
const filters = customControlFilter.getFilters('myFilterId');
console.log(filters);
```

### Set giá trị filter
```javascript
customControlFilter.setFilters('myFilterId', {
    search: 'keyword',
    status: 'active'
});
```

### Xóa tất cả filter
```javascript
customControlFilter.clearFilters('myFilterId');
```

## Events

### Filter Change Event
```javascript
document.addEventListener('customFilterChange', function(e) {
    console.log('Container:', e.detail.containerId);
    console.log('Filters:', e.detail.filters);
});
```

### Custom Button Click Event
```javascript
document.addEventListener('customFilterButtonClick', function(e) {
    console.log('Container:', e.detail.containerId);
    console.log('Button ID:', e.detail.buttonId);
    
    if (e.detail.buttonId === 'exportBtn') {
        // Xử lý export
    }
});
```

## Styling

Component sử dụng Bootstrap 5 classes. Bạn có thể tùy chỉnh CSS:

```css
.custom-control-filter .form-floating label {
    color: #6c757d;
}

.custom-control-filter .btn-outline-secondary:hover {
    background-color: #6c757d;
    border-color: #6c757d;
}
```

## Ví dụ hoàn chỉnh

Xem các file sau để tham khảo:
- `Views/Demo/Grid.cshtml` - Ví dụ cơ bản
- `Views/License/Index.cshtml` - Ví dụ nâng cao với API integration
