// Custom Grid Column Configuration for Admin Account Grid
function customGridColumnConfig() {
    return [
        {
            field: 'Avatar',
            title: 'Avatar',
            type: ColumnTypes.TEXT,
            width: '60px',
            sortable: false
        },
        {
            field: 'FullName',
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
            title: 'Vai trò',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['super_admin', 'admin', 'moderator', 'support']
        },
        {
            field: 'Status',
            title: 'Trạng thái',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['active', 'inactive', 'locked', 'pending']
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

// Action functions for Admin Account management
function deleteItem(id) {
    Swal.fire({
        title: 'Xác nhận xóa Admin',
        text: 'Bạn có chắc chắn muốn xóa tài khoản admin này? Hành động này không thể hoàn tác.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa tài khoản',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Deleting admin account:', id);
            Swal.fire('Đã xóa!', 'Tài khoản admin đã được xóa thành công.', 'success');
            
            // Refresh the grid after delete
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('adminAccountDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

function approveItem(id) {
    Swal.fire({
        title: 'Phê duyệt tài khoản Admin',
        text: 'Bạn có muốn phê duyệt tài khoản admin này?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Phê duyệt',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Approving admin account:', id);
            Swal.fire('Đã phê duyệt!', 'Tài khoản admin đã được phê duyệt.', 'success');
            
            // Refresh the grid
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('adminAccountDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

function suspendItem(id) {
    Swal.fire({
        title: 'Khóa tài khoản Admin',
        text: 'Bạn có muốn khóa tài khoản admin này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ffc107',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Khóa tài khoản',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Locking admin account:', id);
            Swal.fire('Đã khóa!', 'Tài khoản admin đã bị khóa.', 'success');
            
            // Refresh the grid
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('adminAccountDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

// Initialize when DOM is ready
$(document).ready(function() {
    console.log('Admin account grid page loaded');
});
