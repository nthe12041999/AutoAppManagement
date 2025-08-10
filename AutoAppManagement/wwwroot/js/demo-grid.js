// Demo Grid JavaScript
let currentPage = 1;
let pageSize = 10;
let totalRecords = 0;
let totalPages = 0;
let selectedIds = [];

// Khởi tạo grid khi document ready
$(document).ready(function() {
    loadGridData();
    
    // Setup tìm kiếm khi nhấn Enter
    $('#searchText').on('keypress', function(e) {
        if (e.which === 13) {
            applyFilters();
        }
    });
    
    // Setup filter change events
    $('#statusFilter, #departmentFilter').on('change', function() {
        applyFilters();
    });
});

// Load dữ liệu grid
function loadGridData(page = 1) {
    currentPage = page;
    
    const requestData = {
        page: currentPage,
        pageSize: pageSize,
        searchText: $('#searchText').val(),
        statusFilter: $('#statusFilter').val(),
        departmentFilter: $('#departmentFilter').val(),
        sortBy: 'Id',
        sortDirection: 'asc'
    };
    
    // Hiển thị loading
    showLoading();
    
    $.ajax({
        url: '/Demo/GetDemoData',
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

// Render dữ liệu grid
function renderGrid(data) {
    const tbody = $('#gridBody');
    tbody.empty();
    
    if (!data || data.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="13" class="text-center py-4">
                    <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                    <br>Không có dữ liệu
                </td>
            </tr>
        `);
        return;
    }
    
    data.forEach(function(item) {
        const row = `
            <tr>
                <td>
                    <input type="checkbox" class="row-checkbox" value="${item.id}" 
                           onchange="updateSelectedIds(${item.id}, this.checked)">
                </td>
                <td>${item.id}</td>
                <td>
                    <div class="d-flex align-items-center">
                        <div class="avatar-sm bg-primary rounded-circle d-flex align-items-center justify-content-center mr-2">
                            <span class="text-white font-weight-bold">${item.name.charAt(0)}</span>
                        </div>
                        <span>${item.name}</span>
                    </div>
                </td>
                <td>
                    <a href="mailto:${item.email}" class="text-decoration-none">${item.email}</a>
                </td>
                <td>
                    <a href="tel:${item.phone}" class="text-decoration-none">${item.phone}</a>
                </td>
                <td>
                    <span class="badge badge-${getDepartmentBadgeClass(item.department)}">${item.department}</span>
                </td>
                <td>${item.position}</td>
                <td>
                    <span class="badge badge-${getStatusBadgeClass(item.status)}">${getStatusText(item.status)}</span>
                </td>
                <td>${formatDate(item.joinDate)}</td>
                <td class="text-right">
                    <span class="font-weight-bold">${formatCurrency(item.salary)}</span>
                </td>
                <td class="text-center">
                    <div class="progress" style="height: 20px;">
                        <div class="progress-bar bg-${getScoreColor(item.score)}" 
                             style="width: ${item.score}%" 
                             title="${item.score} điểm">
                            ${item.score}
                        </div>
                    </div>
                </td>
                <td class="text-center">
                    ${item.isActive ? 
                        '<i class="fas fa-check-circle text-success" title="Hoạt động"></i>' : 
                        '<i class="fas fa-times-circle text-danger" title="Không hoạt động"></i>'
                    }
                </td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <button type="button" class="btn btn-info" onclick="viewItem(${item.id})" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button type="button" class="btn btn-warning" onclick="editItem(${item.id})" title="Sửa">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button type="button" class="btn btn-danger" onclick="deleteItem(${item.id})" title="Xóa">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
        tbody.append(row);
    });
}

// Cập nhật phân trang
function updatePagination(response) {
    totalRecords = response.totalRecords;
    totalPages = response.totalPages;
    currentPage = response.currentPage;
    
    const pagination = $('#pagination');
    pagination.empty();
    
    if (totalPages <= 1) return;
    
    // Nút Previous
    pagination.append(`
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadGridData(${currentPage - 1})" ${currentPage === 1 ? 'tabindex="-1"' : ''}>
                <i class="fas fa-chevron-left"></i>
            </a>
        </li>
    `);
    
    // Các số trang
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);
    
    if (startPage > 1) {
        pagination.append(`<li class="page-item"><a class="page-link" href="#" onclick="loadGridData(1)">1</a></li>`);
        if (startPage > 2) {
            pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        }
    }
    
    for (let i = startPage; i <= endPage; i++) {
        pagination.append(`
            <li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="loadGridData(${i})">${i}</a>
            </li>
        `);
    }
    
    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        }
        pagination.append(`<li class="page-item"><a class="page-link" href="#" onclick="loadGridData(${totalPages})">${totalPages}</a></li>`);
    }
    
    // Nút Next
    pagination.append(`
        <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadGridData(${currentPage + 1})" ${currentPage === totalPages ? 'tabindex="-1"' : ''}>
                <i class="fas fa-chevron-right"></i>
            </a>
        </li>
    `);
}

// Cập nhật thông tin bản ghi
function updateRecordInfo(response) {
    const showingFrom = totalRecords > 0 ? (currentPage - 1) * pageSize + 1 : 0;
    const showingTo = Math.min(currentPage * pageSize, totalRecords);
    
    $('#showingFrom').text(showingFrom);
    $('#showingTo').text(showingTo);
    $('#totalRecords').text(totalRecords);
    
    // Cập nhật thông tin đã chọn
    updateSelectedInfo();
}

// Helper functions cho badge classes
function getDepartmentBadgeClass(department) {
    const classes = {
        'IT': 'primary',
        'Marketing': 'success',
        'Sales': 'warning',
        'HR': 'info',
        'Finance': 'danger',
        'Operations': 'secondary'
    };
    return classes[department] || 'light';
}

function getStatusBadgeClass(status) {
    const classes = {
        'Active': 'success',
        'Inactive': 'secondary',
        'Pending': 'warning',
        'Suspended': 'danger'
    };
    return classes[status] || 'light';
}

function getStatusText(status) {
    const texts = {
        'Active': 'Hoạt động',
        'Inactive': 'Không hoạt động',
        'Pending': 'Chờ duyệt',
        'Suspended': 'Tạm ngưng'
    };
    return texts[status] || status;
}

function getScoreColor(score) {
    if (score >= 90) return 'success';
    if (score >= 80) return 'info';
    if (score >= 70) return 'warning';
    return 'danger';
}

// Format functions
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

// Filter functions
function applyFilters() {
    currentPage = 1;
    selectedIds = [];
    updateSelectedInfo();
    loadGridData();
}

function clearFilters() {
    $('#searchText').val('');
    $('#statusFilter').val('');
    $('#departmentFilter').val('');
    applyFilters();
}

function changePageSize() {
    pageSize = parseInt($('#pageSize').val());
    currentPage = 1;
    loadGridData();
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
    
    updateSelectedInfo();
}

function updateSelectedIds(id, isChecked) {
    if (isChecked) {
        if (selectedIds.indexOf(id) === -1) {
            selectedIds.push(id);
        }
    } else {
        selectedIds = selectedIds.filter(x => x !== id);
    }
    
    // Cập nhật checkbox select all
    const totalCheckboxes = $('.row-checkbox').length;
    const checkedCheckboxes = $('.row-checkbox:checked').length;
    $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes && totalCheckboxes > 0);
    
    updateSelectedInfo();
}

function updateSelectedInfo() {
    const selectedCount = selectedIds.length;
    $('#selectedCount').text(selectedCount);
    
    if (selectedCount > 0) {
        $('#selectedInfo').show();
    } else {
        $('#selectedInfo').hide();
    }
}

// CRUD functions
function showCreateModal() {
    $('#modalTitle').text('Thêm mới');
    $('#itemForm')[0].reset();
    $('#itemId').val('');
    $('#itemModal').modal('show');
}

function editItem(id) {
    showNotification('Chức năng sửa đang được phát triển', 'info');
}

function viewItem(id) {
    showNotification('Chức năng xem chi tiết đang được phát triển', 'info');
}

function saveItem() {
    showNotification('Chức năng lưu đang được phát triển', 'info');
}

function deleteItem(id) {
    if (confirm('Bạn có chắc chắn muốn xóa bản ghi này?')) {
        showNotification('Chức năng xóa đang được phát triển', 'info');
    }
}

function exportData() {
    showNotification('Chức năng xuất Excel đang được phát triển', 'info');
}

// Utility functions
function refreshGrid() {
    loadGridData(currentPage);
}

function showLoading() {
    $('#gridBody').html(`
        <tr>
            <td colspan="13" class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="sr-only">Đang tải...</span>
                </div>
                <br>
                <span class="text-muted">Đang tải dữ liệu...</span>
            </td>
        </tr>
    `);
}

function hideLoading() {
    // Loading sẽ được ẩn khi render dữ liệu
}

function showError(message) {
    showNotification(message, 'error');
}

function showNotification(message, type = 'info') {
    // Simple notification - có thể thay thế bằng toast notification
    const alertClass = type === 'error' ? 'alert-danger' : 
                      type === 'success' ? 'alert-success' : 
                      type === 'warning' ? 'alert-warning' : 'alert-info';
    
    const notification = $(`
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            <button type="button" class="close" data-dismiss="alert">
                <span>&times;</span>
            </button>
            ${message}
        </div>
    `);
    
    $('body').append(notification);
    
    // Tự động ẩn sau 3 giây
    setTimeout(() => {
        notification.alert('close');
    }, 3000);
}
