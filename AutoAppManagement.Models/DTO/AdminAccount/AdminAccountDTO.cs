using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.AdminAccount
{
    /// <summary>
    /// DTO thông tin tài khoản admin
    /// </summary>
    public class AdminAccountDTO
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
        public string Department { get; set; }
        public string Position { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Notes { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedDate { get; set; }
        public string LockedReason { get; set; }

        // Permission information
        public List<AdminPermissionDTO> Permissions { get; set; } = new List<AdminPermissionDTO>();
        public List<string> Roles { get; set; } = new List<string>();

        // Statistics
        public int TotalLogins { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public bool IsOnline { get; set; }
        public string LastIpAddress { get; set; }
        public string LastUserAgent { get; set; }
    }

    /// <summary>
    /// DTO quyền hạn admin
    /// </summary>
    public class AdminPermissionDTO
    {
        public long Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionCode { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsGranted { get; set; }
        public DateTime? GrantedDate { get; set; }
        public long? GrantedBy { get; set; }
        public string GrantedByName { get; set; }
    }

    /// <summary>
    /// DTO lịch sử đăng nhập admin
    /// </summary>
    public class AdminLoginHistoryDTO
    {
        public long Id { get; set; }
        public long AdminId { get; set; }
        public string AdminName { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string DeviceInfo { get; set; }
        public string Location { get; set; }
        public bool IsSuccessful { get; set; }
        public string FailureReason { get; set; }
        public TimeSpan? SessionDuration { get; set; }
        public string LoginMethod { get; set; }
    }

    /// <summary>
    /// DTO admin đang online
    /// </summary>
    public class OnlineAdminDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LastActivityTime { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public TimeSpan OnlineDuration { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// DTO thống kê tài khoản admin
    /// </summary>
    public class AdminAccountStatistics
    {
        public int TotalAdmins { get; set; }
        public int ActiveAdmins { get; set; }
        public int InactiveAdmins { get; set; }
        public int LockedAdmins { get; set; }
        public int OnlineAdmins { get; set; }
        public int NewAdminsThisMonth { get; set; }
        public int NewAdminsThisWeek { get; set; }
        public int NewAdminsToday { get; set; }

        // Role statistics
        public Dictionary<string, int> AdminsByRole { get; set; } = new Dictionary<string, int>();

        // Department statistics
        public Dictionary<string, int> AdminsByDepartment { get; set; } = new Dictionary<string, int>();

        // Activity statistics
        public int LoginsToday { get; set; }
        public int LoginsThisWeek { get; set; }
        public int LoginsThisMonth { get; set; }
        public int UniqueLoginsToday { get; set; }

        // Growth statistics
        public double AdminGrowthRate { get; set; }
        public double ActiveAdminRate { get; set; }
        public double LoginSuccessRate { get; set; }

        // Recent activities
        public List<RecentAdminActivityDTO> RecentActivities { get; set; } = new List<RecentAdminActivityDTO>();

        // Charts data
        public List<ChartDataPointDTO> AdminGrowthChart { get; set; } = new List<ChartDataPointDTO>();
        public List<ChartDataPointDTO> AdminStatusChart { get; set; } = new List<ChartDataPointDTO>();
        public List<ChartDataPointDTO> AdminRoleChart { get; set; } = new List<ChartDataPointDTO>();
        public List<ChartDataPointDTO> LoginActivityChart { get; set; } = new List<ChartDataPointDTO>();
    }

    /// <summary>
    /// DTO hoạt động gần đây của admin
    /// </summary>
    public class RecentAdminActivityDTO
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string AdminName { get; set; }
        public DateTime ActivityTime { get; set; }
        public string IpAddress { get; set; }
        public string Details { get; set; }
        public string Severity { get; set; }
    }

    /// <summary>
    /// DTO điểm dữ liệu cho biểu đồ
    /// </summary>
    public class ChartDataPointDTO
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public string Color { get; set; }
        public DateTime? Date { get; set; }
        public string Category { get; set; }
    }

    /// <summary>
    /// DTO tìm kiếm admin
    /// </summary>
    public class SearchAdminAccountDTO
    {
        public string Keyword { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? LastLoginFrom { get; set; }
        public DateTime? LastLoginTo { get; set; }
        public bool? IsOnline { get; set; }
        public bool? IsLocked { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortDirection { get; set; } = "DESC";
    }

    /// <summary>
    /// DTO kết quả tìm kiếm có phân trang
    /// </summary>
    public class PagedAdminAccountResultDTO
    {
        public List<AdminAccountDTO> Admins { get; set; } = new List<AdminAccountDTO>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        // Additional info
        public AdminAccountStatistics Statistics { get; set; }
        public List<string> AvailableRoles { get; set; } = new List<string>();
        public List<string> AvailableStatuses { get; set; } = new List<string>();
        public List<string> AvailableDepartments { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO thay đổi trạng thái admin
    /// </summary>
    public class ChangeAdminAccountStatusDTO
    {
        [Required(ErrorMessage = "ID admin không được để trống")]
        public long AdminId { get; set; }

        [Required(ErrorMessage = "Trạng thái mới không được để trống")]
        public string NewStatus { get; set; }

        public string Reason { get; set; }
        public bool SendNotification { get; set; } = true;
        public long? ChangedBy { get; set; }
    }

    /// <summary>
    /// DTO phân quyền admin
    /// </summary>
    public class AssignPermissionsDTO
    {
        [Required(ErrorMessage = "ID admin không được để trống")]
        public long AdminId { get; set; }

        [Required(ErrorMessage = "Danh sách quyền không được để trống")]
        public List<string> PermissionCodes { get; set; } = new List<string>();

        public string Reason { get; set; }
        public bool SendNotification { get; set; } = true;
        public long? AssignedBy { get; set; }
    }

    /// <summary>
    /// DTO đổi mật khẩu admin
    /// </summary>
    public class ChangeAdminPasswordDTO
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        public bool ForceLogoutOtherSessions { get; set; } = true;
    }

    /// <summary>
    /// DTO reset mật khẩu admin
    /// </summary>
    public class ResetAdminPasswordDTO
    {
        [Required(ErrorMessage = "ID admin không được để trống")]
        public long AdminId { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string NewPassword { get; set; }

        public bool SendEmailNotification { get; set; } = true;
        public bool ForcePasswordChange { get; set; } = true;
        public string Reason { get; set; }
        public long? ResetBy { get; set; }
    }

    /// <summary>
    /// DTO gửi thông báo
    /// </summary>
    public class SendNotificationDTO
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Message { get; set; }

        public string Type { get; set; } = "info";
        public string Priority { get; set; } = "normal";
        public DateTime? ScheduledTime { get; set; }
        public bool RequireAcknowledgment { get; set; } = false;
    }

    /// <summary>
    /// DTO broadcast thông báo
    /// </summary>
    public class BroadcastNotificationDTO
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Message { get; set; }

        public string Type { get; set; } = "info";
        public string Priority { get; set; } = "normal";
        public List<string> TargetRoles { get; set; } = new List<string>();
        public List<string> TargetDepartments { get; set; } = new List<string>();
        public DateTime? ScheduledTime { get; set; }
        public bool RequireAcknowledgment { get; set; } = false;
        public long? SentBy { get; set; }
    }
}
