# Hướng dẫn Form Data Binding System

## Tổng quan
Hệ thống Form Data Binding tự động hóa việc tạo form, bind dữ liệu và validate trong ứng dụng AutoAppManagement.

## Kiến trúc

```
┌─────────────────┐
│   Index.cshtml  │ 
│   (DataGrid)    │
└────────┬────────┘
         │ Click View/Edit
         ▼
┌─────────────────┐
│  data-grid.js   │
│loadDetailFormModal
└────────┬────────┘
         │ Load Form
         ▼
┌─────────────────┐
│CustomerForms.cshtml
│ (data attributes)│
└────────┬────────┘
         │ Parse & Generate
         ▼
┌─────────────────┐
│form-control-binder
│    (Auto UI)    │
└────────┬────────┘
         │ Load Data
         ▼
┌─────────────────┐
│  API GetById    │
│  (Fetch Data)   │
└────────┬────────┘
         │ Bind
         ▼
┌─────────────────┐
│  Form Rendered  │
│  with Data      │
└─────────────────┘
```

## Components

### 1. DataGrid Component (`data-grid.js`)

#### Chức năng chính:
- Auto-generate table từ data attributes
- Load data với pagination
- Handle actions (View/Edit/Delete)
- Load form modal
- Bind data vào form

#### Methods quan trọng:

```javascript
// Load form modal
loadDetailFormModal(config, mode, itemId)

// Load item data từ API  
loadItemData(config, itemId, mode)

// Bind data vào form
bindDataToForm(data, mode)

// Set value cho element
setElementValue($element, value)
```

### 2. FormControlBinder (`form-control-binder.js`)

#### Chức năng:
- Parse data attributes
- Generate HTML controls
- Real-time validation
- Auto-submit forms

#### Control Types hỗ trợ:

| Type | Description | Example |
|------|-------------|---------|
| text | Text input | `data-type="text"` |
| email | Email input | `data-type="email"` |
| tel | Phone input | `data-type="tel"` |
| number | Number input | `data-type="number"` |
| date | Date picker | `data-type="date"` |
| textarea | Multi-line text | `data-type="textarea"` |
| select | Dropdown | `data-type="select"` |
| switch | Toggle switch | `data-type="switch"` |
| checkbox | Checkbox | `data-type="checkbox"` |
| radio | Radio buttons | `data-type="radio"` |

### 3. Form Helpers (`form-helpers.js`)

Utility functions:
- `formatPhoneNumber()` - Format số điện thoại VN
- `validateEmail()` - Validate email
- `formatCurrency()` - Format tiền VND
- `getRelativeTime()` - Thời gian tương đối
- `debounce()` - Debounce function
- `showToast()` - Show notification

## Cách sử dụng

### 1. Tạo Form với Data Attributes

```html
<!-- Text Input -->
<div data-type="text"
     data-name="name"
     data-label="Họ tên"
     data-placeholder="Nhập họ tên"
     data-required>
</div>

<!-- Email Input -->
<div data-type="email"
     data-name="email"
     data-label="Email"
     data-required>
</div>

<!-- Select -->
<div data-type="select"
     data-name="gender"
     data-label="Giới tính"
     data-options='[
         {"value":"1","text":"Nam"},
         {"value":"2","text":"Nữ"}
     ]'>
</div>

<!-- Date -->
<div data-type="date"
     data-name="dateOfBirth"
     data-label="Ngày sinh">
</div>

<!-- Textarea -->
<div data-type="textarea"
     data-name="notes"
     data-label="Ghi chú"
     data-maxLength="500">
</div>

<!-- Switch -->
<div data-type="switch"
     data-name="isActive"
     data-label="Kích hoạt">
</div>
```

### 2. Initialize FormControlBinder

```javascript
// Initialize khi document ready
$(document).ready(function() {
    window.formControlBinder.init('#CustomerForm');
});
```

### 3. Setup DataGrid với Modal

```html
<div data-component="data-grid"
     data-container-id="accountDataGrid"
     data-get-url="/Account/GetPaging"
     data-detail-form="CustomerForms"
     data-has-add="true"
     data-has-refresh="true">
</div>
```

### 4. API Response Format

```json
{
    "success": true,
    "data": {
        "id": 123,
        "name": "Nguyễn Văn A",
        "email": "customer@gmail.com",
        "phone": "0912345678",
        "dateOfBirth": "1990-05-15",
        "gender": "1",
        "address": "123 Nguyễn Huệ, Q1, HCM",
        "notes": "VIP Customer",
        "isActive": true
    }
}
```

## Validation

### Required Fields
```html
<div data-type="text" data-name="name" data-required></div>
```

### Custom Validation Messages
```html
<div data-type="email" 
     data-name="email"
     data-required
     data-required-message="Email là bắt buộc"
     data-pattern-message="Email không đúng định dạng">
</div>
```

### Real-time Validation
```javascript
// Tự động validate khi user input
$form.on('input', 'input, select, textarea', (e) => {
    formControlBinder.validateField(e.target);
});
```

## Modes

### Add Mode
- Form trống
- Tất cả fields có thể edit
- Validation cho required fields

### Edit Mode
- Load data từ API
- Bind data vào form
- Track changes

### View Mode
- Load data từ API
- Tất cả fields read-only
- Ẩn nút Save
- Đổi nút Cancel thành "Đóng"

## Events

### dataLoaded Event
```javascript
$modal.on('dataLoaded', function(e, params) {
    const { data, mode } = params;
    // Custom processing
});
```

### formValidated Event
```javascript
$form.on('formValidated', function(e, detail) {
    const { isValid, formData } = detail;
    // Handle validation result
});
```

### formSubmitSuccess Event
```javascript
$form.on('formSubmitSuccess', function(e, detail) {
    const { response, formData } = detail;
    // Handle success
});
```

## Styling

### CSS Classes

| Class | Description |
|-------|-------------|
| `.form-control` | Standard input style |
| `.form-control-plaintext` | View mode style |
| `.is-invalid` | Error state |
| `.is-valid` | Valid state |
| `.invalid-feedback` | Error message |

### Custom Styles (`form-styles.css`)
- Avatar preview with hover effect
- Animated validation states
- Loading spinner
- Success/Error animations
- Responsive adjustments

## Advanced Features

### 1. Phone Number Formatting
```javascript
$('input[type="tel"]').formatPhone();
```

### 2. Character Counter
```javascript
$('textarea').charCounter(500);
```

### 3. Avatar Preview
```javascript
$('#avatarFile').on('change', function(e) {
    const file = e.target.files[0];
    // Preview logic
});
```

### 4. Debounced Search
```javascript
const search = FormHelpers.debounce(function(term) {
    // Search logic
}, 300);
```

## Testing

### Mock Data Controller
```csharp
[HttpGet]
public IActionResult GetById(long id)
{
    var mockData = new {
        id = id,
        name = "Test User",
        email = "test@example.com",
        // ... other fields
    };
    
    return Json(new {
        success = true,
        data = mockData
    });
}
```

### Test Cases

1. **Add New Record**
   - Click "Thêm mới"
   - Fill form
   - Validate required fields
   - Submit

2. **Edit Record**
   - Click "Chỉnh sửa"
   - Data auto-loaded
   - Modify fields
   - Save changes

3. **View Record**
   - Click "Xem chi tiết"
   - All fields read-only
   - Avatar displayed
   - Only "Đóng" button

## Troubleshooting

### Data không bind

**Kiểm tra:**
- Field name match với API response
- Console errors
- API response format

### Validation không hoạt động

**Kiểm tra:**
- `data-required` attribute
- FormControlBinder đã init
- jQuery loaded

### Modal không mở

**Kiểm tra:**
- Bootstrap loaded
- `data-detail-form` attribute
- Controller action exists

## Best Practices

1. **Always escape HTML** để prevent XSS
2. **Use debounce** cho search/filter
3. **Cache selectors** để improve performance
4. **Validate server-side** không chỉ client
5. **Use semantic HTML** và ARIA labels
6. **Test on mobile** devices
7. **Handle loading states** properly
8. **Show clear error messages**

## Performance Tips

1. **Lazy load** data khi cần
2. **Use pagination** cho large datasets
3. **Cache API responses** khi phù hợp
4. **Minimize DOM operations**
5. **Use event delegation**
6. **Optimize images** trước upload

## Security

1. **Validate input** cả client và server
2. **Escape output** để prevent XSS
3. **Use CSRF tokens** cho forms
4. **Sanitize file uploads**
5. **Check permissions** server-side
6. **Use HTTPS** cho production

## Roadmap

### Planned Features
- [ ] Drag & drop file upload
- [ ] Multi-language support
- [ ] Export to PDF/Excel
- [ ] Bulk operations
- [ ] Offline mode
- [ ] Real-time collaboration
- [ ] Audit trail
- [ ] Custom validators

## Support

Nếu gặp vấn đề hoặc cần hỗ trợ:
1. Check console logs
2. Review this documentation
3. Check GitHub issues
4. Contact development team

