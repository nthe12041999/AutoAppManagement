/**
 * Form Validation Mixin
 * Provides comprehensive form validation with data attributes
 *
 * Usage:
 * <input type="email" data-rule="required|email" data-message="Email is required">
 * <input type="text" data-rule="required|min:3|max:50" data-regex="^[a-zA-Z\s]+$">
 */
class FormMixin {
    constructor(container, options = {}) {
        this.container = $(container);
        this.options = {
            validateOnSubmit: true,
            validateOnBlur: true,
            validateOnInput: false,
            showErrorMessages: true,
            errorClass: 'is-invalid',
            successClass: 'is-valid',
            errorMessageClass: 'invalid-feedback',
            submitButton: '[type="submit"]',
            resetButton: '[type="reset"]',
            apiUrl: null,
            method: 'POST',
            trackChanges: true,
            confirmBeforeSubmit: false,
            ...options
        };

        this.validators = new Map();
        this.errors = new Map();
        this.isSubmitting = false;
        this.originalData = {};

        this.init();
    }

    // Initialize form validation
    init() {
        console.log('🔧 FormMixin initialized');
        this.registerDefaultValidators();
        this.bindEvents();
        this.setupErrorContainers();
        if (this.options.trackChanges) {
            this.captureOriginalData();
        }
    }

    // Register default validation rules
    registerDefaultValidators() {
        // Required validation
        this.registerValidator('required', (value, params, element) => {
            if ($(element).is(':checkbox') || $(element).is(':radio')) {
                return $(element).is(':checked');
            }
            return value && value.trim().length > 0;
        }, 'Trường này là bắt buộc');

        // Email validation
        this.registerValidator('email', (value, params, element) => {
            if (!value) return true; // Skip if empty (use required for that)
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(value);
        }, 'Email không hợp lệ');

        // Min length validation
        this.registerValidator('min', (value, params, element) => {
            if (!value) return true;
            const minLength = parseInt(params[0]);
            return value.length >= minLength;
        }, 'Tối thiểu {0} ký tự');

        // Max length validation
        this.registerValidator('max', (value, params, element) => {
            if (!value) return true;
            const maxLength = parseInt(params[0]);
            return value.length <= maxLength;
        }, 'Tối đa {0} ký tự');

        // Numeric validation
        this.registerValidator('numeric', (value, params, element) => {
            if (!value) return true;
            return /^\d+$/.test(value);
        }, 'Chỉ được nhập số');

        // Alpha validation (letters only)
        this.registerValidator('alpha', (value, params, element) => {
            if (!value) return true;
            return /^[a-zA-ZÀ-ỹ\s]+$/.test(value);
        }, 'Chỉ được nhập chữ cái');

        // Phone validation
        this.registerValidator('phone', (value, params, element) => {
            if (!value) return true;
            const phoneRegex = /^(\+84|84|0)[3|5|7|8|9][0-9]{8}$/;
            return phoneRegex.test(value.replace(/\s/g, ''));
        }, 'Số điện thoại không hợp lệ');

        // Confirm field validation (password confirmation)
        this.registerValidator('confirm', (value, params, element) => {
            if (!value) return true;
            const targetField = params[0];
            const targetValue = this.container.find(`[name="${targetField}"]`).val();
            return value === targetValue;
        }, 'Xác nhận không khớp');
    }

    // Register custom validator
    registerValidator(name, validatorFn, defaultMessage) {
        this.validators.set(name, {
            validate: validatorFn,
            message: defaultMessage
        });
    }

    // Event binding
    bindEvents() {
        const self = this;

        // Form submission
        if (this.options.validateOnSubmit) {
            this.container.on('submit', function(e) {
                if (!self.validateForm()) {
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
                // Continue with form submission
                self.handleSubmit(e);
            });
        }

        // Field blur validation
        if (this.options.validateOnBlur) {
            this.container.on('blur', '[data-rule]', function() {
                self.validateField(this);
            });
        }

        // Field input validation (real-time)
        if (this.options.validateOnInput) {
            this.container.on('input', '[data-rule]', function() {
                // Debounce validation
                clearTimeout($(this).data('validation-timeout'));
                $(this).data('validation-timeout', setTimeout(() => {
                    self.validateField(this);
                }, 300));
            });
        }

        // Reset form
        this.container.on('click', this.options.resetButton, function(e) {
            e.preventDefault();
            self.resetForm();
        });

        // Clear field error on focus
        this.container.on('focus', '[data-rule]', function() {
            self.clearFieldError(this);
        });

        // Submit button click
        this.container.on('click', '[data-action="submit"]', function(e) {
            e.preventDefault();
            if (self.validateForm()) {
                self.handleSubmit(e);
            }
        });
        
        // Reset button
        this.form.on('click', '[data-action="reset"]', function(e) {
            e.preventDefault();
            self.reset();
        });
        
        // Cancel button
        this.form.on('click', '[data-action="cancel"]', function(e) {
            e.preventDefault();
            self.cancel();
        });
        
        // Field validation on blur
        if (this.options.autoValidate) {
            this.form.on('blur', '[data-validate]', function() {
                self.validateField($(this));
            });
        }
        
        // Real-time validation for specific fields
        this.form.on('input', '[data-validate="realtime"]', function() {
            self.validateField($(this));
        });
        
        // File upload handling
        this.form.on('change', '[data-type="file"]', function() {
            self.handleFileUpload($(this));
        });
        
        // Dynamic field addition/removal
        this.form.on('click', '[data-action="add-field"]', function(e) {
            e.preventDefault();
            self.addDynamicField($(this));
        });
        
        this.form.on('click', '[data-action="remove-field"]', function(e) {
            e.preventDefault();
            self.removeDynamicField($(this));
        });
        
        // Auto-save functionality
        if (this.options.autoSave) {
            this.form.on('input change', '[data-field]', function() {
                clearTimeout(self.autoSaveTimeout);
                self.autoSaveTimeout = setTimeout(() => {
                    self.autoSave();
                }, self.options.autoSaveDelay || 2000);
            });
        }
    }
    
    // Setup validation rules
    setupValidation() {
        this.form.find('[data-validate]').each((index, element) => {
            const $element = $(element);
            const field = $element.attr('data-field') || $element.attr('name');
            const rules = $element.attr('data-validate').split('|');
            
            this.validationRules[field] = rules;
        });
    }
    
    // Handle form submission
    async handleSubmit() {
        if (this.isSubmitting) return;
        
        // Validate form
        if (!this.validate()) {
            this.onValidationFailed();
            return;
        }
        
        // Confirm before submit
        if (this.options.confirmBeforeSubmit) {
            const confirmMessage = this.form.attr('data-confirm') || 'Bạn có chắc chắn muốn lưu thay đổi?';
            if (!confirm(confirmMessage)) {
                return;
            }
        }
        
        this.isSubmitting = true;
        this.showSubmitting();
        
        try {
            const formData = this.getFormData();
            const result = await this.submitForm(formData);
            
            if (result.success) {
                this.onSubmitSuccess(result);
                if (this.options.resetAfterSubmit) {
                    this.reset();
                }
            } else {
                this.onSubmitError(result);
            }
        } catch (error) {
            this.onSubmitError({ message: 'Có lỗi xảy ra khi gửi form' });
        } finally {
            this.isSubmitting = false;
            this.hideSubmitting();
        }
    }
    
    // Submit form to API
    async submitForm(formData) {
        if (!this.options.apiUrl) {
            // If no API URL, just return the data for manual handling
            return { success: true, data: formData };
        }
        
        const response = await $.ajax({
            url: this.options.apiUrl,
            type: this.options.method,
            data: formData,
            processData: false,
            contentType: false
        });
        
        return response;
    }
    
    // Get form data
    getFormData() {
        const formData = new FormData();
        
        this.form.find('[data-field]').each((index, element) => {
            const $element = $(element);
            const field = $element.attr('data-field') || $element.attr('name');
            const type = $element.attr('data-type') || $element.attr('type');
            
            if (type === 'checkbox') {
                formData.append(field, $element.prop('checked'));
            } else if (type === 'radio') {
                if ($element.prop('checked')) {
                    formData.append(field, $element.val());
                }
            } else if (type === 'file') {
                const files = $element[0].files;
                for (let i = 0; i < files.length; i++) {
                    formData.append(field, files[i]);
                }
            } else {
                formData.append(field, $element.val());
            }
        });
        
        return formData;
    }
    
    // Get form data as object
    getFormDataAsObject() {
        const data = {};
        
        this.form.find('[data-field]').each((index, element) => {
            const $element = $(element);
            const field = $element.attr('data-field') || $element.attr('name');
            const type = $element.attr('data-type') || $element.attr('type');
            
            if (type === 'checkbox') {
                data[field] = $element.prop('checked');
            } else if (type === 'radio') {
                if ($element.prop('checked')) {
                    data[field] = $element.val();
                }
            } else if (type === 'file') {
                data[field] = $element[0].files;
            } else {
                data[field] = $element.val();
            }
        });
        
        return data;
    }
    
    // Set form data
    setFormData(data) {
        Object.keys(data).forEach(field => {
            const $element = this.form.find(`[data-field="${field}"]`);
            if ($element.length) {
                const type = $element.attr('data-type') || $element.attr('type');
                
                if (type === 'checkbox') {
                    $element.prop('checked', data[field]);
                } else if (type === 'radio') {
                    $element.filter(`[value="${data[field]}"]`).prop('checked', true);
                } else {
                    $element.val(data[field]);
                }
            }
        });
        
        if (this.options.trackChanges) {
            this.captureOriginalData();
        }
    }
    
    // Validate entire form
    validate() {
        let isValid = true;
        const errors = [];
        
        this.form.find('[data-validate]').each((index, element) => {
            const fieldValid = this.validateField($(element));
            if (!fieldValid) {
                isValid = false;
            }
        });
        
        if (this.options.showValidationSummary) {
            this.showValidationSummary(errors);
        }
        
        return isValid;
    }
    
    // Validate single field
    validateField($element) {
        const field = $element.attr('data-field') || $element.attr('name');
        const value = $element.val();
        const rules = this.validationRules[field] || [];
        const errors = [];
        
        rules.forEach(rule => {
            const error = this.validateRule(value, rule, $element);
            if (error) {
                errors.push(error);
            }
        });
        
        // Update field UI
        if (errors.length > 0) {
            $element.addClass('is-invalid').removeClass('is-valid');
            this.showFieldError($element, errors[0]);
            return false;
        } else {
            $element.addClass('is-valid').removeClass('is-invalid');
            this.hideFieldError($element);
            return true;
        }
    }
    
    // Validate individual rule
    validateRule(value, rule, $element) {
        const [ruleName, ruleValue] = rule.split(':');
        
        switch (ruleName) {
            case 'required':
                if (!value || value.trim() === '') {
                    return 'Trường này là bắt buộc';
                }
                break;
            case 'email':
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (value && !emailRegex.test(value)) {
                    return 'Email không hợp lệ';
                }
                break;
            case 'min':
                if (value && value.length < parseInt(ruleValue)) {
                    return `Tối thiểu ${ruleValue} ký tự`;
                }
                break;
            case 'max':
                if (value && value.length > parseInt(ruleValue)) {
                    return `Tối đa ${ruleValue} ký tự`;
                }
                break;
            case 'numeric':
                if (value && isNaN(value)) {
                    return 'Phải là số';
                }
                break;
            case 'phone':
                const phoneRegex = /^[0-9+\-\s()]+$/;
                if (value && !phoneRegex.test(value)) {
                    return 'Số điện thoại không hợp lệ';
                }
                break;
            case 'url':
                try {
                    if (value) new URL(value);
                } catch {
                    return 'URL không hợp lệ';
                }
                break;
        }
        
        return null;
    }
    
    // Show field error
    showFieldError($element, message) {
        const errorElement = $element.siblings('.invalid-feedback');
        if (errorElement.length) {
            errorElement.text(message);
        } else {
            $element.after(`<div class="invalid-feedback">${message}</div>`);
        }
    }
    
    // Hide field error
    hideFieldError($element) {
        $element.siblings('.invalid-feedback').remove();
    }
    
    // Show validation summary
    showValidationSummary(errors) {
        const summaryElement = this.form.find('[data-element="validation-summary"]');
        if (summaryElement.length && errors.length > 0) {
            const errorList = errors.map(error => `<li>${error}</li>`).join('');
            summaryElement.html(`<ul class="mb-0">${errorList}</ul>`).show();
        } else if (summaryElement.length) {
            summaryElement.hide();
        }
    }
    
    // Show submitting state
    showSubmitting() {
        const submitButton = this.form.find('[data-action="submit"]');
        submitButton.prop('disabled', true);
        
        const originalText = submitButton.text();
        submitButton.attr('data-original-text', originalText);
        submitButton.html('<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...');
        
        this.form.attr('data-submitting', 'true');
    }
    
    // Hide submitting state
    hideSubmitting() {
        const submitButton = this.form.find('[data-action="submit"]');
        submitButton.prop('disabled', false);
        
        const originalText = submitButton.attr('data-original-text');
        if (originalText) {
            submitButton.text(originalText);
        }
        
        this.form.attr('data-submitting', 'false');
    }
    
    // Reset form
    reset() {
        this.form[0].reset();
        this.form.find('.is-valid, .is-invalid').removeClass('is-valid is-invalid');
        this.form.find('.invalid-feedback').remove();
        
        if (this.options.trackChanges) {
            this.captureOriginalData();
        }
        
        this.onReset();
    }
    
    // Cancel form (restore original data)
    cancel() {
        if (this.options.trackChanges && this.hasChanges()) {
            if (confirm('Bạn có thay đổi chưa lưu. Bạn có chắc chắn muốn hủy?')) {
                this.setFormData(this.originalData);
                this.onCancel();
            }
        } else {
            this.onCancel();
        }
    }
    
    // Capture original data for change tracking
    captureOriginalData() {
        this.originalData = this.getFormDataAsObject();
    }
    
    // Check if form has changes
    hasChanges() {
        const currentData = this.getFormDataAsObject();
        return JSON.stringify(currentData) !== JSON.stringify(this.originalData);
    }
    
    // Handle file upload
    handleFileUpload($element) {
        const files = $element[0].files;
        const maxSize = parseInt($element.attr('data-max-size')) || 5 * 1024 * 1024; // 5MB default
        const allowedTypes = $element.attr('data-allowed-types')?.split(',') || [];
        
        for (let file of files) {
            if (file.size > maxSize) {
                alert(`File ${file.name} quá lớn. Kích thước tối đa: ${maxSize / 1024 / 1024}MB`);
                $element.val('');
                return;
            }
            
            if (allowedTypes.length > 0 && !allowedTypes.includes(file.type)) {
                alert(`File ${file.name} không đúng định dạng. Cho phép: ${allowedTypes.join(', ')}`);
                $element.val('');
                return;
            }
        }
        
        this.onFileSelected(files);
    }
    
    // Auto-save functionality
    autoSave() {
        if (this.hasChanges()) {
            const data = this.getFormDataAsObject();
            this.onAutoSave(data);
        }
    }
    
    // Event hooks (override in implementation)
    onSubmitSuccess(result) {
        console.log('Form submitted successfully:', result);
    }
    
    onSubmitError(error) {
        console.log('Form submission error:', error);
    }
    
    onValidationFailed() {
        console.log('Form validation failed');
    }
    
    onReset() {
        console.log('Form reset');
    }
    
    onCancel() {
        console.log('Form cancelled');
    }
    
    onFileSelected(files) {
        console.log('Files selected:', files);
    }
    
    onAutoSave(data) {
        console.log('Auto-saving:', data);
    }
}

// Export for use
window.FormMixin = FormMixin;
