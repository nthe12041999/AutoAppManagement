# Data Binding Demo - Hướng dẫn xem chi tiết và bind dữ liệu

## Tổng quan Flow

Hệ thống đã được cập nhật để hỗ trợ bind dữ liệu tự động khi xem chi tiết hoặc chỉnh sửa record.

## Luồng hoạt động

### 1. User click vào nút View/Edit trong DataGrid

```javascript
// DataGrid tự động generate dropdown menu với các actions
<a onclick="loadDetailFormModal(this, 'view', 123)">Xem chi tiết</a>
<a onclick="loadDetailFormModal(this, 'edit', 123)">Chỉnh sửa</a>
```

### 2. Load Form Modal

```javascript
loadDetailFormModal(config, mode, itemId) {
    // Load CustomerForms.cshtml vào modal
    $.get('/Account/CustomerForms?mode=' + mode + '&id=' + itemId)
        .done((html) => {
            // Render modal với form
            this.renderDetailFormModal(modalContainer, html, config, mode, itemId);
        });
}
```

### 3. Init FormControlBinder & Load Data

```javascript
// Sau khi form được render
Promise.all(loads).then(() => {
    // Init form controls từ data attributes
    window.formControlBinder.init();
    
    // Load data nếu là mode view/edit
    if ((mode === 'edit' || mode === 'view') && itemId) {
        this.loadItemData(config, itemId, mode);
    }
});
```

### 4. Fetch Data từ API

```javascript
loadItemData(config, itemId, mode) {
    // Call API: GET /Account/GetById/123
    calGetAPIAuthen('/Account/GetById/' + itemId, {},
        (response) => {
            if (response && response.Data) {
                // Bind data vào form
                this.bindDataToForm(response.Data, mode);
            }
        }
    );
}
```

### 5. Bind Data vào Form

```javascript
bindDataToForm(data, mode) {
    // Tìm tất cả elements có name hoặc data-name
    const $elements = $modal.find('[name], [data-name]');
    
    $elements.each((_, element) => {
        const fieldName = element.name || element.getAttribute('data-name');
        const value = data[fieldName];
        
        // Set value cho element
        this.setElementValue(element, value);
        
        // Nếu là view mode, disable field
        if (mode === 'view') {
            element.disabled = true;
            element.readOnly = true;
        }
    });
}
```

## Sample Data Response

```json
{
    "success": true,
    "data": {
        "id": 123,
        "name": "Nguyễn Văn A",
        "email": "nguyenvana@gmail.com",
        "phone": "0912345678",
        "dateOfBirth": "1990-01-15",
        "gender": "1",
        "address": "123 Nguyễn Huệ, Q1, HCM",
        "notes": "Khách hàng VIP",
        "avatarUrl": "/uploads/avatars/123.jpg",
        "createdDate": "2024-01-01T10:00:00",
        "status": "active",
        "isVerified": true
    }
}
```

## Form Controls với Data Binding

### Text Input
```html
<div data-type="text" 
     data-name="name" 
     data-label="Họ tên"
     data-value="Nguyễn Văn A">
</div>
<!-- Sau khi bind sẽ generate: -->
<input type="text" name="name" value="Nguyễn Văn A">
```

### Select
```html
<div data-type="select"
     data-name="gender"
     data-label="Giới tính"
     data-options='[{"value":"1","text":"Nam"},{"value":"2","text":"Nữ"}]'
     data-value="1">
</div>
<!-- Sau khi bind: option với value="1" sẽ được selected -->
```

### Date Input
```html
<div data-type="date"
     data-name="dateOfBirth"
     data-label="Ngày sinh"
     data-value="1990-01-15">
</div>
<!-- Format tự động từ ISO date sang yyyy-MM-dd -->
```

### Checkbox/Switch
```html
<div data-type="switch"
     data-name="isVerified"
     data-label="Đã xác thực"
     data-value="true">
</div>
<!-- Checkbox sẽ được checked nếu value = true/1/"true" -->
```

## View Mode vs Edit Mode

### View Mode
- Tất cả fields đều bị disable
- Nút Save bị ẩn
- Nút Cancel đổi thành "Đóng"
- Fields có class `form-control-plaintext`

### Edit Mode
- Fields có thể edit
- Hiện nút Save và Cancel
- Real-time validation khi user input

## Testing

### 1. Mock API Response
Để test, có thể tạo mock data trong AccountController:

```csharp
[HttpGet("GetById/{id}")]
public async Task<IActionResult> GetById(long id)
{
    // Mock data for testing
    var mockData = new
    {
        id = id,
        name = "Test User " + id,
        email = $"user{id}@example.com",
        phone = "0901234567",
        dateOfBirth = "1990-05-15",
        gender = "1",
        address = "123 Test Street",
        notes = "Test notes for user " + id,
        isVerified = true,
        status = "active"
    };
    
    ResOutput.SuccessEventHandler(mockData);
    return Ok(ResOutput);
}
```

### 2. Console Debug
Mở F12 Console để xem:
- `console.log('Loading item data from:', getItemUrl);`
- `console.log('Binding data to form:', data);`

### 3. Test Cases

#### View Mode
1. Click "Xem chi tiết" trong grid
2. Modal mở với data được bind
3. Tất cả fields read-only
4. Chỉ có nút Đóng

#### Edit Mode
1. Click "Chỉnh sửa" trong grid
2. Modal mở với data được bind
3. Fields có thể edit
4. Có nút Lưu và Hủy
5. Validation khi submit

#### Add Mode
1. Click "Thêm mới"
2. Modal mở với form trống
3. Tất cả fields empty
4. Required fields validation

## Troubleshooting

### Data không bind
- Kiểm tra field name match với property trong response data
- Check console có lỗi không
- Verify API trả về đúng format

### Field không disable trong view mode
- Kiểm tra mode được truyền đúng
- Check element selector đúng

### Date không hiển thị đúng
- Format date từ API phải là ISO string
- Browser timezone có thể ảnh hưởng

## Customization

### Custom data binding
```javascript
// Listen to dataLoaded event
$modal.on('dataLoaded', function(e, data) {
    // Custom processing
    if (data.avatarUrl) {
        $('#avatarPreview').attr('src', data.avatarUrl);
    }
});
```

### Override field value
```javascript
// Sau khi data được bind
window.currentModalData = data;
$('#customField').val(data.customValue);
```

