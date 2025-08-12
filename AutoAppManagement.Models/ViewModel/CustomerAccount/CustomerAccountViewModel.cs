using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.ViewModel.CustomerAccount
{
    /// <summary>
    /// ViewModel cho trang đăng nhập khách hàng
    /// </summary>
    public class CustomerLoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }

        // Thông tin thiết bị (sẽ được lấy từ JavaScript)
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string BrowserInfo { get; set; }
    }

    /// <summary>
    /// ViewModel cho trang quản lý thiết bị
    /// </summary>
    public class DeviceManagementViewModel
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public List<CustomerDeviceViewModel> Devices { get; set; } =
            new List<CustomerDeviceViewModel>();
        public int MaxDevicesAllowed { get; set; }
        public bool CanAddMoreDevices => Devices.Count < MaxDevicesAllowed;
    }

    /// <summary>
    /// ViewModel cho thiết bị khách hàng
    /// </summary>
    public class CustomerDeviceViewModel
    {
        public long Id { get; set; }
        public string DeviceId { get; set; }

        [Display(Name = "Tên thiết bị")]
        public string DeviceName { get; set; }

        [Display(Name = "Loại thiết bị")]
        public string DeviceType { get; set; }

        [Display(Name = "Hệ điều hành")]
        public string OperatingSystem { get; set; }

        [Display(Name = "Phiên bản OS")]
        public string OSVersion { get; set; }

        [Display(Name = "Thông tin trình duyệt")]
        public string BrowserInfo { get; set; }

        [Display(Name = "Địa chỉ IP")]
        public string IpAddress { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Lần đăng nhập cuối")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Thiết bị chính")]
        public bool IsPrimaryDevice { get; set; }

        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        public string StatusDisplayName =>
            Status switch
            {
                "Active" => "Đang hoạt động",
                "Inactive" => "Không hoạt động",
                "Blocked" => "Bị chặn",
                _ => "Không xác định",
            };

        public string StatusCssClass =>
            Status switch
            {
                "Active" => "badge badge-success",
                "Inactive" => "badge badge-secondary",
                "Blocked" => "badge badge-danger",
                _ => "badge badge-warning",
            };
    }

    /// <summary>
    /// ViewModel cho trang quản lý license
    /// </summary>
    public class LicenseManagementViewModel
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public List<CustomerLicenseViewModel> Licenses { get; set; } =
            new List<CustomerLicenseViewModel>();
        public CustomerLicenseViewModel ActiveLicense { get; set; }
    }

    /// <summary>
    /// ViewModel cho license khách hàng
    /// </summary>
    public class CustomerLicenseViewModel
    {
        public long Id { get; set; }
        public string LicenseKey { get; set; }

        [Display(Name = "Tên license")]
        public string LicenseName { get; set; }

        [Display(Name = "Loại license")]
        public string LicenseType { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Số thiết bị tối đa")]
        public int MaxDevices { get; set; }

        [Display(Name = "Số user tối đa")]
        public int MaxUsers { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày hết hạn")]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Tự động gia hạn")]
        public bool IsAutoRenewal { get; set; }

        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Đơn vị tiền tệ")]
        public string Currency { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedDate { get; set; }

        public bool IsValid => ExpiryDate > DateTime.Now && Status == "Active";
        public int DaysUntilExpiry => (ExpiryDate - DateTime.Now).Days;
        public bool IsExpiringSoon => DaysUntilExpiry <= 30 && DaysUntilExpiry > 0;
        public bool IsExpired => ExpiryDate <= DateTime.Now;

        public string StatusDisplayName =>
            Status switch
            {
                "Active" => "Đang hoạt động",
                "Expired" => "Đã hết hạn",
                "Suspended" => "Bị tạm ngưng",
                "Cancelled" => "Đã hủy",
                _ => "Không xác định",
            };

        public string StatusCssClass =>
            Status switch
            {
                "Active" when IsValid => "badge badge-success",
                "Active" when IsExpiringSoon => "badge badge-warning",
                "Active" when IsExpired => "badge badge-danger",
                "Expired" => "badge badge-danger",
                "Suspended" => "badge badge-warning",
                "Cancelled" => "badge badge-secondary",
                _ => "badge badge-light",
            };
    }

    /// <summary>
    /// ViewModel cho tạo license mới
    /// </summary>
    public class CreateLicenseViewModel
    {
        [Required(ErrorMessage = "ID tài khoản không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Tên license không được để trống")]
        [Display(Name = "Tên license")]
        [MaxLength(100, ErrorMessage = "Tên license không được vượt quá 100 ký tự")]
        public string LicenseName { get; set; }

        [Required(ErrorMessage = "Loại license không được để trống")]
        [Display(Name = "Loại license")]
        public string LicenseType { get; set; }

        [Display(Name = "Mô tả")]
        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Số thiết bị tối đa không được để trống")]
        [Display(Name = "Số thiết bị tối đa")]
        [Range(1, 100, ErrorMessage = "Số thiết bị tối đa phải từ 1 đến 100")]
        public int MaxDevices { get; set; } = 1;

        [Required(ErrorMessage = "Số user tối đa không được để trống")]
        [Display(Name = "Số user tối đa")]
        [Range(1, 1000, ErrorMessage = "Số user tối đa phải từ 1 đến 1000")]
        public int MaxUsers { get; set; } = 1;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        [Display(Name = "Ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddYears(1);

        [Required(ErrorMessage = "Giá không được để trống")]
        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Display(Name = "Đơn vị tiền tệ")]
        public string Currency { get; set; } = "VND";

        [Display(Name = "Các tính năng được phép")]
        public string AllowedFeatures { get; set; }

        [Display(Name = "Giới hạn sử dụng")]
        public string UsageLimits { get; set; }

        // Dropdown options
        public List<string> LicenseTypeOptions { get; set; } =
            new List<string> { "Basic", "Premium", "Enterprise", "Trial" };

        public List<string> CurrencyOptions { get; set; } =
            new List<string> { "VND", "USD", "EUR" };
    }

    /// <summary>
    /// ViewModel cho dashboard khách hàng
    /// </summary>
    public class CustomerDashboardViewModel
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }

        // Thông tin license
        public CustomerLicenseViewModel ActiveLicense { get; set; }
        public bool HasValidLicense => ActiveLicense?.IsValid == true;

        // Thông tin thiết bị
        public List<CustomerDeviceViewModel> RecentDevices { get; set; } =
            new List<CustomerDeviceViewModel>();
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }

        // Thống kê
        public DateTime LastLoginDate { get; set; }
        public string CurrentDeviceId { get; set; }

        // Cảnh báo
        public List<string> Warnings { get; set; } = new List<string>();
        public bool HasWarnings => Warnings.Any();
    }

    /// <summary>
    /// ViewModel cho gia hạn license
    /// </summary>
    public class RenewLicenseViewModel
    {
        [Required(ErrorMessage = "License key không được để trống")]
        public string LicenseKey { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn mới không được để trống")]
        [Display(Name = "Ngày hết hạn mới")]
        [DataType(DataType.Date)]
        public DateTime NewExpiryDate { get; set; }

        // Thông tin hiện tại
        public string CurrentLicenseName { get; set; }
        public DateTime CurrentExpiryDate { get; set; }
        public string LicenseType { get; set; }
    }

    /// <summary>
    /// ViewModel for creating new license
    /// </summary>
    public class CustomerLicenseCreateViewModel
    {
        public string LicenseName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int MaxDevices { get; set; }
        public string AllowedFeatures { get; set; } = string.Empty;
        public string UsageLimits { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// ViewModel for updating existing license
    /// </summary>
    public class CustomerLicenseUpdateViewModel
    {
        public long Id { get; set; }
        public string LicenseName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int MaxDevices { get; set; }
        public string AllowedFeatures { get; set; } = string.Empty;
        public string UsageLimits { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
