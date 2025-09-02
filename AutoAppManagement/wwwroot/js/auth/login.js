/**
 * Login Page JavaScript
 * Handles login form submission using fetch-api.js
 */

$(document).ready(function() {
    console.log('Login.js loaded successfully!');
    
    // Check if required dependencies are loaded
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded!');
        return;
    }
    
    if (typeof callPostAPIPublic === 'undefined') {
        console.error('fetch-api.js is not loaded or callPostAPIPublic function not found!');
        return;
    }
    
    console.log('All dependencies loaded. Initializing login form...');
    
    // Initialize login page
    initializeLoginForm();
});

/**
 * Initialize login form and event handlers
 */
function initializeLoginForm() {
    const $loginForm = $('#loginForm');
    const $submitBtn = $loginForm.find('button[type="submit"]');
    
    // Form submission handler
    $loginForm.on('submit', function(e) {
        e.preventDefault();
        handleLogin();
    });
    
    // Button click handler
    $submitBtn.on('click', function(e) {
        e.preventDefault();
        handleLogin();
    });
    
    // Enter key on password field
    $('#password').on('keypress', function(e) {
        if (e.which === 13) { // Enter key
            handleLogin();
        }
    });
    
    // Real-time validation
    $('#userName, #password').on('input', function() {
        clearFieldError($(this));
    });
    
    console.log('Login form initialized successfully!');
}

/**
 * Handle login form submission
 */
function handleLogin() {
    console.log('handleLogin() called');
    
    const $form = $('#loginForm');
    const $submitBtn = $form.find('button[type="submit"]');
    
    // Get form data
    const formData = {
        userName: $('#userName').val().trim(),
        password: $('#password').val().trim(),
        rememberMe: $('#rememberMe').is(':checked')
    };
    
    console.log('Form data:', formData);
    
    // Validate form
    if (!validateLoginForm(formData)) {
        console.log('Form validation failed');
        return;
    }
    
    console.log('Form validation passed, calling API...');
    
    // Show loading state
    showLoading(true);
    setButtonLoading($submitBtn, true);
    hideError();
    
    // Call login API using fetch-api.js
    callPostAPIPublic('/Auth/Login', formData,
        function(response) {
            console.log('Login API success response:', response);
            // Success callback
            handleLoginSuccess(response);
        },
        function(error) {
            console.log('Login API error response:', error);
            // Error callback
            handleLoginError(error);
        }
    );
}

/**
 * Validate login form data
 * @param {Object} formData - Form data object
 * @returns {boolean} - True if valid, false otherwise
 */
function validateLoginForm(formData) {
    let isValid = true;
    
    // Clear previous validation
    clearAllErrors();
    
    // Validate username
    if (!formData.userName) {
        showFieldError('#userName', 'Vui lòng nhập tên đăng nhập');
        isValid = false;
    } else if (formData.userName.length < 3) {
        showFieldError('#userName', 'Tên đăng nhập phải có ít nhất 3 ký tự');
        isValid = false;
    }
    
    // Validate password
    if (!formData.password) {
        showFieldError('#password', 'Vui lòng nhập mật khẩu');
        isValid = false;
    } else if (formData.password.length < 6) {
        showFieldError('#password', 'Mật khẩu phải có ít nhất 6 ký tự');
        isValid = false;
    }
    
    return isValid;
}

/**
 * Handle successful login response
 * @param {Object} response - API response
 */
function handleLoginSuccess(response) {
    console.log('handleLoginSuccess response:', response);
    
    if (response && response.isSuccess) {
        // lưu token vào cookie
        
        // Redirect after short delay
        setTimeout(() => {
            if (response.data && response.data.redirectUrl) {
                window.location.href = response.data.redirectUrl;
            } else {
                // Default redirect
                window.location.href = '/Home/Index';
            }
        }, 1500);
        
    } else {
        // API returned isSuccess=false
        const errorMessage = response.message || 'Đăng nhập không thành công';
        handleLoginError({ message: errorMessage });
    }
}

/**
 * Handle login error response
 * @param {Object} error - Error object
 */
function handleLoginError(error) {
    showLoading(false);
    setButtonLoading($('#loginForm button[type="submit"]'), false);
    
    let errorMessage = 'Có lỗi xảy ra trong quá trình đăng nhập';
    
    if (error && error.message) {
        errorMessage = error.message;
    } else if (error && typeof error === 'string') {
        errorMessage = error;
    }
    
    showError(errorMessage);
    
    // Focus back to username field
    $('#userName').focus();
}

/**
 * Show/hide loading spinner
 * @param {boolean} show - True to show, false to hide
 */
function showLoading(show) {
    const $form = $('#loginForm');
    const $spinner = $('#loadingSpinner');
    
    if (show) {
        $form.addClass('d-none');
        $spinner.removeClass('d-none');
    } else {
        $form.removeClass('d-none');
        $spinner.addClass('d-none');
    }
}

/**
 * Set button loading state
 * @param {jQuery} $button - Button element
 * @param {boolean} loading - True for loading, false for normal
 */
function setButtonLoading($button, loading) {
    if (loading) {
        $button.prop('disabled', true);
        $button.html('<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...');
    } else {
        $button.prop('disabled', false);
        $button.html('<i class="bi bi-box-arrow-in-right me-2"></i>Đăng nhập');
    }
}

/**
 * Show error message
 * @param {string} message - Error message
 */
function showError(message) {
    const $errorAlert = $('#errorAlert');
    const $errorMessage = $('#errorMessage');
    
    $errorMessage.text(message);
    $errorAlert.removeClass('d-none');
    
    // Auto hide after 5 seconds
    setTimeout(() => {
        hideError();
    }, 5000);
}

/**
 * Hide error message
 */
function hideError() {
    $('#errorAlert').addClass('d-none');
}

/**
 * Show success message
 * @param {string} message - Success message
 */
function showSuccess(message) {
    // Remove any existing success alert
    $('.alert-success').remove();
    
    const successHtml = `
        <div class="alert alert-success" role="alert">
            <i class="bi bi-check-circle me-2"></i>
            ${message}
        </div>
    `;
    
    $('#errorAlert').after(successHtml);
}

/**
 * Show field-specific error
 * @param {string} fieldSelector - CSS selector for field
 * @param {string} message - Error message
 */
function showFieldError(fieldSelector, message) {
    const $field = $(fieldSelector);
    $field.addClass('is-invalid');
    
    // Remove existing error message
    $field.siblings('.invalid-feedback').remove();
    
    // Add error message
    $field.after(`<div class="invalid-feedback">${message}</div>`);
}

/**
 * Clear field error
 * @param {jQuery} $field - Field element
 */
function clearFieldError($field) {
    $field.removeClass('is-invalid is-valid');
    $field.siblings('.invalid-feedback').remove();
}

/**
 * Clear all form errors
 */
function clearAllErrors() {
    $('#loginForm .form-control').removeClass('is-invalid is-valid');
    $('#loginForm .invalid-feedback').remove();
    hideError();
}

/**
 * Show demo credentials (for development)
 */
function showDemoCredentials() {
    if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
        const demoText = `
            <div class="alert alert-info mt-3">
                <strong>Demo Credentials:</strong><br>
                • admin / admin123<br>
                • superadmin / super123<br>
                • customer1 / 123456
            </div>
        `;
        $('.login-card .card-body').append(demoText);
    }
}

// Show demo credentials on page load (only in development)
$(document).ready(function() {
    showDemoCredentials();
});
