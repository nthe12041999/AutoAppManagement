using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class License : BaseEntity
{
    /// <summary>
    /// ID của tài khoản khách hàng
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// Mã license duy nhất
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string LicenseKey { get; set; }

    /// <summary>
    /// Tên gói license
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LicenseName { get; set; }

    /// <summary>
    /// Loại license (Basic, Premium, Enterprise, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LicenseType { get; set; }

    /// <summary>
    /// Mô tả chi tiết về license
    /// </summary>
    [MaxLength(1000)]
    public string Description { get; set; }

    /// <summary>
    /// Số lượng thiết bị tối đa được phép
    /// </summary>
    public int MaxDevices { get; set; } = 1;

    /// <summary>
    /// Số lượng user tối đa được phép
    /// </summary>
    public int MaxUsers { get; set; } = 1;

    /// <summary>
    /// Ngày bắt đầu hiệu lực
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày hết hạn
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Có tự động gia hạn không
    /// </summary>
    public bool IsAutoRenewal { get; set; } = false;

    /// <summary>
    /// Giá trị license (để tính toán)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    /// <summary>
    /// Thông tin thanh toán
    /// </summary>
    [MaxLength(500)]
    public string PaymentInfo { get; set; }

    /// <summary>
    /// Các tính năng được phép (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string AllowedFeatures { get; set; }

    /// <summary>
    /// Giới hạn sử dụng (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string UsageLimits { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
    public virtual Account CreatedByNavigation { get; set; }
    public virtual Account UpdatedByNavigation { get; set; }
}
