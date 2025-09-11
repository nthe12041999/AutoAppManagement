using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Features master table - Chứa tất cả features có thể có trong hệ thống
    /// </summary>
    [Table("features")]
    public class Feature : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(200)]
        public string? Icon { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsBeta { get; set; } = false;

        public int PriorityOrder { get; set; } = 0;

        [StringLength(100)]
        public string? ResourceType { get; set; }

        public int? DefaultLimit { get; set; }
    }

    /// <summary>
    /// Feature Usage Summary - View model cho thống kê
    /// </summary>
    public class FeatureUsageSummary
    {
        public long UserId { get; set; }
        public long FeatureId { get; set; }
        public string FeatureCode { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int TotalUsage { get; set; }
        public decimal TotalResourceConsumed { get; set; }
        public DateTime FirstUsed { get; set; }
        public DateTime LastUsed { get; set; }
        public int UsageDays { get; set; }
    }
}
