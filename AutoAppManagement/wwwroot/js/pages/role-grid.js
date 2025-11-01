// Custom Grid Column Configuration for Role Grid (defines columns structure)
function customGridColumnConfig() {
    return [
        {
            field: 'Id',
            title: 'ID',
            type: ColumnTypes.TEXT,
            sortable: true,
            width: '60px'
        },
        {
            field: 'Name',
            title: 'Tên vai trò',
            type: ColumnTypes.TEXT,
            sortable: true,
            width: '200px'
        },
        {
            field: 'Code',
            title: 'Mã vai trò',
            type: ColumnTypes.TEXT,
            sortable: true,
            width: '150px'
        },
        {
            field: 'Description',
            title: 'Mô tả',
            type: ColumnTypes.TEXT,
            sortable: false,
            width: '250px'
        },
        {
            field: 'Group',
            title: 'Nhóm',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['admin', 'management', 'staff', 'customer'],
            width: '120px'
        },
        {
            field: 'UserCount',
            title: 'Số người dùng',
            type: ColumnTypes.NUMBER,
            sortable: true,
            width: '120px'
        },
        {
            field: 'PermissionCount',
            title: 'Số quyền hạn',
            type: ColumnTypes.NUMBER,
            sortable: true,
            width: '120px'
        },
        {
            field: 'Status',
            title: 'Trạng thái',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['active', 'inactive', 'pending'],
            width: '120px'
        },
        {
            field: 'CreatedDate',
            title: 'Ngày tạo',
            type: ColumnTypes.DATE,
            sortable: true,
            width: '120px'
        }
    ];
}

// Custom Grid Actions Configuration for Role Grid
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
            type: 'permissions',
            title: 'Quản lý quyền',
            icon: 'bi-shield-check'
        },
        {
            type: 'delete',
            title: 'Xóa',
            icon: 'bi-trash'
        }
    ];
}

// Action functions for Role management
function addRole() {
    // Open modal for adding new role
    const url = '/Role/DetailRole?mode=add';
    
    $.get(url)
        .done(function(html) {
            // Create modal container
            const modalHtml = `
                <div class="modal fade modal-container" id="roleModal" tabindex="-1">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            ${html}
                        </div>
                    </div>
                </div>
            `;
            
            // Remove existing modal
            $('.modal-container').remove();
            
            // Add new modal
            $('body').append(modalHtml);
            
            // Show modal
            const modal = new bootstrap.Modal(document.getElementById('roleModal'));
            modal.show();
        })
        .fail(function() {
            Swal.fire('Lỗi!', 'Không thể tải form thêm vai trò.', 'error');
        });
}

function deleteItem(id) {
    Swal.fire({
        title: 'Xác nhận xóa',
        text: 'Bạn có chắc chắn muốn xóa vai trò này? Hành động này không thể hoàn tác.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            // Simulate delete API call
            console.log('Deleting role:', id);
            Swal.fire('Đã xóa!', 'Vai trò đã được xóa thành công.', 'success');
            
            // Refresh the grid after delete
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('roleDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

function assignPermissions(id) {
    // Load manage permissions view in modal
    const url = `/Role/ManagePermissions?roleId=${id}`;

    $.get(url)
        .done(function(html) {
            // Update modal content
            $('#permissionsContent').html(html);

            // Show modal
            $('#managePermissionsModal').modal('show');
        })
        .fail(function() {
            Swal.fire('Lỗi!', 'Không thể tải trang quản lý quyền hạn.', 'error');
        });
}

function managePermissions(id) {
    assignPermissions(id);
}

function activateItem(id) {
    Swal.fire({
        title: 'Kích hoạt vai trò',
        text: 'Bạn có muốn kích hoạt vai trò này?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Kích hoạt',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Activating role:', id);
            Swal.fire('Đã kích hoạt!', 'Vai trò đã được kích hoạt.', 'success');
            
            // Refresh the grid
            if (window.dataGridInstance) {
                const config = window.dataGridInstance.getGrid('roleDataGrid');
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        }
    });
}

// Initialize when DOM is ready
$(document).ready(function() {
    console.log('Role grid page loaded');

    // Handle save permissions button
    $('#savePermissions').click(function() {
        if (typeof window.saveRolePermissions === 'function') {
            window.saveRolePermissions();
        }
    });

    // Global function to refresh role grid
    window.refreshRoleGrid = function() {
        if (window.dataGridInstance) {
            const config = window.dataGridInstance.getGrid('roleDataGrid');
            if (config) {
                window.dataGridInstance.refreshData(config);
            }
        }
    };

    // No need for separate action buttons since DataGrid has built-in "Add" button
    // The DataGrid component will handle add/edit/delete operations via its toolbar
});
