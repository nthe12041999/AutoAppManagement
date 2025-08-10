/**
 * Customer DataGrid Configuration
 * Extends base DataGrid with customer-specific features
 */

class CustomerDataGrid extends DataGrid {
    constructor() {
        super('customerTable');
        this.initializeCustomerSpecificFeatures();
    }

    // Define custom actions for customer accounts
    getCustomActions() {
        return [
            { action: 'viewCustomer', title: 'Xem chi tiết', icon: 'bi bi-eye', cssClass: 'btn-outline-primary' },
            { action: 'editCustomer', title: 'Chỉnh sửa', icon: 'bi bi-pencil', cssClass: 'btn-outline-warning' },
            { action: 'toggleCustomerStatus', title: 'Khóa/Mở', icon: 'bi bi-lock', cssClass: 'btn-outline-secondary' },
            { action: 'deleteCustomer', title: 'Xóa', icon: 'bi bi-trash', cssClass: 'btn-outline-danger' }
        ];
    }

    // Update stats when data is loaded
    onDataLoaded(response) {
        const data = response.data || response;
        if (Array.isArray(data)) {
            const totalCustomers = data.length;
            const activeCustomers = data.filter(c => c.Status === 'Active' && !c.IsLocked).length;
            const premiumCustomers = data.filter(c => c.Role === 'Premium' || c.Role === 'VIP').length;
            const onlineCustomers = data.filter(c => c.OnlineStatus === 'Online').length;

            // Update statistics cards using StatisticsCards component
            if (window.customerStatisticsStats) {
                window.customerStatisticsStats.updateValues({
                    totalCustomers,
                    activeCustomers,
                    premiumCustomers,
                    onlineCustomers
                });
            }
        }
    }

    // Custom cell renderers
    renderUserCell(data, field) {
        const name = data.Name || 'N/A';
        const userName = data.UserName || '';
        const level = data.Level || 1;

        let avatarClass = 'bg-primary';
        if (level >= 3) avatarClass = 'bg-warning';      // VIP
        else if (level >= 2) avatarClass = 'bg-success'; // Premium

        return `
            <div class="d-flex align-items-center">
                <div class="avatar-circle ${avatarClass} text-white me-3">
                    <i class="bi bi-person-fill"></i>
                </div>
                <div>
                    <div class="fw-bold">${name}</div>
                    <small class="text-muted">${userName}</small>
                </div>
            </div>
        `;
    }

    renderRoleCell(data, field) {
        const role = data.Role || 'Customer';
        const badgeClass = {
            'vip': 'bg-warning',
            'premium': 'bg-info',
            'customer': 'bg-secondary'
        }[role.toLowerCase()] || 'bg-secondary';

        return `<span class="badge ${badgeClass}">${role}</span>`;
    }

    renderStatusCell(data, field) {
        const status = data.Status || 'Unknown';
        const isLocked = data.IsLocked || false;

        let badgeClass = 'bg-secondary';
        let displayStatus = status;

        if (isLocked) {
            badgeClass = 'bg-danger';
            displayStatus = 'Đã khóa';
        } else {
            const statusMap = {
                'active': { class: 'bg-success', text: 'Hoạt động' },
                'expired': { class: 'bg-warning', text: 'Hết hạn' },
                'inactive': { class: 'bg-secondary', text: 'Không hoạt động' },
                'locked': { class: 'bg-danger', text: 'Đã khóa' }
            };

            const mapped = statusMap[status.toLowerCase()];
            if (mapped) {
                badgeClass = mapped.class;
                displayStatus = mapped.text;
            }
        }

        return `<span class="badge ${badgeClass}">${displayStatus}</span>`;
    }

    // Initialize customer-specific features
    initializeCustomerSpecificFeatures() {
        // Add custom cell renderers
        this.addCellRenderer('Name', this.renderUserCell.bind(this));
        this.addCellRenderer('Role', this.renderRoleCell.bind(this));
        this.addCellRenderer('Status', this.renderStatusCell.bind(this));
    }

    // Override export to use customer-specific endpoint
    exportToExcel() {
        window.open('/CustomerAccount/ExportCustomerAccountsToExcel', '_blank');
    }
}

// Global action functions (called by DataGrid buttons)
function viewCustomer(id) {
    if (window.customerModal) {
        window.customerModal.showViewModal(id);
    }
}

function editCustomer(id) {
    if (window.customerModal) {
        window.customerModal.showEditModal(id);
    }
}

async function toggleCustomerStatus(id) {
    if (confirm('Bạn có chắc chắn muốn thay đổi trạng thái của khách hàng này?')) {
        try {
            const response = await fetch(`/CustomerAccount/ToggleCustomerStatus/${id}`, { method: 'POST' });
            const result = await response.json();

            if (result.success) {
                alert('Đã thay đổi trạng thái thành công!');
                window.customerTableGrid?.refresh();
            } else {
                alert('Lỗi: ' + result.message);
            }
        } catch (error) {
            alert('Có lỗi xảy ra khi thay đổi trạng thái');
        }
    }
}

async function deleteCustomer(id) {
    if (confirm('Bạn có chắc chắn muốn xóa khách hàng này? Hành động này không thể hoàn tác.')) {
        try {
            const response = await fetch(`/CustomerAccount/DeleteCustomerAccount/${id}`, { method: 'POST' });
            const result = await response.json();

            if (result.success) {
                alert('Đã xóa khách hàng thành công!');
                window.customerTableGrid?.refresh();
            } else {
                alert('Lỗi: ' + result.message);
            }
        } catch (error) {
            alert('Có lỗi xảy ra khi xóa khách hàng');
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 Initializing Customer DataGrid...');
    
    // Create customer datagrid instance
    window.customerDataGrid = new CustomerDataGrid();
    
    console.log('✅ Customer DataGrid initialized successfully');
});

// Export for global access
window.CustomerDataGrid = CustomerDataGrid;
