/**
 * Submit Data - Enhanced form submission handler
 * Override default form submission behavior
 */

/**
 * Submit Data function - Main entry point for form submissions
 * @param {string} url - Submit URL
 * @param {Object} formData - Form data object
 * @param {string} method - HTTP method (GET, POST, PUT, DELETE)
 * @param {HTMLElement} button - Submit button element
 * @param {HTMLElement} form - Form element
 */
function submitData(url, formData, method = 'POST', button = null, form = null) {
    console.log('=== SubmitData Called ===');
    console.log('URL:', url);
    console.log('Method:', method);
    console.log('FormData:', formData);
    console.log('Button:', button);
    console.log('Form:', form);

    // Show loading state
    if (button) {
        const originalText = button.innerHTML;
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...';
        
        // Store original text for restoration
        button.setAttribute('data-original-text', originalText);
    }

    // Enhanced form submission with additional features
    if (method.toUpperCase() === 'GET') {
        callGetAPIAuthen(url, formData,
            (response) => {
                handleSubmitSuccess(response, form, button, formData);
            },
            (error) => {
                handleSubmitError(error, form, button);
            }
        );
    } else {
        callPostAPIAuthen(url, formData,
            (response) => {
                handleSubmitSuccess(response, form, button, formData);
            },
            (error) => {
                handleSubmitError(error, form, button);
            }
        );
    }
}

/**
 * Handle successful form submission
 * @param {Object} response - API response
 * @param {HTMLElement} form - Form element
 * @param {HTMLElement} button - Submit button
 * @param {Object} formData - Original form data
 */
function handleSubmitSuccess(response, form, button, formData) {
    console.log('SubmitData Success:', response);
    
    // Restore button state
    if (button) {
        button.disabled = false;
        const originalText = button.getAttribute('data-original-text');
        if (originalText) {
            button.innerHTML = originalText;
            button.removeAttribute('data-original-text');
        }
    }

    // Check response format
    if (response && response.IsSuccess !== undefined) {
        if (response.IsSuccess) {
            showSubmitNotification('✅ ' + (response.Message || 'Lưu thành công!'), 'success');
            
            // Auto close modal if in modal
            autoCloseModal(form);
            
            // Auto refresh grid if available
            autoRefreshGrid();
            
            // Trigger success event
            if (form) {
                form.dispatchEvent(new CustomEvent('submitDataSuccess', {
                    detail: { response, formData }
                }));
            }
        } else {
            showSubmitNotification('❌ ' + (response.Message || 'Có lỗi xảy ra!'), 'error');
        }
    } else {
        // Generic success
        showSubmitNotification('✅ Lưu thành công!', 'success');
        autoCloseModal(form);
        autoRefreshGrid();
        
        if (form) {
            form.dispatchEvent(new CustomEvent('submitDataSuccess', {
                detail: { response, formData }
            }));
        }
    }
}

/**
 * Handle form submission error
 * @param {Object} error - Error object
 * @param {HTMLElement} form - Form element
 * @param {HTMLElement} button - Submit button
 */
function handleSubmitError(error, form, button) {
    console.log('SubmitData Error:', error);
    
    // Restore button state
    if (button) {
        button.disabled = false;
        const originalText = button.getAttribute('data-original-text');
        if (originalText) {
            button.innerHTML = originalText;
            button.removeAttribute('data-original-text');
        }
    }

    let errorMessage = 'Có lỗi xảy ra khi lưu dữ liệu!';
    
    if (error && error.Message) {
        errorMessage = error.Message;
    }

    showSubmitNotification('❌ ' + errorMessage, 'error');

    // Trigger error event
    if (form) {
        form.dispatchEvent(new CustomEvent('submitDataError', {
            detail: { error, formData: formData }
        }));
    }
}

/**
 * Auto close modal after successful submit
 * @param {HTMLElement} form - Form element
 */
function autoCloseModal(form) {
    if (form) {
        const modal = form.closest('.modal');
        if (modal && typeof $ !== 'undefined') {
            $(modal).modal('hide');
        }
    }
}

/**
 * Auto refresh data grid after successful submit
 */
function autoRefreshGrid() {
    // Refresh grid if available
    if (window.dataGridInstance) {
        // Try to find and refresh any active grid
        const $grids = $('[data-component="data-grid"]');
        $grids.each((index, grid) => {
            const gridId = grid.id;
            if (gridId) {
                const config = window.dataGridInstance.getGrid(gridId);
                if (config) {
                    window.dataGridInstance.refreshData(config);
                }
            }
        });
    }
}

/**
 * Show notification message
 * @param {string} message - Message to show
 * @param {string} type - Notification type (success, error, info, warning)
 */
function showSubmitNotification(message, type = 'info') {
    // Try SweetAlert first
    if (typeof Swal !== 'undefined') {
        const icon = type === 'error' ? 'error' : type === 'success' ? 'success' : 'info';
        Swal.fire({
            title: type === 'error' ? 'Lỗi!' : type === 'success' ? 'Thành công!' : 'Thông báo',
            text: message,
            icon: icon,
            timer: 3000,
            showConfirmButton: false
        });
        return;
    }

    // Fallback to custom notification
    const alertClass = type === 'error' ? 'alert-danger' :
                      type === 'success' ? 'alert-success' :
                      type === 'warning' ? 'alert-warning' : 'alert-info';

    const notification = document.createElement('div');
    notification.className = `alert ${alertClass} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 350px; max-width: 500px;';
    notification.innerHTML = `
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        <div class="d-flex align-items-center">
            ${message}
        </div>
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 4000);
}

// Export to global scope
window.submitData = submitData;
