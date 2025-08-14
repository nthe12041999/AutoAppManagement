// Custom Grid Column Configuration for Role Grid (defines columns structure)
function customGridColumnConfig() {
    return [
        {
            field: 'id',
            title: 'ID',
            type: ColumnTypes.TEXT,
            sortable: true,
            width: '60px'
        },
        {
            field: 'name',
            title: 'Tên vai trò',
            type: ColumnTypes.TEXT,
            sortable: true,
            width: '200px'
        },
        {
            field: 'code',
            title: 'Mã vai trò',
            type: ColumnTypes.TEXT,
            sortable: true,
            width: '150px'
        },
        {
            field: 'description',
            title: 'Mô tả',
            type: ColumnTypes.TEXT,
            sortable: false,
            width: '250px'
        },
        {
            field: 'group',
            title: 'Nhóm',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['admin', 'management', 'staff', 'customer'],
            width: '120px'
        },
        {
            field: 'userCount',
            title: 'Số người dùng',
            type: ColumnTypes.NUMBER,
            sortable: true,
            width: '120px'
        },
        {
            field: 'permissionCount',
            title: 'Số quyền hạn',
            type: ColumnTypes.NUMBER,
            sortable: true,
            width: '120px'
        },
        {
            field: 'status',
            title: 'Trạng thái',
            type: ColumnTypes.ENUM,
            sortable: true,
            enumValues: ['active', 'inactive', 'pending'],
            width: '120px'
        },
        {
            field: 'createdDate',
            title: 'Ngày tạo',
            type: ColumnTypes.DATE,
            sortable: true,
            width: '120px'
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
    Swal.fire({
        title: 'Phân quyền cho vai trò',
        text: 'Chọn quyền hạn để gán cho vai trò này',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Phân quyền',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Assigning permissions to role:', id);
            Swal.fire('Thành công!', 'Đã phân quyền thành công.', 'success');
            
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
    
    // No need for separate action buttons since DataGrid has built-in "Add" button
    // The DataGrid component will handle add/edit/delete operations via its toolbar
});
