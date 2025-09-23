using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Feature: BaseOriginEntity
{

    [StringLength(100)]
    public string Code { get; set; }

    [StringLength(200)]
    public string Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(200)]
    public string? Icon { get; set; }

    public bool IsBeta { get; set; }

    public int PriorityOrder { get; set; }

    [StringLength(100)]
    public string? ResourceType { get; set; }

    public int? DefaultLimit { get; set; }

    [InverseProperty("Feature")]
    public virtual ICollection<FeatureUsageTracking> FeatureUsageTrackings { get; set; } = new List<FeatureUsageTracking>();
}
