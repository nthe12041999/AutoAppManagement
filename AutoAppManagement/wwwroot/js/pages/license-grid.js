// Custom Grid Column Configuration for License Grid (defines columns structure)
function customGridColumnConfig() {
    return [
        {
            field: 'licenseKey',
            title: 'License Key',
            type: ColumnTypes.TEXT,
            sortable: true,
            width: '180px'
        },
        {
            field: 'customer',
            title: 'Khách hàng',
            type: ColumnTypes.TEXT,
            sortable: true
        },
        {
            field: 'type',
            title: 'Loại License',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['basic', 'premium', 'enterprise', 'trial']
        },
        {
            field: 'status',
            title: 'Trạng thái',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['active', 'expired', 'expiring', 'suspended', 'pending']
        },
        {
            field: 'createdDate',
            title: 'Ngày tạo',
            type: ColumnTypes.DATE,
            sortable: true
        },
        {
            field: 'expiryDate',
            title: 'Ngày hết hạn',
            type: ColumnTypes.DATE,
            sortable: true
        },
        {
            field: 'maxDevices',
            title: 'Số thiết bị',
            type: ColumnTypes.NUMBER,
            sortable: true
        },
        {
            field: 'price',
            title: 'Giá',
            type: ColumnTypes.MONEY,
            sortable: true
        },
        {
            field: 'isActive',
            title: 'Kích hoạt',
            type: ColumnTypes.BOOL,
            sortable: true
        }
    ];
}

// Action functions for License management
function deleteItem(id) {
    Swal.fire({
        title: 'Xác nhận xóa License',
        text: 'Bạn có chắc chắn muốn xóa license này? Hành động này không thể hoàn tác.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa License',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            // Simulate delete API call
            console.log('Deleting license:', id);
            Swal.fire('Đã xóa!', 'License đã được xóa thành công.', 'success');
            
            // Refresh the grid after delete
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('licenseDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

function approveItem(id) {
    Swal.fire({
        title: 'Phê duyệt License',
        text: 'Bạn có muốn phê duyệt license này?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Phê duyệt',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Approving license:', id);
            Swal.fire('Đã phê duyệt!', 'License đã được phê duyệt thành công.', 'success');
            
            // Refresh the grid
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('licenseDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

function suspendItem(id) {
    Swal.fire({
        title: 'Tạm ngưng License',
        text: 'Bạn có muốn tạm ngưng license này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ffc107',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Tạm ngưng',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Suspending license:', id);
            Swal.fire('Đã tạm ngưng!', 'License đã được tạm ngưng.', 'success');
            
            // Refresh the grid
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('licenseDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

// Initialize when DOM is ready
$(document).ready(function() {
    console.log('License grid page loaded');
});
