// AccountDetail class: idempotent initializer for account detail partials
class AccountDetail {
    constructor(container) {
        // container can be jQuery object or DOM element
        this.$root = (typeof $ !== 'undefined' && container && container.jquery) ? container : (container ? $(container) : $(document));
        this._inited = false;
        this.init();
    }

    init() {
        // Init form control binder
        window.formControlBinder.init($('#CustomerForm'));
        
        // Setup avatar preview
        this.setupAvatarPreview();
        
        // Setup additional event handlers
        this.setupEventHandlers();
        
        // Listen for data loaded event
        this.listenDataLoaded();
    }
    
    /**
     * Setup avatar upload and preview
     */
    setupAvatarPreview() {
        const $avatarFile = $('#avatarFile');
        
        if ($avatarFile.length > 0) {
            $avatarFile.on('change', function(e) {
                const file = e.target.files[0];
                if (file && file.type.startsWith('image/')) {
                    const reader = new FileReader();
                    reader.onload = function(e) {
                        // Create or update preview
                        let $preview = $('#avatarPreview');
                        if ($preview.length === 0) {
                            $preview = $('<img id="avatarPreview" class="img-thumbnail me-3" style="width: 100px; height: 100px; object-fit: cover;">');
                            $avatarFile.parent().prepend($preview);
                        }
                        $preview.attr('src', e.target.result);
                    };
                    reader.readAsDataURL(file);
                }
            });
        }
    }
    
    /**
     * Setup additional event handlers
     */
    setupEventHandlers() {
        // Phone number formatting
        $('input[type="tel"]').on('input', function() {
            let value = $(this).val().replace(/\D/g, '');
            if (value.length > 0) {
                // Format as Vietnamese phone: 0912 345 678
                if (value.length <= 4) {
                    value = value;
                } else if (value.length <= 7) {
                    value = value.slice(0, 4) + ' ' + value.slice(4);
                } else {
                    value = value.slice(0, 4) + ' ' + value.slice(4, 7) + ' ' + value.slice(7, 10);
                }
                $(this).val(value);
            }
        });
        
        // Email validation on blur
        $('input[type="email"]').on('blur', function() {
            const email = $(this).val();
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            
            if (email && !emailRegex.test(email)) {
                $(this).addClass('is-invalid');
                let $feedback = $(this).parent().find('.invalid-feedback');
                if ($feedback.length === 0) {
                    $feedback = $('<div class="invalid-feedback">Email không đúng định dạng</div>');
                    $(this).parent().append($feedback);
                }
            } else {
                $(this).removeClass('is-invalid');
                $(this).parent().find('.invalid-feedback').remove();
            }
        });
    }
    
    /**
     * Listen for data loaded event
     */
    listenDataLoaded() {
        const $modal = $('.modal.show');
        
        $modal.on('dataLoaded', (e, params) => {
            const { data, mode } = params;
            
            // Handle avatar display
            if (data.imgAvatar || data.avatarUrl) {
                const avatarUrl = data.imgAvatar || data.avatarUrl;
                let $preview = $('#avatarPreview');
                if ($preview.length === 0) {
                    $preview = $('<img id="avatarPreview" class="img-thumbnail me-3" style="width: 100px; height: 100px; object-fit: cover;">');
                    $('#avatarFile').parent().prepend($preview);
                }
                $preview.attr('src', avatarUrl);
            }
            
            // Format date if needed
            if (data.dateOfBirth) {
                const date = new Date(data.dateOfBirth);
                const formatted = date.toISOString().split('T')[0];
                $('input[name="dateOfBirth"]').val(formatted);
            }
            
            // Store original data for comparison
            window.originalFormData = JSON.parse(JSON.stringify(data));
            
            // If view mode, make all controls read-only
            if (mode === 'view') {
                this.setViewMode();
            }
        });
    }
    
    /**
     * Set form to view mode (read-only)
     */
    setViewMode() {
        // Use the global setFormViewMode function
        if (typeof window.setFormViewMode === 'function') {
            window.setFormViewMode('#customerForm', 'Xem Chi Tiết Khách Hàng');
        } else {
            // Fallback to manual method if global function not available
            this.setViewModeManual();
        }
    }
    
    /**
     * Manual view mode setup (fallback)
     */
    setViewModeManual() {
        const form = $('#customerForm');
        
        // Disable all form controls
        form.find('input, select, textarea, button[type="submit"]').each(function() {
            const $element = $(this);
            
            // Skip hidden inputs
            if ($element.attr('type') === 'hidden') {
                return;
            }
            
            // Disable the element
            $element.prop('disabled', true);
            $element.prop('readonly', true);
            $element.addClass('view-mode-disabled');
            
            // Handle different control types for better UX
            if ($element.is('select')) {
                const selectedText = $element.find('option:selected').text() || 'Chưa chọn';
                $element.hide();
                $element.after(`<div class="form-control-plaintext view-mode-display">${selectedText}</div>`);
                
            } else if ($element.attr('type') === 'checkbox') {
                const isChecked = $element.is(':checked');
                const label = isChecked ? '<i class="bi bi-check-circle text-success"></i> Có' : '<i class="bi bi-x-circle text-muted"></i> Không';
                $element.hide();
                $element.parent().find('label').hide();
                $element.parent().append(`<div class="form-control-plaintext view-mode-display">${label}</div>`);
                
            } else if ($element.attr('type') === 'radio') {
                const isChecked = $element.is(':checked');
                if (isChecked) {
                    const labelText = $element.parent().find('label').text() || 'Đã chọn';
                    $element.hide();
                    $element.parent().find('label').hide();
                    $element.parent().append(`<div class="form-control-plaintext view-mode-display">${labelText}</div>`);
                } else {
                    $element.parent().hide();
                }
                
            } else if ($element.is('textarea')) {
                const value = $element.val() || 'Trống';
                $element.hide();
                $element.after(`<div class="form-control-plaintext view-mode-display" style="white-space: pre-wrap;">${value}</div>`);
                
            } else if ($element.attr('type') === 'date') {
                const dateValue = $element.val();
                let displayValue = 'Chưa chọn';
                if (dateValue) {
                    const date = new Date(dateValue);
                    displayValue = date.toLocaleDateString('vi-VN');
                }
                $element.hide();
                $element.after(`<div class="form-control-plaintext view-mode-display">${displayValue}</div>`);
                
            } else if ($element.attr('type') === 'email' || $element.attr('type') === 'tel' || $element.attr('type') === 'text') {
                const value = $element.val() || 'Trống';
                $element.hide();
                $element.after(`<div class="form-control-plaintext view-mode-display">${value}</div>`);
            }
        });
        
        // Handle switch controls specifically
        form.find('.form-switch input[type="checkbox"]').each(function() {
            const $switch = $(this);
            const isChecked = $switch.is(':checked');
            const label = isChecked ? '<i class="bi bi-toggle-on text-success fs-4"></i> Bật' : '<i class="bi bi-toggle-off text-muted fs-4"></i> Tắt';
            $switch.closest('.form-switch').hide();
            $switch.closest('.form-switch').after(`<div class="form-control-plaintext view-mode-display">${label}</div>`);
        });
        
        // Hide file upload controls
        form.find('input[type="file"]').closest('.mb-3, .row').hide();
        
        // Hide all buttons except Close/Cancel
        form.find('button[type="submit"], .btn-primary, .btn-success').hide();
        
        // Update modal title
        $('.modal-title').html('<i class="bi bi-eye me-2"></i>Xem Chi Tiết Khách Hàng');
        
        // Update footer buttons
        $('.modal-footer .btn-primary, .modal-footer .btn-success').hide();
        $('.modal-footer .btn-secondary').text('Đóng').removeClass('btn-secondary').addClass('btn-outline-primary');
        
        // Add view mode indicator
        if (!form.find('.view-mode-indicator').length) {
            form.prepend(`
                <div class="alert alert-info view-mode-indicator d-flex align-items-center" role="alert">
                    <i class="bi bi-info-circle me-2"></i>
                    <span>Đang ở chế độ xem. Tất cả các trường đã được vô hiệu hóa.</span>
                </div>
            `);
        }
    }
}

// Expose class and helper initializer
window.AccountDetail = AccountDetail;
window.initAccountDetail = function(container) {
    try {
        return new AccountDetail(container);
    } catch (e) {
        console.error('initAccountDetail error', e);
    }
};