using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

/// <summary>
/// Entity cho tính năng tool
/// </summary>
public partial class ToolFeature : BaseEntity
{
    /// <summary>
    /// ID của tool chứa feature này
    /// </summary>
    public long ToolId { get; set; }

    /// <summary>
    /// ID của tool version (optional, null = áp dụng cho tất cả versions)
    /// </summary>
    public long? ToolVersionId { get; set; }

    /// <summary>
    /// Mã định danh tính năng (unique trong scope của tool)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FeatureCode { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị của tính năng
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FeatureName { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả chi tiết về tính năng
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Danh mục tính năng (Analytics, Reporting, Export, etc.)
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Loại tính năng (Feature, Resource, API)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FeatureType { get; set; } = "Feature";

    /// <summary>
    /// Độ ưu tiên của tính năng (để sắp xếp hiển thị)
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Có yêu cầu license để sử dụng không
    /// </summary>
    public bool RequiresLicense { get; set; } = true;

    /// <summary>
    /// Giới hạn mặc định (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? DefaultLimits { get; set; }

    /// <summary>
    /// Metadata bổ sung (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? Metadata { get; set; }

    // Navigation properties
    public virtual ICollection<LicenseFeature> LicenseFeatures { get; set; } = new List<LicenseFeature>();
    public virtual ICollection<FeatureUsage> FeatureUsages { get; set; } = new List<FeatureUsage>();
    public virtual Account? CreatedByNavigation { get; set; }
    public virtual Account? UpdatedByNavigation { get; set; }
}

/// <summary>
/// Entity mapping giữa License và ToolFeature
/// </summary>
public partial class LicenseFeature : BaseEntity
{
    /// <summary>
    /// ID của license
    /// </summary>
    [Required]
    public long LicenseId { get; set; }

    /// <summary>
    /// ID của tool feature
    /// </summary>
    [Required]
    public long ToolFeatureId { get; set; }

    /// <summary>
    /// Tính năng có được bật cho license này không
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Giới hạn tài nguyên cho tính năng này (JSON format)
    /// Example: {"maxCalls": 1000, "maxStorage": "10GB", "maxConcurrent": 5}
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? ResourceLimits { get; set; }

    /// <summary>
    /// Quota sử dụng (JSON format)
    /// Example: {"daily": 100, "monthly": 2000, "total": 10000}
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? UsageQuota { get; set; }

    /// <summary>
    /// Ngày bắt đầu hiệu lực
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// Ngày kết thúc hiệu lực
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Cấu hình bổ sung (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? Configuration { get; set; }

    // Navigation properties
    public virtual License License { get; set; } = null!;
    public virtual ToolFeature ToolFeature { get; set; } = null!;
    public virtual Account? CreatedByNavigation { get; set; }
    public virtual Account? UpdatedByNavigation { get; set; }
}

/// <summary>
/// Entity tracking việc sử dụng tính năng
/// </summary>
public partial class FeatureUsage : BaseEntity
{
    /// <summary>
    /// ID của account sử dụng
    /// </summary>
    [Required]
    public long AccountId { get; set; }

    /// <summary>
    /// ID của license được sử dụng
    /// </summary>
    [Required]
    public long LicenseId { get; set; }

    /// <summary>
    /// ID của tool feature được sử dụng
    /// </summary>
    [Required]
    public long ToolFeatureId { get; set; }

    /// <summary>
    /// Loại sử dụng (Access, Resource, API_Call, Storage, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UsageType { get; set; } = "Access";

    /// <summary>
    /// Số lần sử dụng
    /// </summary>
    public int UsageCount { get; set; } = 1;

    /// <summary>
    /// Lượng tài nguyên đã tiêu thụ
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal ResourceConsumed { get; set; } = 0;

    /// <summary>
    /// Dữ liệu chi tiết về việc sử dụng (JSON format)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? UsageData { get; set; }

    /// <summary>
    /// Thời điểm sử dụng
    /// </summary>
    public DateTime UsageDate { get; set; }

    /// <summary>
    /// IP address của user
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Session ID
    /// </summary>
    [MaxLength(100)]
    public string? SessionId { get; set; }

    /// <summary>
    /// Request ID để tracking
    /// </summary>
    [MaxLength(100)]
    public string? RequestId { get; set; }

    // Navigation properties
    public virtual Tool Tool { get; set; } = null!;
    public virtual ToolVersion? ToolVersion { get; set; }
    public virtual ICollection<LicenseFeature> LicenseFeatures { get; set; } = new List<LicenseFeature>();
    public virtual ICollection<FeatureUsage> FeatureUsages { get; set; } = new List<FeatureUsage>();
}

/// <summary>
/// Entity theo dõi việc sử dụng tính năng
/// </summary>
public partial class FeatureUsage : BaseEntity
{
}
