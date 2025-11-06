# Hướng dẫn sử dụng View Mode cho Forms

## Tổng quan

View Mode là tính năng cho phép hiển thị form ở chế độ chỉ xem (read-only), tất cả các input, select, textarea và button sẽ bị vô hiệu hóa và hiển thị dưới dạng text thuần để người dùng có thể xem thông tin mà không thể chỉnh sửa.

## Cách sử dụng

### 1. Sử dụng Function Global (Khuyến nghị)

```javascript
// Sử dụng function global để set view mode cho bất kỳ form nào
window.setFormViewMode('#formId', 'Tiêu đề modal tùy chọn');

// Ví dụ:
window.setFormViewMode('#customerForm', 'Xem Chi Tiết Khách Hàng');
window.setFormViewMode('#licenseForm', 'Xem Chi Tiết License');
window.setFormViewMode('#roleForm', 'Xem Chi Tiết Vai Trò');
```

### 2. Sử dụng trong Detail Classes

```javascript
class YourDetailClass {
    setViewMode() {
        // Sử dụng function global
        if (typeof window.setFormViewMode === 'function') {
            window.setFormViewMode('#yourForm', 'Xem Chi Tiết Your Entity');
        } else {
            // Fallback method nếu global function chưa load
            this.setViewModeManual();
        }
    }
    
    setViewModeManual() {
        // Implementation manual tương tự như trong account-detail.js
    }
}
```

### 3. Trigger View Mode từ DataLoaded Event

```javascript
$modal.on('dataLoaded', (e, params) => {
    const { data, mode } = params;
    
    // Bind data vào form...
    
    // Nếu mode = 'view', set view mode
    if (mode === 'view') {
        window.setFormViewMode('#yourForm', 'Xem Chi Tiết');
    }
});
```

## Các tính năng của View Mode

### 1. Disable Form Controls
- Tất cả `input`, `select`, `textarea` bị disable
- Các button submit/save bị ẩn
- Form controls được thêm class `view-mode-disabled`

### 2. Hiển thị dữ liệu dạng Text
- **Select boxes**: Hiển thị text của option được chọn
- **Checkboxes**: Hiển thị icon ✓ Có hoặc ✗ Không 
- **Radio buttons**: Hiển thị text của option được chọn
- **Textareas**: Hiển thị text với white-space preserved
- **Date inputs**: Hiển thị ngày theo format Việt Nam
- **Text inputs**: Hiển thị text thuần
- **Switch controls**: Hiển thị icon toggle với text Bật/Tắt

### 3. UI Improvements
- Thêm alert box thông báo đang ở chế độ xem
- Thay đổi tiêu đề modal với icon eye
- Thay đổi button "Hủy" thành "Đóng"
- Ẩn file upload controls
- Thêm class `view-mode` cho form

## CSS Classes

### Classes được tự động thêm:
- `.view-mode-disabled`: Cho các control bị disable
- `.view-mode-display`: Cho các text hiển thị thay thế
- `.view-mode-indicator`: Cho alert box thông báo
- `.view-mode`: Cho form element

### CSS có sẵn:
```css
.view-mode-disabled {
    background-color: #f8f9fa !important;
    border-color: #e9ecef !important;
    color: #6c757d !important;
    cursor: not-allowed !important;
}

.view-mode-display {
    background-color: transparent !important;
    border: none !important;
    padding: 0.375rem 0 !important;
    font-weight: 500;
    color: #495057;
    min-height: 1.5rem;
}

.view-mode-indicator {
    border-left: 4px solid #007bff !important;
    background-color: #e7f3ff !important;
    border-color: #b3d9ff !important;
}
```

## Ví dụ Implementation

### Customer Form
```javascript
// Trong account-detail.js
class AccountDetail {
    setViewMode() {
        window.setFormViewMode('#customerForm', 'Xem Chi Tiết Khách Hàng');
    }
}
```

### License Form
```javascript
// Trong license-detail.js
class LicenseDetail {
    setViewMode() {
        window.setFormViewMode('#licenseForm', 'Xem Chi Tiết License');
    }
}
```

### Role Form  
```javascript
// Trong role-detail.js
class RoleDetail {
    setViewMode() {
        window.setFormViewMode('#roleForm', 'Xem Chi Tiết Vai Trò');
    }
}
```

## Data Binding và View Mode

### Workflow chuẩn:
1. Load data từ API
2. Bind data vào form controls
3. Nếu mode = 'view', call `setFormViewMode()`

```javascript
$modal.on('dataLoaded', (e, params) => {
    const { data, mode } = params;
    
    // 1. Bind data
    window.formControlBinder.bindData('#yourForm', data);
    
    // 2. Set view mode if needed
    if (mode === 'view') {
        window.setFormViewMode('#yourForm', 'Xem Chi Tiết');
    }
});
```

## Lưu ý quan trọng

### 1. Load Order
- Đảm bảo `base-detail.js` được load trước khi sử dụng `setFormViewMode`
- Có fallback method nếu global function chưa sẵn sàng

### 2. Form ID
- Luôn sử dụng ID chính xác của form
- Kiểm tra form có tồn tại trước khi call function

### 3. Modal Integration
- Function tự động detect nếu form nằm trong modal
- Tự động cập nhật modal title và footer buttons

### 4. Responsive
- View mode tương thích với responsive design
- CSS đã được tối ưu cho mobile

## Troubleshooting

### Lỗi thường gặp:

1. **Function not found**: 
   - Kiểm tra `base-detail.js` đã được include
   - Sử dụng fallback method

2. **Form không được disable**:
   - Kiểm tra form selector đúng chưa
   - Kiểm tra form có controls cần disable không

3. **CSS không hiển thị đúng**:
   - Kiểm tra `customer-forms.css` đã được include
   - Kiểm tra order của CSS files

## Performance

- Function được tối ưu để chỉ thực hiện 1 lần DOM traversal
- Sử dụng efficient selectors
- Không ảnh hưởng đến performance của page