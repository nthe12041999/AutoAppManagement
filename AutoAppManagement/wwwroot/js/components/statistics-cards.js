/**
 * Statistics Cards Component
 * Dynamic statistics cards with configurable data binding
 */

class StatisticsCards {
    constructor(containerId, config = {}) {
        this.containerId = containerId;
        this.container = document.getElementById(containerId);
        
        if (!this.container) {
            console.error('Statistics container not found:', containerId);
            return;
        }

        this.config = {
            cards: [],
            autoUpdate: true,
            animateNumbers: true,
            animationDuration: 1000,
            ...config
        };

        this.init();
    }

    init() {
        console.log('🚀 Initializing Statistics Cards:', this.containerId);
        
        if (this.config.cards.length > 0) {
            this.render();
        }
        
        console.log('✅ Statistics Cards initialized');
    }

    // Set cards configuration
    setCards(cards) {
        this.config.cards = cards;
        this.render();
    }

    // Add a single card
    addCard(card) {
        this.config.cards.push(card);
        this.render();
    }

    // Update card values
    updateValues(values) {
        Object.keys(values).forEach(key => {
            const element = document.getElementById(key);
            if (element) {
                const newValue = values[key];
                
                if (this.config.animateNumbers) {
                    this.animateNumber(element, newValue);
                } else {
                    element.textContent = newValue;
                }
            }
        });
    }

    // Render all cards
    render() {
        if (!this.container || this.config.cards.length === 0) return;

        const cardsHtml = this.config.cards.map(card => this.renderCard(card)).join('');
        
        this.container.innerHTML = `
            <div class="row mb-4">
                ${cardsHtml}
            </div>
        `;

        console.log(`✅ Rendered ${this.config.cards.length} statistics cards`);
    }

    // Render individual card
    renderCard(card) {
        const {
            id,
            title,
            value = 0,
            icon,
            color = 'primary',
            colSize = 'col-md-3',
            textColor = 'text-white',
            iconOpacity = 'opacity-50'
        } = card;

        return `
            <div class="${colSize}">
                <div class="card border-0 bg-${color} ${textColor}">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <h6 class="card-title text-white-50 mb-1">${title}</h6>
                                <h3 class="mb-0" id="${id}">${value}</h3>
                            </div>
                            <i class="${icon} fs-1 ${iconOpacity}"></i>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    // Animate number changes
    animateNumber(element, targetValue) {
        const startValue = parseInt(element.textContent) || 0;
        const difference = targetValue - startValue;
        const duration = this.config.animationDuration;
        const startTime = performance.now();

        const animate = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // Easing function (ease-out)
            const easeOut = 1 - Math.pow(1 - progress, 3);
            const currentValue = Math.round(startValue + (difference * easeOut));
            
            element.textContent = currentValue;
            
            if (progress < 1) {
                requestAnimationFrame(animate);
            } else {
                element.textContent = targetValue; // Ensure final value is exact
            }
        };

        requestAnimationFrame(animate);
    }

    // Get current values
    getCurrentValues() {
        const values = {};
        this.config.cards.forEach(card => {
            const element = document.getElementById(card.id);
            if (element) {
                values[card.id] = parseInt(element.textContent) || 0;
            }
        });
        return values;
    }

    // Reset all values to 0
    reset() {
        this.config.cards.forEach(card => {
            const element = document.getElementById(card.id);
            if (element) {
                element.textContent = '0';
            }
        });
    }

    // Show loading state
    showLoading() {
        this.config.cards.forEach(card => {
            const element = document.getElementById(card.id);
            if (element) {
                element.innerHTML = '<div class="spinner-border spinner-border-sm" role="status"></div>';
            }
        });
    }

    // Hide loading state
    hideLoading() {
        this.config.cards.forEach(card => {
            const element = document.getElementById(card.id);
            if (element) {
                element.textContent = card.value || '0';
            }
        });
    }
}

// Predefined card configurations
const StatisticsCardTemplates = {
    // Customer statistics
    customer: [
        {
            id: 'totalCustomers',
            title: 'Tổng khách hàng',
            value: 0,
            icon: 'bi bi-people-fill',
            color: 'primary'
        },
        {
            id: 'activeCustomers',
            title: 'Đang hoạt động',
            value: 0,
            icon: 'bi bi-check-circle-fill',
            color: 'success'
        },
        {
            id: 'premiumCustomers',
            title: 'Premium/VIP',
            value: 0,
            icon: 'bi bi-star-fill',
            color: 'warning'
        },
        {
            id: 'onlineCustomers',
            title: 'Online',
            value: 0,
            icon: 'bi bi-circle-fill',
            color: 'info'
        }
    ],

    // Admin statistics
    admin: [
        {
            id: 'totalAdmins',
            title: 'Tổng quản trị',
            value: 0,
            icon: 'bi bi-shield-fill',
            color: 'primary'
        },
        {
            id: 'activeAdmins',
            title: 'Đang hoạt động',
            value: 0,
            icon: 'bi bi-check-circle-fill',
            color: 'success'
        },
        {
            id: 'superAdmins',
            title: 'Super Admin',
            value: 0,
            icon: 'bi bi-crown-fill',
            color: 'warning'
        },
        {
            id: 'onlineAdmins',
            title: 'Online',
            value: 0,
            icon: 'bi bi-circle-fill',
            color: 'info'
        }
    ],

    // Sales statistics
    sales: [
        {
            id: 'totalSales',
            title: 'Tổng doanh thu',
            value: 0,
            icon: 'bi bi-currency-dollar',
            color: 'primary'
        },
        {
            id: 'todaySales',
            title: 'Hôm nay',
            value: 0,
            icon: 'bi bi-calendar-check',
            color: 'success'
        },
        {
            id: 'monthSales',
            title: 'Tháng này',
            value: 0,
            icon: 'bi bi-calendar3',
            color: 'warning'
        },
        {
            id: 'yearSales',
            title: 'Năm này',
            value: 0,
            icon: 'bi bi-calendar4-range',
            color: 'info'
        }
    ],

    // Admin statistics
    admin: [
        {
            id: 'totalAdmins',
            title: 'Tổng quản trị viên',
            value: 0,
            icon: 'bi bi-shield-check',
            color: 'primary'
        },
        {
            id: 'activeAdmins',
            title: 'Đang hoạt động',
            value: 0,
            icon: 'bi bi-person-check',
            color: 'success'
        },
        {
            id: 'verifiedAdmins',
            title: 'Đã xác thực',
            value: 0,
            icon: 'bi bi-patch-check',
            color: 'warning'
        },
        {
            id: 'onlineAdmins',
            title: 'Trực tuyến',
            value: 0,
            icon: 'bi bi-circle-fill',
            color: 'info'
        }
    ]
};

// Auto-initialize statistics cards
document.addEventListener('DOMContentLoaded', function() {
    console.log('🔍 Looking for Statistics Cards...');
    
    // Look for containers with data-statistics attribute
    const statisticsContainers = document.querySelectorAll('[data-component="statistics"]');
    statisticsContainers.forEach(container => {
        const containerId = container.id;
        const template = container.dataset.template || 'customer';
        
        if (containerId && StatisticsCardTemplates[template]) {
            console.log('🚀 Auto-initializing Statistics Cards:', containerId);
            
            const stats = new StatisticsCards(containerId);
            stats.setCards(StatisticsCardTemplates[template]);
            
            // Store reference globally
            window[`${containerId}Stats`] = stats;
        }
    });
});
