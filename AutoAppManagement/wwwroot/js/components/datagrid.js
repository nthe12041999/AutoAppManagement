/**
 * DataGrid Component
 * Base class for data grid functionality
 */

class DataGrid {
    constructor(tableId, config = {}) {
        this.tableId = tableId;
        this.table = document.getElementById(tableId);
        this.container = this.table?.closest('[data-component="datagrid"]');
        
        if (!this.container) {
            console.error('DataGrid container not found for table:', tableId);
            return;
        }

        // Read configuration from data attributes
        this.config = {
            tableId: this.container.dataset.tableId || tableId,
            getUrl: this.container.dataset.getUrl || '',
            baseUrl: this.container.dataset.baseUrl || '',
            entity: this.container.dataset.entity || '',
            pageSize: parseInt(this.container.dataset.pageSize) || 10,
            autoLoad: this.container.dataset.autoLoad === 'true',
            hasPaging: this.container.dataset.hasPaging === 'true',
            title: this.container.dataset.title || '',
            titleIcon: this.container.dataset.titleIcon || 'bi bi-table',
            hasExport: this.container.dataset.hasExport === 'true',
            hasRefresh: this.container.dataset.hasRefresh === 'true',
            hasAdd: this.container.dataset.hasAdd === 'true',
            titleAdd: this.container.dataset.titleAdd || 'Thêm mới',
            addAction: this.container.dataset.addAction || 'modal',
            addTarget: this.container.dataset.addTarget || '',
            ...config
        };

        this.currentPage = 1;
        this.totalRecords = 0;
        this.cellRenderers = new Map();

        this.init();
    }

    init() {
        console.log('🚀 Initializing DataGrid:', this.tableId);
        
        // Generate header if needed
        this.generateHeader();
        
        // Generate footer if needed
        this.generateFooter();
        
        // Auto load data if enabled
        if (this.config.autoLoad) {
            this.loadData();
        }

        console.log('✅ DataGrid initialized:', this.tableId);
    }

    generateHeader() {
        // Check if header already exists
        let header = this.container.querySelector('.card-header');
        if (header) return;

        // Create header
        header = document.createElement('div');
        header.className = 'card-header d-flex justify-content-between align-items-center';
        
        // Title section
        const titleSection = document.createElement('div');
        titleSection.innerHTML = `
            <h5 class="card-title mb-0">
                <i class="${this.config.titleIcon} me-2"></i>${this.config.title}
            </h5>
        `;

        // Buttons section
        const buttonsSection = document.createElement('div');
        buttonsSection.className = 'd-flex gap-2';

        // Export button
        if (this.config.hasExport) {
            const exportBtn = document.createElement('button');
            exportBtn.className = 'btn btn-outline-success btn-sm';
            exportBtn.innerHTML = '<i class="bi bi-file-earmark-excel me-1"></i>Xuất Excel';
            exportBtn.onclick = () => this.exportToExcel();
            buttonsSection.appendChild(exportBtn);
        }

        // Refresh button
        if (this.config.hasRefresh) {
            const refreshBtn = document.createElement('button');
            refreshBtn.className = 'btn btn-outline-primary btn-sm';
            refreshBtn.innerHTML = '<i class="bi bi-arrow-clockwise me-1"></i>Làm mới';
            refreshBtn.onclick = () => this.refresh();
            buttonsSection.appendChild(refreshBtn);
        }

        // Add button
        if (this.config.hasAdd) {
            const addBtn = document.createElement('button');
            addBtn.className = 'btn btn-primary btn-sm';
            addBtn.innerHTML = `<i class="bi bi-plus me-1"></i>${this.config.titleAdd}`;
            
            if (this.config.addAction === 'modal' && this.config.addTarget) {
                addBtn.setAttribute('data-bs-toggle', 'modal');
                addBtn.setAttribute('data-bs-target', this.config.addTarget);
            } else {
                addBtn.onclick = () => this.handleAdd();
            }
            buttonsSection.appendChild(addBtn);
        }

        header.appendChild(titleSection);
        header.appendChild(buttonsSection);

        // Insert header at the beginning of card
        this.container.insertBefore(header, this.container.firstChild);
    }

    generateFooter() {
        if (!this.config.hasPaging) return;

        // Check if footer already exists
        let footer = this.container.querySelector('.card-footer');
        if (footer) return;

        // Create footer
        footer = document.createElement('div');
        footer.className = 'card-footer';
        footer.innerHTML = `
            <div class="d-flex justify-content-between align-items-center">
                <div class="text-muted" id="${this.tableId}Info">
                    Hiển thị 0 - 0 trong tổng số 0 bản ghi
                </div>
                <nav aria-label="Phân trang">
                    <ul class="pagination pagination-sm mb-0" id="${this.tableId}Pagination">
                        <!-- Pagination will be generated here -->
                    </ul>
                </nav>
            </div>
        `;

        this.container.appendChild(footer);
    }

    async loadData() {
        try {
            console.log('📡 Loading data from:', this.config.getUrl);
            
            const response = await fetch(this.config.getUrl);
            const result = await response.json();
            
            if (result.success && result.data) {
                this.renderTable(result.data.data || result.data);
                this.updatePagination(result.data.total || (result.data.data ? result.data.data.length : 0));
                
                // Call custom data loaded handler
                if (typeof this.onDataLoaded === 'function') {
                    this.onDataLoaded(result.data);
                }
            } else {
                throw new Error(result.message || 'Failed to load data');
            }
        } catch (error) {
            console.error('❌ Error loading data:', error);
            this.showError('Có lỗi xảy ra khi tải dữ liệu');
        }
    }

    renderTable(data) {
        const tbody = this.table.querySelector('tbody') || this.createTableBody();
        
        if (!data || data.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="100%" class="text-center py-4">
                        <i class="bi bi-inbox fs-1 text-muted"></i>
                        <div class="mt-2 text-muted">Không có dữ liệu</div>
                    </td>
                </tr>
            `;
            return;
        }

        const rows = data.map(item => this.renderRow(item)).join('');
        tbody.innerHTML = rows;
        
        // Call custom table rendered handler
        if (typeof this.onTableRendered === 'function') {
            this.onTableRendered(data);
        }
        
        console.log(`✅ Rendered ${data.length} rows`);
    }

    createTableBody() {
        const tbody = document.createElement('tbody');
        this.table.appendChild(tbody);
        return tbody;
    }

    renderRow(item) {
        const headers = this.table.querySelectorAll('thead th[data-field]');
        const checkboxHeader = this.table.querySelector('thead th[data-type="Checkbox"]');
        const actionsHeader = this.table.querySelector('thead th[data-type="Actions"]');
        
        let row = '<tr>';
        
        // Checkbox column
        if (checkboxHeader) {
            row += `
                <td class="text-center">
                    <input type="checkbox" class="form-check-input row-checkbox" value="${item.Id || item.id}">
                </td>
            `;
        }
        
        // Data columns
        headers.forEach(header => {
            const field = header.dataset.field;
            const type = header.dataset.type;
            row += `<td>${this.renderCell(item, field, type)}</td>`;
        });
        
        // Actions column
        if (actionsHeader) {
            row += `<td>${this.renderActions(item.Id || item.id)}</td>`;
        }
        
        row += '</tr>';
        return row;
    }

    renderCell(item, field, type) {
        // Check for custom renderer
        if (this.cellRenderers.has(field)) {
            return this.cellRenderers.get(field)(item, field);
        }

        const value = item[field] || '';
        
        switch (type) {
            case 'DateTime':
                return this.formatDate(value);
            case 'Badge':
                return `<span class="badge bg-secondary">${value}</span>`;
            case 'Email':
                return value ? `<a href="mailto:${value}">${value}</a>` : '';
            default:
                return value;
        }
    }

    renderActions(id) {
        const actions = this.getCustomActions ? this.getCustomActions() : this.getDefaultActions();
        
        let html = '<div class="btn-group btn-group-sm">';
        actions.forEach(action => {
            html += `
                <button class="btn ${action.cssClass || 'btn-outline-primary'}" 
                        title="${action.title}"
                        onclick="${action.action}(${id})">
                    <i class="${action.icon}"></i>
                </button>
            `;
        });
        html += '</div>';
        
        return html;
    }

    getDefaultActions() {
        return [
            { action: 'view', title: 'Xem', icon: 'bi bi-eye', cssClass: 'btn-outline-primary' },
            { action: 'edit', title: 'Sửa', icon: 'bi bi-pencil', cssClass: 'btn-outline-warning' },
            { action: 'delete', title: 'Xóa', icon: 'bi bi-trash', cssClass: 'btn-outline-danger' }
        ];
    }

    addCellRenderer(field, renderer) {
        this.cellRenderers.set(field, renderer);
    }

    formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN');
    }

    updatePagination(total) {
        this.totalRecords = total;
        
        const infoElement = document.getElementById(`${this.tableId}Info`);
        if (infoElement) {
            const start = (this.currentPage - 1) * this.config.pageSize + 1;
            const end = Math.min(this.currentPage * this.config.pageSize, total);
            infoElement.textContent = `Hiển thị ${start}-${end} trong tổng số ${total} bản ghi`;
        }
    }

    showError(message) {
        const tbody = this.table.querySelector('tbody') || this.createTableBody();
        tbody.innerHTML = `
            <tr>
                <td colspan="100%" class="text-center py-4">
                    <i class="bi bi-exclamation-triangle fs-1 text-danger"></i>
                    <div class="mt-2 text-danger">${message}</div>
                    <button class="btn btn-outline-primary btn-sm mt-2" onclick="window.${this.tableId}Grid?.refresh()">
                        <i class="bi bi-arrow-clockwise me-1"></i>Thử lại
                    </button>
                </td>
            </tr>
        `;
    }

    refresh() {
        console.log('🔄 Refreshing data...');
        this.loadData();
    }

    exportToExcel() {
        console.log('📊 Exporting to Excel...');
        window.open(`${this.config.baseUrl}/ExportToExcel`, '_blank');
    }

    handleAdd() {
        console.log('➕ Add new item...');
    }

    executeAction(action, id) {
        console.log(`⚡ Execute action: ${action} for ID: ${id}`);
    }
}

// Auto-initialize DataGrids when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('🔍 Looking for DataGrid components...');
    
    const dataGrids = document.querySelectorAll('[data-component="datagrid"]');
    dataGrids.forEach(container => {
        const tableId = container.dataset.tableId;
        if (tableId) {
            console.log('🚀 Auto-initializing DataGrid:', tableId);
            window[`${tableId}Grid`] = new DataGrid(tableId);
        }
    });
});
