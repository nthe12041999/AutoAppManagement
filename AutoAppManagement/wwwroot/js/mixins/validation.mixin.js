/**
 * Validation Mixin
 * Comprehensive form validation engine with data attributes
 * 
 * Usage:
 * <form data-validate>
 *   <input type="email" data-rule="required|email" data-message="Email is required">
 *   <input type="text" data-rule="required|min:3|max:50" data-regex="^[a-zA-Z\s]+$">
 * </form>
 * 
 * const validator = new ValidationMixin('form[data-validate]');
 */

class ValidationMixin {
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
            ...options
        };
        
        this.validators = new Map();
        this.errors = new Map();
        
        this.init();
    }

    // Initialize validation
    init() {
        console.log('🔧 ValidationMixin initialized');
        this.registerDefaultValidators();
        this.bindEvents();
        this.setupErrorContainers();
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
            if (!value) return true;
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

        // Alpha numeric validation
        this.registerValidator('alphanumeric', (value, params, element) => {
            if (!value) return true;
            return /^[a-zA-Z0-9À-ỹ\s]+$/.test(value);
        }, 'Chỉ được nhập chữ cái và số');

        // Phone validation
        this.registerValidator('phone', (value, params, element) => {
            if (!value) return true;
            const phoneRegex = /^(\+84|84|0)[3|5|7|8|9][0-9]{8}$/;
            return phoneRegex.test(value.replace(/\s/g, ''));
        }, 'Số điện thoại không hợp lệ');

        // URL validation
        this.registerValidator('url', (value, params, element) => {
            if (!value) return true;
            try {
                new URL(value);
                return true;
            } catch {
                return false;
            }
        }, 'URL không hợp lệ');

        // Date validation
        this.registerValidator('date', (value, params, element) => {
            if (!value) return true;
            const date = new Date(value);
            return date instanceof Date && !isNaN(date);
        }, 'Ngày không hợp lệ');

        // Min value validation
        this.registerValidator('minvalue', (value, params, element) => {
            if (!value) return true;
            const numValue = parseFloat(value);
            const minValue = parseFloat(params[0]);
            return numValue >= minValue;
        }, 'Giá trị tối thiểu là {0}');

        // Max value validation
        this.registerValidator('maxvalue', (value, params, element) => {
            if (!value) return true;
            const numValue = parseFloat(value);
            const maxValue = parseFloat(params[0]);
            return numValue <= maxValue;
        }, 'Giá trị tối đa là {0}');

        // Confirm field validation (password confirmation)
        this.registerValidator('confirm', (value, params, element) => {
            if (!value) return true;
            const targetField = params[0];
            const targetValue = this.container.find(`[name="${targetField}"]`).val();
            return value === targetValue;
        }, 'Xác nhận không khớp');

        // File extension validation
        this.registerValidator('extensions', (value, params, element) => {
            if (!value) return true;
            const allowedExtensions = params[0].split(',').map(ext => ext.trim().toLowerCase());
            const fileExtension = value.split('.').pop().toLowerCase();
            return allowedExtensions.includes(fileExtension);
        }, 'Định dạng file không được phép');

        // File size validation (in KB)
        this.registerValidator('maxsize', (value, params, element) => {
            if (!element.files || !element.files[0]) return true;
            const maxSizeKB = parseInt(params[0]);
            const fileSizeKB = element.files[0].size / 1024;
            return fileSizeKB <= maxSizeKB;
        }, 'Kích thước file tối đa {0}KB');
    }

    // Register custom validator
    registerValidator(name, validatorFn, defaultMessage) {
        this.validators.set(name, {
            validate: validatorFn,
            message: defaultMessage
        });
    }

    // Bind validation events
    bindEvents() {
        const self = this;

        // Form submit validation
        if (this.options.validateOnSubmit) {
            this.container.on('submit', function(e) {
                if (!self.validateForm()) {
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
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
            self.resetValidation();
        });

        // Clear field error on focus
        this.container.on('focus', '[data-rule]', function() {
            self.clearFieldError(this);
        });
    }

    // Setup error message containers
    setupErrorContainers() {
        this.container.find('[data-rule]').each((index, element) => {
            const $element = $(element);
            const fieldName = $element.attr('name') || $element.attr('id') || `field_${index}`;
            
            // Create error container if not exists
            if ($element.siblings(`.${this.options.errorMessageClass}`).length === 0) {
                $element.after(`<div class="${this.options.errorMessageClass}" data-field="${fieldName}"></div>`);
            }
        });
    }

    // Validate entire form
    validateForm() {
        console.log('🔍 Validating form...');
        this.errors.clear();
        let isValid = true;

        this.container.find('[data-rule]').each((index, element) => {
            if (!this.validateField(element)) {
                isValid = false;
            }
        });

        console.log('📋 Form validation result:', isValid ? 'VALID' : 'INVALID');
        console.log('❌ Errors:', Array.from(this.errors.entries()));

        return isValid;
    }

    // Validate single field
    validateField(element) {
        const $element = $(element);
        const value = $element.val() || '';
        const rules = $element.attr('data-rule') || '';
        const customRegex = $element.attr('data-regex');
        const customMessage = $element.attr('data-message');
        const fieldName = $element.attr('name') || $element.attr('id') || 'field';

        // Clear previous errors for this field
        this.clearFieldError(element);

        // Validate rules
        if (rules) {
            const ruleList = rules.split('|');
            for (const rule of ruleList) {
                const [ruleName, ...ruleParams] = rule.split(':');
                const params = ruleParams.length > 0 ? ruleParams[0].split(',') : [];

                if (this.validators.has(ruleName)) {
                    const validator = this.validators.get(ruleName);
                    if (!validator.validate(value, params, element)) {
                        const errorMessage = customMessage || this.formatMessage(validator.message, params);
                        this.setFieldError(element, errorMessage);
                        return false;
                    }
                }
            }
        }

        // Validate custom regex
        if (customRegex && value) {
            try {
                const regex = new RegExp(customRegex);
                if (!regex.test(value)) {
                    const errorMessage = customMessage || 'Định dạng không hợp lệ';
                    this.setFieldError(element, errorMessage);
                    return false;
                }
            } catch (e) {
                console.warn('Invalid regex pattern:', customRegex);
            }
        }

        // Mark field as valid
        this.setFieldValid(element);
        return true;
    }

    // Format error message with parameters
    formatMessage(message, params) {
        return message.replace(/\{(\d+)\}/g, (match, index) => {
            return params[index] || match;
        });
    }

    // Set field error
    setFieldError(element, message) {
        const $element = $(element);
        const fieldName = $element.attr('name') || $element.attr('id') || 'field';
        
        // Store error
        this.errors.set(fieldName, message);
        
        // Add error classes
        $element.removeClass(this.options.successClass).addClass(this.options.errorClass);
        
        // Show error message
        if (this.options.showErrorMessages) {
            const $errorContainer = $element.siblings(`.${this.options.errorMessageClass}`);
            if ($errorContainer.length > 0) {
                $errorContainer.text(message).show();
            }
        }
    }

    // Set field valid
    setFieldValid(element) {
        const $element = $(element);
        const fieldName = $element.attr('name') || $element.attr('id') || 'field';
        
        // Remove error
        this.errors.delete(fieldName);
        
        // Add success classes
        $element.removeClass(this.options.errorClass).addClass(this.options.successClass);
        
        // Hide error message
        const $errorContainer = $element.siblings(`.${this.options.errorMessageClass}`);
        if ($errorContainer.length > 0) {
            $errorContainer.text('').hide();
        }
    }

    // Clear field error
    clearFieldError(element) {
        const $element = $(element);
        const fieldName = $element.attr('name') || $element.attr('id') || 'field';
        
        // Remove error
        this.errors.delete(fieldName);
        
        // Remove classes
        $element.removeClass(`${this.options.errorClass} ${this.options.successClass}`);
        
        // Hide error message
        const $errorContainer = $element.siblings(`.${this.options.errorMessageClass}`);
        if ($errorContainer.length > 0) {
            $errorContainer.text('').hide();
        }
    }

    // Reset validation
    resetValidation() {
        console.log('🔄 Resetting validation...');
        
        // Clear all errors
        this.errors.clear();
        
        // Reset all fields
        this.container.find('[data-rule]').each((index, element) => {
            this.clearFieldError(element);
        });
    }

    // Get validation errors
    getErrors() {
        return Object.fromEntries(this.errors);
    }

    // Check if form is valid
    isValid() {
        return this.errors.size === 0;
    }

    // Static method to initialize validation
    static init(selector = 'form[data-validate]', options = {}) {
        $(selector).each(function() {
            if (!$(this).data('validation-mixin')) {
                $(this).data('validation-mixin', new ValidationMixin(this, options));
            }
        });
    }
}

// Auto-initialize on document ready
$(document).ready(function() {
    ValidationMixin.init();
});

// Export for use in other modules
window.ValidationMixin = ValidationMixin;

/*
VALIDATION RULES DOCUMENTATION:

Basic Rules:
- required: Field must have a value
- email: Must be valid email format
- min:n: Minimum n characters
- max:n: Maximum n characters
- numeric: Only numbers allowed
- alpha: Only letters allowed
- alphanumeric: Letters and numbers only
- phone: Vietnamese phone number format
- url: Valid URL format
- date: Valid date format

Advanced Rules:
- minvalue:n: Minimum numeric value
- maxvalue:n: Maximum numeric value
- confirm:field: Must match another field
- extensions:ext1,ext2: File extensions allowed
- maxsize:kb: Maximum file size in KB

Usage Examples:
<input data-rule="required|min:3|max:50">
<input data-rule="required|email">
<input data-rule="required|phone">
<input data-rule="required|confirm:password">
<input data-rule="extensions:jpg,png|maxsize:1024">

Custom Regex:
<input data-regex="^[A-Z][a-z]+$" data-message="Must start with capital letter">

Multiple Rules:
<input data-rule="required|min:8|max:20" data-regex="^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$">

Custom Messages:
<input data-rule="required|email" data-message="Please enter a valid email address">
*/
