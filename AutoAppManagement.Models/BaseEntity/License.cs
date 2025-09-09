using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class License : BaseEntity
{
    /// <summary>
    /// Mã license duy nhất
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string LicenseKey { get; set; } = string.Empty;

    /// <summary>
    /// Tên gói license
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LicenseName { get; set; } = string.Empty;

    /// <summary>
    /// Loại license (Basic, Premium, Enterprise, etc.)
    /// </summary>
    public LicenseType LicenseType { get; set; }

    /// <summary>
    /// Mô tả chi tiết về license
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

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
    public DateTime StartDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Ngày hết hạn
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Giá trị license (để tính toán)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Giảm giá
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    /// <summary>
    /// Thông tin thanh toán
    /// </summary>
    [MaxLength(500)]
    public string? PaymentInfo { get; set; }

    /// <summary>
    /// Các tính năng được phép (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? AllowedFeatures { get; set; }

    /// <summary>
    /// Giới hạn sử dụng (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? UsageLimits { get; set; }

    // Navigation properties
    public virtual Account? CreatedByNavigation { get; set; }
    public virtual Account? UpdatedByNavigation { get; set; }
    public virtual ICollection<LicenseFeature> LicenseFeatures { get; set; } = new List<LicenseFeature>();
}
