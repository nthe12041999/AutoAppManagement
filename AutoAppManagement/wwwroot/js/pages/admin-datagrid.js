/**
 * Admin DataGrid Configuration
 * Extends base DataGrid with admin-specific features
 */

class AdminDataGrid extends DataGrid {
    constructor() {
        super('adminTable');
        this.initializeAdminSpecificFeatures();
    }

    // Define custom actions for admin accounts
    getCustomActions() {
        return [
            { action: 'viewAdmin', title: 'Xem chi tiết', icon: 'bi bi-eye', cssClass: 'btn-outline-primary' },
            { action: 'editAdmin', title: 'Chỉnh sửa', icon: 'bi bi-pencil', cssClass: 'btn-outline-warning' },
            { action: 'toggleAdminStatus', title: 'Khóa/Mở', icon: 'bi bi-lock', cssClass: 'btn-outline-secondary' },
            { action: 'deleteAdmin', title: 'Xóa', icon: 'bi bi-trash', cssClass: 'btn-outline-danger' }
        ];
    }

    // Update stats when data is loaded - Override for Admin-specific stats
    onDataLoaded(response) {
        const data = response.data || response;
        if (Array.isArray(data)) {
            const totalAdmins = data.length;
            const activeAdmins = data.filter(a => a.IsActive && a.Status === 'Active').length;
            const verifiedAdmins = data.filter(a => a.IsEmailVerified).length;
            const onlineAdmins = data.filter(a => a.OnlineStatus === 'Online').length;

            // Update statistics cards using StatisticsCards component
            // Sử dụng template 'admin' thay vì 'customer'
            if (window.adminStatisticsStats) {
                window.adminStatisticsStats.updateValues({
                    totalAdmins,
                    activeAdmins,
                    verifiedAdmins,
                    onlineAdmins
                });
            }
        }
    }

    // Initialize admin-specific features
    initializeAdminSpecificFeatures() {
        // Add custom cell renderers if needed
        // this.addCellRenderer('Role', this.renderRoleCell.bind(this));
        // this.addCellRenderer('Status', this.renderStatusCell.bind(this));
    }

    // Override export to use admin-specific endpoint
    exportToExcel() {
        window.open('/AdminAccount/ExportAdminAccountsToExcel', '_blank');
    }
}

// Global action functions (called by DataGrid buttons)
function viewAdmin(id) {
    if (window.adminModal) {
        window.adminModal.showViewModal(id);
    }
}

function editAdmin(id) {
    if (window.adminModal) {
        window.adminModal.showEditModal(id);
    }
}

async function toggleAdminStatus(id) {
    if (confirm('Bạn có chắc chắn muốn thay đổi trạng thái của admin này?')) {
        try {
            const response = await fetch(`/AdminAccount/ToggleAdminStatus/${id}`, { method: 'POST' });
            const result = await response.json();

            if (result.success) {
                alert('Đã thay đổi trạng thái thành công!');
                window.adminTableGrid?.refresh();
            } else {
                alert('Lỗi: ' + result.message);
            }
        } catch (error) {
            alert('Có lỗi xảy ra khi thay đổi trạng thái');
        }
    }
}

async function deleteAdmin(id) {
    if (confirm('Bạn có chắc chắn muốn xóa admin này? Hành động này không thể hoàn tác.')) {
        try {
            const response = await fetch(`/AdminAccount/DeleteAdminAccount/${id}`, { method: 'POST' });
            const result = await response.json();

            if (result.success) {
                alert('Đã xóa admin thành công!');
                window.adminTableGrid?.refresh();
            } else {
                alert('Lỗi: ' + result.message);
            }
        } catch (error) {
            alert('Có lỗi xảy ra khi xóa admin');
        }
    }
}
