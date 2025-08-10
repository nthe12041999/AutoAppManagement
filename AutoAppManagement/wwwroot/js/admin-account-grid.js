// Admin Account Grid JavaScript
let currentPage = 1;
let pageSize = 10;
let totalRecords = 0;
let totalPages = 0;
let selectedIds = [];

// Initialize grid when document is ready
$(document).ready(function() {
    loadAdminAccounts();
    
    // Setup search on enter key
    $('#searchText').on('keypress', function(e) {
        if (e.which === 13) {
            applyFilters();
        }
    });
});

// Load admin accounts data
function loadAdminAccounts(page = 1) {
    currentPage = page;
    
    const searchText = $('#searchText').val();
    const roleFilter = $('#roleFilter').val();
    const statusFilter = $('#statusFilter').val();
    
    const requestData = {
        page: currentPage,
        pageSize: pageSize,
        searchText: searchText,
        filters: {
            role: roleFilter,
            status: statusFilter
        },
        sortBy: 'CreatedDate',
        sortDirection: 'desc'
    };
    
    // Show loading
    showLoading();
    
    $.ajax({
        url: '/AdminAccount/GetAdminAccounts',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function(response) {
            hideLoading();
            
            if (response.success) {
                renderGrid(response.data);
                updatePagination(response);
                updateRecordInfo(response);
            } else {
                showError('Lỗi tải dữ liệu: ' + response.message);
            }
        },
        error: function(xhr, status, error) {
            hideLoading();
            showError('Lỗi kết nối: ' + error);
        }
    });
}

// Render grid data
function renderGrid(data) {
    const tbody = $('#gridBody');
    tbody.empty();
    
    if (!data || data.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="16" class="text-center">
                    <i class="fas fa-inbox"></i> Không có dữ liệu
                </td>
            </tr>
        `);
        return;
    }
    
    data.forEach(function(admin) {
        const row = `
            <tr>
                <td>
                    <input type="checkbox" class="row-checkbox" value="${admin.id}" 
                           onchange="updateSelectedIds(${admin.id}, this.checked)">
                </td>
                <td>${admin.id}</td>
                <td>
                    <div class="d-flex align-items-center">
                        <img src="${admin.avatar || '/images/default-avatar.png'}" 
                             class="rounded-circle mr-2" width="32" height="32">
                        <span>${admin.fullName}</span>
                    </div>
                </td>
                <td>
                    <span>${admin.email}</span>
                    ${admin.isEmailVerified ? '<i class="fas fa-check-circle text-success ml-1" title="Email đã xác thực"></i>' : '<i class="fas fa-exclamation-circle text-warning ml-1" title="Email chưa xác thực"></i>'}
                </td>
                <td>${admin.userName}</td>
                <td>
                    <span>${admin.phoneNumber}</span>
                    ${admin.isPhoneVerified ? '<i class="fas fa-check-circle text-success ml-1" title="SĐT đã xác thực"></i>' : '<i class="fas fa-exclamation-circle text-warning ml-1" title="SĐT chưa xác thực"></i>'}
                </td>
                <td>
                    <span class="badge badge-${getRoleBadgeClass(admin.role)}">${admin.role}</span>
                </td>
                <td>${admin.department || '-'}</td>
                <td>${admin.position || '-'}</td>
                <td>
                    <span class="badge badge-${getStatusBadgeClass(admin.accountStatus)}">${admin.accountStatus}</span>
                </td>
                <td>
                    <span class="badge badge-${admin.onlineStatus === 'Online' ? 'success' : 'secondary'}">${admin.onlineStatus}</span>
                </td>
                <td>
                    ${admin.isTwoFactorEnabled ? '<i class="fas fa-shield-alt text-success" title="2FA đã bật"></i>' : '<i class="fas fa-shield-alt text-muted" title="2FA chưa bật"></i>'}
                </td>
                <td>${admin.lastLoginAt || 'Chưa đăng nhập'}</td>
                <td>
                    <span class="badge badge-info">${admin.loginCount}</span>
                    ${admin.failedLoginAttempts > 0 ? `<br><small class="text-danger">Thất bại: ${admin.failedLoginAttempts}</small>` : ''}
                </td>
                <td>${admin.createdDate}</td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <button type="button" class="btn btn-info btn-sm" onclick="viewAdmin(${admin.id})" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button type="button" class="btn btn-warning btn-sm" onclick="editAdmin(${admin.id})" title="Sửa">
                            <i class="fas fa-edit"></i>
                        </button>
                        ${admin.isLocked ? 
                            `<button type="button" class="btn btn-success btn-sm" onclick="toggleLock(${admin.id}, false)" title="Mở khóa">
                                <i class="fas fa-unlock"></i>
                            </button>` :
                            `<button type="button" class="btn btn-secondary btn-sm" onclick="toggleLock(${admin.id}, true)" title="Khóa">
                                <i class="fas fa-lock"></i>
                            </button>`
                        }
                        <button type="button" class="btn btn-danger btn-sm" onclick="deleteAdmin(${admin.id})" title="Xóa">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
        tbody.append(row);
    });
}

// Update pagination
function updatePagination(response) {
    totalRecords = response.totalRecords;
    totalPages = response.totalPages;
    currentPage = response.currentPage;
    
    const pagination = $('#pagination');
    pagination.empty();
    
    if (totalPages <= 1) return;
    
    // Previous button
    pagination.append(`
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadAdminAccounts(${currentPage - 1})">
                <i class="fas fa-chevron-left"></i>
            </a>
        </li>
    `);
    
    // Page numbers
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);
    
    if (startPage > 1) {
        pagination.append(`<li class="page-item"><a class="page-link" href="#" onclick="loadAdminAccounts(1)">1</a></li>`);
        if (startPage > 2) {
            pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        }
    }
    
    for (let i = startPage; i <= endPage; i++) {
        pagination.append(`
            <li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="loadAdminAccounts(${i})">${i}</a>
            </li>
        `);
    }
    
    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        }
        pagination.append(`<li class="page-item"><a class="page-link" href="#" onclick="loadAdminAccounts(${totalPages})">${totalPages}</a></li>`);
    }
    
    // Next button
    pagination.append(`
        <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadAdminAccounts(${currentPage + 1})">
                <i class="fas fa-chevron-right"></i>
            </a>
        </li>
    `);
}

// Update record info
function updateRecordInfo(response) {
    const showingFrom = (currentPage - 1) * pageSize + 1;
    const showingTo = Math.min(currentPage * pageSize, totalRecords);
    
    $('#showingFrom').text(showingFrom);
    $('#showingTo').text(showingTo);
    $('#totalRecords').text(totalRecords);
}

// Helper functions for badge classes
function getRoleBadgeClass(role) {
    switch (role) {
        case 'Admin': return 'danger';
        case 'Moderator': return 'warning';
        case 'Support': return 'info';
        case 'Viewer': return 'secondary';
        default: return 'light';
    }
}

function getStatusBadgeClass(status) {
    switch (status) {
        case 'Active': return 'success';
        case 'Inactive': return 'secondary';
        case 'Locked': return 'danger';
        case 'Pending Verification': return 'warning';
        default: return 'light';
    }
}

// Filter functions
function applyFilters() {
    currentPage = 1;
    loadAdminAccounts();
}

function clearFilters() {
    $('#searchText').val('');
    $('#roleFilter').val('');
    $('#statusFilter').val('');
    applyFilters();
}

// Selection functions
function toggleSelectAll() {
    const selectAll = $('#selectAll').is(':checked');
    $('.row-checkbox').prop('checked', selectAll);
    
    if (selectAll) {
        selectedIds = $('.row-checkbox').map(function() {
            return parseInt($(this).val());
        }).get();
    } else {
        selectedIds = [];
    }
}

function updateSelectedIds(id, isChecked) {
    if (isChecked) {
        if (selectedIds.indexOf(id) === -1) {
            selectedIds.push(id);
        }
    } else {
        selectedIds = selectedIds.filter(x => x !== id);
    }
    
    // Update select all checkbox
    const totalCheckboxes = $('.row-checkbox').length;
    const checkedCheckboxes = $('.row-checkbox:checked').length;
    $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
}

// CRUD functions
function showCreateModal() {
    $('#modalTitle').text('Thêm Admin Account');
    $('#adminForm')[0].reset();
    $('#adminId').val('');
    $('#password').prop('required', true);
    $('#adminModal').modal('show');
}

function editAdmin(id) {
    // TODO: Load admin data and show edit modal
    showError('Chức năng đang phát triển');
}

function viewAdmin(id) {
    // TODO: Show admin details modal
    showError('Chức năng đang phát triển');
}

function saveAdmin() {
    // TODO: Implement save functionality
    showError('Chức năng đang phát triển');
}

function deleteAdmin(id) {
    if (confirm('Bạn có chắc chắn muốn xóa admin account này?')) {
        // TODO: Implement delete functionality
        showError('Chức năng đang phát triển');
    }
}

function toggleLock(id, isLock) {
    const action = isLock ? 'khóa' : 'mở khóa';
    if (confirm(`Bạn có chắc chắn muốn ${action} tài khoản này?`)) {
        // TODO: Implement lock/unlock functionality
        showError('Chức năng đang phát triển');
    }
}

// Utility functions
function refreshGrid() {
    loadAdminAccounts(currentPage);
}

function showLoading() {
    $('#gridBody').html(`
        <tr>
            <td colspan="16" class="text-center">
                <i class="fas fa-spinner fa-spin"></i> Đang tải dữ liệu...
            </td>
        </tr>
    `);
}

function hideLoading() {
    // Loading will be hidden when data is rendered
}

function showError(message) {
    alert('Lỗi: ' + message);
}

function showSuccess(message) {
    alert('Thành công: ' + message);
}
