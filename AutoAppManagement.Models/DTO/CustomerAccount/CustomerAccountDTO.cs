using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.CustomerAccount
{
    /// <summary>
    /// DTO cho đăng nhập với thông tin device
    /// </summary>
    public class CustomerLoginDTO
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
    public class CustomerLoginResponseDTO
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DeviceId { get; set; }
        public CustomerLicenseInfoDTO LicenseInfo { get; set; }
        public string AccessToken { get; set; }
        public DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// DTO thông tin license
    /// </summary>
    public class CustomerLicenseInfoDTO
    {
        public string LicenseKey { get; set; }
        public string LicenseName { get; set; }
        public string LicenseType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsValid { get; set; }
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public string Status { get; set; }
        public int DaysUntilExpiry { get; set; }
    }

    /// <summary>
    /// DTO đăng ký thiết bị
    /// </summary>
    public class RegisterDeviceDTO
    {
        [Required(ErrorMessage = "ID tài khoản không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Thông tin thiết bị không được để trống")]
        public DeviceInfoDTO DeviceInfo { get; set; }
    }

    /// <summary>
    /// DTO thông tin thiết bị khách hàng
    /// </summary>
    public class CustomerDeviceDTO
    {
        public long Id { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string BrowserInfo { get; set; }
        public string IpAddress { get; set; }
        public string Status { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsPrimaryDevice { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO tạo license
    /// </summary>
    public class CreateLicenseDTO
    {
        [Required(ErrorMessage = "ID tài khoản không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Tên license không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên license không được vượt quá 100 ký tự")]
        public string LicenseName { get; set; }

        [Required(ErrorMessage = "Loại license không được để trống")]
        [MaxLength(50, ErrorMessage = "Loại license không được vượt quá 50 ký tự")]
        public string LicenseType { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; }

        [Range(1, 100, ErrorMessage = "Số thiết bị tối đa phải từ 1 đến 100")]
        public int MaxDevices { get; set; } = 1;

        [Range(1, 1000, ErrorMessage = "Số user tối đa phải từ 1 đến 1000")]
        public int MaxUsers { get; set; } = 1;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        public DateTime ExpiryDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [MaxLength(10, ErrorMessage = "Đơn vị tiền tệ không được vượt quá 10 ký tự")]
        public string Currency { get; set; } = "VND";

        public string AllowedFeatures { get; set; }
        public string UsageLimits { get; set; }
    }

    /// <summary>
    /// DTO gia hạn license
    /// </summary>
    public class RenewLicenseDTO
    {
        [Required(ErrorMessage = "License key không được để trống")]
        public string LicenseKey { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn mới không được để trống")]
        public DateTime NewExpiryDate { get; set; }
    }

    /// <summary>
    /// DTO xóa thiết bị
    /// </summary>
    public class RemoveDeviceDTO
    {
        [Required(ErrorMessage = "Device ID không được để trống")]
        public string DeviceId { get; set; }

        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }
    }

    /// <summary>
    /// DTO kiểm tra quyền truy cập
    /// </summary>
    public class ValidateAccessDTO
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Device ID không được để trống")]
        public string DeviceId { get; set; }
    }

    /// <summary>
    /// DTO kết quả kiểm tra quyền truy cập
    /// </summary>
    public class AccessValidationResultDTO
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public CustomerLicenseInfoDTO LicenseInfo { get; set; }
        public CustomerDeviceDTO DeviceInfo { get; set; }
        public DateTime ValidationTime { get; set; }
    }

    /// <summary>
    /// DTO thống kê license
    /// </summary>
    public class LicenseStatisticsDTO
    {
        public Dictionary<string, int> LicenseTypeCount { get; set; }
        public int TotalActiveLicenses { get; set; }
        public int TotalExpiredLicenses { get; set; }
        public int TotalExpiringLicenses { get; set; }
        public DateTime StatisticsDate { get; set; }
    }

    /// <summary>
    /// DTO thống kê thiết bị
    /// </summary>
    public class DeviceStatisticsDTO
    {
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public int InactiveDevices { get; set; }
        public Dictionary<string, int> DeviceTypeCount { get; set; }
        public Dictionary<string, int> OperatingSystemCount { get; set; }
        public DateTime StatisticsDate { get; set; }
    }
}
