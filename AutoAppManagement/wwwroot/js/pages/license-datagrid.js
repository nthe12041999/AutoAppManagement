/**
 * License DataGrid Page
 * Handles license management with DataGrid component
 */

// License DataGrid Configuration
class LicenseDataGrid extends DataGridMixin {
    constructor(container) {
        super(container);
        
        // Define columns
        this.columns = [
            { name: 'select', type: 'Checkbox', width: '50px', sortable: false },
            { name: 'licenseKey', title: 'License Key', sortable: true },
            { name: 'customerName', title: 'Khách hàng', sortable: true },
            { name: 'type', title: 'Loại License', sortable: true },
            { name: 'maxUsers', title: 'Số User', sortable: true },
            { name: 'expiryDate', title: 'Ngày hết hạn', sortable: true },
            { name: 'status', title: 'Trạng thái', sortable: true },
            { name: 'actions', type: 'Actions', width: '120px', sortable: false }
        ];
        
        // Define action buttons
        this.actionButtons = [
            { action: 'view', title: 'Xem chi tiết', icon: 'bi-eye', class: 'btn-outline-primary' },
            { action: 'edit', title: 'Chỉnh sửa', icon: 'bi-pencil', class: 'btn-outline-warning' },
            { action: 'renew', title: 'Gia hạn', icon: 'bi-arrow-clockwise', class: 'btn-outline-success' },
            { action: 'suspend', title: 'Tạm dừng', icon: 'bi-pause', class: 'btn-outline-danger' }
        ];
    }
    
    // Handle stats update
    onDataLoaded(response) {
        if (response.stats) {
            $('#totalLicenses').text(response.stats.total || 0);
            $('#activeLicenses').text(response.stats.active || 0);
            $('#expiringSoon').text(response.stats.expiringSoon || 0);
            $('#expiredLicenses').text(response.stats.expired || 0);
        }
    }
    
    // Initialize dropdowns after table is rendered
    onTableRendered(data) {
        console.log('🔧 Initializing license dropdowns after table render...');
        
        setTimeout(() => {
            if (typeof bootstrap !== 'undefined') {
                const dropdownElements = this.container.find('[data-bs-toggle="dropdown"]');
                console.log('📋 Found dropdown elements:', dropdownElements.length);
                
                dropdownElements.each(function() {
                    const element = this;
                    if (!bootstrap.Dropdown.getInstance(element)) {
                        new bootstrap.Dropdown(element);
                        console.log('✅ License dropdown initialized');
                    }
                });
                
                console.log('✅ All license dropdowns initialized');
            } else {
                console.warn('❌ Bootstrap not available for dropdown initialization');
            }
        }, 100);
    }

    // Handle action button clicks
    onActionClick(action, rowData) {
        console.log('🎯 Action clicked:', action, rowData);

        switch(action) {
            case 'view':
                this.loadLicenseModal('view', rowData.id);
                break;

            case 'edit':
                this.loadLicenseModal('edit', rowData.id);
                break;

            case 'renew':
                this.showRenewModal(rowData);
                break;

            case 'suspend':
                this.confirmSuspend(rowData);
                break;

            default:
                console.warn('Unknown action:', action);
        }
    }

    // Show renew modal
    showRenewModal(rowData) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Gia hạn License',
                html: `
                    <div class="text-start">
                        <div class="mb-3">
                            <label class="form-label">License Key:</label>
                            <input type="text" class="form-control" value="${rowData.licenseKey}" readonly>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Ngày hết hạn hiện tại:</label>
                            <input type="text" class="form-control" value="${rowData.expiryDate}" readonly>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Ngày hết hạn mới:</label>
                            <input type="date" class="form-control" id="newExpiryDate">
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Lý do gia hạn:</label>
                            <textarea class="form-control" id="renewReason" rows="3"></textarea>
                        </div>
                    </div>
                `,
                showCancelButton: true,
                confirmButtonText: 'Gia hạn',
                cancelButtonText: 'Hủy',
                confirmButtonColor: '#28a745',
                preConfirm: () => {
                    const newDate = document.getElementById('newExpiryDate').value;
                    const reason = document.getElementById('renewReason').value;
                    if (!newDate) {
                        Swal.showValidationMessage('Vui lòng chọn ngày hết hạn mới');
                        return false;
                    }
                    return { newDate, reason };
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    this.renewLicense(rowData.id, result.value);
                }
            });
        }
    }

    // Confirm suspend license
    confirmSuspend(rowData) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Tạm dừng License',
                text: `Bạn có chắc chắn muốn tạm dừng license "${rowData.licenseKey}"?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Tạm dừng',
                cancelButtonText: 'Hủy'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.suspendLicense(rowData.id);
                }
            });
        }
    }
}

// License Filter Configuration
class LicenseFilter extends FilterMixin {
    constructor(container) {
        super(container);
        
        this.filters = {
            search: '#searchInput',
            type: '#typeFilter',
            status: '#statusFilter',
            dateFrom: '#dateFrom',
            dateTo: '#dateTo'
        };
    }

    // Load license modal with specific mode
    loadLicenseModal(mode, id) {
        // Show modal first
        const modal = new bootstrap.Modal(document.getElementById('licenseModal'));
        modal.show();

        // Load content
        fetch(`/License/GetLicenseForm?mode=${mode}&id=${id}`)
            .then(response => response.text())
            .then(html => {
                document.getElementById('licenseModalBody').innerHTML = html;

                // Update modal title
                let title = '';
                let icon = '';
                switch(mode) {
                    case 'create':
                        title = 'Tạo License mới';
                        icon = 'plus-circle';
                        break;
                    case 'edit':
                        title = 'Chỉnh sửa License';
                        icon = 'pencil-square';
                        break;
                    case 'view':
                        title = 'Chi tiết License';
                        icon = 'eye';
                        break;
                }
                document.getElementById('licenseModalLabel').innerHTML = `<i class="bi bi-${icon} me-2"></i>${title}`;
            })
            .catch(error => {
                console.error('Error loading license form:', error);
                document.getElementById('licenseModalBody').innerHTML = `
                    <div class="alert alert-danger">
                        <i class="bi bi-exclamation-triangle me-2"></i>
                        Có lỗi xảy ra khi tải form. Vui lòng thử lại.
                    </div>
                `;
            });
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 Initializing License Management page...');
    
    // Initialize DataGrid
    const licenseGrid = new LicenseDataGrid($('[data-component="datagrid"]'));
    
    // Initialize Filter
    const licenseFilter = new LicenseFilter($('.card').eq(1)); // Filter card
    
    // Connect filter to grid
    licenseFilter.onFilterChange = function(filters) {
        licenseGrid.applyFilters(filters);
    };
    
    // Global reference for debugging
    window.licenseGrid = licenseGrid;
    window.licenseFilter = licenseFilter;
    
    console.log('🎉 License Management page ready!');
});

// Export for global access
window.LicenseDataGrid = LicenseDataGrid;
window.LicenseFilter = LicenseFilter;
