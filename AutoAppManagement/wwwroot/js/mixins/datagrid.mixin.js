/**
 * DataGrid Mixin - Reusable DataGrid functionality
 * Usage: const grid = new DataGridMixin('#myTable', options);
 */
console.log('📦 Loading DataGrid Mixin...');
class DataGridMixin {
    // Data Type Constants (matching C# enum int values)
    static DATA_TYPES = {
        TEXT: 0,
        NUMBER: 1,
        CHECKBOX: 2,
        RADIO: 3,
        DATETIME: 4,
        DATE: 5,
        TIME: 6,
        CURRENCY: 7,
        BADGE: 8,
        USER: 9,
        AVATAR: 10,
        IMAGE: 11,
        LINK: 12,
        EMAIL: 13,
        PHONE: 14,
        BOOLEAN: 15,
        ACTIONS: 16
    };
    constructor(selector, options = {}) {
        console.log('🔧 DataGrid constructor called with:', selector, options);
        this.container = $(selector);
        console.log('📋 Container found:', this.container.length, 'elements');
        this.tableId = this.container.attr('data-table-id') || options.tableId || 'dataTable';
        console.log('🆔 Table ID:', this.tableId);

        // Get API URL from data attribute or options
        this.apiUrl = this.container.attr('data-get-url') ||
                      this.container.attr('data-api-url') ||
                      options.apiUrl || '/api/data';

        // Get other settings from data attributes
        this.currentPage = 1;
        this.pageSize = parseInt(this.container.attr('data-page-size')) || options.pageSize || 10;
        this.isLoading = false;
        this.filters = {};
        this.selectedRows = new Set();

        // Type renderers registry
        this.typeRenderers = new Map();

        // Default options
        this.options = {
            getUrl: this.container.attr('data-get-url') || options.getUrl,
            baseUrl: this.container.attr('data-base-url') || options.baseUrl || '',
            exportUrl: this.container.attr('data-export-url') || options.exportUrl,
            pageSize: parseInt(this.container.attr('data-page-size')) || options.pageSize || 10,
            showCheckbox: this.container.attr('data-show-checkbox') !== 'false',
            showActions: this.container.attr('data-show-actions') !== 'false',
            showPagination: this.container.attr('data-has-paging') !== 'false',
            showExport: this.container.attr('data-show-export') !== 'false',
            showRefresh: this.container.attr('data-show-refresh') !== 'false',
            autoLoad: this.container.attr('data-auto-load') !== 'false',
            entity: this.container.attr('data-entity') || options.entity || 'item',
            ...options
        };

        this.registerDefaultTypes();
        this.generateHeader();
        this.generateTable();
        this.init();
    }

    init() {
        this.bindEvents();
        if (this.options.autoLoad) {
            this.loadData(1);
        }
    }

    // Generate header from data attributes
    generateHeader() {
        const title = this.container.attr('data-title');
        const titleIcon = this.container.attr('data-title-icon');
        const hasExport = this.container.attr('data-has-export') === 'true';
        const hasRefresh = this.container.attr('data-has-refresh') === 'true';
        const hasAdd = this.container.attr('data-has-add') === 'true';
        const titleAdd = this.container.attr('data-title-add') || 'Thêm';
        const addUrl = this.container.attr('data-add-url') || '#';

        console.log('🏗️ generateHeader called with:', {
            title, titleIcon, hasExport, hasRefresh, hasAdd, titleAdd, addUrl
        });

        // Skip if no title or header already exists
        if (!title || this.container.find('[data-section="header"]').length > 0) {
            console.log('⏭️ Skipping header generation:', {
                noTitle: !title,
                headerExists: this.container.find('[data-section="header"]').length > 0
            });
            return;
        }

        let headerHtml = `
            <div class="card-header d-flex justify-content-between align-items-center" data-section="header">
                <h5 class="card-title mb-0" data-element="title">`;

        if (titleIcon) {
            headerHtml += `<i class="${titleIcon} me-2" data-element="title-icon"></i>`;
        }

        headerHtml += `${title}</h5>`;

        if (hasAdd || hasExport || hasRefresh) {
            headerHtml += `<div class="d-flex gap-2" data-section="toolbar">`;

            if (hasAdd) {
                console.log('➕ Adding add button with title:', titleAdd);
                headerHtml += `
                    <button class="btn btn-primary btn-sm" data-action="add" data-target="create" data-url="${addUrl}">
                        <i class="bi bi-plus-lg me-1"></i>${titleAdd}
                    </button>`;
            }

            if (hasExport) {
                headerHtml += `
                    <button class="btn btn-outline-success btn-sm" data-action="export" data-target="excel">
                        <i class="bi bi-file-earmark-excel me-1"></i></button>`;
            }

            if (hasRefresh) {
                headerHtml += `
                    <button class="btn btn-outline-primary btn-sm" data-action="refresh" data-target="table">
                        <i class="bi bi-arrow-clockwise me-1"></i></button>`;
            }

            headerHtml += `</div>`;
        }

        headerHtml += `</div>`;

        // Prepend header to card
        this.container.prepend(headerHtml);

        // Bind header events after DOM update
        setTimeout(() => {
            this.bindHeaderEvents();
        }, 100);
    }

    // Bind header button events
    bindHeaderEvents() {
        console.log('🔗 Binding header events...');

        // Find and bind add button
        const addButton = this.container.find('[data-action="add"]');
        if (addButton.length > 0) {
            console.log('✅ Found add button, binding click event');

            // Remove any existing handlers to prevent duplicates
            addButton.off('click.datagrid');

            // Bind new handler
            addButton.on('click.datagrid', (e) => {
                console.log('🔘 Add button clicked via header event');
                e.preventDefault();
                e.stopPropagation();

                this.handleAction('add', 'create', null, e.target);
            });
        } else {
            console.warn('❌ Add button not found in header');
        }

        // Bind other header buttons (export, refresh)
        this.container.find('[data-action="export"]').off('click.datagrid').on('click.datagrid', (e) => {
            e.preventDefault();
            this.handleAction('export', $(e.target).attr('data-target'), null, e.target);
        });

        this.container.find('[data-action="refresh"]').off('click.datagrid').on('click.datagrid', (e) => {
            e.preventDefault();
            this.handleAction('refresh', $(e.target).attr('data-target'), null, e.target);
        });

        console.log('✅ Header events bound successfully');
    }

    // Generate table structure if not exists
    generateTable() {
        console.log('🏗️ generateTable called');
        console.log('📋 Container:', this.container);

        // Check if table already exists
        const existingTable = this.container.find('table');
        console.log('🔍 Existing tables found:', existingTable.length);

        if (existingTable.length > 0) {
            this.table = existingTable;
            this.tableId = existingTable.attr('id') || this.tableId;
            console.log('✅ Using existing table:', this.tableId);
            return;
        }

        // Generate table structure
        const tableHtml = `
            <div class="card-body p-0" data-section="table-container">
                <div class="table-responsive" data-element="table-wrapper">
                    <table class="table table-hover mb-0" id="${this.tableId}">
                        <thead class="table-light" data-section="table-header">
                            <!-- Headers will be auto-generated from data -->
                        </thead>
                        <tbody data-section="table-body">
                            <!-- Data will be loaded here -->
                        </tbody>
                    </table>
                </div>
            </div>
        `;

        // Append table to card
        console.log('📝 Appending table HTML...');
        this.container.append(tableHtml);
        this.table = this.container.find('table');
        console.log('✅ Table created:', this.table.length);
        console.log('📊 tbody check:', this.container.find('[data-section="table-body"]').length);
    }

    // Register default data types
    registerDefaultTypes() {
        // Basic types
        this.registerType('text', (item, column, cellAttrs, value) => {
            return `<td ${cellAttrs}>${value || ''}</td>`;
        });

        this.registerType('number', (item, column, cellAttrs, value) => {
            const formatted = this.formatNumber(value);
            return `<td ${cellAttrs} class="text-end">${formatted}</td>`;
        });

        this.registerType('checkbox', (item, column, cellAttrs) => {
            const itemId = item.id || item.Id || '';
            return `
                <td ${cellAttrs}>
                    <input type="checkbox" class="form-check-input row-checkbox" value="${itemId}"
                           data-action="select-row" data-target="${itemId}" data-entity="${this.options.entity || 'item'}">
                </td>
            `;
        });

        this.registerType('radio', (item, column, cellAttrs, value) => {
            return `
                <td ${cellAttrs}>
                    <input type="radio" class="form-check-input" name="${column.field}" value="${value}"
                           data-action="select-radio" data-target="${item.id}">
                </td>
            `;
        });

        this.registerType('badge', (item, column, cellAttrs, value) => {
            const badgeClass = this.getBadgeClass(column.field, value);
            return `<td ${cellAttrs}><span class="badge ${badgeClass}">${value}</span></td>`;
        });

        this.registerType('currency', (item, column, cellAttrs, value) => {
            const formatted = this.formatCurrency(value);
            return `<td ${cellAttrs} class="text-end">${formatted}</td>`;
        });

        this.registerType('date', (item, column, cellAttrs, value) => {
            const formatted = this.formatDate(value);
            return `<td ${cellAttrs}>${formatted}</td>`;
        });

        this.registerType('time', (item, column, cellAttrs, value) => {
            const formatted = this.formatTime(value);
            return `<td ${cellAttrs}>${formatted}</td>`;
        });

        this.registerType('datetime', (item, column, cellAttrs, value) => {
            const formatted = this.formatDateTime(value);
            return `<td ${cellAttrs}>${formatted}</td>`;
        });

        this.registerType('email', (item, column, cellAttrs, value) => {
            if (!value) return `<td ${cellAttrs}>-</td>`;
            return `<td ${cellAttrs}><a href="mailto:${value}" class="text-decoration-none">${value}</a></td>`;
        });

        this.registerType('phone', (item, column, cellAttrs, value) => {
            if (!value) return `<td ${cellAttrs}>-</td>`;
            return `<td ${cellAttrs}><a href="tel:${value}" class="text-decoration-none">${value}</a></td>`;
        });

        this.registerType('link', (item, column, cellAttrs, value) => {
            if (!value) return `<td ${cellAttrs}>-</td>`;
            return `<td ${cellAttrs}><a href="${value}" target="_blank" class="text-decoration-none">${value}</a></td>`;
        });

        this.registerType('boolean', (item, column, cellAttrs, value) => {
            const isTrue = value === true || value === 'true' || value === 1 || value === '1';
            const icon = isTrue ? 'bi-check-circle text-success' : 'bi-x-circle text-muted';
            const text = isTrue ? 'Yes' : 'No';
            return `<td ${cellAttrs}><i class="bi ${icon}"></i> ${text}</td>`;
        });

        this.registerType('image', (item, column, cellAttrs, value) => {
            if (!value) return `<td ${cellAttrs}>-</td>`;
            return `
                <td ${cellAttrs}>
                    <img src="${value}" class="rounded" width="40" height="40"
                         style="object-fit: cover;" onerror="this.src='/images/no-image.png'">
                </td>
            `;
        });

        this.registerType('user', (item, column, cellAttrs) => {
            const avatarClass = this.getAvatarClass(item.role || item.type);
            const avatarIcon = this.getAvatarIcon(item.role || item.type);
            const displayName = item.fullName || item.name || item.title || 'Unknown';
            const subText = item.username || item.email || item.code || '';
            const onlineStatus = item.onlineStatus ? this.getOnlineStatusBadge(item.onlineStatus) : '';

            return `
                <td ${cellAttrs}>
                    <div class="d-flex align-items-center" data-layout="user-display">
                        <div class="avatar-circle ${avatarClass} text-white me-3"
                             data-element="avatar" data-role="${(item.role || '').toLowerCase()}">
                            <i class="${avatarIcon}" data-element="avatar-icon"></i>
                        </div>
                        <div data-element="user-details">
                            <div class="fw-bold" data-field="displayName">${displayName}</div>
                            ${subText ? `<small class="text-muted" data-field="subText">${subText}</small>` : ''}
                            <div class="d-flex align-items-center mt-1" data-layout="user-meta">
                                ${onlineStatus}
                                <small class="text-muted ms-2" data-field="id">ID: ${item.id}</small>
                            </div>
                        </div>
                    </div>
                </td>
            `;
        });

        this.registerType('avatar', (item, column, cellAttrs) => {
            const avatarClass = this.getAvatarClass(item.role || item.type);
            const avatarIcon = this.getAvatarIcon(item.role || item.type);
            const displayText = item.fullName || item.name || item.title || 'Unknown';

            return `
                <td ${cellAttrs}>
                    <div class="d-flex align-items-center">
                        <div class="avatar-circle ${avatarClass} text-white me-2" data-element="avatar">
                            <i class="${avatarIcon}"></i>
                        </div>
                        <span>${displayText}</span>
                    </div>
                </td>
            `;
        });

        this.registerType('actions', (item, column, cellAttrs) => {
            const actions = this.getActionButtons();
            const itemId = item.id || item.Id || '';

            let html = `
                <td ${cellAttrs}>
                    <div class="dropdown">
                        <button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button"
                                data-bs-toggle="dropdown" aria-expanded="false" title="Actions">
                            <i class="bi bi-three-dots"></i>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end">
            `;

            actions.forEach(action => {
                let dataAttrs = '';

                // Add data attributes for data-driven actions
                if (action.confirm) {
                    dataAttrs += ` data-confirm="true"`;
                    dataAttrs += ` data-confirm-message="${action.confirmMessage || 'Bạn có chắc chắn?'}"`;
                }
                if (action.apiUrl) {
                    const apiUrl = action.apiUrl.replace('{id}', itemId);
                    dataAttrs += ` data-api-url="${apiUrl}"`;
                    dataAttrs += ` data-api-method="${action.method || 'POST'}"`;
                }
                if (action.successMessage) {
                    dataAttrs += ` data-success-message="${action.successMessage}"`;
                }
                if (action.errorMessage) {
                    dataAttrs += ` data-error-message="${action.errorMessage}"`;
                }
                if (action.refreshGrid) {
                    dataAttrs += ` data-refresh-grid="true"`;
                }
                if (action.redirectUrl) {
                    const redirectUrl = action.redirectUrl.replace('{id}', itemId);
                    dataAttrs += ` data-redirect-url="${redirectUrl}"`;
                }

                // Determine icon color class based on action
                let iconClass = '';
                if (action.action === 'delete') {
                    iconClass = 'text-danger';
                } else if (action.action === 'edit') {
                    iconClass = 'text-warning';
                } else if (action.action === 'view') {
                    iconClass = 'text-primary';
                } else if (action.action === 'toggle-status') {
                    iconClass = 'text-secondary';
                }

                html += `
                    <li>
                        <a class="dropdown-item" href="#"
                           data-action="${action.action}" data-target="${itemId}"
                           data-entity="${this.options.entity || 'item'}"${dataAttrs}>
                            <i class="${action.icon} ${iconClass} me-2"></i>
                            ${action.title}
                        </a>
                    </li>
                `;
            });

            html += `
                        </ul>
                    </div>
                </td>
            `;
            return html;
        });
    }

    // Register a custom data type
    registerType(typeName, renderer) {
        this.typeRenderers.set(typeName, renderer);
    }

    // Check if type is registered
    hasType(typeName) {
        return this.typeRenderers.has(typeName);
    }

    // Get type renderer
    getTypeRenderer(typeName) {
        return this.typeRenderers.get(typeName);
    }
    
    // Event binding
    bindEvents() {
        const self = this;
        
        // Select all checkbox
        this.container.on('change', '[data-action="select-all"]', function() {
            console.log('Select all checkbox changed');
            const isChecked = $(this).prop('checked');
            self.selectAll(isChecked);
        });

        // Row checkbox
        this.container.on('change', '[data-action="select-row"]', function() {
            console.log('Row checkbox changed');
            const id = $(this).attr('data-target');
            const isChecked = $(this).prop('checked');
            self.selectRow(id, isChecked);
        });

        // Debug: Also listen for click events on checkboxes
        this.container.on('click', 'input[type="checkbox"]', function(e) {
            console.log('Checkbox clicked:', $(this).attr('data-action'), 'checked:', $(this).prop('checked'));
        });
        
        // Action buttons (both button and dropdown item) - exclude checkboxes
        this.container.on('click', '[data-action]:not([type="checkbox"])', function(e) {
            e.preventDefault();
            const action = $(this).attr('data-action');
            const target = $(this).attr('data-target');
            const entity = $(this).attr('data-entity');

            console.log('🔘 Action button clicked:', {
                action: action,
                target: target,
                entity: entity,
                element: $(this)[0]
            });

            // Close dropdown if it's a dropdown item
            if ($(this).hasClass('dropdown-item')) {
                console.log('📋 Closing dropdown for dropdown-item');
                const dropdown = $(this).closest('.dropdown').find('.dropdown-toggle');
                if (dropdown.length && typeof bootstrap !== 'undefined') {
                    const bsDropdown = bootstrap.Dropdown.getInstance(dropdown[0]);
                    if (bsDropdown) {
                        bsDropdown.hide();
                    }
                }
            }

            self.handleAction(action, target, entity, $(this));
        });

        // Also handle three dots button clicks specifically
        this.container.on('click', '.btn:has(.bi-three-dots)', function(e) {
            console.log('🔘 Three dots button clicked');
            e.stopPropagation(); // Don't prevent default to allow dropdown to work

            // Ensure Bootstrap dropdown is initialized and toggle it
            if (typeof bootstrap !== 'undefined') {
                const dropdownElement = $(this)[0];
                let dropdown = bootstrap.Dropdown.getInstance(dropdownElement);

                if (!dropdown) {
                    dropdown = new bootstrap.Dropdown(dropdownElement);
                    console.log('✅ Bootstrap dropdown initialized');
                }

                // Force toggle dropdown
                dropdown.toggle();
                console.log('✅ Dropdown toggled');
            } else {
                console.warn('❌ Bootstrap not available for dropdown');
            }
        });
        
        // Pagination
        this.container.on('click', '[data-action="goto-page"]', function(e) {
            e.preventDefault();
            const page = parseInt($(this).attr('data-page'));
            if (page && page !== self.currentPage) {
                self.loadData(page);
            }
        });
    }
    
    // Load data from API
    async loadData(page = 1, additionalParams = {}) {
        debugger
        if (this.isLoading) return;
        
        this.isLoading = true;
        this.showLoading();
        
        const params = {
            page: page,
            pageSize: this.pageSize,
            ...this.filters,
            ...additionalParams
        };
        
        try {
            const response = await $.ajax({
                url: this.apiUrl,
                type: 'GET',
                data: params
            });

            console.log('🔄 API Response:', response);

            if (response.isSuccess) {
                // Extract data from nested structure: response.data.data
                const tableData = response.data?.data || response.data || [];
                const pagination = {
                    total: response.data?.total || tableData.length,
                    page: response.data?.page || page,
                    pageSize: response.data?.pageSize || this.pageSize
                };

                console.log('📊 Table Data:', tableData);
                console.log('📄 Pagination:', pagination);

                this.renderTable(tableData);
                this.renderPagination(pagination);
                this.updateStats({ total: pagination.total });
                this.currentPage = page;
                this.onDataLoaded(response);
            } else {
                this.showError(response.message || 'Có lỗi xảy ra khi tải dữ liệu');
            }
        } catch (error) {
            this.showError('Không thể kết nối đến server');
        } finally {
            this.isLoading = false;
            this.hideLoading();
        }
    }
    
    // Render table data
    renderTable(data) {
        let tbody = this.container.find('[data-section="table-body"]');

        // Auto-create tbody if not exists
        if (tbody.length === 0) {
            tbody = this.createTableBody();
            this.container.find('table').append(tbody);
        }

        let html = '';

        if (data.length === 0) {
            html = this.renderEmptyState();
        } else {
            data.forEach(item => {
                html += this.renderTableRow(item);
            });
        }

        tbody.html(html);
        this.applyColumnStyles();
        this.onTableRendered(data);
    }

    // Create table body if not exists
    createTableBody() {
        return $('<tbody data-section="table-body" data-loading="false"></tbody>');
    }

    // Apply column styles (width, flex, etc.)
    applyColumnStyles() {
        const columns = this.getColumnDefinitions();
        const table = this.container.find('table');

        // Apply styles to header
        columns.forEach((column, index) => {
            const headerTh = this.container.find(`[data-section="table-header"] th:eq(${index})`);
            const bodyTds = this.container.find(`[data-section="table-body"] td:nth-child(${index + 1})`);

            let styles = {};

            // Width
            if (column.width) {
                styles.width = column.width;
                if (!column.flex || column.flex === 'none') {
                    styles.flex = 'none';
                }
            }

            // Flex
            if (column.flex && column.flex !== 'none') {
                styles.flex = column.flex;
            }

            // Min/Max width
            if (column.minWidth) styles.minWidth = column.minWidth;
            if (column.maxWidth) styles.maxWidth = column.maxWidth;

            // Apply styles
            if (Object.keys(styles).length > 0) {
                headerTh.css(styles);
                bodyTds.css(styles);
            }
        });

        // Determine table layout
        const hasFlexColumns = columns.some(col => col.flex && col.flex !== 'none');
        const hasFixedColumns = columns.some(col => col.width);
        const autoColumns = columns.filter(col => !col.width && (!col.flex || col.flex === 'none'));

        if (hasFlexColumns) {
            // Use auto layout for flex columns
            table.css({
                'table-layout': 'auto',
                'width': '100%'
            });
        } else if (autoColumns.length > 0 && hasFixedColumns) {
            // Mixed: some fixed, some auto = auto columns get equal distribution
            table.css({
                'table-layout': 'auto',
                'width': '100%'
            });
        } else if (autoColumns.length === columns.length) {
            // All auto = equal distribution
            table.css({
                'table-layout': 'fixed',
                'width': '100%'
            });

            // Set equal width for all columns
            const equalWidth = `${100 / columns.length}%`;
            columns.forEach((column, index) => {
                const headerTh = this.container.find(`[data-section="table-header"] th:eq(${index})`);
                const bodyTds = this.container.find(`[data-section="table-body"] td:nth-child(${index + 1})`);
                headerTh.css('width', equalWidth);
                bodyTds.css('width', equalWidth);
            });
        }
    }

    // Get column count from table header
    getColumnCount() {
        return this.container.find('[data-section="table-header"] th').length;
    }

    // Export functionality
    export() {
        const exportUrl = this.options.exportUrl || `${this.options.baseUrl}/ExportExcel`;
        const params = new URLSearchParams(this.filters || {});
        window.location.href = `${exportUrl}?${params.toString()}`;
    }
    
    // Render single table row based on column definitions
    renderTableRow(item) {
        const columns = this.getColumnDefinitions();
        let html = `<tr data-id="${item.id}" data-row="data" data-entity="${this.options.entity || 'item'}"`;

        // Add data attributes for filtering/styling
        Object.keys(item).forEach(key => {
            if (typeof item[key] === 'string' || typeof item[key] === 'number') {
                html += ` data-${key.toLowerCase()}="${String(item[key]).toLowerCase()}"`;
            }
        });
        html += '>';

        columns.forEach(column => {
            html += this.renderTableCell(item, column);
        });

        html += '</tr>';
        return html;
    }

    // Get column definitions from table header
    getColumnDefinitions() {
        console.log('🔍 Getting column definitions...');
        const columns = [];
        const headerThs = this.container.find('[data-section="table-header"] th');
        console.log('📊 Found header th elements:', headerThs.length);

        headerThs.each((index, th) => {
            const $th = $(th);
            const field = $th.attr('data-field');
            const type = $th.attr('data-type') || 'text';

            columns.push({
                column: field || `col_${index}`, // Use field or fallback to index
                field: field,
                type: type.toLowerCase(),
                width: $th.attr('width') || $th.attr('data-width'),
                flex: $th.attr('data-flex'),
                minWidth: $th.attr('data-min-width'),
                maxWidth: $th.attr('data-max-width'),
                sortable: $th.attr('data-sortable') !== 'false' // Default true unless explicitly false
            });
        });
        console.log('✅ Column definitions:', columns);
        return columns;
    }

    // Render single table cell based on column type
    renderTableCell(item, column) {
        const value = column.field ? item[column.field] : '';
        const cellAttrs = `data-cell="${column.column}" data-column="${column.column}" data-value="${value}" data-type="${column.type}"`;

        // Check if custom type renderer exists
        if (this.hasType(column.type)) {
            const renderer = this.getTypeRenderer(column.type);
            return renderer.call(this, item, column, cellAttrs, value);
        }

        // Fallback to text type
        return this.getTypeRenderer('text').call(this, item, column, cellAttrs, value);
    }

    // Static method to register global types (available to all instances)
    static registerGlobalType(typeName, renderer) {
        if (!DataGridMixin.globalTypes) {
            DataGridMixin.globalTypes = new Map();
        }
        DataGridMixin.globalTypes.set(typeName, renderer);
    }

    // Static method to get global type
    static getGlobalType(typeName) {
        return DataGridMixin.globalTypes?.get(typeName);
    }

    // Check global types in addition to instance types
    hasType(typeName) {
        return this.typeRenderers.has(typeName) || DataGridMixin.globalTypes?.has(typeName);
    }

    // Get type renderer (check instance first, then global)
    getTypeRenderer(typeName) {
        return this.typeRenderers.get(typeName) || DataGridMixin.globalTypes?.get(typeName);
    }
    
    // Render empty state
    renderEmptyState() {
        return `
            <tr data-state="empty">
                <td colspan="100%" class="text-center py-4 text-muted" data-cell="empty">
                    <i class="bi bi-inbox fs-1" data-icon="empty"></i>
                    <div class="mt-2" data-text="empty-message">Không có dữ liệu</div>
                </td>
            </tr>
        `;
    }
    
    // Render pagination
    renderPagination(pagination) {
        if (!this.options.showPagination) return;

        // Handle different pagination structures
        const currentPage = pagination.page || pagination.currentPage || this.currentPage || 1;
        const pageSize = pagination.pageSize || this.pageSize || 10;
        const totalRecords = pagination.total || pagination.totalRecords || 0;
        const totalPages = Math.ceil(totalRecords / pageSize);

        console.log('📄 Pagination Info:', { currentPage, pageSize, totalRecords, totalPages });

        // Find or create pagination container
        let paginationContainer = this.container.find('[data-section="pagination"]');
        if (paginationContainer.length === 0) {
            // Auto-create pagination if not exists
            paginationContainer = this.createPaginationContainer();
            this.container.append(paginationContainer);
        }

        // Update pagination info
        const start = totalRecords > 0 ? (currentPage - 1) * pageSize + 1 : 0;
        const end = Math.min(currentPage * pageSize, totalRecords);

        const infoElement = paginationContainer.find('[data-element="pagination-info"]');
        if (infoElement.length) {
            infoElement
                .text(`Hiển thị ${start}-${end} trong tổng số ${totalRecords} bản ghi`)
                .attr('data-start', start)
                .attr('data-end', end)
                .attr('data-total', totalRecords);
        }

        // Generate pagination buttons
        const paginationList = paginationContainer.find('[data-element="pagination-list"]');
        if (paginationList.length) {
            let paginationHtml = '';

            // Previous button
            const prevDisabled = currentPage <= 1 ? 'disabled' : '';
            paginationHtml += `
                <li class="page-item ${prevDisabled}" data-page="prev" data-target="${currentPage - 1}">
                    <a class="page-link" href="#" data-action="goto-page" data-page="${currentPage - 1}">Trước</a>
                </li>
            `;

            // Page numbers
            const startPage = Math.max(1, currentPage - 2);
            const endPage = Math.min(totalPages, currentPage + 2);

            for (let i = startPage; i <= endPage; i++) {
                const activeClass = i === currentPage ? 'active' : '';
                paginationHtml += `
                    <li class="page-item ${activeClass}" data-page="${i}">
                        <a class="page-link" href="#" data-action="goto-page" data-page="${i}">${i}</a>
                    </li>
                `;
            }

            // Next button
            const nextDisabled = currentPage >= totalPages ? 'disabled' : '';
            paginationHtml += `
                <li class="page-item ${nextDisabled}" data-page="next" data-target="${currentPage + 1}">
                    <a class="page-link" href="#" data-action="goto-page" data-page="${currentPage + 1}">Sau</a>
                </li>
            `;

            paginationList.html(paginationHtml);
        }
    }

    // Create pagination container if not exists
    createPaginationContainer() {
        return $(`
            <div class="card-footer" data-section="pagination" data-component="pagination">
                <div class="d-flex justify-content-between align-items-center" data-layout="pagination-wrapper">
                    <div class="text-muted" data-element="pagination-info" data-format="records-info">
                        Đang tải...
                    </div>
                    <nav aria-label="Phân trang" data-element="pagination-nav">
                        <ul class="pagination pagination-sm mb-0" data-element="pagination-list" data-size="sm">
                            <!-- Pagination will be generated by JavaScript -->
                        </ul>
                    </nav>
                </div>
            </div>
        `);
    }
    
    // Update statistics
    updateStats(stats) {
        if (!stats) return;
        
        Object.keys(stats).forEach(key => {
            const element = this.container.find(`[data-stat="${key}"]`);
            if (element.length) {
                element.text(stats[key]);
            }
        });
    }
    
    // Show loading state
    showLoading() {
        console.log('🔄 showLoading called');
        console.log('📋 Container:', this.container);
        console.log('🔍 Looking for tbody...');

        let tbody = this.container.find('[data-section="table-body"]');
        console.log('📊 Found tbody elements:', tbody.length);

        // Auto-create tbody if not exists
        if (tbody.length === 0) {
            console.log('⚠️ No tbody found, creating...');
            tbody = this.createTableBody();
            this.container.find('table').append(tbody);
            console.log('✅ Created tbody:', tbody.length);
        }

        // Always create fresh loading state
        const columnCount = this.getColumnCount();
        const loadingHtml = this.createLoadingRow(columnCount);
        tbody.html(loadingHtml);

        this.container.attr('data-loading', 'true');
    }

    // Create loading row HTML
    createLoadingRow(columnCount) {
        return `
            <tr data-state="loading" data-colspan="${columnCount}">
                <td colspan="${columnCount}" class="text-center py-4" data-cell="loading">
                    <div class="spinner-border text-primary" role="status" data-element="spinner">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <div class="mt-2" data-element="loading-text">Đang tải dữ liệu...</div>
                </td>
            </tr>
        `;
    }
    
    // Hide loading state
    hideLoading() {
        const loadingRow = this.container.find('[data-state="loading"]');
        if (loadingRow.length) {
            loadingRow.hide();
        }
        
        this.container.attr('data-loading', 'false');
    }
    
    // Show error message
    showError(message) {
        const tbody = this.container.find('[data-section="table-body"]');
        const errorHtml = `
            <tr data-state="error">
                <td colspan="100%" class="text-center py-4 text-danger" data-cell="error">
                    <i class="bi bi-exclamation-triangle fs-1" data-icon="error"></i>
                    <div class="mt-2" data-text="error-message">${message}</div>
                    <button class="btn btn-outline-primary btn-sm mt-2" data-action="retry" data-target="reload">
                        <i class="bi bi-arrow-clockwise me-1"></i>Thử lại
                    </button>
                </td>
            </tr>
        `;
        tbody.html(errorHtml);
    }
    
    // Handle actions
    async handleAction(action, target, entity, element) {
        const $element = $(element);

        // Check if it's a data-driven action
        if ($element.attr('data-confirm') === 'true') {
            await this.handleConfirmAction($element, target);
            return;
        }

        // Check if it's a simple API action
        if ($element.attr('data-api-url')) {
            await this.handleApiAction($element, target);
            return;
        }

        // Built-in actions
        switch (action) {
            case 'add':
                console.log('🔘 DataGrid add action triggered');
                const addAction = this.container.attr('data-add-action');
                const addTarget = this.container.attr('data-add-target');
                const addUrl = this.container.attr('data-add-url') || $(element).attr('data-url');

                console.log('🔧 Add action config:', {
                    addAction,
                    addTarget,
                    addUrl
                });

                if (addAction === 'modal' && addTarget) {
                    console.log('📋 Opening modal:', addTarget);
                    this.openModal(addTarget);
                } else if (addUrl && addUrl !== '#') {
                    console.log('🔗 Navigating to URL:', addUrl);
                    window.location.href = addUrl;
                } else {
                    console.log('🎯 Custom action');
                    this.onAction(action, target, entity, element);
                }
                break;
            case 'refresh':
                this.refresh();
                break;
            case 'export':
                this.export();
                break;
            case 'retry':
                this.loadData(this.currentPage);
                break;
            default:
                this.onAction(action, target, entity, element);
                break;
        }
    }

    // Handle confirm actions (data-confirm="true")
    async handleConfirmAction($element, target) {
        const confirmMessage = $element.attr('data-confirm-message') || 'Bạn có chắc chắn?';
        const confirmed = await this.showConfirm(confirmMessage);
        if (!confirmed) return;

        // After confirmation, handle as API action
        await this.handleApiAction($element, target);
    }

    // Handle API actions (data-api-url)
    async handleApiAction($element, target) {
        const apiUrl = $element.attr('data-api-url');
        const method = $element.attr('data-api-method') || 'POST';
        const successMessage = $element.attr('data-success-message') || 'Thành công!';
        const errorMessage = $element.attr('data-error-message') || 'Có lỗi xảy ra';
        const refreshGrid = $element.attr('data-refresh-grid') === 'true';
        const redirectUrl = $element.attr('data-redirect-url');

        if (!apiUrl) {
            console.error('data-api-url is required for API actions');
            return;
        }

        try {
            // Show loading state
            this.setButtonLoading($element, true);

            const response = await $.ajax({
                url: apiUrl.replace('{id}', target) + (apiUrl.includes('{id}') ? '' : '/' + target),
                type: method,
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                }
            });

            if (response.success) {
                // Show success message
                if (successMessage) {
                    await this.showAlert(successMessage, 'Thành công');
                }

                // Refresh grid if needed
                if (refreshGrid) {
                    this.refresh();
                }

                // Redirect if needed
                if (redirectUrl) {
                    window.location.href = redirectUrl.replace('{id}', target);
                }

                // Call success callback if defined
                this.onApiActionSuccess(response, $element, target);

            } else {
                await this.showAlert(errorMessage + ': ' + (response.message || 'Unknown error'), 'Lỗi');
                this.onApiActionError(response, $element, target);
            }
        } catch (error) {
            await this.showAlert('Không thể kết nối đến server', 'Lỗi');
            this.onApiActionError(error, $element, target);
        } finally {
            this.setButtonLoading($element, false);
        }
    }

    // Set button loading state
    setButtonLoading($button, loading) {
        if (loading) {
            $button.prop('disabled', true);
            const originalHtml = $button.html();
            $button.attr('data-original-html', originalHtml);
            $button.html('<span class="spinner-border spinner-border-sm me-1"></span>Loading...');
        } else {
            $button.prop('disabled', false);
            const originalHtml = $button.attr('data-original-html');
            if (originalHtml) {
                $button.html(originalHtml);
                $button.removeAttr('data-original-html');
            }
        }
    }

    // Show confirm dialog (can be overridden)
    async showConfirm(message) {
        if (window.ModalMixin && ModalMixin.confirm) {
            return await ModalMixin.confirm(message);
        }
        return confirm(message);
    }

    // Show alert dialog (can be overridden)
    async showAlert(message, title = 'Thông báo') {
        if (window.ModalMixin && ModalMixin.alert) {
            return ModalMixin.alert(message, title);
        }
        alert(message);
    }
    
    // Selection methods
    selectAll(isChecked) {
        this.container.find('[data-action="select-row"]').prop('checked', isChecked);
        
        if (isChecked) {
            this.container.find('[data-action="select-row"]').each((index, element) => {
                this.selectedRows.add($(element).attr('data-target'));
            });
        } else {
            this.selectedRows.clear();
        }
        
        this.onSelectionChanged();
    }
    
    selectRow(id, isChecked) {
        if (isChecked) {
            this.selectedRows.add(id);
        } else {
            this.selectedRows.delete(id);
        }
        
        // Update select all checkbox state
        const totalCheckboxes = this.container.find('[data-action="select-row"]').length;
        const checkedCheckboxes = this.selectedRows.size;
        const selectAllCheckbox = this.container.find('[data-action="select-all"]');
        
        if (checkedCheckboxes === 0) {
            selectAllCheckbox.prop('indeterminate', false).prop('checked', false);
        } else if (checkedCheckboxes === totalCheckboxes) {
            selectAllCheckbox.prop('indeterminate', false).prop('checked', true);
        } else {
            selectAllCheckbox.prop('indeterminate', true);
        }
        
        this.onSelectionChanged();
    }
    
    // Utility methods
    setFilter(field, value) {
        this.filters[field] = value;
    }
    
    clearFilters() {
        this.filters = {};
    }
    
    refresh() {
        this.loadData(this.currentPage);
    }
    
    export() {
        const params = new URLSearchParams(this.filters);
        window.location.href = `${this.apiUrl}/export?${params.toString()}`;
    }
    
    getSelectedIds() {
        return Array.from(this.selectedRows);
    }

    // Get action buttons configuration (override in implementation)
    getActionButtons() {
        // Priority 1: Override method in subclass (highest priority)
        // This allows clean override without data attributes

        // Priority 2: Constructor options
        if (this.options.actionButtons && this.options.actionButtons.length > 0) {
            return this.options.actionButtons;
        }

        // Priority 3: Data attribute (for fully data-driven approach)
        const dataActions = this.container.attr('data-actions');
        if (dataActions) {
            try {
                return JSON.parse(dataActions);
            } catch (e) {
                console.warn('Invalid data-actions JSON:', dataActions);
            }
        }

        // Priority 4: Default actions
        return [
            { action: 'view', title: 'Xem', icon: 'bi bi-eye', cssClass: 'btn-outline-primary' },
            { action: 'edit', title: 'Sửa', icon: 'bi bi-pencil', cssClass: 'btn-outline-warning' },
            { action: 'delete', title: 'Xóa', icon: 'bi bi-trash', cssClass: 'btn-outline-danger' }
        ];
    }

    // Helper methods for rendering (can be overridden)
    getAvatarClass(role) {
        const roleMap = {
            'super admin': 'bg-danger',
            'admin': 'bg-primary',
            'moderator': 'bg-warning',
            'support': 'bg-info',
            'user': 'bg-success',
            'editor': 'bg-secondary'
        };
        return roleMap[role?.toLowerCase()] || 'bg-secondary';
    }

    getAvatarIcon(role) {
        const iconMap = {
            'super admin': 'bi bi-shield-fill',
            'admin': 'bi bi-person-gear',
            'moderator': 'bi bi-person-check',
            'support': 'bi bi-headset',
            'user': 'bi bi-person-fill',
            'editor': 'bi bi-pencil-square'
        };
        return iconMap[role?.toLowerCase()] || 'bi bi-person-fill';
    }

    getBadgeClass(field, value) {
        const fieldMaps = {
            status: {
                'active': 'bg-success',
                'hoạt động': 'bg-success',
                'inactive': 'bg-secondary',
                'không hoạt động': 'bg-secondary',
                'pending': 'bg-warning',
                'chờ duyệt': 'bg-warning',
                'blocked': 'bg-danger',
                'bị khóa': 'bg-danger'
            },
            role: {
                'super admin': 'bg-danger',
                'admin': 'bg-primary',
                'moderator': 'bg-warning',
                'support': 'bg-info',
                'user': 'bg-success',
                'editor': 'bg-secondary'
            },
            priority: {
                'high': 'bg-danger',
                'cao': 'bg-danger',
                'medium': 'bg-warning',
                'trung bình': 'bg-warning',
                'low': 'bg-info',
                'thấp': 'bg-info'
            }
        };

        const fieldMap = fieldMaps[field?.toLowerCase()];
        if (fieldMap) {
            return fieldMap[value?.toLowerCase()] || 'bg-light text-dark';
        }

        return 'bg-light text-dark';
    }

    getOnlineStatusBadge(status) {
        const statusMap = {
            'online': 'bg-success',
            'away': 'bg-warning',
            'offline': 'bg-secondary',
            'busy': 'bg-danger'
        };
        const badgeClass = statusMap[status?.toLowerCase()] || 'bg-secondary';
        return `<span class="badge ${badgeClass} me-2">${status}</span>`;
    }

    formatNumber(value) {
        if (!value && value !== 0) return '0';
        return new Intl.NumberFormat('vi-VN').format(value);
    }

    formatCurrency(value) {
        if (!value && value !== 0) return '0 ₫';
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(value);
    }

    formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN');
    }

    formatTime(timeString) {
        if (!timeString) return '';
        const date = new Date(timeString);
        return date.toLocaleTimeString('vi-VN');
    }

    formatDateTime(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleString('vi-VN');
    }

    // Modal handling
    openModal(modalSelector) {
        console.log('📋 DataGrid opening modal:', modalSelector);

        try {
            const modalElement = document.querySelector(modalSelector);
            if (!modalElement) {
                console.error('❌ Modal element not found:', modalSelector);
                return;
            }

            // Try to use global modal instance first
            if (window.adminModal && modalSelector === '#adminModal') {
                console.log('✅ Using global AdminModal instance');
                window.adminModal.openModal('create');
                return;
            }

            // Fallback to Bootstrap modal
            console.log('✅ Using Bootstrap Modal fallback');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();

        } catch (error) {
            console.error('❌ Error opening modal:', error);
        }
    }

    // Event hooks (override in implementation)
    onDataLoaded(response) {
        // Override this method to handle data loaded event
    }

    onTableRendered(data) {
        // Override this method to handle table rendered event
    }

    onAction(action, target, entity, element) {
        // Override this method to handle custom actions
        console.log('Action:', action, 'Target:', target, 'Entity:', entity);
    }

    onSelectionChanged() {
        // Override this method to handle selection changes
        console.log('Selected rows:', this.getSelectedIds());
    }

    onApiActionSuccess(response, element, target) {
        // Override this method to handle API action success
        console.log('API action success:', response, target);
    }

    onApiActionError(error, element, target) {
        // Override this method to handle API action error
        console.log('API action error:', error, target);
    }
}

// Export for use
window.DataGridMixin = DataGridMixin;
