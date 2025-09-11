using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Feature Usage Tracking - Track actual feature usage
    /// </summary>
    [Table("feature_usage_tracking")]
    public class FeatureUsageTracking : BaseEntity
    {
        public long UserId { get; set; }

        public long FeatureId { get; set; }

        public DateTime UsageDate { get; set; }

        public int UsageCount { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ResourceAmount { get; set; } = 1;

        [StringLength(100)]
        public string UsageType { get; set; } = "Access";

        public string? Metadata { get; set; } // JSON metadata

        // Navigation properties
        [ForeignKey("FeatureId")]
        public virtual Feature Feature { get; set; } = null!;
    }
}
