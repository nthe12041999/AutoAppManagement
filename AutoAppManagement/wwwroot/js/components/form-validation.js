/**
 * Form Validation Functions
 * Simple jQuery-based form validation functions
 */

// Validation patterns
var validationPatterns = {
    email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
    phone: /^(\+84|84|0)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$/,
    phoneInternational: /^[\+]?[1-9][\d]{0,15}$/
};

// Validation messages
var validationMessages = {
    required: 'Trường này là bắt buộc',
    email: 'Vui lòng nhập địa chỉ email hợp lệ',
    phone: 'Vui lòng nhập số điện thoại hợp lệ',
    phoneVietnam: 'Số điện thoại không đúng định dạng Việt Nam',
    minLength: 'Tối thiểu {min} ký tự',
    maxLength: 'Tối đa {max} ký tự',
    min: 'Giá trị tối thiểu là {min}',
    max: 'Giá trị tối đa là {max}'
};

// Show field error
function showFieldError(field, message) {
    $(field).removeClass('is-valid').addClass('is-invalid');
    
    var feedback = $(field).siblings('.invalid-feedback');
    if (feedback.length === 0) {
        feedback = $('<div class="invalid-feedback"></div>');
        $(field).after(feedback);
    }
    feedback.text(message);
}

// Show field success
function showFieldSuccess(field) {
    $(field).removeClass('is-invalid').addClass('is-valid');
    $(field).siblings('.invalid-feedback').text('');
}

// Clear field validation
function clearFieldValidation(field) {
    $(field).removeClass('is-valid is-invalid');
    $(field).siblings('.invalid-feedback').text('');
}

// Validate required field
function validateRequired(field) {
    var value = $(field).val().trim();
    
    if ($(field).prop('required') && value === '') {
        showFieldError(field, validationMessages.required);
        return false;
    }
    
    return true;
}

// Validate email
function validateEmail(field) {
    var value = $(field).val().trim();
    
    if (value === '') return true; // Let required validation handle empty
    
    if (!validationPatterns.email.test(value)) {
        showFieldError(field, validationMessages.email);
        return false;
    }
    
    return true;
}

// Validate phone
function validatePhone(field) {
    var value = $(field).val().trim().replace(/[\s-]/g, '');
    
    if (value === '') return true; // Let required validation handle empty
    
    var pattern = $(field).data('phone-pattern') === 'vietnam' ? 
        validationPatterns.phone : validationPatterns.phoneInternational;
    
    if (!pattern.test(value)) {
        var message = $(field).data('phone-pattern') === 'vietnam' ? 
            validationMessages.phoneVietnam : validationMessages.phone;
        showFieldError(field, message);
        return false;
    }
    
    return true;
}

// Validate length
function validateLength(field) {
    var value = $(field).val().trim();
    var minLength = $(field).attr('minlength');
    var maxLength = $(field).attr('maxlength');
    
    if (minLength && value.length < parseInt(minLength)) {
        showFieldError(field, validationMessages.minLength.replace('{min}', minLength));
        return false;
    }
    
    if (maxLength && value.length > parseInt(maxLength)) {
        showFieldError(field, validationMessages.maxLength.replace('{max}', maxLength));
        return false;
    }
    
    return true;
}

// Validate number range
function validateNumberRange(field) {
    var value = parseFloat($(field).val());
    var min = $(field).attr('min');
    var max = $(field).attr('max');
    
    if (isNaN(value)) return true;
    
    if (min !== undefined && value < parseFloat(min)) {
        showFieldError(field, validationMessages.min.replace('{min}', min));
        return false;
    }
    
    if (max !== undefined && value > parseFloat(max)) {
        showFieldError(field, validationMessages.max.replace('{max}', max));
        return false;
    }
    
    return true;
}

// Validate single field
function validateField(field) {
    var isValid = true;
    
    // Clear previous validation
    clearFieldValidation(field);
    
    // Validate required
    if (!validateRequired(field)) {
        isValid = false;
    }
    
    // Validate email
    if (isValid && $(field).attr('type') === 'email') {
        if (!validateEmail(field)) {
            isValid = false;
        }
    }
    
    // Validate phone
    if (isValid && $(field).attr('type') === 'tel') {
        if (!validatePhone(field)) {
            isValid = false;
        }
    }
    
    // Validate length
    if (isValid && ($(field).attr('minlength') || $(field).attr('maxlength'))) {
        if (!validateLength(field)) {
            isValid = false;
        }
    }
    
    // Validate number range
    if (isValid && $(field).attr('type') === 'number') {
        if (!validateNumberRange(field)) {
            isValid = false;
        }
    }
    
    // Show success if valid
    if (isValid && $(field).val().trim() !== '') {
        showFieldSuccess(field);
    }
    
    return isValid;
}

// Validate entire form
function validateForm(formId) {
    var form = $('#' + formId);
    var isFormValid = true;
    
    // Validate all inputs
    form.find('input, select, textarea').each(function() {
        if (!validateField(this)) {
            isFormValid = false;
        }
    });
    
    // Add/remove was-validated class
    if (isFormValid) {
        form.removeClass('was-validated');
    } else {
        form.addClass('was-validated');
    }
    
    return isFormValid;
}

// Reset form validation
function resetFormValidation(formId) {
    var form = $('#' + formId);
    
    form.removeClass('was-validated');
    form.find('input, select, textarea').each(function() {
        clearFieldValidation(this);
    });
}

// Set custom error for specific field
function setFieldError(fieldId, message) {
    var field = $('#' + fieldId);
    showFieldError(field, message);
}

// Clear error for specific field
function clearFieldError(fieldId) {
    var field = $('#' + fieldId);
    clearFieldValidation(field);
}

// Initialize form validation
function initFormValidation(formId, options) {
    options = options || {};
    var form = $('#' + formId);
    
    if (form.length === 0) {
        console.warn('Form not found: ' + formId);
        return;
    }
    
    // Set novalidate attribute
    form.attr('novalidate', '');
    
    // Attach blur events
    if (options.validateOnBlur !== false) {
        form.find('input, select, textarea').on('blur', function() {
            validateField(this);
        });
    }
    
    // Attach input events for real-time validation
    if (options.validateOnInput === true) {
        form.find('input, select, textarea').on('input', function() {
            var $this = $(this);
            if ($this.hasClass('is-invalid') || $this.hasClass('is-valid')) {
                validateField(this);
            }
        });
    }
    
    console.log('Form validation initialized: ' + formId);
}

// Auto-initialize forms with data-validate attribute
$(document).ready(function() {
    // Initialize forms with data-validate
    $('form[data-validate]').each(function() {
        var formId = $(this).attr('id');
        if (formId) {
            var options = {};
            
            // Parse options from data attribute
            if ($(this).data('validate-options')) {
                try {
                    options = JSON.parse($(this).data('validate-options'));
                } catch (e) {
                    console.warn('Invalid validation options for form: ' + formId);
                }
            }
            
            initFormValidation(formId, options);
        }
    });
    
    console.log('✅ Form validation functions loaded');
});
