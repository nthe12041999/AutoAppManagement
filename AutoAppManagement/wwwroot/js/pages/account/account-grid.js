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
            field: 'LicenseName',
            title: 'License',
            type: ColumnTypes.TEXT,
            sortable: true
        },
        {
            field: 'StatusName',
            title: 'Trạng thái',
            type: ColumnTypes.TEXT,
            sortable: true
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

/**
 * Custom data loading - SIMPLIFIED
 * Chỉ gửi RequestedColumns từ grid config xuống backend
 */
function customDataLoader(pageIndex, pageSize, filter, sortField) {
    return new Promise(async (resolve, reject) => {
        try {
            // Lấy grid config để extract columns
            const gridConfig = window.currentGridConfig || getCustomGridConfig();
            
            // Tạo request đơn giản - chỉ gửi columns
            const request = new PagingRequestBuilder()
                .setPaging(pageIndex, pageSize)
                .setFilter(filter || "")
                .setSort(sortField || "Id")
                .extractColumnsFromGridConfig(gridConfig)  // Chỉ extract columns thôi
                .build();

            console.log('Request với columns từ grid:', request);

            const response = await fetch('/Account/GetPaging', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(request)
            });

            const result = await response.json();
            
            if (result.IsSuccess) {
                console.log('Data loaded:', result.Data);
                resolve(result.Data);
            } else {
                console.error('Error:', result.Message);
                reject(new Error(result.Message));
            }
        } catch (error) {
            console.error('Error in customDataLoader:', error);
            reject(error);
        }
    });
}

/**
 * Custom grid configuration với data loader
 */
function getCustomGridConfig() {
    const config = {
        gridId: 'customerAccountDataGrid',
        columns: customGridColumnConfig(),
        actions: customGridActionsConfig(),
        dataLoader: customDataLoader,  // Sử dụng custom data loader
        pageSize: 10,
        enablePaging: true,
        enableSorting: true,
        enableFiltering: true,
        allowEdit: true,
        allowDelete: true,
        modalFormUrl: '/Account/CustomerForms'
    };
    
    // Store current config globally để customDataLoader có thể access
    window.currentGridConfig = config;
    
    return config;
}

// Custom Grid Actions Configuration for Customer Account Grid
function customGridActionsConfig() {
    return [
        {
            type: 'view',
            title: 'Xem chi tiết',
            icon: 'bi-eye'
        },
        {
            type: 'edit',
            title: 'Chỉnh sửa',
            icon: 'bi-pencil'
        },
        {
            type: 'suspend',
            title: 'Tạm khóa',
            icon: 'bi-lock',
            class: 'btn-warning'
        },
        {
            type: 'delete',
            title: 'Xóa',
            icon: 'bi-trash',
            class: 'btn-danger'
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
