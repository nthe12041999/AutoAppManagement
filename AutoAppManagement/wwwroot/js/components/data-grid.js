/**
 * DataGrid - Auto-generate Bootstrap Table from data attributes
 * Usage: <div data-component="data-grid" data-entity="license" data-columns="..." ...></div>
 */

class DataGrid {
    constructor() {
        this.grids = new Map();
        this.init();
    }

    /**
     * Initialize data grid components
     */
    init() {
        console.log('DataGrid.init() called');
        // Find all data-grid components using jQuery
        const $gridComponents = $('[data-component="data-grid"]');
        console.log('Found data-grid components:', $gridComponents.length);

        $gridComponents.each((_, component) => {
            console.log('Processing grid component:', component);
            this.renderGrid(component);
        });
    }

    /**
     * Render grid component
     * @param {HTMLElement} component - Grid component element
     */
    renderGrid(component) {
        console.log('renderGrid called for component:', component);
        const config = this.parseConfig(component);
        console.log('Parsed config:', config);

        // Check if component already has table structure
        const $existingTable = $(component).find('table');
        const existingTable = $existingTable.length > 0 ? $existingTable[0] : null;

        if (existingTable) {
            // Use existing table structure, just setup AJAX loading
            config.tableId = existingTable.id;
            config.useExistingTable = true;

            // Use customGridColumnConfig if available, otherwise use default columns
            if (typeof window.customGridColumnConfig === 'function') {
                try {
                    const customColumns = window.customGridColumnConfig();
                    if (Array.isArray(customColumns) && customColumns.length > 0) {
                        config.columns = customColumns;
                    } else {
                        // Use default columns if no custom config
                        config.columns = [];
                    }
                } catch (error) {
                    console.error('Error calling customGridColumnConfig:', error);
                    // Use default columns on error
                    config.columns = [];
                }
            } else {
                // Use default columns if no custom config function
                config.columns = [];
            }
        } else {
            // Generate new table structure
            const html = this.generateGridHTML(config);
            component.innerHTML = html;
            config.useExistingTable = false;
        }

        // Set grid container attribute
        component.setAttribute('data-grid-container', config.containerId);

        // Initialize grid logic
        this.initializeGridLogic(config);

        // Load data if URL provided
        if (config.getUrl) {
            console.log('Loading data for grid:', config.containerId);
            this.loadData(config);
        } else {
            console.warn('No getUrl provided for grid:', config.containerId);
        }

        // Store grid instance
        this.grids.set(config.containerId, config);
        console.log('Grid stored and rendered:', config.containerId);
    }

    /**
     * Parse configuration from data attributes
     * @param {HTMLElement} component - Grid component element
     * @returns {object} Configuration object
     */
    parseConfig(component) {
        const config = {
            containerId: component.getAttribute('data-container-id') || `dataGrid_${Date.now()}`,
            entity: component.getAttribute('data-entity') || 'Item',
            entityPlural: component.getAttribute('data-entity-plural') || 'Items',
            titleGrid: component.getAttribute('data-title-grid') || null, // Custom grid title

            // Data source
            getUrl: component.getAttribute('data-get-url') || null,
            detailUrl: component.getAttribute('data-detail-url') || null,
            detailForm: component.getAttribute('data-detail-form') || null, // New: Modal form from another view

            // Columns configuration
            columns: this.parseColumns(component.getAttribute('data-columns')),

            // Actions
            hasAdd: component.getAttribute('data-has-add') !== 'false',
            hasRefresh: component.getAttribute('data-has-refresh') !== 'false',
            hasExport: component.getAttribute('data-has-export') !== 'false',
            hasSelectAll: component.getAttribute('data-has-select-all') !== 'false',
            
            // Actions column configuration (default true, hide only when explicitly false)
            hasActions: component.getAttribute('data-actions') !== 'false',
            hasView: component.getAttribute('data-view') !== 'false',
            hasEdit: component.getAttribute('data-edit') !== 'false',
            hasDelete: component.getAttribute('data-delete') !== 'false',
            hasApprove: component.getAttribute('data-approve') === 'true',
            hasSuspend: component.getAttribute('data-suspend') === 'true',

            // Labels
            addLabel: component.getAttribute('data-add-label'),
            refreshLabel: component.getAttribute('data-refresh-label'),
            exportLabel: component.getAttribute('data-export-label'),

            // Sample data (fallback when no URL provided)
            sampleData: this.parseSampleData(component.getAttribute('data-sample-data')),

            // Pagination
            hasPagination: component.getAttribute('data-has-pagination') !== 'false',
            pageSize: parseInt(component.getAttribute('data-page-size')) || 10,
            totalItems: parseInt(component.getAttribute('data-total-items')) || 100
        };

        // Always append actions column (visibility controlled by hasActions flag)
        const actionsColumn = {
            key: 'actions',
            title: 'Thao tác',
            type: 'actions',
            className: 'text-center',
            sortable: false,
            visible: config.hasActions, // Control visibility
            buttons: []
        };

        // Add buttons based on flags (only if actions are visible)
        if (config.hasActions) {
            // Check if custom actions config is available
            if (typeof window.customGridActionsConfig === 'function') {
                try {
                    const customActions = window.customGridActionsConfig();
                    if (Array.isArray(customActions) && customActions.length > 0) {
                        actionsColumn.buttons = customActions.map(action => ({
                            type: action.type,
                            className: `btn btn-sm btn-${this.getActionButtonClass(action.type)} me-1`,
                            icon: action.icon,
                            title: action.title
                        }));
                    } else {
                        this.addDefaultActions(config, actionsColumn);
                    }
                } catch (error) {
                    console.error('Error calling customGridActionsConfig:', error);
                    this.addDefaultActions(config, actionsColumn);
                }
            } else {
                this.addDefaultActions(config, actionsColumn);
            }
        }

        // Always add actions column to the end
        config.columns.push(actionsColumn);

        return config;
    }

    /**
     * Add default actions based on config flags
     */
    addDefaultActions(config, actionsColumn) {
        if (config.hasView) {
            actionsColumn.buttons.push({
                type: 'view',
                className: 'btn btn-sm btn-info me-1',
                icon: 'bi-eye',
                title: 'Xem chi tiết'
            });
        }

        if (config.hasEdit) {
            actionsColumn.buttons.push({
                type: 'edit',
                className: 'btn btn-sm btn-warning me-1',
                icon: 'bi-pencil',
                title: 'Chỉnh sửa'
            });
        }

        if (config.hasDelete) {
            actionsColumn.buttons.push({
                type: 'delete',
                className: 'btn btn-sm btn-danger me-1',
                icon: 'bi-trash',
                title: 'Xóa'
            });
        }

        if (config.hasApprove) {
            actionsColumn.buttons.push({
                type: 'approve',
                className: 'btn btn-sm btn-success me-1',
                icon: 'bi-check-circle',
                title: 'Phê duyệt'
            });
        }

        if (config.hasSuspend) {
            actionsColumn.buttons.push({
                type: 'suspend',
                className: 'btn btn-sm btn-secondary me-1',
                icon: 'bi-pause-circle',
                title: 'Tạm ngưng'
            });
        }
    }

    /**
     * Get button class based on action type
     */
    getActionButtonClass(actionType) {
        const classMap = {
            'view': 'info',
            'edit': 'warning',
            'delete': 'danger',
            'approve': 'success',
            'suspend': 'secondary',
            'permissions': 'info',
            'assign': 'success'
        };
        return classMap[actionType] || 'outline-primary';
    }

    /**
     * Parse columns configuration
     * @param {string} columnsStr - Columns string
     * @returns {Array} Columns array
     */
    parseColumns() {
        // Check if customGridColumnConfig is available and use it first
        if (typeof window.customGridColumnConfig === 'function') {
            try {
                const customColumns = window.customGridColumnConfig();
                if (Array.isArray(customColumns) && customColumns.length > 0) {
                    return customColumns;
                }
            } catch (error) {
                console.error('Error calling customGridColumnConfig:', error);
            }
        }
    }

    /**
     * Parse sample data
     * @param {string} dataStr - Sample data string
     * @returns {Array} Sample data array
     */
    parseSampleData(dataStr) {
        if(!dataStr) {
            return [
                { id: 1, name: 'Sample Item 1', status: 'active', badge: 'success' },
                { id: 2, name: 'Sample Item 2', status: 'inactive', badge: 'secondary' },
                { id: 3, name: 'Sample Item 3', status: 'pending', badge: 'warning' }
            ];
        }

        try {
            return JSON.parse(dataStr);
        } catch(e) {
            console.error('Invalid sample data JSON:', e);
            return [];
        }
    }

    /**
     * Calculate column count for colspan
     * @param {object} config - Configuration object
     * @returns {number} Column count
     */
    getColumnCount(config) {
        let count = 0;
        
        // Count visible data columns
        config.columns.forEach(col => {
            if (col.type !== 'actions' || col.visible !== false) {
                count++;
            }
        });
        
        // Add select all column
        if (config.hasSelectAll) {
            count++;
        }
        
        return count;
    }

    /**
     * Load data from AJAX URL with pagination support
     * @param {object} config - Configuration object
     * @param {number} page - Current page number (default: 1)
     * @param {number} pageSize - Items per page (default: 10) 
     * @param {string} filter - Search filter (optional)
     */
    loadData(config, page = 1, pageSize = 10, filter = null) {
        if (!config.getUrl) {
            console.warn('No data-get-url provided for grid:', config.containerId);
            return;
        }

        // Show loading state
        this.showLoading(config);

        // Prepare pagination data for GetPaging API
        const pagingData = {
            page: page,
            pageSize: pageSize,
            filter: filter || ""  // Đảm bảo luôn là string, không phải null/undefined
        };

        // Use calGetAPIAuthen with pagination parameters
        calGetAPIAuthen(config.getUrl, pagingData,
            (response) => {
                // Handle paginated response
                if (response && response.data) {
                    // Store pagination info for later use
                    config.pagination = {
                        currentPage: response.data.currentPage || page,
                        totalPages: response.data.totalPages || 1,
                        totalCount: response.data.totalCount || 0,
                        pageSize: response.data.pageSize || pageSize
                    };
                    
                    this.renderTableData(config, response.data.data);
                    this.renderPagination(config); // Render pagination controls
                } else {
                    this.renderTableData(config, response.data || response);
                }
                this.hideLoading(config);
            },
            (error) => {
                console.error('Error loading data:', error);
                this.showError(config, 'Không thể tải dữ liệu. Vui lòng thử lại.');
                this.hideLoading(config);
            }
        );
    }

    /**
     * Refresh data from server
     * @param {object} config - Configuration object
     * @param {number} page - Current page number
     * @param {number} pageSize - Items per page
     * @param {string} filter - Search filter
     */
    refreshData(config, page = 1, pageSize = 10, filter = null) {
        this.loadData(config, page, pageSize, filter);
    }

    /**
     * Render pagination controls
     * @param {object} config - Configuration object
     */
    renderPagination(config) {
        if (!config.pagination) return;

        const { currentPage, totalPages, totalCount, pageSize } = config.pagination;
        const $container = $(`#${config.containerId}`);
        
        // Remove existing pagination
        $container.find('.pagination-container').remove();
        
        if (totalPages <= 1) return; // Don't show pagination if only 1 page

        // Create pagination HTML
        let paginationHtml = `
            <div class="pagination-container d-flex justify-content-between align-items-center mt-3">
                <div class="pagination-info">
                    <span class="text-muted">
                        Hiển thị ${(currentPage - 1) * pageSize + 1} - ${Math.min(currentPage * pageSize, totalCount)} 
                        của ${totalCount} bản ghi
                    </span>
                </div>
                <nav aria-label="Table pagination">
                    <ul class="pagination pagination-sm mb-0">
        `;

        // Previous button
        if (currentPage > 1) {
            paginationHtml += `
                <li class="page-item">
                    <a class="page-link" href="#" data-page="${currentPage - 1}">‹</a>
                </li>
            `;
        } else {
            paginationHtml += `<li class="page-item disabled"><span class="page-link">‹</span></li>`;
        }

        // Page numbers
        const startPage = Math.max(1, currentPage - 2);
        const endPage = Math.min(totalPages, currentPage + 2);

        if (startPage > 1) {
            paginationHtml += `<li class="page-item"><a class="page-link" href="#" data-page="1">1</a></li>`;
            if (startPage > 2) {
                paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
        }

        for (let i = startPage; i <= endPage; i++) {
            if (i === currentPage) {
                paginationHtml += `<li class="page-item active"><span class="page-link">${i}</span></li>`;
            } else {
                paginationHtml += `<li class="page-item"><a class="page-link" href="#" data-page="${i}">${i}</a></li>`;
            }
        }

        if (endPage < totalPages) {
            if (endPage < totalPages - 1) {
                paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
            paginationHtml += `<li class="page-item"><a class="page-link" href="#" data-page="${totalPages}">${totalPages}</a></li>`;
        }

        // Next button
        if (currentPage < totalPages) {
            paginationHtml += `
                <li class="page-item">
                    <a class="page-link" href="#" data-page="${currentPage + 1}">›</a>
                </li>
            `;
        } else {
            paginationHtml += `<li class="page-item disabled"><span class="page-link">›</span></li>`;
        }

        paginationHtml += `
                    </ul>
                </nav>
            </div>
        `;

        // Append pagination to container
        $container.append(paginationHtml);

        // Bind pagination click events
        $container.find('.pagination a.page-link').on('click', (e) => {
            e.preventDefault();
            const page = parseInt($(e.target).data('page'));
            if (page && page !== currentPage) {
                this.loadData(config, page, pageSize, config.currentFilter);
            }
        });
    }

    /**
     * Show loading state
     * @param {object} config - Configuration object
     */
    showLoading(config) {
        const $tableBody = $(`#${config.containerId}TableBody`);
        // const tableBody = $tableBody.length > 0 ? $tableBody[0] : null;
        if ($tableBody.length > 0) {
            const colCount = this.getColumnCount(config);
            $tableBody.html(`
                <tr>
                    <td colspan="${colCount}" class="text-center py-4">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Đang tải...</span>
                        </div>
                        <div class="mt-2">Đang tải dữ liệu...</div>
                    </td>
                </tr>
            `);
        }
    }

    /**
     * Hide loading state
     * @param {object} config - Configuration object
     */
    hideLoading(_config) {
        // Loading will be replaced by actual data or error message
    }

    /**
     * Show error message
     * @param {object} config - Configuration object
     * @param {string} message - Error message
     */
    showError(config, message) {
        const $tableBody = $(`#${config.containerId}TableBody`);
        if ($tableBody.length > 0) {
            const colCount = this.getColumnCount(config);
            $tableBody.html(`
                <tr>
                    <td colspan="${colCount}" class="text-center py-4 text-danger">
                        <i class="bi bi-exclamation-triangle fs-1"></i>
                        <div class="mt-2">${message}</div>
                        <button class="btn btn-outline-primary btn-sm mt-2" onclick="${config.containerId}Refresh()">
                            <i class="bi bi-arrow-clockwise"></i> Thử lại
                        </button>
                    </td>
                </tr>
            `);
        }
    }

    /**
     * Render table data
     * @param {object} config - Configuration object
     * @param {Array} data - Data array
     */
    renderTableData(config, data) {
        let tableBody;

        if (config.useExistingTable && config.tableId) {
            // Use existing table
            const $table = $(`#${config.tableId}`);
            let $tableBody = $table.find('tbody');

            if ($tableBody.length === 0) {
                // Create tbody if not exists
                $tableBody = $('<tbody></tbody>');
                $table.append($tableBody);
            }
            tableBody = $tableBody[0];
        } else {
            // Use generated table
            const $tableBody = $(`#${config.containerId}TableBody`);
            tableBody = $tableBody.length > 0 ? $tableBody[0] : null;
        }

        if (!tableBody) {
            console.error('Table body not found for config:', config);
            return;
        }

        if (!data || !Array.isArray(data) || data.length === 0) {
            const colCount = this.getColumnCount(config);
            tableBody.innerHTML = `
                <tr>
                    <td colspan="${colCount}" class="text-center py-4 text-muted">
                        <i class="bi bi-inbox fs-1"></i>
                        <div class="mt-2">Không có dữ liệu</div>
                    </td>
                </tr>
            `;
            return;
        }

        let html = '';
        data.forEach((item, index) => {
            html += `<tr>`;

            // Select checkbox
            if(config.hasSelectAll) {
                html += `
                    <td>
                        <input type="checkbox" class="form-check-input row-checkbox" value="${item.id || index}">
                    </td>
                `;
            }

            // Data columns (including actions if visible)
            config.columns.forEach(column => {
                // Skip actions column if not visible
                if (column.type === 'actions' && column.visible === false) {
                    return;
                }
                const value = this.getColumnValue(item, column, config.columns);
                html += `<td>${value}</td>`;
            });

            html += `</tr>`;
        });

        tableBody.innerHTML = html;

        // Re-initialize dropdowns after rendering new data
        this.initializeDropdowns(config);
    }

    /**
     * Get column value from data item
     * @param {object} item - Data item
     * @param {object} column - Column configuration
     * @param {Array} columns - All columns configuration
     * @returns {string} Formatted value
     */
    getColumnValue(item, column, columns = []) {
        let value = item[column.field] || '';

        // Use custom grid column renderer if available (supports individual column customization)
        if (typeof window.customGridColumnRenderer === 'function') {
            const customResult = window.customGridColumnRenderer(item, column, value, columns);
            if (customResult !== null && customResult !== undefined) {
                return customResult;
            }
        }

        // Handle different column types
        switch (column.type) {
            case 'text':
                return value || '';

            case 'number':
                if (!value) return '';
                const numValue = parseFloat(value);
                if (isNaN(numValue)) return value;
                
                const format = column.format || {};
                const decimal = format.decimal || 0;
                // const thousandSeparator = format.thousandSeparator || ',';
                
                return numValue.toLocaleString('vi-VN', {
                    minimumFractionDigits: decimal,
                    maximumFractionDigits: decimal
                });

            case 'money':
                if (!value) return '';
                const moneyValue = parseFloat(value);
                if (isNaN(moneyValue)) return value;
                
                const moneyFormat = column.format || {};
                const currency = moneyFormat.currency || 'VND';
                const showSymbol = moneyFormat.showSymbol !== false;
                
                let moneyHtml = moneyValue.toLocaleString('vi-VN');
                if (showSymbol && currency === 'VND') {
                    moneyHtml += ' ₫';
                } else if (showSymbol && currency === 'USD') {
                    moneyHtml = '$' + moneyHtml;
                }
                
                return `<span class="text-success fw-bold">${moneyHtml}</span>`;

            case 'date':
                if (!value) return '';
                try {
                    const date = new Date(value);
                    const dateFormat = column.format || {};
                    const locale = dateFormat.locale || 'vi-VN';
                    
                    return date.toLocaleDateString(locale);
                } catch (e) {
                    return value;
                }

            case 'datetime':
                if (!value) return '';
                try {
                    const date = new Date(value);
                    const datetimeFormat = column.format || {};
                    const relative = datetimeFormat.relative || false;
                    
                    if (relative) {
                        const now = new Date();
                        const diffMs = now - date;
                        const diffMins = Math.floor(diffMs / 60000);
                        const diffHours = Math.floor(diffMins / 60);
                        const diffDays = Math.floor(diffHours / 24);

                        if (diffMins < 1) return '<span class="text-success">Vừa xong</span>';
                        if (diffMins < 60) return `<span class="text-info">${diffMins} phút trước</span>`;
                        if (diffHours < 24) return `<span class="text-warning">${diffHours} giờ trước</span>`;
                        if (diffDays < 7) return `<span class="text-muted">${diffDays} ngày trước</span>`;
                        
                        return `<span class="text-muted">${date.toLocaleDateString('vi-VN')}</span>`;
                    } else {
                        const locale = datetimeFormat.locale || 'vi-VN';
                        const showSeconds = datetimeFormat.showSeconds || false;
                        
                        const options = {
                            year: 'numeric',
                            month: '2-digit', 
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit'
                        };
                        
                        if (showSeconds) {
                            options.second = '2-digit';
                        }
                        
                        return date.toLocaleString(locale, options);
                    }
                } catch (e) {
                    return value;
                }

            case 'time':
                if (!value) return '';
                try {
                    const date = new Date(value);
                    const timeFormat = column.format || {};
                    const showSeconds = timeFormat.showSeconds || false;
                    
                    const options = {
                        hour: '2-digit',
                        minute: '2-digit'
                    };
                    
                    if (showSeconds) {
                        options.second = '2-digit';
                    }
                    
                    return date.toLocaleTimeString('vi-VN', options);
                } catch (e) {
                    return value;
                }

            case 'enum':
                if (!value) return '';
                
                const enumFormat = column.format || {};
                const badge = enumFormat.badge !== false;
                const badgeColors = enumFormat.badgeColors || {};
                const showEnumIcon = enumFormat.showIcon || false;
                
                if (badge) {
                    const badgeClass = badgeColors[value?.toLowerCase()] || 'secondary';
                    let enumHtml = `<span class="badge bg-${badgeClass}">`;
                    
                    if (showEnumIcon && enumFormat.icons && enumFormat.icons[value]) {
                        enumHtml += `<i class="bi ${enumFormat.icons[value]} me-1"></i>`;
                    }
                    
                    enumHtml += `${value}</span>`;
                    return enumHtml;
                } else {
                    return value;
                }

            case 'bool':
                const boolFormat = column.format || {};
                const trueText = boolFormat.trueText || 'Có';
                const falseText = boolFormat.falseText || 'Không';
                const trueClass = boolFormat.trueClass || 'text-success';
                const falseClass = boolFormat.falseClass || 'text-danger';
                const showBoolIcon = boolFormat.showIcon || false;
                const trueIcon = boolFormat.trueIcon || 'bi-check-circle';
                const falseIcon = boolFormat.falseIcon || 'bi-x-circle';
                
                const boolValue = value === true || value === 'true' || value === 1 || value === '1';
                const displayText = boolValue ? trueText : falseText;
                const displayClass = boolValue ? trueClass : falseClass;
                const displayIcon = boolValue ? trueIcon : falseIcon;
                
                let boolHtml = `<span class="${displayClass} fw-bold">`;
                if (showBoolIcon) {
                    boolHtml += `<i class="bi ${displayIcon} me-1"></i>`;
                }
                boolHtml += `${displayText}</span>`;
                
                return boolHtml;

            case 'actions':
                return this.generateActionsColumn(item, column);

            default:
                return value || '';
        }
    }

    /**
     * Generate actions column HTML
     * @param {object} item - Data item
     * @param {object} column - Column configuration
     * @returns {string} Actions HTML
     */
    generateActionsColumn(item, column) {
        if (!column.buttons || !Array.isArray(column.buttons) || column.buttons.length === 0) {
            // Default actions dropdown
            return `
                <div class="dropdown pss-none">
                    <button class="btn btn-sm btn-light border-0" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="bi bi-three-dots-vertical text-muted"></i>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end shadow-sm border-0" style="min-width: 150px;">
                        <li><a class="dropdown-item py-2" href="#" onclick="loadDetailFormModal(this, 'view', ${item.id}); return false;"><i class="bi bi-eye text-primary me-2"></i>Xem chi tiết</a></li>
                        <li><a class="dropdown-item py-2" href="#" onclick="loadDetailFormModal(this, 'edit', ${item.id}); return false;"><i class="bi bi-pencil text-warning me-2"></i>Chỉnh sửa</a></li>
                        <li><hr class="dropdown-divider my-1"></li>
                        <li><a class="dropdown-item py-2 text-danger" href="#" onclick="deleteItem(${item.id}); return false;"><i class="bi bi-trash me-2"></i>Xóa</a></li>
                    </ul>
                </div>
            `;
        }

        // Custom actions dropdown with configured buttons
        let actionsHtml = `
            <div class="dropdown pss-none">
                <button class="btn btn-sm btn-light border-0" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bi bi-three-dots-vertical text-muted"></i>
                </button>
                <ul class="dropdown-menu dropdown-menu-end shadow-sm border-0" style="min-width: 150px;">
        `;
        
        column.buttons.forEach((button, index) => {
            const icon = button.icon || 'bi-gear';
            const title = button.title || 'Action';
            const buttonType = button.type || 'view';
            
            // Determine action function name and CSS class
            let actionFunction = '';
            let itemClass = 'py-2';
            let iconClass = '';
            
            switch(buttonType) {
                case 'view':
                    actionFunction = `loadDetailFormModal(this, 'view', ${item.id})`;
                    iconClass = 'text-primary';
                    break;
                case 'edit':
                    actionFunction = `loadDetailFormModal(this, 'edit', ${item.id})`;
                    iconClass = 'text-warning';
                    break;
                case 'delete':
                    actionFunction = `deleteItem(${item.id})`;
                    itemClass = 'py-2 text-danger';
                    iconClass = '';
                    break;
                case 'approve':
                    actionFunction = `approveItem(${item.id})`;
                    iconClass = 'text-success';
                    break;
                case 'suspend':
                    actionFunction = `suspendItem(${item.id})`;
                    iconClass = 'text-secondary';
                    break;
                case 'permissions':
                    actionFunction = `managePermissions(${item.id})`;
                    iconClass = 'text-info';
                    break;
                case 'assign':
                    actionFunction = `assignPermissions(${item.id})`;
                    iconClass = 'text-success';
                    break;
                default:
                    actionFunction = `defaultAction(${item.id})`;
                    iconClass = 'text-muted';
            }
            
            // Add divider before delete action if it's not the first item
            if (buttonType === 'delete' && index > 0) {
                actionsHtml += `<li><hr class="dropdown-divider my-1"></li>`;
            }
            
            actionsHtml += `
                <li>
                    <a class="dropdown-item ${itemClass}" href="#" onclick="${actionFunction}; return false;">
                        <i class="${icon} ${iconClass} me-2"></i>${title}
                    </a>
                </li>
            `;
        });
        
        actionsHtml += `
                </ul>
            </div>
        `;
        
        return actionsHtml;
    }

    /**
     * Generate grid HTML
     * @param {object} config - Configuration object
     * @returns {string} Generated HTML
     */
    generateGridHTML(config) {
        // Determine grid title
        const gridTitle = config.titleGrid || `Danh sách ${config.entityPlural}`;
        
        let html = `
            <!-- Data Grid -->
            <div class="card">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">
                        <i class="bi bi-list-ul me-2"></i>${gridTitle}
                    </h5>
                    <div>
        `;

        // Action buttons
        if(config.hasRefresh) {
            html += `
                <button type="button" class="btn btn-outline-secondary btn-sm me-2" onclick="${config.containerId}Refresh()">
                    <i class="bi bi-arrow-clockwise"></i> ${config.refreshLabel || ''}
                </button>
            `;
        }

        if(config.hasExport) {
            html += `
                <button type="button" class="btn btn-success btn-sm me-2" onclick="${config.containerId}Export()">
                    <i class="bi bi-download"></i> ${config.exportLabel || ''}
                </button>
            `;
        }

        if(config.hasAdd) {
            html += `
                <button type="button" class="btn btn-primary btn-sm" onclick="${config.containerId}Add()">
                    <i class="bi bi-plus-circle"></i> ${config.addLabel || ''}
                </button>
            `;
        }

        html += `
                    </div>
                </div>
                
                <!-- Search Box -->
                <div class="card-header border-top py-2">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="input-group input-group-sm">
                                <span class="input-group-text">
                                    <i class="bi bi-search"></i>
                                </span>
                                <input type="text" class="form-control" id="${config.containerId}SearchInput" 
                                       placeholder="Tìm kiếm..." data-grid-search>
                                <button class="btn btn-outline-secondary" type="button" id="${config.containerId}SearchBtn">
                                    Tìm
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="card-body p-0">
                    <div class="table-responsive">
                        <table class="table table-hover mb-0">
                            <thead class="table-light">
                                <tr>
        `;

        // Select all checkbox
        if(config.hasSelectAll) {
            html += `
                <th style="width: 50px;">
                    <input type="checkbox" class="form-check-input" id="${config.containerId}SelectAll">
                </th>
            `;
        }

        // Column headers
        config.columns.forEach(col => {
            if(col.type !== 'actions') {
                const widthStyle = col.width ? ` style="width: ${col.width};"` : '';
                const columnTitle = col.title || col.label || col.field || '';
                html += `<th${widthStyle}>${columnTitle}</th>`;
            }
        });

        // Actions column (always exists but check visibility)
        const actionsCol = config.columns.find(col => col.type === 'actions');
        if(actionsCol && actionsCol.visible !== false) {
            const widthStyle = actionsCol.width ? ` style="width: ${actionsCol.width};"` : ' style="width: 120px;"';
            const actionsTitle = actionsCol.title || actionsCol.label || 'Actions';
            html += `<th class="text-center"${widthStyle}>${actionsTitle}</th>`;
        }

        html += `
                                </tr>
                            </thead>
                            <tbody id="${config.containerId}TableBody">
        `;

        // Sample data rows (only if no URL provided)
        if (!config.getUrl) {
            config.sampleData.forEach((item, index) => {
            html += `<tr>`;

            // Select checkbox
            if(config.hasSelectAll) {
                html += `
                    <td>
                        <input type="checkbox" class="form-check-input row-checkbox" value="${item.id || index}">
                    </td>
                `;
            }

            // Data columns
            config.columns.forEach(col => {
                if(col.type === 'text') {
                    html += `<td>${item[col.key] || '-'}</td>`;
                } else if(col.type === 'badge') {
                    const badgeClass = item.badge || 'secondary';
                    html += `<td><span class="badge bg-${badgeClass}">${item[col.key] || '-'}</span></td>`;
                } else if(col.type === 'icon') {
                    html += `<td><i class="bi bi-${item[col.key] || 'circle'}"></i></td>`;
                }
            });

            // Actions column (Dropdown style) - check visibility
            if(actionsCol && actionsCol.visible !== false) {
                html += `
                    <td class="text-center">
                        <div class="dropdown">
                            <button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button"
                                    data-bs-toggle="dropdown" aria-expanded="false" title="Actions">
                                <i class="bi bi-three-dots"></i>
                            </button>
                            <ul class="dropdown-menu dropdown-menu-end">
                                <li>
                                    <a class="dropdown-item" href="#" onclick="${config.containerId}View(${item.id || index}); return false;">
                                        <i class="bi bi-eye text-primary me-2"></i>Xem chi tiết
                                    </a>
                                </li>
                                <li>
                                    <a class="dropdown-item" href="#" onclick="${config.containerId}Edit(${item.id || index}); return false;">
                                        <i class="bi bi-pencil text-warning me-2"></i>Chỉnh sửa
                                    </a>
                                </li>
                                <li><hr class="dropdown-divider"></li>
                                <li>
                                    <a class="dropdown-item" href="#" onclick="${config.containerId}Delete(${item.id || index}); return false;">
                                        <i class="bi bi-trash text-danger me-2"></i>Xóa
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </td>
                `;
            }

            html += `</tr>`;
            });
        }

        html += `
                            </tbody>
                        </table>
                    </div>
                </div>
        `;

        // Pagination
        if(config.hasPagination) {
            // const totalPages = Math.ceil(config.totalItems / config.pageSize);
            const currentPage = 1;
            const startItem = (currentPage - 1) * config.pageSize + 1;
            const endItem = Math.min(currentPage * config.pageSize, config.totalItems);

            html += `
                <div class="card-footer">
                    <div class="d-flex justify-content-between align-items-center">
                        <div class="text-muted">
                            Hiển thị <strong>${startItem}-${endItem}</strong> trong tổng số <strong>${config.totalItems}</strong> ${config.entityPlural.toLowerCase()}
                        </div>
                        <nav>
                            <ul class="pagination pagination-sm mb-0">
                                <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                                    <span class="page-link">Trước</span>
                                </li>
                                <li class="page-item active">
                                    <span class="page-link">${currentPage}</span>
                                </li>
                                <li class="page-item">
                                    <a class="page-link" href="#">2</a>
                                </li>
                                <li class="page-item">
                                    <a class="page-link" href="#">3</a>
                                </li>
                                <li class="page-item">
                                    <a class="page-link" href="#">Sau</a>
                                </li>
                            </ul>
                        </nav>
                    </div>
                </div>
            `;
        }

        html += `
            </div>
        `;

        return html;
    }

    /**
     * Initialize grid logic
     * @param {object} config - Configuration object
     */
    initializeGridLogic(config) {
        // Select all functionality
        if(config.hasSelectAll) {
            const $selectAllCheckbox = $(`#${config.containerId}SelectAll`);
            if($selectAllCheckbox.length > 0) {
                $selectAllCheckbox.on('change', function () {
                    const isChecked = $(this).prop('checked');
                    $('.row-checkbox').prop('checked', isChecked);
                });
            }
        }

        // Initialize search functionality
        this.initializeSearch(config);

        // Create global action functions
        this.createGlobalFunctions(config);

        // Initialize Bootstrap dropdowns
        this.initializeDropdowns(config);
    }

    /**
     * Initialize search functionality
     * @param {object} config - Configuration object
     */
    initializeSearch(config) {
        const $searchInput = $(`#${config.containerId}SearchInput`);
        const $searchBtn = $(`#${config.containerId}SearchBtn`);

        if ($searchInput.length > 0) {
            // Search button click
            $searchBtn.on('click', () => {
                const searchTerm = $searchInput.val().trim();
                config.currentFilter = searchTerm;
                this.loadData(config, 1, 10, searchTerm); // Reset to page 1 when searching
            });

            // Enter key search
            $searchInput.on('keypress', (e) => {
                if (e.which === 13) { // Enter key
                    const searchTerm = $searchInput.val().trim();
                    config.currentFilter = searchTerm;
                    this.loadData(config, 1, 10, searchTerm); // Reset to page 1 when searching
                }
            });

            // Clear search when input is empty
            $searchInput.on('input', (e) => {
                if ($(e.target).val().trim() === '') {
                    config.currentFilter = null;
                    this.loadData(config, 1, 10, null); // Reload without filter
                }
            });
        }
    }

    /**
     * Initialize Bootstrap dropdowns
     * @param {object} config - Configuration object
     */
    initializeDropdowns(config) {
        // Wait a bit for DOM to be ready
        setTimeout(() => {
            // Use jQuery if available for better compatibility
            if(typeof $ !== 'undefined') {
                const $dropdowns = $(`[data-component="data-grid"][data-container-id="${config.containerId}"] .dropdown-toggle`);

                $dropdowns.each(function () {
                    const $this = $(this);

                    // Remove any existing click handlers
                    $this.off('click.dropdown');

                    // Add manual click handler
                    $this.on('click.dropdown', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        // Close all other dropdowns
                        $('.dropdown-menu.show').removeClass('show');

                        // Toggle this dropdown
                        const $menu = $this.next('.dropdown-menu');
                        if($menu.length) {
                            $menu.toggleClass('show');
                        }
                    });
                });

                // Click outside to close
                $(document).off('click.dropdown-outside').on('click.dropdown-outside', function (e) {
                    if(!$(e.target).closest('.dropdown').length) {
                        $('.dropdown-menu.show').removeClass('show');
                    }
                });

                return; // Exit early if jQuery worked
            }

            // Fallback to jQuery
            let $dropdownElements = $(`[data-grid-container="${config.containerId}"] .dropdown-toggle`);

            if($dropdownElements.length === 0) {
                // Fallback: find all dropdown toggles in the component
                const $gridContainer = $(`[data-component="data-grid"][data-container-id="${config.containerId}"]`);
                if($gridContainer.length > 0) {
                    $dropdownElements = $gridContainer.find('.dropdown-toggle');
                }
            }

            $dropdownElements.each((_, element) => {
                try {
                    // Initialize Bootstrap dropdown
                    if(typeof bootstrap !== 'undefined' && bootstrap.Dropdown) {
                        new bootstrap.Dropdown(element);
                    } else {
                        // Fallback: manual toggle using jQuery
                        $(element).on('click', function (e) {
                            e.preventDefault();
                            e.stopPropagation();

                            // Close all other dropdowns first
                            $('.dropdown-menu.show').removeClass('show');

                            const $menu = $(this).next('.dropdown-menu');
                            if($menu.length > 0) {
                                $menu.toggleClass('show');
                            }
                        });
                    }
                } catch(error) {
                    console.error('Error initializing dropdown:', error);
                    // Fallback: manual toggle using jQuery
                    $(element).on('click', function (e) {
                        e.preventDefault();
                        const $menu = $(this).next('.dropdown-menu');
                        if($menu.length > 0) {
                            $menu.toggleClass('show');
                        }
                    });
                }
            });

            // Add click outside to close dropdown using jQuery
            $(document).on('click', function (e) {
                if(!$(e.target).closest('.dropdown').length) {
                    $('.dropdown-menu.show').removeClass('show');
                }
            });
        }, 100);
    }

    /**
     * Create global action functions
     * @param {object} config - Configuration object
     */
    createGlobalFunctions(config) {
        const containerId = config.containerId;
        const entity = config.entity;

        // View function
        window[`${containerId}View`] = function (id) {
            alert(`Xem chi tiết ${entity} ID: ${id}`);
        };

        // Edit function
        window[`${containerId}Edit`] = function (id) {
            alert(`Chỉnh sửa ${entity} ID: ${id}`);
        };

        // Delete function
        window[`${containerId}Delete`] = function (id) {
            if(confirm(`Bạn có chắc chắn muốn xóa ${entity} ID: ${id}?`)) {
                alert(`Đã xóa ${entity} ID: ${id}`);
            }
        };

        // Add function - only create if not already exists
        const addFunctionName = `${containerId}Add`;
        if (typeof window[addFunctionName] !== 'function') {
            window[addFunctionName] = function () {
                // Check if detail-form is specified
                if (config.detailForm) {
                    // Load modal from external view
                    this.loadDetailFormModal(config, 'add');
                } else {
                    // Default behavior
                    alert(`Thêm ${entity} mới`);
                }
            }.bind(this);
        }

        // Refresh function
        window[`${containerId}Refresh`] = () => {
            const gridInstance = window.dataGridInstance;
            if (gridInstance && config.getUrl) {
                // Preserve current pagination and filter
                const currentPage = config.pagination ? config.pagination.currentPage : 1;
                const pageSize = config.pagination ? config.pagination.pageSize : 10;
                const currentFilter = config.currentFilter || null;
                
                gridInstance.refreshData(config, currentPage, pageSize, currentFilter);
            } else {
                alert(`Làm mới dữ liệu ${entity}`);
            }
        };

        // Export function
        window[`${containerId}Export`] = function () {
            alert(`Xuất dữ liệu ${entity} ra Excel`);
        };
    }

    /**
     * Load detail form modal from external view
     * @param {object} config - Configuration object
     * @param {string} mode - Modal mode: 'add', 'view', 'edit'
     * @param {number} itemId - Item ID for view/edit modes
     */
    loadDetailFormModal(config, mode = 'add', itemId = null) {
        // Check if modal container already exists
        let modalContainer = $(`#${config.containerId}ModalContainer`);

        if (modalContainer.length === 0) {
            // Create modal container
            modalContainer = $(`<div id="${config.containerId}ModalContainer" class="modal-container"></div>`);
            $('body').append(modalContainer);
        }
        
        // Show loading
        modalContainer.html(`
            <div class="modal fade show" tabindex="-1" style="display: block; background: rgba(0,0,0,0.5);">
                <div class="modal-dialog modal-xl">
                    <div class="modal-content">
                        <div class="modal-body text-center py-5">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">Đang tải...</span>
                            </div>
                            <div class="mt-2">Đang tải form...</div>
                        </div>
                    </div>
                </div>
            </div>
        `);
        
        // Build URL to load form
        // Determine controller from current page URL or use a more flexible approach
        let controller = 'Demo'; // default fallback
        
        // Try to get controller from current URL path
        const currentPath = window.location.pathname;
        const pathParts = currentPath.split('/').filter(part => part.length > 0);
        if (pathParts.length > 0) {
            controller = pathParts[0]; // First part is usually the controller
        }
        
        const formUrl = `/${controller}/${config.detailForm}?mode=modal&entity=${config.entity}`;
        
        // Load form content via jQuery.get (for HTML response)
        if (typeof $ !== 'undefined') {
            $.get(formUrl)
                .done((html) => {
                    this.renderDetailFormModal(modalContainer, html, config, mode, itemId);
                })
                .fail((error) => {
                    console.error('Error loading form:', error);
                    this.showFormLoadError(modalContainer, config);
                });
        } else {
            // Use fetch API as fallback
            fetch(formUrl)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    return response.text();
                })
                .then(html => {
                    this.renderDetailFormModal(modalContainer, html, config, mode, itemId);
                })
                .catch(error => {
                    console.error('Error loading form:', error);
                    this.showFormLoadError(modalContainer, config);
                });
        }
    }

    /**
     * Render loaded form modal
     * @param {HTMLElement} modalContainer - Modal container element
     * @param {string} html - Loaded HTML content
     * @param {object} config - Configuration object
     * @param {string} mode - Modal mode: 'add', 'view', 'edit'
     * @param {number} itemId - Item ID for view/edit modes
     */
    renderDetailFormModal(modalContainer, html, config, mode = 'add', _itemId = null) {
        // Extract form content from loaded HTML
        const $tempDiv = $('<div>').html(html);

        // Look for form or modal in the loaded content
        const $form = $tempDiv.find('form');
        const $modal = $tempDiv.find('.modal');
        const form = $form.length > 0 ? $form[0] : ($modal.length > 0 ? $modal[0] : $tempDiv[0]);
        
        // Determine modal title and icon based on mode
        let modalTitle = '';
        let modalIcon = '';
        
        switch(mode) {
            case 'add':
                modalTitle = `Thêm ${config.entity} Mới`;
                modalIcon = 'bi-plus-circle';
                break;
            case 'view':
                modalTitle = `Xem Chi Tiết ${config.entity}`;
                modalIcon = 'bi-eye';
                break;
            case 'edit':
                modalTitle = `Chỉnh Sửa ${config.entity}`;
                modalIcon = 'bi-pencil';
                break;
            default:
                modalTitle = `${config.entity}`;
                modalIcon = 'bi-info-circle';
        }
        
        // Create modal wrapper
        modalContainer.html(`
            <div class="modal fade show" id="${config.containerId}DetailModal" tabindex="-1" style="display: block; background: rgba(0,0,0,0.5);">
                <div class="modal-dialog modal-xl">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">
                                <i class="bi ${modalIcon} me-2"></i>${modalTitle}
                            </h5>
                            <button type="button" class="btn-close" onclick="this.closest('.modal-container').remove()"></button>
                        </div>
                        <div class="modal-body">
                            ${form.innerHTML}
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" onclick="this.closest('.modal-container').remove()">
                                <i class="bi bi-x-circle me-1"></i>Hủy
                            </button>
                            <button type="button" class="btn btn-primary" id="saveBtn" onclick="saveModalForm()">
                                <i class="bi bi-check-circle me-1"></i>Lưu
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `);
        
        // Modal container is already appended to body, just show it

        // Bind form controls after modal is added to DOM
        setTimeout(() => {
            if (window.formControlBinder) {
                window.formControlBinder.init();
            }

            // Add real-time validation clearing
            const $modal = $('.modal.show');
            $modal.on('input change', 'input, select, textarea', function() {
                const $element = $(this);

                // Clear error state when user starts typing
                if ($element.hasClass('is-invalid') || $element.hasClass('validation-error')) {
                    $element.removeClass('is-invalid border-danger validation-error');
                    $element.removeAttr('style'); // Xóa toàn bộ inline style

                    // Reset về style mặc định
                    $element.css({
                        'border': '',
                        'border-color': '',
                        'box-shadow': '',
                        'background-color': ''
                    });

                    // Remove error message
                    $element.closest('.mb-3, .form-group, .col').find('.invalid-feedback').remove();
                }
            });
        }, 100);
    }

    /**
     * Show form load error
     * @param {HTMLElement} modalContainer - Modal container element
     * @param {object} config - Configuration object
     */
    showFormLoadError(modalContainer, config) {
        modalContainer.html(`
            <div class="modal fade show" tabindex="-1" style="display: block; background: rgba(0,0,0,0.5);">
                <div class="modal-dialog modal-xl">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-danger">
                                <i class="bi bi-exclamation-triangle me-2"></i>Lỗi
                            </h5>
                            <button type="button" class="btn-close" onclick="this.closest('.modal-container').remove()"></button>
                        </div>
                        <div class="modal-body text-center">
                            <i class="bi bi-exclamation-triangle text-danger fs-1"></i>
                            <div class="mt-2">Không thể tải form. Vui lòng thử lại.</div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" onclick="this.closest('.modal-container').remove()">
                                Đóng
                            </button>
                            <button type="button" class="btn btn-primary" onclick="this.closest('.modal-container').remove(); ${config.containerId}Add();">
                                Thử lại
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `);
    }

    /**
     * Get grid instance by container ID
     * @param {string} containerId - Container ID
     * @returns {object} Grid configuration
     */
    getGrid(containerId) {
        return this.grids.get(containerId);
    }

    /**
     * Get all grids
     * @returns {Map} All grid instances
     */
    getAllGrids() {
        return this.grids;
    }
}

// Test if script is loading
console.log('DataGrid script loaded!');

// Wait for jQuery to be available
function waitForJQuery() {
    if (typeof $ !== 'undefined') {
        console.log('jQuery is available, initializing DataGrid...');
        initializeDataGrid();
    } else {
        console.log('jQuery not available yet, waiting...');
        setTimeout(waitForJQuery, 100);
    }
}

// Start waiting for jQuery
waitForJQuery();

// Also try with vanilla JS as fallback
document.addEventListener('DOMContentLoaded', function() {
    console.log('DataGrid: Vanilla DOM ready, trying to initialize...');
    setTimeout(initializeDataGrid, 500);
});

function initializeDataGrid() {
    if (window.dataGridInstance) {
        console.log('DataGrid: Already initialized, skipping...');
        return;
    }

    if (typeof $ === 'undefined') {
        console.error('jQuery not available, cannot initialize DataGrid');
        return;
    }

    try {
        console.log('DataGrid: Creating new instance...');
        window.dataGridInstance = new DataGrid();
        console.log('DataGrid: Initialized successfully');
    } catch(error) {
        console.error('DataGrid initialization error:', error);
    }
}

// Global function for action buttons to load detail form modal
window.loadDetailFormModal = function(element, mode, itemId) {
    try {
        // Find the closest data-grid component
        const gridComponent = element.closest('[data-component="data-grid"]');
        if (!gridComponent) {
            console.error('Could not find data-grid component');
            return;
        }
        
        // Get detail form path
        const detailForm = gridComponent.getAttribute('data-detail-form');
        if (!detailForm) {
            console.error('No data-detail-form attribute found');
            return;
        }
        
        // Parse config for this grid
        const config = window.dataGridInstance.parseConfig(gridComponent);
        
        // Load the form modal with mode
        window.dataGridInstance.loadDetailFormModal(config, mode, itemId);
    } catch(error) {
        console.error('Error loading detail form modal:', error);
    }
};

// Global function to save modal form
window.saveModalForm = function() {
    const $modal = $('.modal.show');
    if ($modal.length === 0) {
        console.error('No active modal found');
        return;
    }

    const $form = $modal.find('.modal-content');
    if ($form.length === 0) {
        console.error('No form found in modal');
        return;
    }

    const $saveBtn = $modal.find('#saveBtn');
    if ($saveBtn.length > 0) {
        $saveBtn.prop('disabled', true);
        $saveBtn.html('<i class="bi bi-hourglass-split me-1"></i>Đang lưu...');
    }

    // Get form data using custom data attributes
    const formData = buildFormDataFromAttributes($form);

    if (!formData.isValid) {
        // KHÔNG CHO ĐI TIẾP - Hiển thị lỗi chi tiết
        const errorCount = formData.errors.length;
        const firstError = formData.errors[0];

        // Toast tổng quan
        showToast(`Có ${errorCount} lỗi cần sửa. Vui lòng kiểm tra lại!`, 'error');

        // Alert chi tiết lỗi đầu tiên
        const errorMessages = formData.errors.map(err => `• ${err.message}`).join('\n');
        alert(`❌ KHÔNG THỂ LƯU!\n\nCác lỗi cần sửa:\n${errorMessages}\n\n👆 Vui lòng sửa các lỗi trên trước khi tiếp tục.`);

        // Focus vào control lỗi đầu tiên
        if (firstError && firstError.element) {
            $(firstError.element).focus();
        }

        // Không cho submit
        return;
    }

    const url = $form.attr('action');

    // Submit form using callPostAPIAuthen
    callPostAPIAuthen(url, formData.data,
        (data) => {
            if (data.success) {
                // Show success message
                showToast('Lưu thành công!', 'success');

                // Close modal
                $modal.closest('.modal-container').remove();

                // Reload grid if available
                if (window.currentDataGrid) {
                    window.currentDataGrid.loadData();
                }
            } else {
                showToast(data.message || 'Có lỗi xảy ra', 'error');
            }

            // Restore button state
            if ($saveBtn.length > 0) {
                $saveBtn.prop('disabled', false);
                $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
            }
        },
        (error) => {
            console.error('Form submission error:', error);
            showToast('Có lỗi xảy ra khi lưu', 'error');
            
            // Restore button state
            if ($saveBtn.length > 0) {
                $saveBtn.prop('disabled', false);
                $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
            }
        }
    );
};

// Toast notification function using jQuery
window.showToast = function(message, type = 'info') {
    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'info'} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi bi-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-triangle' : 'info-circle'} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;

    // Create toast container if not exists
    let $toastContainer = $('#toast-container');
    if ($toastContainer.length === 0) {
        $toastContainer = $('<div id="toast-container" class="toast-container position-fixed top-0 end-0 p-3"></div>');
        $('body').append($toastContainer);
    }

    const $toast = $(toastHtml);
    $toastContainer.append($toast);

    const bsToast = new bootstrap.Toast($toast[0]);
    bsToast.show();

    // Remove toast after it's hidden
    $toast.on('hidden.bs.toast', function() {
        $(this).remove();
    });
};

/**
 * Build form data from data attributes instead of FormData
 * @param {jQuery} $container - Container to search for elements
 * @returns {Object} - {data: {}, isValid: boolean, errors: []}
 */
function buildFormDataFromAttributes($container) {
    const result = {
        data: {},
        isValid: true,
        errors: []
    };

    // Find all form elements (input, select, textarea) and data-name elements
    const $elements = $container.find('input, select, textarea, [data-name]');
    console.log('Found elements:', $elements.length);

    $elements.each((_, element) => {
        const $element = $(element);
        const name = $element.attr('name') || $element.attr('data-name');
        const value = getElementValue($element);
        const isRequired = $element.is('[required]') || $element.is('[data-required]');
        const dataType = $element.attr('type') || $element.attr('data-type');

        console.log('Processing element:', { name, value, isRequired, dataType, element });

        if (!name) {
            console.log('Skipping element without name:', element);
            return; // Skip if no name
        }

        // Validate element
        const validation = validateElement($element, value, isRequired, dataType);

        if (!validation.isValid) {
            result.isValid = false;
            result.errors.push({
                name: name,
                message: validation.message,
                element: element
            });

            // Add visual feedback - BÔI ĐỎ VIỀN
            $element.addClass('is-invalid border-danger validation-error');
            $element.removeClass('is-valid');

            // Add red border style - BÔI ĐỎ VIỀN RÕ RÀNG HƠN
            $element.attr('style',
                'border: 3px solid #dc3545 !important; ' +
                'border-color: #dc3545 !important; ' +
                'box-shadow: 0 0 0 0.4rem rgba(220, 53, 69, 0.6) !important; ' +
                'background-color: rgba(220, 53, 69, 0.1) !important;'
            );

            // Add error message
            const $parent = $element.closest('.mb-3, .form-group, .col');
            let $errorDiv = $parent.find('.invalid-feedback');

            if ($errorDiv.length > 0) {
                $errorDiv.text(validation.message);
            } else {
                $errorDiv = $(`<div class="invalid-feedback d-block text-danger fw-bold">${validation.message}</div>`);
                $parent.append($errorDiv);
            }

            // Scroll to first error (if this is the first error)
            if (result.errors.length === 1) {
                // Scroll to element
                setTimeout(() => {
                    $element[0].scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                }, 100);

                // Focus on error element với delay để đảm bảo scroll xong
                setTimeout(() => {
                    $element.focus();
                    $element.select(); // Select text nếu có

                    // Trigger focus event để đảm bảo styling
                    $element.trigger('focus');
                }, 500);
            }
        } else {
            // Remove error state - XÓA VIỀN ĐỎ
            $element.removeClass('is-invalid border-danger');
            $element.addClass('is-valid');

            // Remove red border style - XÓA VIỀN ĐỎ
            $element.css({
                'border': '',
                'border-color': '',
                'box-shadow': '',
                'background-color': ''
            });

            // Remove error message
            $element.closest('.mb-3, .form-group, .col').find('.invalid-feedback').remove();
        }

        // Add to data object
        if (result.data[name] !== undefined) {
            // Handle multiple values (arrays)
            if (!Array.isArray(result.data[name])) {
                result.data[name] = [result.data[name]];
            }
            result.data[name].push(value);
            console.log('Added to existing array:', name, result.data[name]);
        } else {
            result.data[name] = value;
            console.log('Added new property:', name, value);
        }
    });

    console.log('Final result:', result);
    return result;
}

/**
 * Clear all validation errors in container
 * @param {jQuery} $container - Container to clear errors
 */
function clearValidationErrors($container) {
    const $elements = $container.find('input, select, textarea');

    $elements.each((_, element) => {
        const $element = $(element);

        // Remove error classes and styles
        $element.removeClass('is-invalid border-danger validation-error');
        $element.removeAttr('style'); // Xóa toàn bộ inline style

        // Reset về style mặc định
        $element.css({
            'border': '',
            'border-color': '',
            'box-shadow': '',
            'background-color': ''
        });

        // Remove error messages
        $element.closest('.mb-3, .form-group, .col').find('.invalid-feedback').remove();
    });
}

/**
 * Get value from element based on its type
 * @param {jQuery} $element - Element to get value from
 * @returns {*} - Element value
 */
function getElementValue($element) {
    // Check if this is a data-type element (div that should be converted to input)
    const dataType = $element.attr('data-type');
    if (dataType) {
        // For data-type elements, try to find the actual input inside or use data-value
        const $input = $element.find('input, select, textarea').first();
        if ($input.length > 0) {
            return getActualElementValue($input);
        }

        // Fallback to data-value
        const dataValue = $element.attr('data-value');
        return dataValue || '';
    }

    // Get value from actual form elements
    return getActualElementValue($element);
}

function getActualElementValue($element) {
    const tagName = $element[0].tagName.toLowerCase();
    const type = $element.attr('type');

    switch (tagName) {
        case 'input':
            if (type === 'checkbox' || type === 'radio') {
                return $element.is(':checked') ? $element.val() : null;
            }
            return $element.val();

        case 'select':
            return $element.val();

        case 'textarea':
            return $element.val();

        default:
            // Fallback to data-value if exists
            const dataValue = $element.attr('data-value');
            return dataValue || $element.text() || '';
    }
}

/**
 * Validate element value
 * @param {jQuery} $element - Element to validate
 * @param {*} value - Value to validate
 * @param {boolean} isRequired - Is field required
 * @param {string} dataType - Data type for validation
 * @returns {Object} - {isValid: boolean, message: string}
 */
function validateElement($element, value, isRequired, dataType) {
    const result = { isValid: true, message: '' };

    // Get field name from various sources
    let fieldName = $element.attr('data-label') ||
                   $element.attr('placeholder') ||
                   $element.closest('.mb-3, .form-group').find('label').text() ||
                   $element.attr('name') ||
                   'Trường này';

    // Clean up field name
    fieldName = fieldName.replace('*', '').trim();

    // Required validation
    if (isRequired && (value === null || value === undefined || value === '')) {
        result.isValid = false;
        result.message = `${fieldName} là bắt buộc`;
        return result;
    }

    // Skip other validations if empty and not required
    if (!value) {
        return result;
    }

    // Type-specific validation
    switch (dataType) {
        case 'email':
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(value)) {
                result.isValid = false;
                result.message = `${fieldName} không đúng định dạng email`;
            }
            break;

        case 'number':
            if (isNaN(value)) {
                result.isValid = false;
                result.message = `${fieldName} phải là số`;
            }
            break;

        case 'phone':
            const phoneRegex = /^[0-9+\-\s()]+$/;
            if (!phoneRegex.test(value)) {
                result.isValid = false;
                result.message = `${fieldName} không đúng định dạng số điện thoại`;
            }
            break;

        case 'url':
            try {
                new URL(value);
            } catch {
                result.isValid = false;
                result.message = `${fieldName} không đúng định dạng URL`;
            }
            break;
    }

    // Length validation
    const minLength = $element.attr('data-min-length');
    const maxLength = $element.attr('data-max-length');

    if (minLength && value.length < parseInt(minLength)) {
        result.isValid = false;
        result.message = `${fieldName} phải có ít nhất ${minLength} ký tự`;
    }

    if (maxLength && value.length > parseInt(maxLength)) {
        result.isValid = false;
        result.message = `${fieldName} không được vượt quá ${maxLength} ký tự`;
    }

    return result;
}

// Export for manual initialization if needed
window.DataGrid = DataGrid;
