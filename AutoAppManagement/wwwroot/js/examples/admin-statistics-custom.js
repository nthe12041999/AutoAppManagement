/**
 * Admin Statistics Custom Examples
 * Ví dụ về cách tùy chỉnh thống kê admin với các thẻ khác nhau
 */

// Ví dụ 1: Override template admin mặc định với các thẻ tùy chỉnh
function createCustomAdminStats() {
    const customAdminStats = new StatisticsCards('customAdminStats', {
        animateNumbers: true,
        animationDuration: 1200
    });

    // Tùy chỉnh các thẻ admin với màu sắc và icon khác
    const customAdminCards = [
        {
            id: 'totalAdmins',
            title: 'Tổng Admin',
            value: 0,
            icon: 'bi bi-people-fill',
            color: 'primary',
            colSize: 'col-xl-3 col-md-6'
        },
        {
            id: 'superAdmins',
            title: 'Super Admin',
            value: 0,
            icon: 'bi bi-shield-fill-check',
            color: 'danger',
            colSize: 'col-xl-3 col-md-6'
        },
        {
            id: 'moderators',
            title: 'Moderator',
            value: 0,
            icon: 'bi bi-person-badge',
            color: 'warning',
            colSize: 'col-xl-3 col-md-6'
        },
        {
            id: 'activeToday',
            title: 'Hoạt động hôm nay',
            value: 0,
            icon: 'bi bi-activity',
            color: 'success',
            colSize: 'col-xl-3 col-md-6'
        }
    ];

    customAdminStats.setCards(customAdminCards);
    return customAdminStats;
}

// Ví dụ 2: Thẻ thống kê hệ thống với gradient colors
function createSystemStatsCards() {
    const systemStats = new StatisticsCards('systemStats');

    const systemCards = [
        {
            id: 'systemHealth',
            title: 'Tình trạng hệ thống',
            value: '99.9%',
            icon: 'bi bi-heart-pulse',
            color: 'success',
            colSize: 'col-lg-4'
        },
        {
            id: 'serverLoad',
            title: 'Tải server',
            value: '45%',
            icon: 'bi bi-cpu',
            color: 'info',
            colSize: 'col-lg-4'
        },
        {
            id: 'memoryUsage',
            title: 'Sử dụng RAM',
            value: '2.1GB',
            icon: 'bi bi-memory',
            color: 'warning',
            colSize: 'col-lg-4'
        }
    ];

    systemStats.setCards(systemCards);
    return systemStats;
}

// Ví dụ 3: Thẻ thống kê bảo mật
function createSecurityStatsCards() {
    const securityStats = new StatisticsCards('securityStats');

    const securityCards = [
        {
            id: 'loginAttempts',
            title: 'Lần đăng nhập hôm nay',
            value: 0,
            icon: 'bi bi-door-open',
            color: 'primary'
        },
        {
            id: 'failedLogins',
            title: 'Đăng nhập thất bại',
            value: 0,
            icon: 'bi bi-shield-x',
            color: 'danger'
        },
        {
            id: 'blockedIPs',
            title: 'IP bị chặn',
            value: 0,
            icon: 'bi bi-ban',
            color: 'warning'
        },
        {
            id: 'securityAlerts',
            title: 'Cảnh báo bảo mật',
            value: 0,
            icon: 'bi bi-exclamation-triangle',
            color: 'info'
        }
    ];

    securityStats.setCards(securityCards);
    return securityStats;
}

// Ví dụ 4: Cách override và cập nhật dữ liệu từ API
async function updateAdminStatsFromAPI() {
    try {
        // Gọi API để lấy dữ liệu thống kê admin
        const response = await fetch('/api/admin/statistics');
        const data = await response.json();

        if (data.success && window.adminStatisticsStats) {
            // Cập nhật các giá trị thống kê
            window.adminStatisticsStats.updateValues({
                totalAdmins: data.statistics.totalAdmins,
                activeAdmins: data.statistics.activeAdmins,
                verifiedAdmins: data.statistics.verifiedAdmins,
                onlineAdmins: data.statistics.onlineAdmins
            });
        }
    } catch (error) {
        console.error('Lỗi khi cập nhật thống kê admin:', error);
    }
}

// Ví dụ 5: Tạo thẻ động dựa trên dữ liệu
function createDynamicAdminCards(adminData) {
    const dynamicStats = new StatisticsCards('dynamicAdminStats');

    // Tính toán thống kê từ dữ liệu
    const stats = calculateAdminStatistics(adminData);

    // Tạo thẻ dựa trên kết quả tính toán
    const dynamicCards = [
        {
            id: 'totalCount',
            title: 'Tổng số',
            value: stats.total,
            icon: 'bi bi-people',
            color: stats.total > 50 ? 'primary' : 'secondary'
        },
        {
            id: 'activeRate',
            title: 'Tỷ lệ hoạt động',
            value: `${stats.activeRate}%`,
            icon: 'bi bi-graph-up',
            color: stats.activeRate > 80 ? 'success' : stats.activeRate > 50 ? 'warning' : 'danger'
        },
        {
            id: 'newThisMonth',
            title: 'Mới tháng này',
            value: stats.newThisMonth,
            icon: 'bi bi-person-plus',
            color: 'info'
        },
        {
            id: 'avgLoginTime',
            title: 'Thời gian đăng nhập TB',
            value: stats.avgLoginTime,
            icon: 'bi bi-clock',
            color: 'warning'
        }
    ];

    dynamicStats.setCards(dynamicCards);
    return dynamicStats;
}

// Hàm tính toán thống kê từ dữ liệu admin
function calculateAdminStatistics(adminData) {
    const total = adminData.length;
    const active = adminData.filter(admin => admin.IsActive && admin.Status === 'Active').length;
    const activeRate = total > 0 ? Math.round((active / total) * 100) : 0;
    
    const currentMonth = new Date().getMonth();
    const currentYear = new Date().getFullYear();
    const newThisMonth = adminData.filter(admin => {
        const createdDate = new Date(admin.CreatedDate);
        return createdDate.getMonth() === currentMonth && createdDate.getFullYear() === currentYear;
    }).length;

    // Tính thời gian đăng nhập trung bình (giả lập)
    const avgLoginTime = '2.5h';

    return {
        total,
        activeRate,
        newThisMonth,
        avgLoginTime
    };
}

// Ví dụ 6: Sử dụng trong AdminDataGrid
class CustomAdminDataGrid extends AdminDataGrid {
    onDataLoaded(response) {
        const data = response.data || response;
        if (Array.isArray(data)) {
            // Tính toán thống kê tùy chỉnh
            const customStats = this.calculateCustomStatistics(data);

            // Cập nhật thẻ thống kê tùy chỉnh
            if (window.customAdminStatsStats) {
                window.customAdminStatsStats.updateValues(customStats);
            }

            // Gọi phương thức cha để cập nhật thống kê mặc định
            super.onDataLoaded(response);
        }
    }

    calculateCustomStatistics(data) {
        const totalAdmins = data.length;
        const superAdmins = data.filter(a => a.Role === 'SuperAdmin').length;
        const moderators = data.filter(a => a.Role === 'Moderator').length;
        
        // Tính admin hoạt động hôm nay
        const today = new Date().toDateString();
        const activeToday = data.filter(a => {
            return a.LastLoginDate && new Date(a.LastLoginDate).toDateString() === today;
        }).length;

        return {
            totalAdmins,
            superAdmins,
            moderators,
            activeToday
        };
    }
}

// Ví dụ sử dụng trong HTML:
/*
<!-- Thống kê admin mặc định -->
<div id="adminStatistics" data-component="statistics" data-template="admin"></div>

<!-- Thống kê admin tùy chỉnh -->
<div id="customAdminStats"></div>

<!-- Thống kê hệ thống -->
<div id="systemStats"></div>

<!-- Thống kê bảo mật -->
<div id="securityStats"></div>

<script>
// Khởi tạo các thống kê tùy chỉnh
document.addEventListener('DOMContentLoaded', function() {
    // Tạo thống kê admin tùy chỉnh
    window.customAdminStatsStats = createCustomAdminStats();
    
    // Tạo thống kê hệ thống
    window.systemStatsStats = createSystemStatsCards();
    
    // Tạo thống kê bảo mật
    window.securityStatsStats = createSecurityStatsCards();
    
    // Cập nhật dữ liệu từ API
    updateAdminStatsFromAPI();
});
</script>
*/
