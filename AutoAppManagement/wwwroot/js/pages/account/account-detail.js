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
        // Disable all inputs
        $('#CustomerForm').find('input, select, textarea').each(function() {
            $(this).prop('disabled', true);
            $(this).prop('readonly', true);
            
            // Convert to plain text display for better UX
            if ($(this).is('select')) {
                const text = $(this).find('option:selected').text();
                $(this).hide();
                $(`<div class="form-control-plaintext">${text}</div>`).insertAfter($(this));
            } else if ($(this).attr('type') === 'checkbox' || $(this).attr('type') === 'radio') {
                const checked = $(this).is(':checked');
                $(this).hide();
                const label = checked ? 'Có' : 'Không';
                $(`<div class="form-control-plaintext">${label}</div>`).insertAfter($(this).parent());
            }
        });
        
        // Hide file upload
        $('#avatarFile').parent().hide();
        
        // Update modal title
        $('.modal-title').html('<i class="bi bi-eye me-2"></i>Xem Chi Tiết Khách Hàng');
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