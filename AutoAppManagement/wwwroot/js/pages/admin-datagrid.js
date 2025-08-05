/**
 * Admin DataGrid Page Script
 * Handles admin management with DataGrid and Filter components
 */

// Admin DataGrid Implementation - Clean Override!
class AdminDataGrid extends DataGridMixin {
    constructor(selector, options = {}) {
        super(selector, options);
    }

    // Override method to return actions (Product Catalog style)
    getActionButtons() {
        const baseUrl = this.options.baseUrl;
        return [
            {
                action: 'view',
                title: 'Xem chi tiết',
                icon: 'bi bi-eye',
                cssClass: 'btn-outline-primary',
                redirectUrl: `${baseUrl}/Details/{id}`
            },
            {
                action: 'edit',
                title: 'Chỉnh sửa',
                icon: 'bi bi-pencil',
                cssClass: 'btn-outline-warning',
                redirectUrl: `${baseUrl}/Edit/{id}`
            },
            {
                action: 'toggle-status',
                title: 'Khóa/Mở',
                icon: 'bi bi-lock',
                cssClass: 'btn-outline-secondary',
                confirm: true,
                confirmMessage: 'Bạn có chắc chắn muốn thay đổi trạng thái của admin này?',
                apiUrl: `${baseUrl}/ToggleStatus`,
                method: 'POST',
                successMessage: 'Đã thay đổi trạng thái thành công!',
                errorMessage: 'Có lỗi xảy ra khi thay đổi trạng thái',
                refreshGrid: true
            }
        ];
    }

    // Handle stats update
    onDataLoaded(response) {
        if (response.stats) {
            $('#totalAdmins').text(response.stats.total || 0);
            $('#activeAdmins').text(response.stats.active || 0);
            $('#onlineAdmins').text(response.stats.online || 0);
        }
    }

    // Initialize dropdowns after table is rendered
    onTableRendered(data) {
        console.log('🔧 Initializing dropdowns after table render...');

        // Initialize all Bootstrap dropdowns
        setTimeout(() => {
            if (typeof bootstrap !== 'undefined') {
                const dropdownElements = this.container.find('[data-bs-toggle="dropdown"]');
                console.log('📋 Found dropdown elements:', dropdownElements.length);

                dropdownElements.each(function() {
                    const element = this;
                    if (!bootstrap.Dropdown.getInstance(element)) {
                        new bootstrap.Dropdown(element);
                        console.log('✅ Dropdown initialized for element');
                    }
                });

                console.log('✅ All dropdowns initialized');
            } else {
                console.warn('❌ Bootstrap not available for dropdown initialization');
            }
        }, 100);
    }
}

// Admin Filter Implementation
class AdminFilter extends FilterMixin {
    constructor(selector, dataGrid) {
        super(selector, {
            targetGrid: dataGrid,
            autoSubmit: false,
            resetOnSubmit: false
        });
    }
    
    onSubmit() {
        // Apply filters to grid
        this.targetGrid.applyFilters(this.getFilters());
    }
    
    onReset() {
        // Reset grid filters
        this.targetGrid.resetFilters();
    }
}

// Page initialization
window.addEventListener('load', function() {
    console.log('🚀 Initializing Admin DataGrid page...');

    // Check if jQuery is available
    if (typeof $ === 'undefined') {
        console.error('❌ jQuery is not loaded!');
        return;
    }

    console.log('jQuery version:', $.fn.jquery);
    console.log('DataGridMixin available:', typeof DataGridMixin);

    try {
        // Initialize DataGrid
        const adminGrid = new AdminDataGrid('[data-component="datagrid"]');
        console.log('✅ AdminDataGrid initialized');

        // Initialize Filter (only if filter component exists)
        const filterElement = $('[data-component="filter"]');
        let adminFilter = null;
        if (filterElement.length > 0) {
            adminFilter = new AdminFilter('[data-component="filter"]', adminGrid);
            console.log('✅ AdminFilter initialized');
        } else {
            console.log('ℹ️ No filter component found, skipping filter initialization');
        }
        
        // Global reference for debugging
        window.adminGrid = adminGrid;
        window.adminFilter = adminFilter;

        console.log('🎉 Admin DataGrid page ready!');
        
    } catch (error) {
        console.error('❌ Error initializing Admin DataGrid page:', error);
    }
});

// Export for global access
window.AdminDataGrid = AdminDataGrid;
window.AdminFilter = AdminFilter;
