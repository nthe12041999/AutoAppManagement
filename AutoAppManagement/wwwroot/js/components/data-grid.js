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

        // Find all data-grid components
        const gridComponents = document.querySelectorAll('[data-component="data-grid"]');
        console.log('Found data-grid components:', gridComponents.length);

        gridComponents.forEach((component, index) => {
            console.log(`Processing data-grid component ${index + 1}:`, component);
            this.renderGrid(component);
        });
    }

    /**
     * Render grid component
     * @param {HTMLElement} component - Grid component element
     */
    renderGrid(component) {
        console.log('DataGrid.renderGrid() called for:', component);

        const config = this.parseConfig(component);
        console.log('Parsed config:', config);

        const html = this.generateGridHTML(config);
        console.log('Generated HTML length:', html.length);

        // Set grid container attribute
        component.setAttribute('data-grid-container', config.containerId);

        // Render HTML
        component.innerHTML = html;
        console.log('HTML rendered to component');

        // Initialize grid logic
        this.initializeGridLogic(config);

        // Store grid instance
        this.grids.set(config.containerId, config);

        console.log(`DataGrid rendered: ${config.containerId}`, config);
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

            // Columns configuration
            columns: this.parseColumns(component.getAttribute('data-columns')),

            // Actions
            hasAdd: component.getAttribute('data-has-add') !== 'false',
            hasRefresh: component.getAttribute('data-has-refresh') !== 'false',
            hasExport: component.getAttribute('data-has-export') !== 'false',
            hasSelectAll: component.getAttribute('data-has-select-all') !== 'false',

            // Labels
            addLabel: component.getAttribute('data-add-label') || `Thêm ${component.getAttribute('data-entity') || 'Item'}`,
            refreshLabel: component.getAttribute('data-refresh-label') || 'Làm mới',
            exportLabel: component.getAttribute('data-export-label') || 'Xuất Excel',

            // Sample data
            sampleData: this.parseSampleData(component.getAttribute('data-sample-data')),

            // Pagination
            hasPagination: component.getAttribute('data-has-pagination') !== 'false',
            pageSize: parseInt(component.getAttribute('data-page-size')) || 10,
            totalItems: parseInt(component.getAttribute('data-total-items')) || 100
        };

        return config;
    }

    /**
     * Parse columns configuration
     * @param {string} columnsStr - Columns string
     * @returns {Array} Columns array
     */
    parseColumns(columnsStr) {
        if(!columnsStr) {
            return [
                { key: 'id', label: 'ID', type: 'text', width: '80px' },
                { key: 'name', label: 'Tên', type: 'text' },
                { key: 'status', label: 'Trạng thái', type: 'badge' },
                { key: 'actions', label: 'Thao tác', type: 'actions', width: '120px' }
            ];
        }

        return columnsStr.split(',').map(col => {
            const [key, label, type, width] = col.split(':');
            return {
                key: key.trim(),
                label: (label || key).trim(),
                type: (type || 'text').trim(),
                width: width ? width.trim() : null
            };
        });
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
     * Generate grid HTML
     * @param {object} config - Configuration object
     * @returns {string} Generated HTML
     */
    generateGridHTML(config) {
        let html = `
            <!-- Data Grid -->
            <div class="card">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">
                        <i class="bi bi-list-ul me-2"></i>Danh sách ${config.entityPlural}
                    </h5>
                    <div>
        `;

        // Action buttons
        if(config.hasRefresh) {
            html += `
                <button type="button" class="btn btn-outline-secondary btn-sm me-2" onclick="${config.containerId}Refresh()">
                    <i class="bi bi-arrow-clockwise"></i> ${config.refreshLabel}
                </button>
            `;
        }

        if(config.hasExport) {
            html += `
                <button type="button" class="btn btn-success btn-sm me-2" onclick="${config.containerId}Export()">
                    <i class="bi bi-download"></i> ${config.exportLabel}
                </button>
            `;
        }

        if(config.hasAdd) {
            html += `
                <button type="button" class="btn btn-primary btn-sm" onclick="${config.containerId}Add()">
                    <i class="bi bi-plus-circle"></i> ${config.addLabel}
                </button>
            `;
        }

        html += `
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
                html += `<th${widthStyle}>${col.label}</th>`;
            }
        });

        // Actions column
        const actionsCol = config.columns.find(col => col.type === 'actions');
        if(actionsCol) {
            const widthStyle = actionsCol.width ? ` style="width: ${actionsCol.width};"` : ' style="width: 120px;"';
            html += `<th class="text-center"${widthStyle}>${actionsCol.label}</th>`;
        }

        html += `
                                </tr>
                            </thead>
                            <tbody id="${config.containerId}TableBody">
        `;

        // Sample data rows
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

            // Actions column (Dropdown style)
            if(actionsCol) {
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

        html += `
                            </tbody>
                        </table>
                    </div>
                </div>
        `;

        // Pagination
        if(config.hasPagination) {
            const totalPages = Math.ceil(config.totalItems / config.pageSize);
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
        console.log('Initializing grid logic for:', config.containerId);

        // Select all functionality
        if(config.hasSelectAll) {
            const selectAllCheckbox = document.getElementById(`${config.containerId}SelectAll`);
            if(selectAllCheckbox) {
                selectAllCheckbox.addEventListener('change', function () {
                    const checkboxes = document.querySelectorAll('.row-checkbox');
                    checkboxes.forEach(checkbox => {
                        checkbox.checked = this.checked;
                    });
                });
            }
        }

        // Create global action functions
        this.createGlobalFunctions(config);

        // Initialize Bootstrap dropdowns
        this.initializeDropdowns(config);

        console.log(`Grid logic initialized for: ${config.containerId}`);
    }

    /**
     * Initialize Bootstrap dropdowns
     * @param {object} config - Configuration object
     */
    initializeDropdowns(config) {
        console.log('Initializing Bootstrap dropdowns for:', config.containerId);
        console.log('Bootstrap available:', typeof bootstrap !== 'undefined');
        console.log('Bootstrap.Dropdown available:', typeof bootstrap !== 'undefined' && bootstrap.Dropdown);

        // Wait a bit for DOM to be ready
        setTimeout(() => {
            // Use jQuery if available for better compatibility
            if(typeof $ !== 'undefined') {
                const $dropdowns = $(`[data-component="data-grid"][data-container-id="${config.containerId}"] .dropdown-toggle`);
                console.log('Found dropdown elements (jQuery):', $dropdowns.length);

                $dropdowns.each(function () {
                    const $this = $(this);
                    console.log('Processing dropdown:', this);

                    // Remove any existing click handlers
                    $this.off('click.dropdown');

                    // Add manual click handler
                    $this.on('click.dropdown', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        console.log('Dropdown clicked:', this);

                        // Close all other dropdowns
                        $('.dropdown-menu.show').removeClass('show');

                        // Toggle this dropdown
                        const $menu = $this.next('.dropdown-menu');
                        if($menu.length) {
                            $menu.toggleClass('show');
                            console.log('Toggled dropdown menu:', $menu.hasClass('show'));
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

            // Fallback to vanilla JS
            let dropdownElements = document.querySelectorAll(`[data-grid-container="${config.containerId}"] .dropdown-toggle`);

            if(dropdownElements.length === 0) {
                // Fallback: find all dropdown toggles in the component
                const gridContainer = document.querySelector(`[data-component="data-grid"][data-container-id="${config.containerId}"]`);
                if(gridContainer) {
                    dropdownElements = gridContainer.querySelectorAll('.dropdown-toggle');
                }
            }

            console.log('Found dropdown elements:', dropdownElements.length);
            console.log('Dropdown elements:', dropdownElements);

            dropdownElements.forEach((element, index) => {
                console.log(`Processing dropdown ${index}:`, element);
                try {
                    // Initialize Bootstrap dropdown
                    if(typeof bootstrap !== 'undefined' && bootstrap.Dropdown) {
                        new bootstrap.Dropdown(element);
                        console.log('Initialized dropdown for element:', element);
                    } else {
                        console.warn('Bootstrap Dropdown not available');
                        // Fallback: manual toggle
                        element.addEventListener('click', function (e) {
                            e.preventDefault();
                            e.stopPropagation();

                            // Close all other dropdowns first
                            document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                                menu.classList.remove('show');
                            });

                            const menu = this.nextElementSibling;
                            if(menu && menu.classList.contains('dropdown-menu')) {
                                menu.classList.toggle('show');
                            }
                        });
                    }
                } catch(error) {
                    console.error('Error initializing dropdown:', error);
                    // Fallback: manual toggle
                    element.addEventListener('click', function (e) {
                        e.preventDefault();
                        const menu = this.nextElementSibling;
                        if(menu && menu.classList.contains('dropdown-menu')) {
                            menu.classList.toggle('show');
                        }
                    });
                }
            });

            // Add click outside to close dropdown
            document.addEventListener('click', function (e) {
                if(!e.target.closest('.dropdown')) {
                    document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                        menu.classList.remove('show');
                    });
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
            console.log(`View ${entity}:`, id);
            alert(`Xem chi tiết ${entity} ID: ${id}`);
        };

        // Edit function
        window[`${containerId}Edit`] = function (id) {
            console.log(`Edit ${entity}:`, id);
            alert(`Chỉnh sửa ${entity} ID: ${id}`);
        };

        // Delete function
        window[`${containerId}Delete`] = function (id) {
            console.log(`Delete ${entity}:`, id);
            if(confirm(`Bạn có chắc chắn muốn xóa ${entity} ID: ${id}?`)) {
                alert(`Đã xóa ${entity} ID: ${id}`);
            }
        };

        // Add function
        window[`${containerId}Add`] = function () {
            console.log(`Add new ${entity}`);
            alert(`Thêm ${entity} mới`);
        };

        // Refresh function
        window[`${containerId}Refresh`] = function () {
            console.log(`Refresh ${entity} data`);
            alert(`Làm mới dữ liệu ${entity}`);
        };

        // Export function
        window[`${containerId}Export`] = function () {
            console.log(`Export ${entity} data`);
            alert(`Xuất dữ liệu ${entity} ra Excel`);
        };
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

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    console.log('DataGrid: DOM loaded, initializing...');
    try {
        window.dataGridInstance = new DataGrid();
    } catch(error) {
        console.error('DataGrid initialization error:', error);
    }
});

// Export for manual initialization if needed
window.DataGrid = DataGrid;
