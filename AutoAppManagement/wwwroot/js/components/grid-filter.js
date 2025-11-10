/**
 * GridFilter - Bootstrap Card Style Filter (Standard Component)
 * Usage: <div data-component="card-filter" data-has-search="true" data-has-role="true" ...></div>
 * This is the STANDARD filter component for all grid/table pages
 */

class GridFilter {
    constructor() {
        this.filters = new Map();
        this.init();
    }

    /**
     * Initialize auto filter components
     */
    init() {
        // Find all card-filter components using jQuery
        const $filterComponents = $('[data-component="card-filter"]');

        $filterComponents.each((index, component) => {
            this.renderFilter(component);
        });
    }

    /**
     * Render filter component
     * @param {HTMLElement} component - Filter component element
     */
    renderFilter(component) {
        const config = this.parseConfig(component);
        const html = this.generateFilterHTML(config);

        // Set filter container attribute using jQuery
        const $component = $(component);
        $component.attr('data-filter-container', config.containerId);

        // Render HTML
        $component.html(html);

        // Initialize filter logic
        this.initializeFilterLogic(config);

        // Store filter instance
        this.filters.set(config.containerId, config);
    }

    /**
     * Parse configuration from data attributes
     * @param {HTMLElement} component - Filter component element
     * @returns {object} Configuration object
     */
    parseConfig(component) {
        const config = {
            containerId: component.getAttribute('data-container-id') || `cardFilter_${Date.now()}`,

            // Features
            hasSearch: component.getAttribute('data-has-search') === 'true',
            hasRole: component.getAttribute('data-has-role') === 'true',
            hasStatus: component.getAttribute('data-has-status') === 'true',
            hasCategory: component.getAttribute('data-has-category') === 'true',
            hasDateRange: component.getAttribute('data-has-date-range') === 'true',
            hasPriority: component.getAttribute('data-has-priority') === 'true',
            hasType: component.getAttribute('data-has-type') === 'true',

            // Labels
            searchLabel: component.getAttribute('data-search-label') || 'Tìm kiếm',
            searchPlaceholder: component.getAttribute('data-search-placeholder') || 'Tìm kiếm...',

            // Options
            roleOptions: this.parseOptions(component.getAttribute('data-role-options')),
            statusOptions: this.parseOptions(component.getAttribute('data-status-options')),
            categoryOptions: this.parseOptions(component.getAttribute('data-category-options')),
            priorityOptions: this.parseOptions(component.getAttribute('data-priority-options')),
            typeOptions: this.parseOptions(component.getAttribute('data-type-options')),

            // API loading for category
            categoryLoadFromApi: component.getAttribute('data-category-load-from-api') === 'true',
            categoryApiUrl: component.getAttribute('data-category-api-url') || '/License/GetAll',

            // Behavior
            debounceMs: parseInt(component.getAttribute('data-debounce-ms')) || 300,
            autoApply: component.getAttribute('data-auto-apply') !== 'false'
        };

        return config;
    }

    /**
     * Parse options string into array
     * @param {string} optionsStr - Options string (format: "value1:label1,value2:label2")
     * @returns {Array} Options array
     */
    parseOptions(optionsStr) {
        if(!optionsStr) return [];

        return optionsStr.split(',').map(option => {
            const [value, label] = option.split(':');
            return { value: value.trim(), label: (label || value).trim() };
        });
    }

    /**
     * Generate filter HTML theo pattern AdminAccount
     * @param {object} config - Configuration object
     * @returns {string} Generated HTML
     */
    generateFilterHTML(config) {
        let html = `
            <!-- Filter and Actions -->
            <div class="card mb-4">
                <div class="card-body">
                    <div class="row g-3">
        `;

        // Search input (col-md-3)
        if(config.hasSearch) {
            html += `
                <div class="col-md-3">
                    <div class="form-floating">
                        <input type="text" 
                               class="form-control" 
                               id="${config.containerId}SearchInput"
                               placeholder="${config.searchPlaceholder}"
                               data-filter-type="search"
                               data-filter-name="search">
                        <label for="${config.containerId}SearchInput">
                            <i class="bi bi-search me-2"></i>${config.searchLabel}
                        </label>
                    </div>
                </div>
            `;
        }

        // Role select (col-md-2)
        if(config.hasRole) {
            html += `
                <div class="col-md-2">
                    <select class="form-select" 
                            id="${config.containerId}RoleFilter"
                            data-filter-type="select"
                            data-filter-name="role">
                        <option value="">Tất cả vai trò</option>
            `;

            if(config.roleOptions.length > 0) {
                config.roleOptions.forEach(option => {
                    html += `<option value="${option.value}">${option.label}</option>`;
                });
            } else {
                // Default role options
                html += `
                    <option value="super_admin">Super Admin</option>
                    <option value="admin">Admin</option>
                    <option value="moderator">Moderator</option>
                    <option value="support">Support</option>
                `;
            }

            html += `
                    </select>
                </div>
            `;
        }

        // Status select (col-md-2)
        if(config.hasStatus) {
            html += `
                <div class="col-md-2">
                    <select class="form-select" 
                            id="${config.containerId}StatusFilter"
                            data-filter-type="select"
                            data-filter-name="status">
                        <option value="">Tất cả trạng thái</option>
            `;

            if(config.statusOptions.length > 0) {
                config.statusOptions.forEach(option => {
                    html += `<option value="${option.value}">${option.label}</option>`;
                });
            } else {
                // Default status options
                html += `
                    <option value="active">Hoạt động</option>
                    <option value="inactive">Không hoạt động</option>
                    <option value="locked">Bị khóa</option>
                `;
            }

            html += `
                    </select>
                </div>
            `;
        }

        // Category select (col-md-2)
        if(config.hasCategory) {
            html += `
                <div class="col-md-2">
                    <select class="form-select" 
                            id="${config.containerId}CategoryFilter"
                            data-filter-type="select"
                            data-filter-name="category">
                        <option value="">Tất cả danh mục</option>
            `;

            // If loading from API, options will be populated after API call
            if(!config.categoryLoadFromApi && config.categoryOptions.length > 0) {
                config.categoryOptions.forEach(option => {
                    html += `<option value="${option.value}">${option.label}</option>`;
                });
            }

            html += `
                    </select>
                </div>
            `;
        }

        // Priority select (col-md-2)
        if(config.hasPriority) {
            html += `
                <div class="col-md-2">
                    <select class="form-select" 
                            id="${config.containerId}PriorityFilter"
                            data-filter-type="select"
                            data-filter-name="priority">
                        <option value="">Tất cả mức độ</option>
            `;

            if(config.priorityOptions.length > 0) {
                config.priorityOptions.forEach(option => {
                    html += `<option value="${option.value}">${option.label}</option>`;
                });
            } else {
                // Default priority options
                html += `
                    <option value="high">Cao</option>
                    <option value="medium">Trung bình</option>
                    <option value="low">Thấp</option>
                `;
            }

            html += `
                    </select>
                </div>
            `;
        }

        // Type select (col-md-2)
        if(config.hasType) {
            html += `
                <div class="col-md-2">
                    <select class="form-select" 
                            id="${config.containerId}TypeFilter"
                            data-filter-type="select"
                            data-filter-name="type">
                        <option value="">Tất cả loại</option>
            `;

            if(config.typeOptions.length > 0) {
                config.typeOptions.forEach(option => {
                    html += `<option value="${option.value}">${option.label}</option>`;
                });
            }

            html += `
                    </select>
                </div>
            `;
        }

        // Date range (col-md-3)
        if(config.hasDateRange) {
            html += `
                <div class="col-md-3">
                    <div class="input-group">
                        <input type="date" 
                               class="form-control" 
                               id="${config.containerId}DateFrom"
                               data-filter-type="date"
                               data-filter-name="dateFrom">
                        <span class="input-group-text">đến</span>
                        <input type="date" 
                               class="form-control" 
                               id="${config.containerId}DateTo"
                               data-filter-type="date"
                               data-filter-name="dateTo">
                    </div>
                </div>
            `;
        }

        html += `
                    </div>
                </div>
            </div>
        `;

        return html;
    }

    /**
     * Initialize filter logic
     * @param {object} config - Configuration object
     */
    initializeFilterLogic(config) {
        const container = document.querySelector(`[data-filter-container="${config.containerId}"]`);
        if(!container) {
            console.error('Container not found:', config.containerId);
            return;
        }

        let searchTimeout;

        // Get all filter inputs
        const searchInput = document.getElementById(`${config.containerId}SearchInput`);
        const roleSelect = document.getElementById(`${config.containerId}RoleFilter`);
        const statusSelect = document.getElementById(`${config.containerId}StatusFilter`);
        const categorySelect = document.getElementById(`${config.containerId}CategoryFilter`);
        const prioritySelect = document.getElementById(`${config.containerId}PriorityFilter`);
        const typeSelect = document.getElementById(`${config.containerId}TypeFilter`);
        const dateFromInput = document.getElementById(`${config.containerId}DateFrom`);
        const dateToInput = document.getElementById(`${config.containerId}DateTo`);

        // Update result function
        function updateResult() {
            const filters = {};
            if(searchInput && searchInput.value.trim()) filters.search = searchInput.value.trim();
            if(roleSelect && roleSelect.value.trim()) filters.role = roleSelect.value.trim();
            if(statusSelect && statusSelect.value.trim()) filters.status = statusSelect.value.trim();
            if(categorySelect && categorySelect.value.trim()) filters.category = categorySelect.value.trim();
            if(prioritySelect && prioritySelect.value.trim()) filters.priority = prioritySelect.value.trim();
            if(typeSelect && typeSelect.value.trim()) filters.type = typeSelect.value.trim();
            if(dateFromInput && dateFromInput.value.trim()) filters.dateFrom = dateFromInput.value.trim();
            if(dateToInput && dateToInput.value.trim()) filters.dateTo = dateToInput.value.trim();

            // Trigger custom event
            document.dispatchEvent(new CustomEvent(`${config.containerId}FilterChanged`, {
                detail: { filters: filters, containerId: config.containerId }
            }));
        }

        // Debounced search
        function debouncedSearch() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(updateResult, config.debounceMs);
        }

        // Event listeners
        if(searchInput) {
            searchInput.addEventListener('input', debouncedSearch);
            searchInput.addEventListener('keydown', function (e) {
                if(e.key === 'Escape') { this.value = ''; updateResult(); }
                if(e.key === 'Enter') { updateResult(); }
            });
        }

        if(roleSelect) roleSelect.addEventListener('change', updateResult);
        if(statusSelect) statusSelect.addEventListener('change', updateResult);
        if(categorySelect) categorySelect.addEventListener('change', updateResult);
        if(prioritySelect) prioritySelect.addEventListener('change', updateResult);
        if(typeSelect) typeSelect.addEventListener('change', updateResult);
        if(dateFromInput) dateFromInput.addEventListener('change', updateResult);
        if(dateToInput) dateToInput.addEventListener('change', updateResult);

        // Initial trigger
        if(config.autoApply) {
            updateResult();
        }

        // Load category options from API if needed
        if(config.hasCategory && config.categoryLoadFromApi && config.categoryApiUrl) {
            this.loadCategoryOptionsFromApi(config);
        }
    }

    /**
     * Load category options from API
     * @param {object} config - Configuration object
     */
    loadCategoryOptionsFromApi(config) {
        const categorySelect = document.getElementById(`${config.containerId}CategoryFilter`);
        if (!categorySelect) {
            console.error('Category select not found:', `${config.containerId}CategoryFilter`);
            return;
        }

        // Show loading state
        categorySelect.disabled = true;
        const loadingOption = document.createElement('option');
        loadingOption.value = '';
        loadingOption.textContent = 'Đang tải...';
        categorySelect.appendChild(loadingOption);

        // Call API to get licenses
        if (typeof callGetAPIAuthen === 'function') {
            callGetAPIAuthen(config.categoryApiUrl, {},
                (response) => {
                    // Remove loading option
                    categorySelect.innerHTML = '<option value="">Tất cả danh mục</option>';
                    
                    if (response && response.IsSuccess && response.Data) {
                        const licenses = Array.isArray(response.Data) ? response.Data : 
                                        (response.Data.Data || response.Data.Items || []);
                        
                        licenses.forEach(license => {
                            const option = document.createElement('option');
                            // Use LicenseName as both value and label for filtering
                            // Backend will search in LicenseName field
                            const licenseName = license.LicenseName || license.Name || license.licenseName || '';
                            option.value = licenseName;
                            option.textContent = licenseName;
                            categorySelect.appendChild(option);
                        });
                        
                        console.log('✅ Loaded category options from API:', licenses.length);
                    } else {
                        console.error('❌ Failed to load category options:', response);
                    }
                    
                    categorySelect.disabled = false;
                },
                (error) => {
                    console.error('❌ Error loading category options:', error);
                    categorySelect.innerHTML = '<option value="">Tất cả danh mục</option>';
                    categorySelect.disabled = false;
                }
            );
        } else {
            // Fallback: use fetch API
            fetch(config.categoryApiUrl)
                .then(response => response.json())
                .then(data => {
                    categorySelect.innerHTML = '<option value="">Tất cả danh mục</option>';
                    
                    if (data && data.IsSuccess && data.Data) {
                        const licenses = Array.isArray(data.Data) ? data.Data : 
                                        (data.Data.Data || data.Data.Items || []);
                        
                        licenses.forEach(license => {
                            const option = document.createElement('option');
                            const licenseName = license.LicenseName || license.Name || license.licenseName || '';
                            option.value = licenseName;
                            option.textContent = licenseName;
                            categorySelect.appendChild(option);
                        });
                    }
                    
                    categorySelect.disabled = false;
                })
                .catch(error => {
                    console.error('❌ Error loading category options:', error);
                    categorySelect.innerHTML = '<option value="">Tất cả danh mục</option>';
                    categorySelect.disabled = false;
                });
        }
    }

    /**
     * Get filter instance by container ID
     * @param {string} containerId - Container ID
     * @returns {object} Filter configuration
     */
    getFilter(containerId) {
        return this.filters.get(containerId);
    }

    /**
     * Get all filters
     * @returns {Map} All filter instances
     */
    getAllFilters() {
        return this.filters;
    }
}

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    try {
        window.gridFilterInstance = new GridFilter();
    } catch(error) {
        console.error('GridFilter initialization error:', error);
    }
});

// Export for manual initialization if needed
window.GridFilter = GridFilter;
// Backward compatibility
window.AutoFilterV3 = GridFilter;
