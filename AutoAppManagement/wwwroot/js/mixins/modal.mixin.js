/**
 * Modal Mixin - Reusable Modal functionality
 * Usage: const modal = new ModalMixin('#myModal', options);
 */
class ModalMixin {
    constructor(selector, options = {}) {
        this.modal = $(selector);
        this.isOpen = false;
        this.backdrop = null;
        
        // Default options
        this.options = {
            backdrop: true,
            keyboard: true,
            focus: true,
            show: false,
            size: 'md', // sm, md, lg, xl
            centered: false,
            scrollable: false,
            fullscreen: false,
            closeOnBackdrop: true,
            closeOnEscape: true,
            ...options
        };
        
        this.init();
    }
    
    init() {
        this.setupModal();
        this.bindEvents();
    }
    
    // Setup modal structure and classes
    setupModal() {
        // Add modal classes if not present
        if (!this.modal.hasClass('modal')) {
            this.modal.addClass('modal fade');
        }
        
        // Setup modal dialog
        let modalDialog = this.modal.find('.modal-dialog');
        if (!modalDialog.length) {
            modalDialog = $('<div class="modal-dialog"></div>');
            this.modal.wrapInner(modalDialog);
        }
        
        // Apply size class
        modalDialog.removeClass('modal-sm modal-lg modal-xl modal-fullscreen')
                  .addClass(`modal-${this.options.size}`);
        
        // Apply centered class
        if (this.options.centered) {
            modalDialog.addClass('modal-dialog-centered');
        }
        
        // Apply scrollable class
        if (this.options.scrollable) {
            modalDialog.addClass('modal-dialog-scrollable');
        }
        
        // Apply fullscreen class
        if (this.options.fullscreen) {
            modalDialog.addClass('modal-fullscreen');
        }
        
        // Setup modal content
        let modalContent = modalDialog.find('.modal-content');
        if (!modalContent.length) {
            modalContent = $('<div class="modal-content"></div>');
            modalDialog.wrapInner(modalContent);
        }
        
        // Add data attributes
        this.modal.attr('data-component', 'modal');
        this.modal.attr('data-state', 'closed');
    }
    
    // Event binding
    bindEvents() {
        const self = this;
        
        // Close button
        this.modal.on('click', '[data-action="close"], [data-bs-dismiss="modal"]', function(e) {
            e.preventDefault();
            self.hide();
        });
        
        // Backdrop click
        this.modal.on('click', function(e) {
            if (e.target === this && self.options.closeOnBackdrop) {
                self.hide();
            }
        });
        
        // Escape key
        $(document).on('keydown', function(e) {
            if (e.key === 'Escape' && self.isOpen && self.options.closeOnEscape) {
                self.hide();
            }
        });
        
        // Form submission within modal
        this.modal.on('submit', 'form', function(e) {
            e.preventDefault();
            self.onFormSubmit($(this));
        });
        
        // Action buttons
        this.modal.on('click', '[data-action]', function(e) {
            const action = $(this).attr('data-action');
            if (action !== 'close') {
                e.preventDefault();
                self.onAction(action, $(this));
            }
        });
        
        // Auto-show if specified
        if (this.options.show) {
            this.show();
        }
    }
    
    // Show modal
    show(data = null) {
        if (this.isOpen) return;
        
        this.isOpen = true;
        this.modal.attr('data-state', 'opening');
        
        // Load data if provided
        if (data) {
            this.loadData(data);
        }
        
        // Create backdrop
        if (this.options.backdrop) {
            this.createBackdrop();
        }
        
        // Show modal
        this.modal.addClass('show').css('display', 'block');
        
        // Focus management
        if (this.options.focus) {
            this.setFocus();
        }
        
        // Add body class
        $('body').addClass('modal-open');
        
        // Trigger events
        this.modal.trigger('show.modal');
        this.onShow(data);
        
        // Animation complete
        setTimeout(() => {
            this.modal.attr('data-state', 'open');
            this.modal.trigger('shown.modal');
            this.onShown(data);
        }, 150);
    }
    
    // Hide modal
    hide() {
        if (!this.isOpen) return;
        
        this.modal.attr('data-state', 'closing');
        this.modal.trigger('hide.modal');
        this.onHide();
        
        // Hide modal
        this.modal.removeClass('show');
        
        // Remove backdrop
        if (this.backdrop) {
            this.backdrop.remove();
            this.backdrop = null;
        }
        
        // Animation complete
        setTimeout(() => {
            this.modal.css('display', 'none');
            this.modal.attr('data-state', 'closed');
            this.isOpen = false;
            
            // Remove body class if no other modals
            if ($('.modal.show').length === 0) {
                $('body').removeClass('modal-open');
            }
            
            this.modal.trigger('hidden.modal');
            this.onHidden();
        }, 150);
    }
    
    // Toggle modal
    toggle(data = null) {
        if (this.isOpen) {
            this.hide();
        } else {
            this.show(data);
        }
    }
    
    // Create backdrop
    createBackdrop() {
        this.backdrop = $('<div class="modal-backdrop fade show"></div>');
        $('body').append(this.backdrop);
    }
    
    // Set focus to modal
    setFocus() {
        // Focus on first focusable element
        const focusableElements = this.modal.find('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
        if (focusableElements.length) {
            focusableElements.first().focus();
        } else {
            this.modal.focus();
        }
    }
    
    // Load data into modal
    loadData(data) {
        // Load data into form fields
        if (data && typeof data === 'object') {
            Object.keys(data).forEach(key => {
                const element = this.modal.find(`[data-field="${key}"]`);
                if (element.length) {
                    if (element.is('input[type="checkbox"]')) {
                        element.prop('checked', data[key]);
                    } else if (element.is('input[type="radio"]')) {
                        element.filter(`[value="${data[key]}"]`).prop('checked', true);
                    } else {
                        element.val(data[key]);
                    }
                }
                
                // Load into display elements
                const displayElement = this.modal.find(`[data-display="${key}"]`);
                if (displayElement.length) {
                    displayElement.text(data[key]);
                }
            });
        }
        
        this.onDataLoaded(data);
    }
    
    // Get form data from modal
    getFormData() {
        const data = {};
        
        this.modal.find('[data-field]').each((index, element) => {
            const $element = $(element);
            const field = $element.attr('data-field');
            const type = $element.attr('type');
            
            if (type === 'checkbox') {
                data[field] = $element.prop('checked');
            } else if (type === 'radio') {
                if ($element.prop('checked')) {
                    data[field] = $element.val();
                }
            } else {
                data[field] = $element.val();
            }
        });
        
        return data;
    }
    
    // Set modal title
    setTitle(title) {
        const titleElement = this.modal.find('[data-element="title"], .modal-title');
        if (titleElement.length) {
            titleElement.text(title);
        }
    }
    
    // Set modal body content
    setBody(content) {
        const bodyElement = this.modal.find('[data-element="body"], .modal-body');
        if (bodyElement.length) {
            bodyElement.html(content);
        }
    }
    
    // Set modal footer content
    setFooter(content) {
        const footerElement = this.modal.find('[data-element="footer"], .modal-footer');
        if (footerElement.length) {
            footerElement.html(content);
        }
    }
    
    // Show loading state
    showLoading(message = 'Đang tải...') {
        const loadingHtml = `
            <div class="text-center py-4" data-state="loading">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <div class="mt-2">${message}</div>
            </div>
        `;
        this.setBody(loadingHtml);
    }
    
    // Show error state
    showError(message = 'Có lỗi xảy ra') {
        const errorHtml = `
            <div class="text-center py-4 text-danger" data-state="error">
                <i class="bi bi-exclamation-triangle fs-1"></i>
                <div class="mt-2">${message}</div>
                <button class="btn btn-outline-primary btn-sm mt-2" data-action="retry">
                    <i class="bi bi-arrow-clockwise me-1"></i>Thử lại
                </button>
            </div>
        `;
        this.setBody(errorHtml);
    }
    
    // Load content from URL
    async loadFromUrl(url, data = {}) {
        this.showLoading();
        
        try {
            const response = await $.ajax({
                url: url,
                type: 'GET',
                data: data
            });
            
            if (response.success) {
                if (response.title) this.setTitle(response.title);
                if (response.body) this.setBody(response.body);
                if (response.footer) this.setFooter(response.footer);
                
                this.onContentLoaded(response);
            } else {
                this.showError(response.message);
            }
        } catch (error) {
            this.showError('Không thể tải nội dung');
        }
    }
    
    // Validate modal form
    validate() {
        let isValid = true;
        
        this.modal.find('[data-validate]').each((index, element) => {
            const $element = $(element);
            const value = $element.val();
            const rules = $element.attr('data-validate').split('|');
            
            let fieldValid = true;
            rules.forEach(rule => {
                if (!this.validateRule(value, rule)) {
                    fieldValid = false;
                }
            });
            
            if (!fieldValid) {
                $element.addClass('is-invalid');
                isValid = false;
            } else {
                $element.removeClass('is-invalid');
            }
        });
        
        return isValid;
    }
    
    // Validate single rule
    validateRule(value, rule) {
        const [ruleName, ruleValue] = rule.split(':');
        
        switch (ruleName) {
            case 'required':
                return value && value.trim() !== '';
            case 'email':
                return !value || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
            case 'min':
                return !value || value.length >= parseInt(ruleValue);
            case 'max':
                return !value || value.length <= parseInt(ruleValue);
            default:
                return true;
        }
    }
    
    // Static methods for quick modal creation
    static alert(message, title = 'Thông báo', options = {}) {
        const modalHtml = `
            <div class="modal fade" data-component="modal" data-type="alert">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">${title}</h5>
                            <button type="button" class="btn-close" data-action="close"></button>
                        </div>
                        <div class="modal-body">
                            <p>${message}</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" data-action="close">OK</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        const $modal = $(modalHtml).appendTo('body');
        const modal = new ModalMixin($modal, { ...options, show: true });
        
        // Auto-remove after hide
        $modal.on('hidden.modal', function() {
            $modal.remove();
        });
        
        return modal;
    }
    
    static confirm(message, title = 'Xác nhận', options = {}) {
        return new Promise((resolve) => {
            const modalHtml = `
                <div class="modal fade" data-component="modal" data-type="confirm">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">${title}</h5>
                                <button type="button" class="btn-close" data-action="close"></button>
                            </div>
                            <div class="modal-body">
                                <p>${message}</p>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-action="cancel">Hủy</button>
                                <button type="button" class="btn btn-primary" data-action="confirm">Xác nhận</button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            const $modal = $(modalHtml).appendTo('body');
            const modal = new ModalMixin($modal, { ...options, show: true });
            
            $modal.on('click', '[data-action="confirm"]', function() {
                resolve(true);
                modal.hide();
            });
            
            $modal.on('click', '[data-action="cancel"], [data-action="close"]', function() {
                resolve(false);
                modal.hide();
            });
            
            // Auto-remove after hide
            $modal.on('hidden.modal', function() {
                $modal.remove();
            });
        });
    }
    
    // Event hooks (override in implementation)
    onShow(data) {
        console.log('Modal showing:', data);
    }
    
    onShown(data) {
        console.log('Modal shown:', data);
    }
    
    onHide() {
        console.log('Modal hiding');
    }
    
    onHidden() {
        console.log('Modal hidden');
    }
    
    onDataLoaded(data) {
        console.log('Modal data loaded:', data);
    }
    
    onContentLoaded(response) {
        console.log('Modal content loaded:', response);
    }
    
    onFormSubmit(form) {
        console.log('Modal form submitted:', form);
    }
    
    onAction(action, element) {
        console.log('Modal action:', action, element);
    }
}

// Export for use
window.ModalMixin = ModalMixin;
