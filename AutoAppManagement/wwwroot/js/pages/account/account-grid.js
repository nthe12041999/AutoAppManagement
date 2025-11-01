// Custom Grid Column Configuration for Customer Account Grid
function customGridColumnConfig() {
    return [
        {
            field: 'Avatar',
            title: '',
            type: ColumnTypes.TEXT,
            width: '60px',
            sortable: false
        },
        {
            field: 'Name',
            title: 'Họ và tên',
            type: ColumnTypes.TEXT,
            sortable: true
        },
        {
            field: 'Email',
            title: 'Email',
            type: ColumnTypes.TEXT,
            sortable: true
        },
        {
            field: 'Phone',
            title: 'Số điện thoại',
            type: ColumnTypes.TEXT,
            sortable: true
        },
        {
            field: 'Role',
            title: 'License',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['customer', 'premium', 'vip', 'trial']
        },
        {
            field: 'Status',
            title: 'Trạng thái',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['active', 'inactive', 'suspended', 'pending']
        },
        {
            field: 'LastLogin',
            title: 'Lần cuối đăng nhập',
            type: ColumnTypes.DATETIME,
            sortable: true,
            format: {
                relative: true // Show "2 giờ trước"
            }
        },
        {
            field: 'CreatedDate',
            title: 'Ngày tạo',
            type: ColumnTypes.DATE,
            sortable: true
        },
        {
            field: 'IsVerified',
            title: 'Đã xác thực',
            type: ColumnTypes.BOOL,
            sortable: true
        }
    ];
}

// Action functions for Customer Account management
function deleteItem(id) {
    Swal.fire({
        title: 'Xác nhận xóa khách hàng',
        text: 'Bạn có chắc chắn muốn xóa tài khoản khách hàng này? Hành động này không thể hoàn tác.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa tài khoản',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Deleting customer account:', id);
            Swal.fire('Đã xóa!', 'Tài khoản khách hàng đã được xóa thành công.', 'success');
            
            // Refresh the grid after delete
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('customerAccountDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

function approveItem(id) {
    Swal.fire({
        title: 'Phê duyệt tài khoản khách hàng',
        text: 'Bạn có muốn phê duyệt tài khoản khách hàng này?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Phê duyệt',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Approving customer account:', id);
            Swal.fire('Đã phê duyệt!', 'Tài khoản khách hàng đã được phê duyệt.', 'success');
            
            // Refresh the grid
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('customerAccountDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

function suspendItem(id) {
    Swal.fire({
        title: 'Tạm khóa tài khoản khách hàng',
        text: 'Bạn có muốn tạm khóa tài khoản khách hàng này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ffc107',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Tạm khóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Suspending customer account:', id);
            Swal.fire('Đã tạm khóa!', 'Tài khoản khách hàng đã bị tạm khóa.', 'success');
            
            // Refresh the grid
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('customerAccountDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

// Initialize when DOM is ready
$(document).ready(function() {
    console.log('Customer account grid page loaded');
});
