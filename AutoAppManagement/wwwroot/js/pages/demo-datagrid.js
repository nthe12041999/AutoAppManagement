// Demo DataGrid Page JavaScript
console.log('🚀 Loading Demo DataGrid Page...');

// Demo DataGrid Configuration
const DemoDataGrid = {
    // Configuration
    config: {
        tableId: 'demoTable',
        apiUrl: '/Demo/GetDemoData',
        baseUrl: '/Demo',
        entity: 'demo',
        pageSize: 10
    },

    // Initialize
    init() {
        console.log('🎯 Initializing Demo DataGrid...');
        
        // Setup event listeners
        this.setupEventListeners();
        
        // Initialize filters
        this.initializeFilters();
        
        console.log('✅ Demo DataGrid initialized');
    },

    // Setup event listeners
    setupEventListeners() {
        // Search input
        const searchInput = document.getElementById('searchInput');
        if (searchInput) {
            searchInput.addEventListener('input', this.debounce(() => {
                this.applyFilters();
            }, 500));
        }

        // Filter selects
        const statusFilter = document.getElementById('statusFilter');
        const departmentFilter = document.getElementById('departmentFilter');
        const pageSize = document.getElementById('pageSize');
        const clearFilters = document.getElementById('clearFilters');

        if (statusFilter) {
            statusFilter.addEventListener('change', () => this.applyFilters());
        }

        if (departmentFilter) {
            departmentFilter.addEventListener('change', () => this.applyFilters());
        }

        if (pageSize) {
            pageSize.addEventListener('change', () => this.changePageSize());
        }

        if (clearFilters) {
            clearFilters.addEventListener('click', () => this.clearFilters());
        }
    },

    // Initialize filters
    initializeFilters() {
        // Set default values if needed
        const pageSize = document.getElementById('pageSize');
        if (pageSize) {
            pageSize.value = this.config.pageSize.toString();
        }
    },

    // Apply filters
    applyFilters() {
        console.log('🔍 Applying filters...');
        
        // Get filter values
        const filters = this.getFilterValues();
        
        // Trigger DataGrid refresh with filters
        if (window.DataGridComponent && window.DataGridComponent.instances[this.config.tableId]) {
            window.DataGridComponent.instances[this.config.tableId].refresh(1, filters);
        }
    },

    // Get filter values
    getFilterValues() {
        return {
            searchText: document.getElementById('searchInput')?.value || '',
            statusFilter: document.getElementById('statusFilter')?.value || '',
            departmentFilter: document.getElementById('departmentFilter')?.value || '',
            pageSize: parseInt(document.getElementById('pageSize')?.value || this.config.pageSize)
        };
    },

    // Clear filters
    clearFilters() {
        console.log('🧹 Clearing filters...');
        
        // Clear filter inputs
        const searchInput = document.getElementById('searchInput');
        const statusFilter = document.getElementById('statusFilter');
        const departmentFilter = document.getElementById('departmentFilter');

        if (searchInput) searchInput.value = '';
        if (statusFilter) statusFilter.value = '';
        if (departmentFilter) departmentFilter.value = '';

        // Apply cleared filters
        this.applyFilters();
    },

    // Change page size
    changePageSize() {
        const newPageSize = parseInt(document.getElementById('pageSize')?.value || this.config.pageSize);
        console.log('📄 Changing page size to:', newPageSize);
        
        this.config.pageSize = newPageSize;
        this.applyFilters();
    },

    // Debounce utility
    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    // Custom data formatter for demo grid
    formatData(data) {
        return data.map(item => ({
            ...item,
            // Format salary
            salaryFormatted: new Intl.NumberFormat('vi-VN', {
                style: 'currency',
                currency: 'VND'
            }).format(item.salary),
            
            // Format join date
            joinDateFormatted: new Date(item.joinDate).toLocaleDateString('vi-VN'),
            
            // Format score as progress
            scoreProgress: `
                <div class="progress" style="height: 20px;">
                    <div class="progress-bar bg-${this.getScoreColor(item.score)}" 
                         style="width: ${item.score}%" 
                         title="${item.score} điểm">
                        ${item.score}
                    </div>
                </div>
            `,
            
            // Format active status
            activeStatus: item.isActive ? 
                '<i class="bi bi-check-circle-fill text-success" title="Hoạt động"></i>' : 
                '<i class="bi bi-x-circle-fill text-danger" title="Không hoạt động"></i>'
        }));
    },

    // Get score color
    getScoreColor(score) {
        if (score >= 90) return 'success';
        if (score >= 80) return 'info';
        if (score >= 70) return 'warning';
        return 'danger';
    },

    // Handle row actions
    handleRowAction(action, id, data) {
        console.log('🎬 Row action:', action, 'ID:', id);
        
        switch (action) {
            case 'view':
                this.viewItem(id, data);
                break;
            case 'edit':
                this.editItem(id, data);
                break;
            case 'delete':
                this.deleteItem(id, data);
                break;
            default:
                console.warn('Unknown action:', action);
        }
    },

    // View item
    viewItem(id, data) {
        console.log('👁️ Viewing item:', id);
        
        // Show notification for now
        this.showNotification(`Xem chi tiết item #${id}`, 'info');
        
        // TODO: Implement view modal
    },

    // Edit item
    editItem(id, data) {
        console.log('✏️ Editing item:', id);
        
        // Show notification for now
        this.showNotification(`Sửa item #${id}`, 'info');
        
        // TODO: Populate modal with data and show
        if (window.DemoModal) {
            window.DemoModal.show('edit', data);
        }
    },

    // Delete item
    deleteItem(id, data) {
        console.log('🗑️ Deleting item:', id);
        
        // Show confirmation
        if (confirm(`Bạn có chắc chắn muốn xóa "${data.name}"?`)) {
            // Show notification for now
            this.showNotification(`Đã xóa item #${id}`, 'success');
            
            // TODO: Implement actual delete API call
            // this.callDeleteAPI(id);
        }
    },

    // Show notification
    showNotification(message, type = 'info') {
        // Simple notification - có thể thay thế bằng toast library
        const alertClass = type === 'error' ? 'alert-danger' : 
                          type === 'success' ? 'alert-success' : 
                          type === 'warning' ? 'alert-warning' : 'alert-info';
        
        const notification = document.createElement('div');
        notification.className = `alert ${alertClass} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            ${message}
        `;
        
        document.body.appendChild(notification);
        
        // Auto remove after 3 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 3000);
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    DemoDataGrid.init();
});

// Export for global access
window.DemoDataGrid = DemoDataGrid;
