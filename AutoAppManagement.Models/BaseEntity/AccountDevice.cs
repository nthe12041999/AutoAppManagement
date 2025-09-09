using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class AccountDevice : BaseEntity
{
    /// <summary>
    /// ID của tài khoản khách hàng
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// Device ID duy nhất của thiết bị
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Tên thiết bị
    /// </summary>
    [MaxLength(255)]
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// Loại thiết bị (Mobile, Desktop, Tablet, etc.)
    /// </summary>
    public DeviceType DeviceType { get; set; } = DeviceType.Desktop;

    /// <summary>
    /// Hệ điều hành (Windows, Android, iOS, etc.)
    /// </summary>
    public OperatingSystemType OperatingSystem { get; set; } = OperatingSystemType.Windows;

    /// <summary>
    /// Phiên bản hệ điều hành
    /// </summary>
    [MaxLength(50)]
    public string OSVersion { get; set; } = string.Empty;

    /// <summary>
    /// Thông tin trình duyệt (nếu là web)
    /// </summary>
    [MaxLength(255)]
    public string BrowserInfo { get; set; } = string.Empty;

    /// <summary>
    /// Địa chỉ IP khi đăng ký device
    /// </summary>
    [MaxLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Lần đăng nhập cuối cùng từ thiết bị này
    /// </summary>
    public DateTime? LastLoginDate { get; set; }

    /// <summary>
    /// Có phải thiết bị chính không
    /// </summary>
    public bool IsPrimaryDevice { get; set; } = false;

    public DateTime? RegisteredDate { get; set; }
    public DateTime? LastAccessDate { get; set; }

    // Navigation property
    public virtual Account? Account { get; set; }
}
