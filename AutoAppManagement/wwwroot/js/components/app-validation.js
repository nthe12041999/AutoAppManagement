/**
 * Application Validation Utilities
 * Common validation patterns and helper functions
 */

// Validation patterns
var AppValidation = {
    patterns: {
        email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
        phone: {
            vietnam: /^(\+84|84|0)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$/,
            international: /^[\+]?[1-9][\d]{0,15}$/
        },
        password: {
            weak: /^.{6,}$/,
            medium: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/,
            strong: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/
        },
        username: /^[a-zA-Z0-9_]{3,20}$/,
        vietnameseId: /^\d{12}$/
    },
    
    // Validation messages
    messages: {
        required: 'Trường này là bắt buộc',
        email: 'Vui lòng nhập địa chỉ email hợp lệ',
        phone: 'Vui lòng nhập số điện thoại hợp lệ',
        phoneVietnam: 'Số điện thoại không đúng định dạng Việt Nam',
        password: 'Mật khẩu phải có ít nhất 6 ký tự',
        passwordWeak: 'Mật khẩu quá yếu',
        passwordMismatch: 'Mật khẩu xác nhận không khớp',
        username: 'Tên đăng nhập chỉ được chứa chữ, số và dấu gạch dưới (3-20 ký tự)',
        minLength: 'Tối thiểu {min} ký tự',
        maxLength: 'Tối đa {max} ký tự',
        min: 'Giá trị tối thiểu là {min}',
        max: 'Giá trị tối đa là {max}',
        age: 'Tuổi phải từ {min} đến {max}',
        dateRange: 'Ngày kết thúc phải sau ngày bắt đầu',
        fileType: 'Loại file không được hỗ trợ',
        fileSize: 'Kích thước file vượt quá {max}MB',
        vietnameseId: 'Số CMND/CCCD không hợp lệ'
    }
};

// Helper functions
function formatValidationMessage(template, params) {
    return template.replace(/\{(\w+)\}/g, function(match, key) {
        return params[key] || match;
    });
}

// Validate Vietnamese ID
function validateVietnameseId(id) {
    return AppValidation.patterns.vietnameseId.test(id);
}

// Validate age from birth date
function validateAge(birthDate, minAge, maxAge) {
    minAge = minAge || 18;
    maxAge = maxAge || 100;
    
    if (!birthDate) return false;
    
    var today = new Date();
    var birth = new Date(birthDate);
    var age = today.getFullYear() - birth.getFullYear();
    var monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
        age--;
    }
    
    return age >= minAge && age <= maxAge;
}

// Validate date range
function validateDateRange(startDate, endDate) {
    if (!startDate || !endDate) return false;
    return new Date(startDate) <= new Date(endDate);
}

// Validate file type
function validateFileType(file, allowedTypes) {
    allowedTypes = allowedTypes || [];
    if (!file || allowedTypes.length === 0) return true;
    
    var fileType = file.type || '';
    var fileName = file.name || '';
    var fileExtension = fileName.split('.').pop().toLowerCase();
    
    return allowedTypes.some(function(type) {
        return fileType.includes(type) || fileExtension === type.replace('.', '');
    });
}

// Validate file size
function validateFileSize(file, maxSizeMB) {
    maxSizeMB = maxSizeMB || 5;
    if (!file) return true;
    
    var maxSizeBytes = maxSizeMB * 1024 * 1024;
    return file.size <= maxSizeBytes;
}

// Check password strength
function checkPasswordStrength(password) {
    if (!password) return { score: 0, level: 'none' };
    
    var score = 0;
    
    if (password.length >= 8) score++;
    if (password.length >= 12) score++;
    if (/[a-z]/.test(password)) score++;
    if (/[A-Z]/.test(password)) score++;
    if (/[0-9]/.test(password)) score++;
    if (/[^A-Za-z0-9]/.test(password)) score++;
    
    var levels = ['none', 'very-weak', 'weak', 'fair', 'good', 'strong'];
    return {
        score: score,
        level: levels[Math.min(score, levels.length - 1)]
    };
}

// Common validation functions
function isValidEmail(email) {
    return AppValidation.patterns.email.test(email);
}

function isValidVietnamesePhone(phone) {
    return AppValidation.patterns.phone.vietnam.test(phone.replace(/[\s-]/g, ''));
}

function isValidInternationalPhone(phone) {
    return AppValidation.patterns.phone.international.test(phone.replace(/[\s-]/g, ''));
}

function isValidUsername(username) {
    return AppValidation.patterns.username.test(username);
}

// Auto-validate common patterns on document ready
$(document).ready(function() {
    console.log('✅ App validation utilities loaded');
    
    // You can add global validation behaviors here if needed
    // For example, auto-format phone numbers, etc.
});

// Ext JS bridge (optional): expose as App.util.Validation for Ext usage
(function setupExtValidationBridge(){
    if (!window.Ext || !Ext.define) return;
    if (!window.App) window.App = {}; if (!App.util) App.util = {};
    Ext.define('App.util.Validation', {
        singleton: true,
        patterns: AppValidation.patterns,
        messages: AppValidation.messages,
        format: formatValidationMessage,
        isValidEmail: isValidEmail,
        isValidVietnamesePhone: isValidVietnamesePhone,
        isValidInternationalPhone: isValidInternationalPhone,
        isValidUsername: isValidUsername,
        validateVietnameseId: validateVietnameseId,
        validateAge: validateAge,
        validateDateRange: validateDateRange,
        validateFileType: validateFileType,
        validateFileSize: validateFileSize,
        checkPasswordStrength: checkPasswordStrength
    });
})();