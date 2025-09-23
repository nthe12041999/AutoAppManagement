using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class FeatureUsageTracking: BaseOriginEntity
{
    public long UserId { get; set; }

    public long FeatureId { get; set; }

    public DateTime UsageDate { get; set; }

    public int UsageCount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ResourceAmount { get; set; }

    [StringLength(100)]
    public string UsageType { get; set; }

    public string? Metadata { get; set; }

    public DateTime CreatedDate { get; set; }

    [ForeignKey("FeatureId")]
    [InverseProperty("FeatureUsageTrackings")]
    public virtual Feature Feature { get; set; }
}
