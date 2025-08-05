/**
 * Filter Mixin - Reusable Filter functionality
 * Usage: const filter = new FilterMixin('#filterForm', options);
 */
class FilterMixin {
    constructor(selector, options = {}) {
        this.container = $(selector);
        this.filters = {};
        this.debounceTimeout = null;
        
        // Default options
        this.options = {
            debounceDelay: 300,
            autoSubmit: true,
            resetButton: '[data-action="reset"]',
            submitButton: '[data-action="search"]',
            ...options
        };
        
        this.init();
    }
    
    init() {
        this.bindEvents();
        this.loadInitialValues();
    }
    
    // Event binding
    bindEvents() {
        const self = this;
        
        // Text inputs with debounce
        this.container.on('input', '[data-filter][data-trigger="enter"], [data-filter="search"]', function() {
            clearTimeout(self.debounceTimeout);
            self.debounceTimeout = setTimeout(() => {
                self.handleFilterChange($(this));
            }, self.options.debounceDelay);
        });
        
        // Enter key on search inputs
        this.container.on('keypress', '[data-filter="search"]', function(e) {
            if (e.which === 13) {
                e.preventDefault();
                self.handleFilterChange($(this));
                self.onSubmit();
            }
        });
        
        // Select dropdowns
        this.container.on('change', '[data-filter][data-trigger="change"]', function() {
            self.handleFilterChange($(this));
            if (self.options.autoSubmit) {
                self.onSubmit();
            }
        });
        
        // Submit button
        this.container.on('click', this.options.submitButton, function(e) {
            e.preventDefault();
            self.onSubmit();
        });
        
        // Reset button
        this.container.on('click', this.options.resetButton, function(e) {
            e.preventDefault();
            self.reset();
        });
        
        // Date range inputs
        this.container.on('change', '[data-filter][data-type="date"]', function() {
            self.handleDateRangeChange($(this));
        });
        
        // Number range inputs
        this.container.on('input', '[data-filter][data-type="number"]', function() {
            clearTimeout(self.debounceTimeout);
            self.debounceTimeout = setTimeout(() => {
                self.handleNumberRangeChange($(this));
            }, self.options.debounceDelay);
        });
    }
    
    // Handle filter changes
    handleFilterChange(element) {
        const field = element.attr('data-field') || element.attr('data-filter');
        const value = element.val();
        const type = element.attr('data-type') || 'text';
        
        this.setFilter(field, value, type);
        this.onFilterChanged(field, value, type);
    }
    
    // Handle date range changes
    handleDateRangeChange(element) {
        const field = element.attr('data-field');
        const rangeType = element.attr('data-range'); // 'from' or 'to'
        const value = element.val();
        
        if (!this.filters[field]) {
            this.filters[field] = {};
        }
        
        this.filters[field][rangeType] = value;
        this.onFilterChanged(field, this.filters[field], 'daterange');
    }
    
    // Handle number range changes
    handleNumberRangeChange(element) {
        const field = element.attr('data-field');
        const rangeType = element.attr('data-range'); // 'min' or 'max'
        const value = element.val();
        
        if (!this.filters[field]) {
            this.filters[field] = {};
        }
        
        this.filters[field][rangeType] = value;
        this.onFilterChanged(field, this.filters[field], 'numberrange');
    }
    
    // Set filter value
    setFilter(field, value, type = 'text') {
        if (value === '' || value === null || value === undefined) {
            delete this.filters[field];
        } else {
            this.filters[field] = { value, type };
        }
        
        this.updateFilterUI(field, value);
    }
    
    // Get filter value
    getFilter(field) {
        return this.filters[field]?.value || '';
    }
    
    // Get all filters
    getFilters() {
        const result = {};
        Object.keys(this.filters).forEach(key => {
            const filter = this.filters[key];
            if (filter.type === 'daterange' || filter.type === 'numberrange') {
                result[key] = filter;
            } else {
                result[key] = filter.value;
            }
        });
        return result;
    }
    
    // Get filters for API (flattened)
    getApiFilters() {
        const result = {};
        Object.keys(this.filters).forEach(key => {
            const filter = this.filters[key];
            if (filter.type === 'daterange') {
                if (filter.from) result[`${key}From`] = filter.from;
                if (filter.to) result[`${key}To`] = filter.to;
            } else if (filter.type === 'numberrange') {
                if (filter.min) result[`${key}Min`] = filter.min;
                if (filter.max) result[`${key}Max`] = filter.max;
            } else {
                result[key] = filter.value;
            }
        });
        return result;
    }
    
    // Load initial values from URL or data attributes
    loadInitialValues() {
        // Load from URL parameters
        const urlParams = new URLSearchParams(window.location.search);
        urlParams.forEach((value, key) => {
            const element = this.container.find(`[data-field="${key}"]`);
            if (element.length) {
                element.val(value);
                this.setFilter(key, value);
            }
        });
        
        // Load from data attributes
        this.container.find('[data-filter][data-default]').each((index, element) => {
            const $element = $(element);
            const field = $element.attr('data-field') || $element.attr('data-filter');
            const defaultValue = $element.attr('data-default');
            
            if (!this.filters[field] && defaultValue) {
                $element.val(defaultValue);
                this.setFilter(field, defaultValue);
            }
        });
    }
    
    // Update filter UI
    updateFilterUI(field, value) {
        const element = this.container.find(`[data-field="${field}"]`);
        if (element.length && element.val() !== value) {
            element.val(value);
        }
        
        // Update filter count badge
        this.updateFilterCount();
        
        // Update clear button visibility
        this.updateClearButton();
    }
    
    // Update filter count
    updateFilterCount() {
        const count = Object.keys(this.filters).length;
        const badge = this.container.find('[data-element="filter-count"]');
        
        if (badge.length) {
            if (count > 0) {
                badge.text(count).show();
            } else {
                badge.hide();
            }
        }
    }
    
    // Update clear button
    updateClearButton() {
        const hasFilters = Object.keys(this.filters).length > 0;
        const clearButton = this.container.find('[data-action="clear"]');
        
        if (clearButton.length) {
            clearButton.toggle(hasFilters);
        }
    }
    
    // Reset all filters
    reset() {
        this.filters = {};
        
        // Clear all form inputs
        this.container.find('[data-filter]').each((index, element) => {
            const $element = $(element);
            const tagName = $element.prop('tagName').toLowerCase();
            
            if (tagName === 'select') {
                $element.prop('selectedIndex', 0);
            } else {
                $element.val('');
            }
        });
        
        this.updateFilterCount();
        this.updateClearButton();
        this.onReset();
        
        if (this.options.autoSubmit) {
            this.onSubmit();
        }
    }
    
    // Apply filters from object
    applyFilters(filters) {
        Object.keys(filters).forEach(field => {
            const value = filters[field];
            this.setFilter(field, value);
            
            const element = this.container.find(`[data-field="${field}"]`);
            if (element.length) {
                element.val(value);
            }
        });
        
        this.onFiltersApplied(filters);
    }
    
    // Validate filters
    validateFilters() {
        const errors = [];
        
        this.container.find('[data-filter][data-required="true"]').each((index, element) => {
            const $element = $(element);
            const field = $element.attr('data-field') || $element.attr('data-filter');
            const value = $element.val();
            const label = $element.attr('data-label') || field;
            
            if (!value || value.trim() === '') {
                errors.push(`${label} là bắt buộc`);
                $element.addClass('is-invalid');
            } else {
                $element.removeClass('is-invalid');
            }
        });
        
        // Validate date ranges
        this.container.find('[data-type="date"][data-range="from"]').each((index, element) => {
            const $fromElement = $(element);
            const field = $fromElement.attr('data-field');
            const $toElement = this.container.find(`[data-field="${field}"][data-range="to"]`);
            
            if ($fromElement.val() && $toElement.val()) {
                const fromDate = new Date($fromElement.val());
                const toDate = new Date($toElement.val());
                
                if (fromDate > toDate) {
                    errors.push('Ngày bắt đầu không thể lớn hơn ngày kết thúc');
                    $fromElement.addClass('is-invalid');
                    $toElement.addClass('is-invalid');
                }
            }
        });
        
        return errors;
    }
    
    // Get filter summary for display
    getFilterSummary() {
        const summary = [];
        
        Object.keys(this.filters).forEach(field => {
            const filter = this.filters[field];
            const element = this.container.find(`[data-field="${field}"]`);
            const label = element.attr('data-label') || field;
            
            if (filter.type === 'daterange') {
                if (filter.from && filter.to) {
                    summary.push(`${label}: ${filter.from} - ${filter.to}`);
                } else if (filter.from) {
                    summary.push(`${label}: Từ ${filter.from}`);
                } else if (filter.to) {
                    summary.push(`${label}: Đến ${filter.to}`);
                }
            } else if (filter.type === 'numberrange') {
                if (filter.min && filter.max) {
                    summary.push(`${label}: ${filter.min} - ${filter.max}`);
                } else if (filter.min) {
                    summary.push(`${label}: Từ ${filter.min}`);
                } else if (filter.max) {
                    summary.push(`${label}: Đến ${filter.max}`);
                }
            } else {
                summary.push(`${label}: ${filter.value}`);
            }
        });
        
        return summary;
    }
    
    // Event hooks (override in implementation)
    onFilterChanged(field, value, type) {
        // Override this method to handle filter changes
        console.log('Filter changed:', field, value, type);
    }
    
    onSubmit() {
        // Override this method to handle form submission
        console.log('Filters submitted:', this.getFilters());
    }
    
    onReset() {
        // Override this method to handle form reset
        console.log('Filters reset');
    }
    
    onFiltersApplied(filters) {
        // Override this method to handle filters applied
        console.log('Filters applied:', filters);
    }
}

// Export for use
window.FilterMixin = FilterMixin;
