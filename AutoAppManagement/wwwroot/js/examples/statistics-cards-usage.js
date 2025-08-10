/**
 * Statistics Cards Component - Usage Examples
 * Demonstrates how to use StatisticsCards component
 */

// Example 1: Auto-initialization with predefined templates
// Just add HTML with data attributes:
/*
<div id="customerStats" data-component="statistics" data-template="customer">
    <!-- Cards auto-generated -->
</div>
*/

// Example 2: Manual initialization with custom cards
function initCustomStatistics() {
    const customStats = new StatisticsCards('customStatsContainer', {
        animateNumbers: true,
        animationDuration: 1500
    });

    // Set custom cards
    customStats.setCards([
        {
            id: 'totalUsers',
            title: 'Tổng người dùng',
            value: 150,
            icon: 'bi bi-people',
            color: 'primary',
            colSize: 'col-lg-3 col-md-6'
        },
        {
            id: 'newToday',
            title: 'Mới hôm nay',
            value: 12,
            icon: 'bi bi-person-plus',
            color: 'success',
            colSize: 'col-lg-3 col-md-6'
        },
        {
            id: 'revenue',
            title: 'Doanh thu',
            value: 25000000,
            icon: 'bi bi-currency-dollar',
            color: 'warning',
            colSize: 'col-lg-3 col-md-6'
        },
        {
            id: 'orders',
            title: 'Đơn hàng',
            value: 89,
            icon: 'bi bi-cart',
            color: 'info',
            colSize: 'col-lg-3 col-md-6'
        }
    ]);

    return customStats;
}

// Example 3: Dynamic updates
function updateStatisticsExample() {
    // Get statistics instance
    const stats = window.customerStatisticsStats;
    
    if (stats) {
        // Update multiple values at once
        stats.updateValues({
            totalCustomers: 125,
            activeCustomers: 98,
            premiumCustomers: 15,
            onlineCustomers: 45
        });
    }
}

// Example 4: Loading states
function showLoadingExample() {
    const stats = window.customerStatisticsStats;
    
    if (stats) {
        // Show loading
        stats.showLoading();
        
        // Simulate API call
        setTimeout(() => {
            stats.hideLoading();
            stats.updateValues({
                totalCustomers: 200,
                activeCustomers: 150,
                premiumCustomers: 25,
                onlineCustomers: 75
            });
        }, 2000);
    }
}

// Example 5: Custom card templates
const CustomCardTemplates = {
    // E-commerce statistics
    ecommerce: [
        {
            id: 'totalProducts',
            title: 'Tổng sản phẩm',
            value: 0,
            icon: 'bi bi-box',
            color: 'primary'
        },
        {
            id: 'totalOrders',
            title: 'Đơn hàng',
            value: 0,
            icon: 'bi bi-cart-check',
            color: 'success'
        },
        {
            id: 'totalRevenue',
            title: 'Doanh thu',
            value: 0,
            icon: 'bi bi-currency-dollar',
            color: 'warning'
        },
        {
            id: 'totalCustomers',
            title: 'Khách hàng',
            value: 0,
            icon: 'bi bi-people',
            color: 'info'
        }
    ],

    // System statistics
    system: [
        {
            id: 'cpuUsage',
            title: 'CPU Usage',
            value: 0,
            icon: 'bi bi-cpu',
            color: 'primary'
        },
        {
            id: 'memoryUsage',
            title: 'Memory Usage',
            value: 0,
            icon: 'bi bi-memory',
            color: 'success'
        },
        {
            id: 'diskUsage',
            title: 'Disk Usage',
            value: 0,
            icon: 'bi bi-hdd',
            color: 'warning'
        },
        {
            id: 'networkUsage',
            title: 'Network',
            value: 0,
            icon: 'bi bi-wifi',
            color: 'info'
        }
    ]
};

// Example 6: Real-time updates
function startRealTimeUpdates() {
    const stats = window.customerStatisticsStats;
    
    if (stats) {
        setInterval(async () => {
            try {
                // Fetch real-time data from API
                const response = await fetch('/api/statistics/realtime');
                const data = await response.json();
                
                if (data.success) {
                    stats.updateValues(data.statistics);
                }
            } catch (error) {
                console.error('Error fetching real-time statistics:', error);
            }
        }, 30000); // Update every 30 seconds
    }
}

// Example 7: Integration with DataGrid
function integrateWithDataGrid() {
    // This is how CustomerDataGrid integrates with StatisticsCards
    class ExampleDataGrid extends DataGrid {
        onDataLoaded(response) {
            const data = response.data || response;
            
            if (Array.isArray(data) && window.exampleStatsStats) {
                // Calculate statistics from data
                const stats = this.calculateStatistics(data);
                
                // Update statistics cards
                window.exampleStatsStats.updateValues(stats);
            }
        }
        
        calculateStatistics(data) {
            return {
                totalItems: data.length,
                activeItems: data.filter(item => item.isActive).length,
                premiumItems: data.filter(item => item.isPremium).length,
                onlineItems: data.filter(item => item.isOnline).length
            };
        }
    }
}

// Example 8: Custom animations and styling
function customAnimationExample() {
    const stats = new StatisticsCards('animatedStats', {
        animateNumbers: true,
        animationDuration: 2000 // 2 seconds
    });
    
    // Custom cards with different colors and sizes
    stats.setCards([
        {
            id: 'bigStat',
            title: 'Big Statistic',
            value: 1000000,
            icon: 'bi bi-graph-up',
            color: 'gradient-primary',
            colSize: 'col-md-6'
        },
        {
            id: 'smallStat',
            title: 'Small Stat',
            value: 50,
            icon: 'bi bi-heart',
            color: 'gradient-danger',
            colSize: 'col-md-6'
        }
    ]);
    
    return stats;
}

// Usage in HTML:
/*
<!-- Method 1: Auto-initialization -->
<div id="customerStats" data-component="statistics" data-template="customer"></div>

<!-- Method 2: Manual initialization -->
<div id="customStats"></div>
<script>
    const stats = new StatisticsCards('customStats');
    stats.setCards(CustomCardTemplates.ecommerce);
</script>

<!-- Method 3: With custom configuration -->
<div id="animatedStats" data-component="statistics" data-template="system"></div>
<script>
    // Override default configuration
    window.animatedStatsStats.config.animationDuration = 3000;
</script>
*/
