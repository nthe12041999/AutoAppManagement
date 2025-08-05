# JavaScript Mixins Documentation

## Overview
Các JavaScript Mixins được thiết kế để tái sử dụng và mở rộng dễ dàng. Mỗi mixin cung cấp một chức năng cụ thể và có thể kết hợp với nhau.

## Available Mixins

### 1. DataGridMixin
Quản lý DataGrid với Ajax, pagination, selection, và actions.

**Features:**
- Ajax data loading
- Pagination
- Row selection (single/multiple)
- Action buttons
- Loading/Error states
- Export functionality

**Usage:**
```javascript
// Basic usage
const grid = new DataGridMixin('#myTable', {
    apiUrl: '/api/data',
    pageSize: 10
});

// Extended usage
class MyDataGrid extends DataGridMixin {
    renderTableRow(item) {
        return `<tr data-id="${item.id}">...</tr>`;
    }
    
    onAction(action, target, entity, element) {
        switch (action) {
            case 'edit':
                this.editItem(target);
                break;
        }
    }
}
```

### 2. FilterMixin
Quản lý form filter với validation và auto-submit.

**Features:**
- Multiple filter types (text, select, date range, number range)
- Debounced input
- Auto-submit
- Validation
- URL parameter sync

**Usage:**
```javascript
const filter = new FilterMixin('#filterForm', {
    debounceDelay: 300,
    autoSubmit: true
});

// With custom handlers
class MyFilter extends FilterMixin {
    onSubmit() {
        const filters = this.getApiFilters();
        myDataGrid.applyFilters(filters);
    }
}
```

### 3. FormMixin
Quản lý form với validation, auto-save, và Ajax submission.

**Features:**
- Field validation
- Auto-save
- File upload handling
- Change tracking
- Ajax submission

**Usage:**
```javascript
const form = new FormMixin('#myForm', {
    apiUrl: '/api/save',
    autoValidate: true,
    trackChanges: true
});

// With custom validation
class MyForm extends FormMixin {
    onSubmitSuccess(result) {
        alert('Saved successfully!');
        this.reset();
    }
}
```

### 4. ModalMixin
Quản lý modal với dynamic content và form integration.

**Features:**
- Dynamic content loading
- Form integration
- Backdrop/keyboard handling
- Static methods for alerts/confirms

**Usage:**
```javascript
const modal = new ModalMixin('#myModal');

// Static methods
ModalMixin.alert('Hello World!');
const confirmed = await ModalMixin.confirm('Are you sure?');

// Dynamic content
modal.loadFromUrl('/api/modal-content');
```

## Integration Examples

### Complete DataGrid with Filter
```javascript
// 1. Create DataGrid
class ProductGrid extends DataGridMixin {
    constructor() {
        super('#productTable', {
            apiUrl: '/api/products',
            pageSize: 20
        });
    }
    
    renderTableRow(item) {
        return `
            <tr data-id="${item.id}">
                <td><input type="checkbox" data-action="select-row" data-target="${item.id}"></td>
                <td>${item.name}</td>
                <td>${item.price}</td>
                <td>
                    <button data-action="edit" data-target="${item.id}">Edit</button>
                    <button data-action="delete" data-target="${item.id}">Delete</button>
                </td>
            </tr>
        `;
    }
    
    onAction(action, target) {
        switch (action) {
            case 'edit':
                productModal.show({ id: target });
                break;
            case 'delete':
                this.deleteProduct(target);
                break;
        }
    }
}

// 2. Create Filter
class ProductFilter extends FilterMixin {
    constructor(dataGrid) {
        super('#productFilter');
        this.dataGrid = dataGrid;
    }
    
    onSubmit() {
        const filters = this.getApiFilters();
        this.dataGrid.clearFilters();
        Object.keys(filters).forEach(key => {
            this.dataGrid.setFilter(key, filters[key]);
        });
        this.dataGrid.loadData(1);
    }
}

// 3. Create Modal with Form
class ProductModal extends ModalMixin {
    constructor() {
        super('#productModal');
        this.form = new FormMixin('#productForm', {
            apiUrl: '/api/products',
            autoValidate: true
        });
        
        this.form.onSubmitSuccess = (result) => {
            this.hide();
            productGrid.refresh();
        };
    }
}

// 4. Initialize
const productGrid = new ProductGrid();
const productFilter = new ProductFilter(productGrid);
const productModal = new ProductModal();
```

### Form with Validation
```javascript
class UserForm extends FormMixin {
    constructor() {
        super('#userForm', {
            apiUrl: '/api/users',
            autoValidate: true,
            trackChanges: true
        });
    }
    
    // Custom validation rule
    validateRule(value, rule) {
        if (rule === 'unique-email') {
            // Check email uniqueness via API
            return this.checkEmailUnique(value);
        }
        return super.validateRule(value, rule);
    }
    
    onSubmitSuccess(result) {
        ModalMixin.alert('User saved successfully!');
        this.reset();
    }
    
    onSubmitError(error) {
        ModalMixin.alert('Error: ' + error.message, 'Error');
    }
}
```

## Data Attributes Reference

### DataGrid
```html
<!-- Component -->
<div data-component="datagrid" data-table-id="myTable">
    <!-- Table -->
    <table data-table="main" data-entity="product">
        <thead data-section="table-header">
            <tr data-row="header">
                <th data-column="name" data-field="name" data-sortable="true">Name</th>
            </tr>
        </thead>
        <tbody data-section="table-body">
            <tr data-id="123" data-row="data" data-status="active">
                <td data-cell="name" data-value="Product Name">Product Name</td>
                <td data-cell="actions">
                    <button data-action="edit" data-target="123">Edit</button>
                </td>
            </tr>
        </tbody>
    </table>
</div>
```

### Filter
```html
<form data-component="filter" data-target="myTable">
    <input data-filter="search" data-field="name" data-trigger="enter">
    <select data-filter="category" data-field="category" data-trigger="change">
        <option value="">All</option>
        <option value="electronics">Electronics</option>
    </select>
    <button data-action="search">Search</button>
    <button data-action="reset">Reset</button>
</form>
```

### Form
```html
<form data-component="form" data-api-url="/api/save">
    <input data-field="name" data-validate="required|min:3" data-label="Name">
    <input data-field="email" data-validate="required|email" data-label="Email">
    <button data-action="submit">Save</button>
    <button data-action="reset">Reset</button>
</form>
```

### Modal
```html
<div data-component="modal" data-size="lg">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header" data-section="header">
                <h5 data-element="title">Modal Title</h5>
                <button data-action="close">×</button>
            </div>
            <div class="modal-body" data-element="body">
                Content here
            </div>
            <div class="modal-footer" data-element="footer">
                <button data-action="close">Close</button>
                <button data-action="save">Save</button>
            </div>
        </div>
    </div>
</div>
```

## Best Practices

1. **Extend, don't modify**: Always extend mixins instead of modifying them directly
2. **Use data attributes**: Leverage data attributes for configuration and selection
3. **Override event hooks**: Use provided event hooks for custom behavior
4. **Combine mixins**: Use multiple mixins together for complex functionality
5. **Keep it simple**: Each mixin should have a single responsibility

## Browser Support
- Modern browsers (ES6+)
- jQuery 3.x required
- Bootstrap 5.x for styling (optional)
