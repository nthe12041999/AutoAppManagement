// Custom Grid Column Configuration for Customer Account Grid
function customGridColumnConfig() {
    return [
        //{
        //    field: 'Avatar',
        //    title: '',
        //    type: ColumnTypes.TEXT,
        //    width: '60px',
        //    sortable: false
        //},
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
 * Custom expand columns default - Thêm các columns luôn cần thiết (không hiển thị trên grid)
 * Ví dụ: Status, CreatedBy để xử lý logic FE
 */
function customExpandColumnDefault() {
    return ['DateOfBirth', 'IsLocked']; // Luôn lấy thêm Status và IsLocked để xử lý màu sắc, disable actions, etc.
}

/**
 * Custom column renderer - Customize how columns are displayed
 * @param {object} item - Data item
 * @param {object} column - Column configuration
 * @param {*} value - Current value
 * @param {Array} columns - All columns
 * @returns {string|null} - HTML string or null to use default renderer
 */
function customGridColumnRenderer(item, column, value, columns) {
    // Custom renderer for IsVerified column
    if (column.field === 'IsVerified') {
        const isVerified = value === true || value === 'true' || value === 1 || value === '1';
        const itemId = item.ID || item.Id || item.id;
        
        if (isVerified) {
            // Already verified - show green checkmark
            return `<span class="text-success fw-bold">
                <i class="bi bi-check-circle-fill me-1"></i>Có
            </span>`;
        } else {
            // Not verified - show red "Không" with verify button
            return `<div class="d-flex align-items-center gap-2">
                <span class="text-danger fw-bold">
                    <i class="bi bi-x-circle-fill me-1"></i>Không
                </span>
                <button type="button" 
                        class="btn btn-sm btn-outline-success" 
                        onclick="verifyCustomerAccount(${itemId}); return false;"
                        title="Xác thực tài khoản">
                    <i class="bi bi-check-circle me-1"></i>Xác thực
                </button>
            </div>`;
        }
    }
    
    // Return null to use default renderer for other columns
    return null;
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
        entity: 'Khách hàng',
        entityPlural: 'khách hàng',
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

/**
 * Verify customer account
 * @param {number} id - Customer account ID
 */
function verifyCustomerAccount(id) {
    Swal.fire({
        title: 'Xác thực tài khoản khách hàng',
        text: 'Bạn có chắc chắn muốn xác thực tài khoản khách hàng này?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Xác thực',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            // Call API to verify account
            callPostAPIAuthen('/Account/VerifyCustomerAccount', { id: id },
                (response) => {
                    if (response && response.IsSuccess) {
                        Swal.fire('Thành công!', 'Tài khoản khách hàng đã được xác thực.', 'success');
                        
                        // Refresh the grid
                        if (window.dataGridInstance) {
                            // Try both possible container IDs
                            let config = window.dataGridInstance.getGrid('accountDataGrid');
                            if (!config) {
                                config = window.dataGridInstance.getGrid('customerAccountDataGrid');
                            }
                            if (config) {
                                window.dataGridInstance.refreshData(config);
                            }
                        }
                        
                        // Reload statistics
                        loadAccountStatistics();
                    } else {
                        Swal.fire('Lỗi!', response.Message || 'Không thể xác thực tài khoản.', 'error');
                    }
                },
                (error) => {
                    console.error('Error verifying account:', error);
                    Swal.fire('Lỗi!', 'Có lỗi xảy ra khi xác thực tài khoản.', 'error');
                }
            );
        }
    });
}

/**
 * Load statistics from API and update cards
 */
function loadAccountStatistics() {
    callGetAPIAuthen('/Account/GetCustomerAccountStatistics',
        (response) => {
            if (response && response.IsSuccess && response.Data) {
                const stats = response.Data;
                
                // Update statistics cards
                updateStatisticCard('totalCustomers', stats.TotalCustomers || 0);
                updateStatisticCard('activeCustomers', stats.ActiveCustomers || 0);
                updateStatisticCard('premiumCustomers', stats.PremiumCustomers || 0);
                updateStatisticCard('todayCustomers', stats.NewCustomersThisMonth || 0);
                
                console.log('✅ Statistics loaded:', stats);
            } else {
                console.error('❌ Failed to load statistics:', response);
            }
        },
        (error) => {
            console.error('❌ Error loading statistics:', error);
        }
    );
}

/**
 * Update a statistic card with animation
 */
function updateStatisticCard(elementId, value) {
    const element = document.getElementById(elementId);
    if (element) {
        // Format number with thousand separator
        const formattedValue = typeof value === 'number' 
            ? value.toLocaleString('vi-VN') 
            : value;
        
        // Animate number change
        const currentValue = parseInt(element.textContent.replace(/[^\d]/g, '')) || 0;
        const targetValue = typeof value === 'number' ? value : parseInt(value) || 0;
        
        if (currentValue !== targetValue) {
            animateNumber(element, currentValue, targetValue, formattedValue);
        } else {
            element.textContent = formattedValue;
        }
    }
}

/**
 * Animate number change
 */
function animateNumber(element, startValue, endValue, formattedEndValue) {
    const duration = 1000; // 1 second
    const startTime = performance.now();
    
    const animate = (currentTime) => {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        
        // Easing function (ease-out)
        const easeOut = 1 - Math.pow(1 - progress, 3);
        const currentValue = Math.round(startValue + (endValue - startValue) * easeOut);
        
        element.textContent = currentValue.toLocaleString('vi-VN');
        
        if (progress < 1) {
            requestAnimationFrame(animate);
        } else {
            element.textContent = formattedEndValue;
        }
    };
    
    requestAnimationFrame(animate);
}

// Initialize when DOM is ready
$(document).ready(function() {
    console.log('Customer account grid page loaded');
    
    // Load statistics from API
    loadAccountStatistics();
    
    // Wait for both card-filter and data-grid to be initialized
    setTimeout(() => {
        // The card-filter will trigger FilterChanged event
        // The data-grid will listen to it via initializeCardFilterIntegration
        console.log('✅ Account page initialized with card-filter integration');
    }, 500);
});
