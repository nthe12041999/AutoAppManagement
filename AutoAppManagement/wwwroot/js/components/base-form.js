/**
 * Base Form Component
 * Combines FormMixin and ValidationMixin for complete form handling
 * 
 * Usage:
 * <form data-base-form data-api-url="/api/admin" data-method="POST">
 *   <input type="text" name="fullName" data-rule="required|min:3|max:50" placeholder="Họ và tên">
 *   <input type="email" name="email" data-rule="required|email" placeholder="Email">
 *   <button type="submit">Lưu</button>
 * </form>
 * 
 * const form = new BaseForm('form[data-base-form]');
 */

class BaseForm {
    constructor(container, options = {}) {
        this.container = $(container);
        this.options = {
            apiUrl: null,
            method: 'POST',
            validateOnSubmit: true,
            validateOnBlur: true,
            validateOnInput: false,
            showSuccessMessage: true,
            showErrorMessage: true,
            redirectOnSuccess: null,
            resetOnSuccess: false,
            confirmBeforeSubmit: false,
            submitButton: '[type="submit"]',
            loadingText: 'Đang xử lý...',
            ...options
        };

        // Get options from data attributes
        this.options.apiUrl = this.container.attr('data-api-url') || this.options.apiUrl;
        this.options.method = this.container.attr('data-method') || this.options.method;
        this.options.redirectOnSuccess = this.container.attr('data-redirect-success') || this.options.redirectOnSuccess;
        this.options.confirmBeforeSubmit = this.container.attr('data-confirm') === 'true';

        this.isSubmitting = false;
        this.validator = null;
        this.formMixin = null;

        this.init();
    }

    // Initialize form
    init() {
        console.log('🔧 BaseForm initialized');
        
        // Initialize validation
        this.validator = new ValidationMixin(this.container, {
            validateOnSubmit: this.options.validateOnSubmit,
            validateOnBlur: this.options.validateOnBlur,
            validateOnInput: this.options.validateOnInput
        });

        // Initialize form mixin if available
        if (window.FormMixin) {
            this.formMixin = new FormMixin(this.container, {
                validateOnSubmit: false, // Let BaseForm handle validation
                apiUrl: this.options.apiUrl,
                method: this.options.method
            });
        }

        this.bindEvents();
    }

    // Bind form events
    bindEvents() {
        const self = this;

        // Form submit
        this.container.on('submit', function(e) {
            e.preventDefault();
            self.handleSubmit();
        });

        // Submit button click
        this.container.on('click', this.options.submitButton, function(e) {
            e.preventDefault();
            self.handleSubmit();
        });

        // Reset button
        this.container.on('click', '[type="reset"], [data-action="reset"]', function(e) {
            e.preventDefault();
            self.resetForm();
        });

        // Cancel button
        this.container.on('click', '[data-action="cancel"]', function(e) {
            e.preventDefault();
            self.handleCancel();
        });
    }

    // Handle form submission
    async handleSubmit() {
        if (this.isSubmitting) {
            return;
        }

        console.log('📤 Handling form submit...');

        // Validate form
        if (!this.validator.validateForm()) {
            console.log('❌ Form validation failed');
            this.showValidationErrors();
            return;
        }

        // Confirm before submit if required
        if (this.options.confirmBeforeSubmit) {
            const confirmed = await this.showConfirm('Bạn có chắc chắn muốn lưu thông tin này?');
            if (!confirmed) {
                return;
            }
        }

        // Show loading state
        this.showLoading();

        try {
            // Get form data
            const formData = this.getFormData();
            console.log('📋 Form data:', formData);

            // Submit to API if URL provided
            if (this.options.apiUrl) {
                const response = await this.submitToApi(formData);
                await this.handleSubmitSuccess(response);
            } else {
                // Call custom submit handler
                await this.onSubmit(formData);
            }

        } catch (error) {
            console.error('❌ Form submit error:', error);
            await this.handleSubmitError(error);
        } finally {
            this.hideLoading();
        }
    }

    // Submit form data to API
    async submitToApi(formData) {
        const url = this.options.apiUrl;
        const method = this.options.method.toUpperCase();

        const requestOptions = {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        };

        // Add CSRF token if available
        const csrfToken = $('meta[name="csrf-token"]').attr('content') || 
                         $('input[name="__RequestVerificationToken"]').val();
        if (csrfToken) {
            requestOptions.headers['X-CSRF-TOKEN'] = csrfToken;
        }

        // Add body for POST/PUT requests
        if (['POST', 'PUT', 'PATCH'].includes(method)) {
            if (formData instanceof FormData) {
                requestOptions.body = formData;
                delete requestOptions.headers['Content-Type']; // Let browser set it
            } else {
                requestOptions.body = JSON.stringify(formData);
            }
        }

        const response = await fetch(url, requestOptions);
        
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || `HTTP ${response.status}: ${response.statusText}`);
        }

        return await response.json();
    }

    // Handle successful submission
    async handleSubmitSuccess(response) {
        console.log('✅ Form submit success:', response);

        // Show success message
        if (this.options.showSuccessMessage) {
            const message = response.message || 'Lưu thông tin thành công!';
            await this.showSuccess(message);
        }

        // Reset form if required
        if (this.options.resetOnSuccess) {
            this.resetForm();
        }

        // Redirect if specified
        if (this.options.redirectOnSuccess) {
            setTimeout(() => {
                window.location.href = this.options.redirectOnSuccess;
            }, 1500);
        }

        // Call custom success handler
        await this.onSubmitSuccess(response);
    }

    // Handle submission error
    async handleSubmitError(error) {
        console.error('❌ Form submit error:', error);

        // Show error message
        if (this.options.showErrorMessage) {
            const message = error.message || 'Có lỗi xảy ra khi lưu thông tin!';
            await this.showError(message);
        }

        // Handle validation errors from server
        if (error.errors) {
            this.showServerValidationErrors(error.errors);
        }

        // Call custom error handler
        await this.onSubmitError(error);
    }

    // Get form data
    getFormData() {
        const formData = new FormData(this.container[0]);
        const data = {};
        
        for (let [key, value] of formData.entries()) {
            if (data[key]) {
                // Handle multiple values (checkboxes, etc.)
                if (Array.isArray(data[key])) {
                    data[key].push(value);
                } else {
                    data[key] = [data[key], value];
                }
            } else {
                data[key] = value;
            }
        }
        
        return data;
    }

    // Get form data as FormData object (for file uploads)
    getFormDataAsFormData() {
        return new FormData(this.container[0]);
    }

    // Reset form
    resetForm() {
        console.log('🔄 Resetting form...');
        
        // Reset form fields
        this.container[0].reset();
        
        // Reset validation
        if (this.validator) {
            this.validator.resetValidation();
        }

        // Call custom reset handler
        this.onReset();
    }

    // Handle cancel
    handleCancel() {
        console.log('❌ Form cancelled');
        
        // Check if form has changes
        if (this.hasChanges()) {
            this.showConfirm('Bạn có chắc chắn muốn hủy? Các thay đổi sẽ bị mất.').then(confirmed => {
                if (confirmed) {
                    this.onCancel();
                }
            });
        } else {
            this.onCancel();
        }
    }

    // Check if form has changes
    hasChanges() {
        // Simple implementation - can be enhanced
        const currentData = this.getFormData();
        return Object.keys(currentData).some(key => currentData[key] && currentData[key].toString().trim() !== '');
    }

    // Show loading state
    showLoading() {
        this.isSubmitting = true;
        const $submitBtn = this.container.find(this.options.submitButton);
        $submitBtn.prop('disabled', true);
        
        // Store original text
        if (!$submitBtn.data('original-text')) {
            $submitBtn.data('original-text', $submitBtn.text());
        }
        
        $submitBtn.html(`<span class="spinner-border spinner-border-sm me-2"></span>${this.options.loadingText}`);
    }

    // Hide loading state
    hideLoading() {
        this.isSubmitting = false;
        const $submitBtn = this.container.find(this.options.submitButton);
        $submitBtn.prop('disabled', false);
        
        const originalText = $submitBtn.data('original-text');
        if (originalText) {
            $submitBtn.text(originalText);
        }
    }

    // Show validation errors
    showValidationErrors() {
        const errors = this.validator.getErrors();
        const errorMessages = Object.values(errors);
        
        if (errorMessages.length > 0) {
            this.showError(`Vui lòng kiểm tra lại thông tin:\n${errorMessages.join('\n')}`);
        }
    }

    // Show server validation errors
    showServerValidationErrors(errors) {
        Object.keys(errors).forEach(fieldName => {
            const field = this.container.find(`[name="${fieldName}"]`);
            if (field.length > 0) {
                this.validator.setFieldError(field[0], errors[fieldName]);
            }
        });
    }

    // Utility methods for notifications (can be overridden)
    async showSuccess(message) {
        if (window.Swal) {
            return await Swal.fire('Thành công!', message, 'success');
        } else {
            alert(message);
        }
    }

    async showError(message) {
        if (window.Swal) {
            return await Swal.fire('Lỗi!', message, 'error');
        } else {
            alert(message);
        }
    }

    async showConfirm(message) {
        if (window.Swal) {
            const result = await Swal.fire({
                title: 'Xác nhận',
                text: message,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Hủy'
            });
            return result.isConfirmed;
        } else {
            return confirm(message);
        }
    }

    // Event hooks (override in implementation)
    async onSubmit(formData) {
        console.log('📤 Custom submit handler:', formData);
    }

    async onSubmitSuccess(response) {
        console.log('✅ Custom success handler:', response);
    }

    async onSubmitError(error) {
        console.log('❌ Custom error handler:', error);
    }

    onReset() {
        console.log('🔄 Custom reset handler');
    }

    onCancel() {
        console.log('❌ Custom cancel handler');
        // Default: go back or close modal
        if (window.history.length > 1) {
            window.history.back();
        }
    }

    // Static method to initialize forms
    static init(selector = 'form[data-base-form]', options = {}) {
        $(selector).each(function() {
            if (!$(this).data('base-form')) {
                $(this).data('base-form', new BaseForm(this, options));
            }
        });
    }
}

// Auto-initialize on document ready
$(document).ready(function() {
    BaseForm.init();
});

// Export for use in other modules
window.BaseForm = BaseForm;
