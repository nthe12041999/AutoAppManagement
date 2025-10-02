/**
 * Form Helper Utilities
 * Các hàm tiện ích hỗ trợ xử lý form
 */

const FormHelpers = {
    /**
     * Format phone number theo chuẩn Việt Nam
     * @param {string} phone - Số điện thoại
     * @returns {string} Số điện thoại đã format
     */
    formatPhoneNumber(phone) {
        if (!phone) return '';
        
        // Remove all non-digits
        let cleaned = phone.replace(/\D/g, '');
        
        // Vietnamese phone format: 0XXX XXX XXX
        if (cleaned.length <= 4) {
            return cleaned;
        } else if (cleaned.length <= 7) {
            return cleaned.slice(0, 4) + ' ' + cleaned.slice(4);
        } else {
            return cleaned.slice(0, 4) + ' ' + cleaned.slice(4, 7) + ' ' + cleaned.slice(7, 10);
        }
    },

    /**
     * Validate Vietnamese phone number
     * @param {string} phone - Số điện thoại
     * @returns {boolean} Valid hay không
     */
    validatePhoneNumber(phone) {
        if (!phone) return false;
        const cleaned = phone.replace(/\D/g, '');
        
        // Vietnamese phone patterns
        const patterns = [
            /^(03|05|07|08|09)\d{8}$/,  // Mobile
            /^(024|028)\d{8}$/,          // Landline HN, HCM
            /^(02)\d{9}$/                // Other landlines
        ];
        
        return patterns.some(pattern => pattern.test(cleaned));
    },

    /**
     * Format currency VND
     * @param {number} amount - Số tiền
     * @returns {string} Số tiền đã format
     */
    formatCurrency(amount) {
        if (isNaN(amount)) return '0 ₫';
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    },

    /**
     * Format date to DD/MM/YYYY
     * @param {string|Date} date - Ngày
     * @returns {string} Ngày đã format
     */
    formatDate(date) {
        if (!date) return '';
        
        const d = new Date(date);
        if (isNaN(d.getTime())) return '';
        
        const day = String(d.getDate()).padStart(2, '0');
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const year = d.getFullYear();
        
        return `${day}/${month}/${year}`;
    },

    /**
     * Parse date from DD/MM/YYYY to ISO
     * @param {string} dateStr - Ngày dạng DD/MM/YYYY
     * @returns {string} ISO date string
     */
    parseDate(dateStr) {
        if (!dateStr) return '';
        
        const parts = dateStr.split('/');
        if (parts.length !== 3) return dateStr;
        
        const [day, month, year] = parts;
        return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
    },

    /**
     * Get relative time (e.g., "2 giờ trước")
     * @param {string|Date} date - Thời gian
     * @returns {string} Thời gian tương đối
     */
    getRelativeTime(date) {
        if (!date) return '';
        
        const d = new Date(date);
        const now = new Date();
        const diffMs = now - d;
        const diffMins = Math.floor(diffMs / 60000);
        
        if (diffMins < 1) return 'Vừa xong';
        if (diffMins < 60) return `${diffMins} phút trước`;
        
        const diffHours = Math.floor(diffMins / 60);
        if (diffHours < 24) return `${diffHours} giờ trước`;
        
        const diffDays = Math.floor(diffHours / 24);
        if (diffDays < 7) return `${diffDays} ngày trước`;
        
        const diffWeeks = Math.floor(diffDays / 7);
        if (diffWeeks < 4) return `${diffWeeks} tuần trước`;
        
        const diffMonths = Math.floor(diffDays / 30);
        if (diffMonths < 12) return `${diffMonths} tháng trước`;
        
        const diffYears = Math.floor(diffDays / 365);
        return `${diffYears} năm trước`;
    },

    /**
     * Validate email format
     * @param {string} email - Email
     * @returns {boolean} Valid hay không
     */
    validateEmail(email) {
        if (!email) return false;
        const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return pattern.test(email.toLowerCase());
    },

    /**
     * Slugify Vietnamese text
     * @param {string} text - Text cần slugify
     * @returns {string} Slug
     */
    slugify(text) {
        if (!text) return '';
        
        // Vietnamese character map
        const from = "àáäâầấẫẩăằắẵẳèéëêềếễểìíïîòóöôồốỗổơờớỡởùúüûưừứữửỳýÿỹđ";
        const to   = "aaaaaaaaaaaaaaaaeeeeeeeeeiiiioooooooooooooouuuuuuuuuuyyyyd";
        
        let slug = text.toLowerCase();
        
        // Replace Vietnamese characters
        for (let i = 0; i < from.length; i++) {
            slug = slug.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
        }
        
        // Replace non-alphanumeric with dash
        slug = slug.replace(/[^a-z0-9]+/g, '-');
        
        // Remove leading/trailing dashes
        slug = slug.replace(/^-+|-+$/g, '');
        
        return slug;
    },

    /**
     * Truncate text với ellipsis
     * @param {string} text - Text cần cắt
     * @param {number} maxLength - Độ dài tối đa
     * @returns {string} Text đã cắt
     */
    truncate(text, maxLength = 100) {
        if (!text || text.length <= maxLength) return text;
        return text.substring(0, maxLength - 3) + '...';
    },

    /**
     * Deep clone object
     * @param {object} obj - Object cần clone
     * @returns {object} Cloned object
     */
    deepClone(obj) {
        if (obj === null || typeof obj !== 'object') return obj;
        if (obj instanceof Date) return new Date(obj.getTime());
        if (obj instanceof Array) return obj.map(item => this.deepClone(item));
        
        const cloned = {};
        for (const key in obj) {
            if (obj.hasOwnProperty(key)) {
                cloned[key] = this.deepClone(obj[key]);
            }
        }
        return cloned;
    },

    /**
     * Compare two objects for changes
     * @param {object} original - Original object
     * @param {object} modified - Modified object
     * @returns {object} Changed fields
     */
    getChangedFields(original, modified) {
        const changes = {};
        
        for (const key in modified) {
            if (modified.hasOwnProperty(key)) {
                const origVal = original[key];
                const modVal = modified[key];
                
                if (JSON.stringify(origVal) !== JSON.stringify(modVal)) {
                    changes[key] = {
                        old: origVal,
                        new: modVal
                    };
                }
            }
        }
        
        return changes;
    },

    /**
     * Debounce function
     * @param {function} func - Function to debounce
     * @param {number} wait - Wait time in ms
     * @returns {function} Debounced function
     */
    debounce(func, wait = 300) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    /**
     * Get file size formatted
     * @param {number} bytes - File size in bytes
     * @returns {string} Formatted size
     */
    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    },

    /**
     * Validate file type
     * @param {File} file - File to validate
     * @param {string[]} allowedTypes - Allowed MIME types
     * @returns {boolean} Valid or not
     */
    validateFileType(file, allowedTypes = ['image/jpeg', 'image/png', 'image/gif']) {
        if (!file) return false;
        return allowedTypes.includes(file.type);
    },

    /**
     * Generate random ID
     * @param {number} length - ID length
     * @returns {string} Random ID
     */
    generateId(length = 10) {
        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        let result = '';
        for (let i = 0; i < length; i++) {
            result += chars.charAt(Math.floor(Math.random() * chars.length));
        }
        return result;
    },

    /**
     * Show notification toast
     * @param {string} message - Message
     * @param {string} type - Type: success/error/warning/info
     * @param {number} duration - Duration in ms
     */
    showToast(message, type = 'info', duration = 3000) {
        // Use existing showToast if available
        if (typeof window.showToast === 'function') {
            window.showToast(message, type);
            return;
        }

        // Fallback implementation
        const toastClass = {
            success: 'bg-success',
            error: 'bg-danger',
            warning: 'bg-warning',
            info: 'bg-info'
        };

        const toastHtml = `
            <div class="toast align-items-center text-white ${toastClass[type]} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            document.body.appendChild(container);
        }

        const toastElement = document.createElement('div');
        toastElement.innerHTML = toastHtml;
        const toast = toastElement.firstElementChild;
        container.appendChild(toast);

        const bsToast = new bootstrap.Toast(toast, { delay: duration });
        bsToast.show();

        toast.addEventListener('hidden.bs.toast', () => {
            toast.remove();
        });
    },

    /**
     * Scroll to element with offset
     * @param {string|HTMLElement} element - Element or selector
     * @param {number} offset - Offset from top
     */
    scrollToElement(element, offset = 100) {
        const el = typeof element === 'string' ? document.querySelector(element) : element;
        if (!el) return;

        const elementPosition = el.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.pageYOffset - offset;

        window.scrollTo({
            top: offsetPosition,
            behavior: 'smooth'
        });
    },

    /**
     * Set character counter for textarea
     * @param {string|HTMLElement} textarea - Textarea element or selector
     * @param {number} maxLength - Max length
     */
    setCharacterCounter(textarea, maxLength) {
        const el = typeof textarea === 'string' ? document.querySelector(textarea) : textarea;
        if (!el) return;

        // Create counter element
        const counter = document.createElement('div');
        counter.className = 'char-count';
        counter.textContent = `0 / ${maxLength}`;
        
        // Wrap textarea if not already wrapped
        let wrapper = el.parentElement;
        if (!wrapper.classList.contains('textarea-wrapper')) {
            wrapper = document.createElement('div');
            wrapper.className = 'textarea-wrapper';
            el.parentNode.insertBefore(wrapper, el);
            wrapper.appendChild(el);
        }
        
        wrapper.appendChild(counter);

        // Update counter on input
        el.addEventListener('input', () => {
            const length = el.value.length;
            counter.textContent = `${length} / ${maxLength}`;
            
            // Update counter color
            counter.classList.remove('warning', 'danger');
            if (length > maxLength * 0.9) {
                counter.classList.add('danger');
            } else if (length > maxLength * 0.7) {
                counter.classList.add('warning');
            }
        });
    }
};

// Export for use in other scripts
window.FormHelpers = FormHelpers;

// jQuery plugin for common operations
if (typeof $ !== 'undefined') {
    $.fn.formatPhone = function() {
        return this.each(function() {
            const $input = $(this);
            $input.on('input', function() {
                const formatted = FormHelpers.formatPhoneNumber(this.value);
                this.value = formatted;
            });
        });
    };

    $.fn.charCounter = function(maxLength) {
        return this.each(function() {
            FormHelpers.setCharacterCounter(this, maxLength);
        });
    };
}

