// Custom Grid Column Configuration for Demo Grid (defines columns structure)
function customGridColumnConfig() {
    return [
        {
            field: 'Avatar',
            title: 'Avatar',
            type: ColumnTypes.TEXT, // Using enum constant
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
            enumValues: ['admin', 'user', 'guest', 'vip'] // Define possible values
        },
        {
            field: 'Status',
            title: 'Trạng thái',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['active', 'inactive', 'pending', 'suspended']
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
            field: 'Salary',
            title: 'Lương',
            type: ColumnTypes.MONEY,
            sortable: true
        },
        {
            field: 'BirthDate',
            title: 'Ngày sinh',
            type: ColumnTypes.DATE,
            sortable: true
        },
        {
            field: 'IsActive',
            title: 'Hoạt động',
            type: ColumnTypes.BOOL,
            sortable: true
        }
    ];
}

// Action functions (using default names that auto-generated actions will call)
// Note: viewItem and editItem are now handled by loadDetailFormModal in data-grid.js
// function viewItem(id) {
//     // Redirect to detail page instead of modal
//     location.href = '/Demo/DetailDemo?userId=' + id;
// }

// function editItem(id) {
//     // Show edit info and redirect to detail page
//     Swal.fire({
//         title: 'Chỉnh sửa User',
//         text: 'Chuyển đến trang chi tiết để xem form chỉnh sửa (User ID: ' + id + ')',
//         icon: 'info',
//         showCancelButton: true,
//         confirmButtonText: 'Đi đến trang chi tiết',
//         cancelButtonText: 'Hủy'
//     }).then((result) => {
//         if (result.isConfirmed) {
//             location.href = '/Demo/DetailDemo?userId=' + id + '&mode=edit';
//         }
//     });
// }

function deleteItem(id) {
    Swal.fire({
        title: 'Xác nhận xóa',
        text: 'Bạn có chắc chắn muốn xóa user này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            // Simulate delete API call
            console.log('Deleting user:', id);
            Swal.fire('Đã xóa!', 'User đã được xóa thành công.', 'success');
            
            // Refresh the grid after delete
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('demoUserGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

// Demo statistics toggle functionality
function initStatisticsToggle() {
    const $toggleBtn = $('#toggleDemoStats');
    const $stats = $('#demoStatistics');
    const $toggleText = $('#toggleDemoStatsText');

    if ($toggleBtn.length && $stats.length && $toggleText.length) {
        $toggleBtn.on('click', function() {
            if ($stats.is(':hidden')) {
                $stats.show();
                $toggleText.text('Ẩn thống kê');
                $(this).removeClass('btn-outline-secondary').addClass('btn-secondary');
            } else {
                $stats.hide();
                $toggleText.text('Hiện thống kê');
                $(this).removeClass('btn-secondary').addClass('btn-outline-secondary');
            }
        });
    }
}

// Initialize when DOM is ready
$(document).ready(function() {
    // Initialize statistics toggle
    initStatisticsToggle();
});
