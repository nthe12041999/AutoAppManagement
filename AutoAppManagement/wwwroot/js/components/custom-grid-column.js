/**
 * Custom Grid Column Handler
 * Xử lý các loại column tùy chỉnh cho DataGrid
 * Usage: Định nghĩa column với data-column và data-column-* attributes
 */

/**
 * Hàm xử lý column tùy chỉnh
 * @param {object} item - Dữ liệu của row
 * @param {object} column - Cấu hình column
 * @param {any} value - Giá trị gốc của cell
 * @returns {string|null} HTML content hoặc null để sử dụng default handler
 */
window.customGridColumn = function(item, column, value) {
    // Xử lý theo type của column
    switch(column.type) {
        case 'user-info':
            return renderUserInfo(item, column, value);
        
        case 'status-badge':
            return renderStatusBadge(item, column, value);
        
        case 'role-badge':
            return renderRoleBadge(item, column, value);
        
        case 'phone-number':
            return renderPhoneNumber(item, column, value);
        
        case 'email-link':
            return renderEmailLink(item, column, value);
        
        case 'date-formatted':
            return renderDateFormatted(item, column, value);
        
        case 'datetime-formatted':
            return renderDateTimeFormatted(item, column, value);
        
        case 'currency':
            return renderCurrency(item, column, value);
        
        case 'percentage':
            return renderPercentage(item, column, value);
        
        case 'image-avatar':
            return renderImageAvatar(item, column, value);
        
        case 'icon-text':
            return renderIconText(item, column, value);
        
        case 'link-button':
            return renderLinkButton(item, column, value);
        
        case 'progress-bar':
            return renderProgressBar(item, column, value);
        
        case 'custom-template':
            return renderCustomTemplate(item, column, value);
        
        default:
            // Trả về null để sử dụng default handler
            return null;
    }
};

/**
 * Render thông tin user với avatar và email
 */
function renderUserInfo(item, column, value) {
    const name = value || item.name || item.fullName || '';
    const email = item.email || '';
    const avatar = item.avatar || item.avatarUrl || '';
    const initials = name.substring(0, 2).toUpperCase() || 'U';
    
    const avatarHtml = avatar 
        ? `<img src="${avatar}" class="avatar-circle" alt="${name}">`
        : `<div class="avatar-circle bg-primary text-white">${initials}</div>`;
    
    return `
        <div class="d-flex align-items-center">
            ${avatarHtml}
            <div class="ms-3">
                <div class="fw-bold">${name}</div>
                ${email ? `<small class="text-muted">${email}</small>` : ''}
            </div>
        </div>
    `;
}

/**
 * Render status badge với màu sắc tùy chỉnh
 */
function renderStatusBadge(item, column, value) {
    const statusMap = {
        'active': { class: 'success', text: 'Hoạt động' },
        'inactive': { class: 'secondary', text: 'Không hoạt động' },
        'suspended': { class: 'warning', text: 'Tạm khóa' },
        'banned': { class: 'danger', text: 'Bị cấm' },
        'pending': { class: 'info', text: 'Chờ duyệt' }
    };
    
    const status = statusMap[value] || { class: 'secondary', text: value };
    return `<span class="badge bg-${status.class}">${status.text}</span>`;
}

/**
 * Render role badge
 */
function renderRoleBadge(item, column, value) {
    const roleMap = {
        'admin': { class: 'danger', text: 'Quản trị viên', icon: 'bi-shield-check' },
        'customer': { class: 'primary', text: 'Khách hàng', icon: 'bi-person' },
        'premium': { class: 'warning', text: 'Premium', icon: 'bi-star' },
        'vip': { class: 'success', text: 'VIP', icon: 'bi-gem' }
    };
    
    const role = roleMap[value] || { class: 'secondary', text: value, icon: 'bi-person' };
    return `
        <span class="badge bg-${role.class}">
            <i class="${role.icon} me-1"></i>${role.text}
        </span>
    `;
}

/**
 * Render số điện thoại với link gọi
 */
function renderPhoneNumber(item, column, value) {
    if (!value) return '-';
    return `<a href="tel:${value}" class="text-decoration-none">${value}</a>`;
}

/**
 * Render email với link mailto
 */
function renderEmailLink(item, column, value) {
    if (!value) return '-';
    return `<a href="mailto:${value}" class="text-decoration-none">${value}</a>`;
}

/**
 * Render ngày tháng định dạng Việt Nam
 */
function renderDateFormatted(item, column, value) {
    if (!value) return '-';
    try {
        const date = new Date(value);
        return date.toLocaleDateString('vi-VN');
    } catch (e) {
        return value;
    }
}

/**
 * Render ngày giờ định dạng Việt Nam
 */
function renderDateTimeFormatted(item, column, value) {
    if (!value) return '-';
    try {
        const date = new Date(value);
        return date.toLocaleString('vi-VN');
    } catch (e) {
        return value;
    }
}

/**
 * Render tiền tệ
 */
function renderCurrency(item, column, value) {
    if (!value && value !== 0) return '-';
    const currency = column.currency || 'VND';
    const amount = parseFloat(value);
    
    if (currency === 'VND') {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }
    
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency
    }).format(amount);
}

/**
 * Render phần trăm
 */
function renderPercentage(item, column, value) {
    if (!value && value !== 0) return '-';
    const percent = parseFloat(value);
    return `${percent.toFixed(column.decimals || 1)}%`;
}

/**
 * Render avatar từ hình ảnh
 */
function renderImageAvatar(item, column, value) {
    const imageUrl = value || item.avatar || item.avatarUrl;
    const name = item.name || item.fullName || '';
    const initials = name.substring(0, 2).toUpperCase() || 'U';
    
    if (imageUrl) {
        return `<img src="${imageUrl}" class="avatar-circle" alt="${name}">`;
    } else {
        return `<div class="avatar-circle bg-primary text-white">${initials}</div>`;
    }
}

/**
 * Render icon với text
 */
function renderIconText(item, column, value) {
    const icon = column.icon || 'bi-circle';
    const iconClass = column.iconClass || 'text-primary';
    return `<i class="${icon} ${iconClass} me-2"></i>${value}`;
}

/**
 * Render link button
 */
function renderLinkButton(item, column, value) {
    const url = column.url || '#';
    const target = column.target || '_self';
    const btnClass = column.btnClass || 'btn-outline-primary';
    
    return `<a href="${url}" target="${target}" class="btn btn-sm ${btnClass}">${value}</a>`;
}

/**
 * Render progress bar
 */
function renderProgressBar(item, column, value) {
    const percent = parseFloat(value) || 0;
    const maxValue = column.max || 100;
    const progressPercent = (percent / maxValue) * 100;
    const barClass = column.barClass || 'bg-primary';
    
    return `
        <div class="progress" style="height: 20px;">
            <div class="progress-bar ${barClass}" role="progressbar" 
                 style="width: ${progressPercent}%" 
                 aria-valuenow="${percent}" 
                 aria-valuemin="0" 
                 aria-valuemax="${maxValue}">
                ${percent}${column.unit || ''}
            </div>
        </div>
    `;
}

/**
 * Render custom template
 */
function renderCustomTemplate(item, column, value) {
    let template = column.template || '{value}';
    
    // Replace placeholders
    template = template.replace(/\{value\}/g, value);
    template = template.replace(/\{(\w+)\}/g, (match, key) => {
        return item[key] || '';
    });
    
    return template;
}

// CSS styles cho avatar
if (!document.getElementById('custom-grid-column-styles')) {
    const style = document.createElement('style');
    style.id = 'custom-grid-column-styles';
    style.textContent = `
        .avatar-circle {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            font-weight: bold;
            object-fit: cover;
        }
    `;
    document.head.appendChild(style);
}

console.log('✅ Custom Grid Column handler loaded');

// Ext JS bridge (optional): expose renderer via App.grid.CustomColumn
(function setupExtCustomColumnBridge(){
    if (!window.Ext || !Ext.define) return;
    if (!window.App) window.App = {}; if (!App.grid) App.grid = {};
    Ext.define('App.grid.CustomColumn', {
        singleton: true,
        render: function(item, column, value){
            return window.customGridColumn(item, column, value);
        },
        // Expose individual renderers if needed
        renderers: {
            userInfo: renderUserInfo,
            statusBadge: renderStatusBadge,
            roleBadge: renderRoleBadge,
            phoneNumber: renderPhoneNumber,
            emailLink: renderEmailLink,
            dateFormatted: renderDateFormatted,
            datetimeFormatted: renderDateTimeFormatted,
            currency: renderCurrency,
            percentage: renderPercentage,
            imageAvatar: renderImageAvatar,
            iconText: renderIconText,
            linkButton: renderLinkButton,
            progressBar: renderProgressBar,
            customTemplate: renderCustomTemplate
        }
    });
})();
