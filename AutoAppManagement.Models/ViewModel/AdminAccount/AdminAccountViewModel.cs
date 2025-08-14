using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AutoAppManagement.Models.ViewModel.AdminAccount
{
    /// <summary>
    /// ViewModel hiển thị thông tin tài khoản admin
    /// </summary>
    public class AdminAccountViewModel
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
        public List<string> Permissions { get; set; } = new List<string>();
        public List<string> Roles { get; set; } = new List<string>();

        // Statistics
        public int TotalLogins { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public bool IsOnline { get; set; }
        public string LastIpAddress { get; set; }
    }

    /// <summary>
    /// ViewModel tạo tài khoản admin mới
    /// </summary>
    public class CreateAdminAccountViewModel
    {
        [Required(ErrorMessage = "Họ không được để trống")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có từ 8 đến 100 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public string Status { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        [StringLength(100, ErrorMessage = "Phòng ban không được vượt quá 100 ký tự")]
        public string Department { get; set; }

        [StringLength(100, ErrorMessage = "Chức vụ không được vượt quá 100 ký tự")]
        public string Position { get; set; }

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        public string Notes { get; set; }

        // Avatar file
        public IFormFile AvatarFile { get; set; }

        // Permissions
        public List<string> Permissions { get; set; } = new List<string>();
    }

    /// <summary>
    /// ViewModel cập nhật tài khoản admin
    /// </summary>
    public class UpdateAdminAccountViewModel
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public string Status { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        [StringLength(100, ErrorMessage = "Phòng ban không được vượt quá 100 ký tự")]
        public string Department { get; set; }

        [StringLength(100, ErrorMessage = "Chức vụ không được vượt quá 100 ký tự")]
        public string Position { get; set; }

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        public string Notes { get; set; }

        // Avatar file
        public IFormFile AvatarFile { get; set; }

        // Permissions
        public List<string> Permissions { get; set; } = new List<string>();
    }

    /// <summary>
    /// ViewModel thay đổi mật khẩu admin
    /// </summary>
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có từ 8 đến 100 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// ViewModel thống kê tài khoản admin
    /// </summary>
    public class AdminAccountStatisticsViewModel
    {
        public int TotalAdmins { get; set; }
        public int ActiveAdmins { get; set; }
        public int InactiveAdmins { get; set; }
        public int LockedAdmins { get; set; }
        public int OnlineAdmins { get; set; }
        public int NewAdminsThisMonth { get; set; }
        public int NewAdminsThisWeek { get; set; }

        // Role statistics
        public Dictionary<string, int> AdminsByRole { get; set; } = new Dictionary<string, int>();

        // Department statistics
        public Dictionary<string, int> AdminsByDepartment { get; set; } = new Dictionary<string, int>();

        // Activity statistics
        public int LoginsToday { get; set; }
        public int LoginsThisWeek { get; set; }
        public int LoginsThisMonth { get; set; }

        // Charts data
        public List<ChartDataPoint> AdminGrowthChart { get; set; } = new List<ChartDataPoint>();
        public List<ChartDataPoint> AdminStatusChart { get; set; } = new List<ChartDataPoint>();
        public List<ChartDataPoint> AdminRoleChart { get; set; } = new List<ChartDataPoint>();
        public List<ChartDataPoint> LoginActivityChart { get; set; } = new List<ChartDataPoint>();
    }

    /// <summary>
    /// ViewModel lịch sử đăng nhập
    /// </summary>
    public class LoginHistoryViewModel
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
    }

    /// <summary>
    /// ViewModel tìm kiếm admin
    /// </summary>
    public class SearchAdminAccountViewModel
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
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortDirection { get; set; } = "DESC";
    }

    /// <summary>
    /// ViewModel kết quả tìm kiếm có phân trang
    /// </summary>
    public class PagedAdminAccountResult
    {
        public List<AdminAccountViewModel> Admins { get; set; } = new List<AdminAccountViewModel>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        // Additional info
        public AdminAccountStatisticsViewModel Statistics { get; set; }
        public List<string> AvailableRoles { get; set; } = new List<string>();
        public List<string> AvailableStatuses { get; set; } = new List<string>();
        public List<string> AvailableDepartments { get; set; } = new List<string>();
    }

    /// <summary>
    /// ViewModel thay đổi trạng thái admin
    /// </summary>
    public class ChangeAdminStatusViewModel
    {
        [Required(ErrorMessage = "Trạng thái mới không được để trống")]
        public string NewStatus { get; set; }

        public string Reason { get; set; }
        public bool SendNotification { get; set; } = true;
    }

    /// <summary>
    /// ViewModel khóa/mở khóa admin
    /// </summary>
    public class LockAdminAccountViewModel
    {
        [Required(ErrorMessage = "ID admin không được để trống")]
        public long AdminId { get; set; }

        public bool IsLocked { get; set; }

        [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        public string Reason { get; set; }

        public DateTime? LockUntil { get; set; }
        public bool SendNotification { get; set; } = true;
    }

    /// <summary>
    /// ViewModel phân quyền admin
    /// </summary>
    public class AssignPermissionsViewModel
    {
        [Required(ErrorMessage = "ID admin không được để trống")]
        public long AdminId { get; set; }

        [Required(ErrorMessage = "Danh sách quyền không được để trống")]
        public List<string> Permissions { get; set; } = new List<string>();

        public string Reason { get; set; }
        public bool SendNotification { get; set; } = true;
    }

    /// <summary>
    /// ViewModel điểm dữ liệu cho biểu đồ
    /// </summary>
    public class ChartDataPoint
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public string Color { get; set; }
        public DateTime? Date { get; set; }
    }


}
