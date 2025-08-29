using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public string DeviceId { get; set; }

    /// <summary>
    /// Tên thiết bị
    /// </summary>
    [MaxLength(255)]
    public string DeviceName { get; set; }

    /// <summary>
    /// Loại thiết bị (Mobile, Desktop, Tablet, etc.)
    /// </summary>
    [MaxLength(50)]
    public string DeviceType { get; set; }

    /// <summary>
    /// Hệ điều hành (Windows, Android, iOS, etc.)
    /// </summary>
    [MaxLength(100)]
    public string OperatingSystem { get; set; }

    /// <summary>
    /// Phiên bản hệ điều hành
    /// </summary>
    [MaxLength(50)]
    public string OSVersion { get; set; }

    /// <summary>
    /// Thông tin trình duyệt (nếu là web)
    /// </summary>
    [MaxLength(255)]
    public string BrowserInfo { get; set; }

    /// <summary>
    /// Địa chỉ IP khi đăng ký device
    /// </summary>
    [MaxLength(45)]
    public string IpAddress { get; set; }

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
    public virtual Account Account { get; set; }
}
