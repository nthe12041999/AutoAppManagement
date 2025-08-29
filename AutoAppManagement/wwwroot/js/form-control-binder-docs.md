# Form Control Binder Documentation

## Tổng quan

Form Control Binder là hệ thống data-binding tự động cho phép tạo form controls từ các data attributes. Hệ thống này hỗ trợ 25+ loại control khác nhau và có thể tùy chỉnh CSS classes.

## Cách sử dụng cơ bản

### 1. Syntax cơ bản
```html
<div data-type="@ControlType.Text" 
     data-id="fieldId" 
     data-name="fieldName" 
     data-label="Field Label" 
     data-placeholder="Enter value"
     data-required></div>
```

### 2. Khởi tạo
```javascript
// Auto-init toàn bộ document
window.formControlBinder.init();

// Init cho container cụ thể
window.formControlBinder.init('#myForm');
```

## Data Attributes

### Attributes cơ bản
- `data-type` - Loại control (bắt buộc)
- `data-id` - ID của control
- `data-name` - Name attribute
- `data-label` - Label hiển thị
- `data-value` - Giá trị mặc định
- `data-placeholder` - Placeholder text

### Attributes trạng thái
- `data-required` - Bắt buộc
- `data-disabled` - Disabled
- `data-readonly` - Read-only
- `data-checked` - Checked (cho checkbox/radio)

### Attributes styling
- `data-class` - **CSS classes tùy chỉnh sẽ được append vào class mặc định**
- `data-css-class` - CSS classes (deprecated, dùng data-class)
- `data-help-text` - Help text hiển thị dưới control

### Attributes cho control cụ thể
- `data-options` - JSON options cho Select/Radio/Checkbox
- `data-min/max/step` - Cho Number/Range
- `data-rows` - Cho Textarea
- `data-accept` - Cho File upload
- `data-multiple` - Cho File upload/Select

## CSS Classes với data-class

### Cách hoạt động
Attribute `data-class` sẽ **append thêm** CSS classes vào class mặc định của control, không thay thế.

### Ví dụ:
```html
<!-- Input text với border màu đỏ -->
<div data-type="@ControlType.Text" 
     data-class="border-danger"
     data-label="Input với border đỏ"></div>

<!-- Kết quả: class="form-control border-danger" -->

<!-- Checkbox lớn -->
<div data-type="@ControlType.Checkbox" 
     data-class="form-check-input-lg"
     data-label="Checkbox lớn"></div>

<!-- Kết quả: class="form-check-input form-check-input-lg" -->

<!-- Select với border tròn -->
<div data-type="@ControlType.Select" 
     data-class="rounded-pill border-success"
     data-label="Select tròn"></div>

<!-- Kết quả: class="form-select rounded-pill border-success" -->
```

### CSS Classes phổ biến có thể dùng:

#### Bootstrap Border Classes:
- `border-primary`, `border-secondary`, `border-success`
- `border-danger`, `border-warning`, `border-info`
- `border-light`, `border-dark`

#### Bootstrap Background Classes:
- `bg-light`, `bg-dark`, `bg-primary`
- `bg-success`, `bg-warning`, `bg-danger`

#### Bootstrap Sizing Classes:
- `form-control-sm`, `form-control-lg`
- `form-select-sm`, `form-select-lg`
- `form-check-input-lg`

#### Bootstrap Utility Classes:
- `rounded`, `rounded-pill`, `rounded-0`
- `shadow`, `shadow-sm`, `shadow-lg`
- `text-uppercase`, `text-lowercase`

## Control Types

### Text Inputs
- `@ControlType.Text` - Input text
- `@ControlType.Email` - Input email
- `@ControlType.Password` - Input password
- `@ControlType.Number` - Input number
- `@ControlType.Tel` - Input telephone
- `@ControlType.Url` - Input URL

### Advanced Inputs
- `@ControlType.Textarea` - Textarea
- `@ControlType.Select` - Dropdown select
- `@ControlType.MultiSelect` - Multi-select dropdown
- `@ControlType.Date` - Date picker
- `@ControlType.DateTime` - DateTime picker
- `@ControlType.Time` - Time picker

### Choice Controls
- `@ControlType.Radio` - Radio buttons group
- `@ControlType.Checkbox` - Single checkbox
- `@ControlType.CheckboxGroup` - Multiple checkboxes
- `@ControlType.Toggle` - Switch toggle
- `@ControlType.Switch` - Alias cho Toggle

### Special Controls
- `@ControlType.File` - File upload
- `@ControlType.Image` - Image upload
- `@ControlType.Color` - Color picker
- `@ControlType.Range` - Range slider
- `@ControlType.Hidden` - Hidden input
- `@ControlType.Display` - Read-only display

## Ví dụ thực tế

### Form đăng ký người dùng
```html
<form id="userForm">
    <div class="row">
        <div class="col-md-6">
            <div data-type="@ControlType.Text" 
                 data-id="fullName" 
                 data-name="fullName" 
                 data-label="Họ và tên" 
                 data-class="text-capitalize"
                 data-required></div>
        </div>
        <div class="col-md-6">
            <div data-type="@ControlType.Email" 
                 data-id="email" 
                 data-name="email" 
                 data-label="Email" 
                 data-class="border-primary"
                 data-required></div>
        </div>
    </div>
    
    <div data-type="@ControlType.Select" 
         data-id="country" 
         data-name="country" 
         data-label="Quốc gia"
         data-class="rounded-pill"
         data-options='[
             {"value":"","text":"-- Chọn quốc gia --"},
             {"value":"vn","text":"Việt Nam"},
             {"value":"us","text":"Hoa Kỳ"}
         ]'></div>
         
    <div data-type="@ControlType.Checkbox" 
         data-id="terms" 
         data-name="terms" 
         data-label="Tôi đồng ý với điều khoản sử dụng" 
         data-class="border-success"
         data-required></div>
</form>

<script>
$(document).ready(function() {
    window.formControlBinder.init('#userForm');
});
</script>
```

### Khởi tạo và xử lý events
```javascript
$(document).ready(function() {
    // Khởi tạo form control binder
    window.formControlBinder.init('#myForm');
    
    // Lắng nghe event khi control được bind
    document.addEventListener('controlBound', function(e) {
        console.log('Control bound:', e.detail);
    });
    
    // Lấy dữ liệu form
    $('#submitBtn').on('click', function() {
        const formData = new FormData(document.getElementById('myForm'));
        const data = Object.fromEntries(formData.entries());
        console.log('Form data:', data);
    });
});
```

## Form Validation

### Tự động setup validation
```javascript
// Khi init form, validation sẽ được setup tự động
window.formControlBinder.init('#myForm');

// Listen for validation events
document.getElementById('myForm').addEventListener('formValidated', function(e) {
    if (e.detail.isValid) {
        console.log('Form valid!', e.detail.formData);
    } else {
        console.log('Form invalid!');
    }
});
```

### Validation rules được hỗ trợ
- **required** - Bắt buộc nhập
- **pattern** - Regex pattern
- **minLength/maxLength** - Độ dài tối thiểu/tối đa
- **min/max** - Giá trị tối thiểu/tối đa (cho number)
- **email** - Validation email tự động
- **url** - Validation URL tự động

### Validation methods
```javascript
// Validate toàn bộ form
const isValid = window.formControlBinder.validateForm(form);

// Validate một field
const fieldValid = window.formControlBinder.validateField(field);

// Lấy danh sách lỗi
const errors = window.formControlBinder.getValidationSummary(form);

// Reset validation state
window.formControlBinder.resetValidation(form);

// Lấy form data
const data = window.formControlBinder.getFormData(form);
```

### Custom validation messages
```html
<div data-type="@ControlType.Text"
     data-required
     data-required-message="Vui lòng nhập họ tên"
     data-pattern="^[a-zA-Z\s]+$"
     data-pattern-message="Họ tên chỉ được chứa chữ cái"></div>
```

## Lưu ý quan trọng

1. **data-class append, không replace**: CSS classes trong `data-class` sẽ được thêm vào class mặc định
2. **Auto-init**: Hệ thống tự động khởi tạo khi DOM ready
3. **Event-driven**: Trigger custom event `controlBound` khi control được bind
4. **Bootstrap compatible**: Tất cả controls sử dụng Bootstrap classes
5. **Vietnamese support**: Hỗ trợ ký tự tiếng Việt trong auto-generate ID
6. **Auto validation**: Form validation được setup tự động khi init
7. **Real-time validation**: Validation chạy real-time khi user input/change

## Troubleshooting

### Control không được render
- Kiểm tra `data-type` có đúng không
- Đảm bảo `form-control-binder.js` đã được load
- Kiểm tra console có lỗi JavaScript không

### CSS classes không áp dụng
- Kiểm tra syntax `data-class` có đúng không
- Đảm bảo Bootstrap CSS đã được load
- Kiểm tra class name có tồn tại không

### Options không hiển thị
- Kiểm tra JSON trong `data-options` có valid không
- Sử dụng single quotes cho attribute, double quotes cho JSON
- Kiểm tra escape characters
