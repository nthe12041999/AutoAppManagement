using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.User
{
    /// <summary>
    /// DTO cho đăng nhập với thông tin device
    /// </summary>
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Thông tin thiết bị không được để trống")]
        public DeviceInfoDTO DeviceInfo { get; set; }
    }

    /// <summary>
    /// DTO thông tin thiết bị
    /// </summary>
    public class DeviceInfoDTO
    {
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string BrowserInfo { get; set; }
    }

    /// <summary>
    /// DTO response đăng nhập
    /// </summary>
    public class UserLoginResponseDTO
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DeviceId { get; set; }
        public LicenseInfoDTO LicenseInfo { get; set; }
        public string AccessToken { get; set; }
        public DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// DTO thông tin license
    /// </summary>
    public class LicenseInfoDTO
    {
        public string LicenseKey { get; set; }
        public string LicenseName { get; set; }
        public string LicenseType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public string Status { get; set; }
        public bool IsExpired => ExpiryDate < DateTime.Now;
        public int DaysUntilExpiry => (ExpiryDate - DateTime.Now).Days;
    }

    /// <summary>
    /// DTO thông tin tài khoản người dùng
    /// </summary>
    public class UserAccountDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Notes { get; set; }
        
        // License information
        public LicenseInfoDTO LicenseInfo { get; set; }
        
        // Device information
        public List<CustomerDeviceDTO> Devices { get; set; } = new List<CustomerDeviceDTO>();
        
        // Statistics
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public bool HasActiveLicense { get; set; }
    }

    /// <summary>
    /// DTO thông tin thiết bị khách hàng
    /// </summary>
    public class CustomerDeviceDTO
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string BrowserInfo { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO tạo thiết bị mới
    /// </summary>
    public class CreateDeviceDTO
    {
        [Required(ErrorMessage = "Tên thiết bị không được để trống")]
        public string DeviceName { get; set; }

        [Required(ErrorMessage = "Loại thiết bị không được để trống")]
        public string DeviceType { get; set; }

        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string BrowserInfo { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO tạo license mới
    /// </summary>
    public class CreateLicenseDTO
    {
        [Required(ErrorMessage = "Tên license không được để trống")]
        public string LicenseName { get; set; }

        [Required(ErrorMessage = "Loại license không được để trống")]
        public string LicenseType { get; set; }

        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số thiết bị tối đa phải lớn hơn 0")]
        public int MaxDevices { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Số người dùng tối đa phải lớn hơn 0")]
        public int MaxUsers { get; set; } = 1;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        public DateTime ExpiryDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        public string Currency { get; set; } = "VND";
        public bool IsAutoRenewal { get; set; } = false;
        public string PaymentInfo { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO gia hạn license
    /// </summary>
    public class ExtendLicenseDTO
    {
        [Required(ErrorMessage = "License key không được để trống")]
        public string LicenseKey { get; set; }

        [Range(1, 3650, ErrorMessage = "Số ngày gia hạn phải từ 1 đến 3650")]
        public int ExtendDays { get; set; }

        public string Reason { get; set; }
        public string PaymentInfo { get; set; }
    }

    /// <summary>
    /// DTO thống kê tài khoản người dùng
    /// </summary>
    public class UserAccountStatisticsDTO
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int SuspendedUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int NewUsersThisWeek { get; set; }
        public int PremiumUsers { get; set; }
        public int VipUsers { get; set; }
        public int UsersWithActiveLicense { get; set; }
        public int UsersWithExpiredLicense { get; set; }
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        
        // Growth statistics
        public double UserGrowthRate { get; set; }
        public double ActiveUserRate { get; set; }
        public double LicenseUtilizationRate { get; set; }
        
        // Recent activity
        public List<RecentActivityDTO> RecentActivities { get; set; } = new List<RecentActivityDTO>();
    }

    /// <summary>
    /// DTO hoạt động gần đây
    /// </summary>
    public class RecentActivityDTO
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public DateTime ActivityTime { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
    }

    /// <summary>
    /// DTO tìm kiếm người dùng
    /// </summary>
    public class UserSearchDTO
    {
        public string Keyword { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string LicenseType { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? LastLoginFrom { get; set; }
        public DateTime? LastLoginTo { get; set; }
        public bool? HasActiveLicense { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortDirection { get; set; } = "DESC";
    }

    /// <summary>
    /// DTO kết quả tìm kiếm có phân trang
    /// </summary>
    public class PagedUserResultDTO
    {
        public List<UserAccountDTO> Users { get; set; } = new List<UserAccountDTO>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        
        // Additional info
        public UserAccountStatisticsDTO Statistics { get; set; }
        public List<string> AvailableRoles { get; set; } = new List<string>();
        public List<string> AvailableStatuses { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO thay đổi trạng thái
    /// </summary>
    public class ChangeUserStatusDTO
    {
        [Required(ErrorMessage = "Trạng thái mới không được để trống")]
        public string NewStatus { get; set; }

        public string Reason { get; set; }
        public bool SendNotification { get; set; } = true;
    }

    /// <summary>
    /// DTO đặt lại mật khẩu
    /// </summary>
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        public bool SendEmailNotification { get; set; } = true;
        public string Reason { get; set; }
    }
}
