using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.AccountDevice
{
    public class AccountDeviceDTO
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string OSVersion { get; set; } = string.Empty;
        public string BrowserInfo { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsPrimaryDevice { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class RegisterDeviceRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Device ID không được để trống")]
        [StringLength(255, ErrorMessage = "Device ID không được vượt quá 255 ký tự")]
        public string DeviceId { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Tên thiết bị không được vượt quá 255 ký tự")]
        public string DeviceName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Loại thiết bị không được vượt quá 50 ký tự")]
        public string DeviceType { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Hệ điều hành không được vượt quá 100 ký tự")]
        public string OperatingSystem { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Phiên bản OS không được vượt quá 50 ký tự")]
        public string OSVersion { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Thông tin trình duyệt không được vượt quá 255 ký tự")]
        public string BrowserInfo { get; set; } = string.Empty;

        [StringLength(45, ErrorMessage = "Địa chỉ IP không được vượt quá 45 ký tự")]
        public string IpAddress { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateDeviceRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [StringLength(255, ErrorMessage = "Tên thiết bị không được vượt quá 255 ký tự")]
        public string DeviceName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Loại thiết bị không được vượt quá 50 ký tự")]
        public string DeviceType { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Hệ điều hành không được vượt quá 100 ký tự")]
        public string OperatingSystem { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Phiên bản OS không được vượt quá 50 ký tự")]
        public string OSVersion { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Thông tin trình duyệt không được vượt quá 255 ký tự")]
        public string BrowserInfo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }
}
