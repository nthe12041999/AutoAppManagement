/**
 * User Profile Dropdown JavaScript
 * Handles user profile dropdown interactions and logout functionality
 */

$(document).ready(function() {
    console.log('User profile script loaded!');
    initializeUserProfile();
    
    // Check if elements exist
    console.log('User avatar found:', $('.user-avatar').length);
    console.log('User dropdown found:', $('.user-dropdown').length);
    
    // Only load if we have the API functions available
    if (typeof callGetAPIAuthen !== 'undefined') {
        loadUserInfo();
    } else {
        console.warn('fetch-api.js not loaded, cannot load user info');
    }
});

/**
 * Initialize user profile dropdown
 */
function initializeUserProfile() {
    console.log('Initializing user profile dropdown...');
    
    // Handle avatar click to toggle dropdown
    $('.user-avatar').on('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        console.log('Avatar clicked, toggling dropdown');
        toggleUserDropdown();
    });
    
    // Handle logout click
    $('#logoutBtn').on('click', function(e) {
        e.preventDefault();
        console.log('Logout button clicked');
        handleLogout();
    });
    
    // Close dropdown when clicking outside
    $(document).on('click', function(e) {
        if (!$(e.target).closest('.user-profile').length) {
            hideUserDropdown();
        }
    });
    
    console.log('User profile dropdown initialized');
}

/**
 * Toggle user dropdown visibility
 */
function toggleUserDropdown() {
    const $dropdown = $('.user-dropdown');
    console.log('Toggling dropdown. Current state:', $dropdown.hasClass('show'));
    
    if ($dropdown.hasClass('show')) {
        hideUserDropdown();
    } else {
        showUserDropdown();
    }
}

/**
 * Show user dropdown
 */
function showUserDropdown() {
    console.log('Showing user dropdown');
    $('.user-dropdown').addClass('show');
}

/**
 * Hide user dropdown
 */
function hideUserDropdown() {
    console.log('Hiding user dropdown');
    $('.user-dropdown').removeClass('show');
}

/**
 * Handle user logout
 */
function handleLogout() {
    console.log('Logout initiated...');
    
    // Show confirmation dialog
    if (confirm('Bạn có chắc chắn muốn đăng xuất không?')) {
        console.log('User confirmed logout');
        
        // Show loading state
        showLogoutLoading(true);
        
        // Check if API function exists
        if (typeof callPostAPIAuthen === 'undefined') {
            console.error('callPostAPIAuthen function not found! Using fallback...');
            // Fallback: clear cache and redirect directly
            clearUserCache();
            setTimeout(() => {
                redirectToLogin();
            }, 1000);
            return;
        }
        
        console.log('Calling logout API...');
        
        // Set timeout for API call
        const timeoutId = setTimeout(() => {
            console.warn('Logout API timeout, proceeding with client-side logout');
            clearUserCache();
            redirectToLogin();
        }, 5000); // 5 second timeout
        
        // Call logout API
        callPostAPIAuthen('/Auth/Logout', {},
            function(response) {
                clearTimeout(timeoutId);
                console.log('Logout API success:', response);
                handleLogoutSuccess(response);
            },
            function(error) {
                clearTimeout(timeoutId);
                console.log('Logout API error:', error);
                // Even if API fails, still clear cache and logout
                handleLogoutError(error);
            }
        );
    } else {
        console.log('User cancelled logout');
    }
}

/**
 * Handle successful logout response
 */
function handleLogoutSuccess(response) {
    console.log('Logout success, cleaning up...');
    
    // Clear user cache
    clearUserCache();
    
    // Show success message
    showNotification('Đăng xuất thành công!', 'success');
    
    // Redirect to login page
    setTimeout(() => {
        redirectToLogin();
    }, 1000);
}

/**
 * Handle logout error response
 */
function handleLogoutError(error) {
    console.log('Handling logout error:', error);
    
    showLogoutLoading(false);
    
    let errorMessage = 'Có lỗi xảy ra khi đăng xuất';
    
    if (error && error.message) {
        errorMessage = error.message;
    }
    
    console.warn('Logout error:', errorMessage);
    showNotification(errorMessage, 'warning');
    
    // Even if logout API fails, clear local data and redirect after short delay
    console.log('Proceeding with client-side logout despite API error...');
    setTimeout(() => {
        clearUserCache();
        redirectToLogin();
    }, 1500);
}

/**
 * Clear all user cache and stored data
 */
function clearUserCache() {
    console.log('Clearing user cache and stored data...');
    
    // Clear ASP.NET Core authentication cookies
    const cookiesToClear = [
        'AutoAppManagement.Auth',  // Cookie name từ ServiceCollectionExtensions.cs
        '.AspNetCore.Cookies',
        '.AspNetCore.Session',
        'access_token',
        'refresh_token',
        'auth_token',
        'user_session'
    ];
    
    cookiesToClear.forEach(cookieName => {
        // Clear with different path and domain combinations
        document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; domain=${window.location.hostname}`;
        document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/`;
        document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; domain=.${window.location.hostname}`;
        // Also try without domain
        document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; secure=false`;
    });
    
    // Clear localStorage
    const localStorageKeys = [
        'access_token',
        'refresh_token',
        'user_info',
        'user_session',
        'auth_data',
        'login_data'
    ];
    
    localStorageKeys.forEach(key => {
        localStorage.removeItem(key);
    });
    
    // Clear sessionStorage
    const sessionStorageKeys = [
        'access_token',
        'refresh_token',
        'user_info',
        'temp_data'
    ];
    
    sessionStorageKeys.forEach(key => {
        sessionStorage.removeItem(key);
    });
    
    console.log('User cache cleared successfully');
}

/**
 * Redirect to login page
 */
function redirectToLogin() {
    console.log('Redirecting to login page...');
    window.location.href = '/Auth/Login';
}

/**
 * Show/hide logout loading state
 */
function showLogoutLoading(show) {
    const $logoutBtn = $('#logoutBtn');
    console.log('Setting logout loading state:', show);
    
    if (show) {
        $logoutBtn.html('<span class="spinner-border spinner-border-sm me-2"></span>Đang đăng xuất...');
        $logoutBtn.addClass('disabled');
    } else {
        $logoutBtn.html('<i class="bi bi-box-arrow-right me-2"></i>Đăng xuất');
        $logoutBtn.removeClass('disabled');
    }
}

/**
 * Show notification message
 */
function showNotification(message, type = 'info') {
    console.log('Showing notification:', message, type);
    
    // Remove existing notifications
    $('.user-notification').remove();
    
    const alertClass = {
        'success': 'alert-success',
        'error': 'alert-danger', 
        'warning': 'alert-warning',
        'info': 'alert-info'
    }[type] || 'alert-info';
    
    const icon = {
        'success': 'bi-check-circle',
        'error': 'bi-exclamation-triangle',
        'warning': 'bi-exclamation-triangle', 
        'info': 'bi-info-circle'
    }[type] || 'bi-info-circle';
    
    const notificationHtml = `
        <div class="alert ${alertClass} user-notification" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            <i class="bi ${icon} me-2"></i>
            ${message}
        </div>
    `;
    
    $('body').append(notificationHtml);
    
    // Auto hide after 5 seconds
    setTimeout(() => {
        $('.user-notification').fadeOut(300, function() {
            $(this).remove();
        });
    }, 5000);
}

/**
 * Update user info in dropdown
 */
function updateUserInfo(userInfo) {
    console.log('Updating user info:', userInfo);
    
    if (userInfo) {
        $('.user-name').text(userInfo.name || 'User');
        $('.user-email').text(userInfo.email || 'user@example.com');
        
        if (userInfo.avatar) {
            $('.user-avatar, .user-avatar-large').attr('src', userInfo.avatar);
        }
    }
}

/**
 * Load user info from API
 */
function loadUserInfo() {
    console.log('Loading user info...');
    
    // Simple fallback user info for testing
    const testUserInfo = {
        name: 'Test User',
        email: 'test@example.com',
        role: 'admin'
    };
    
    updateUserInfo(testUserInfo);
    console.log('Test user info loaded');
}
