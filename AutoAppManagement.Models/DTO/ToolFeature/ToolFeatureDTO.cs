using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.ToolFeature
{
    /// <summary>
    /// DTO cho tính năng tool
    /// </summary>
    public class ToolFeatureDTO : BaseEntity.ToolFeature, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    /// <summary>
    /// DTO cho license feature (mapping giữa license và feature)
    /// </summary>
    public class LicenseFeatureDTO : BaseEntity.LicenseFeature, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    /// <summary>
    /// DTO cho usage tracking
    /// </summary>
    public class FeatureUsageDTO : BaseEntity.FeatureUsage, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    // Request DTOs
    public class CreateToolFeatureRequest
    {
        [Required(ErrorMessage = "Mã tính năng không được để trống")]
        [MaxLength(100)]
        public string FeatureCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên tính năng không được để trống")]
        [MaxLength(200)]
        public string FeatureName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Loại tính năng không được để trống")]
        public string FeatureType { get; set; } = "Feature";

        public bool RequiresLicense { get; set; } = true;
        public string? DefaultLimits { get; set; }
    }

    public class UpdateToolFeatureRequest
    {
        [Required]
        public long Id { get; set; }

        [MaxLength(200)]
        public string? FeatureName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public bool? IsActive { get; set; }
        public bool? RequiresLicense { get; set; }
        public string? DefaultLimits { get; set; }
        public string? Status { get; set; }
    }

    public class AssignFeatureToLicenseRequest
    {
        [Required]
        public long LicenseId { get; set; }

        [Required]
        public long ToolFeatureId { get; set; }

        public bool IsEnabled { get; set; } = true;
        public string? ResourceLimits { get; set; }
        public string? UsageQuota { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }

    public class CheckFeatureAccessRequest
    {
        [Required]
        public long AccountId { get; set; }

        [Required]
        public string FeatureCode { get; set; } = string.Empty;

        public string? LicenseKey { get; set; }
        public string UsageType { get; set; } = "Access";
        public decimal ResourceAmount { get; set; } = 1;
    }

    // Response DTOs
    public class FeatureAccessCheckResult
    {
        public bool HasAccess { get; set; }
        public string? Reason { get; set; }
        public bool IsLicenseValid { get; set; }
        public bool IsFeatureEnabled { get; set; }
        public bool IsWithinLimits { get; set; }
        public FeatureLimitInfo? LimitInfo { get; set; }
        public LicenseFeatureDTO? LicenseFeature { get; set; }
    }

    public class FeatureLimitInfo
    {
        public string LimitType { get; set; } = string.Empty; // Daily, Monthly, Total
        public decimal MaxAllowed { get; set; }
        public decimal CurrentUsage { get; set; }
        public decimal Remaining => Math.Max(0, MaxAllowed - CurrentUsage);
        public bool IsExceeded => CurrentUsage >= MaxAllowed;
        public DateTime? ResetDate { get; set; }
    }

    // Search and Filter DTOs
    public class ToolFeatureSearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? FeatureType { get; set; }
        public bool? IsActive { get; set; }
        public bool? RequiresLicense { get; set; }
        public string? Status { get; set; }
    }

    public class FeatureUsageReportRequest
    {
        public long? AccountId { get; set; }
        public long? LicenseId { get; set; }
        public string? FeatureCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? UsageType { get; set; }
        public string? ReportType { get; set; } = "Summary"; // Summary, Detailed
    }

    public class FeatureUsageReport
    {
        public string FeatureCode { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
        public int TotalUsageCount { get; set; }
        public decimal TotalResourceConsumed { get; set; }
        public DateTime FirstUsage { get; set; }
        public DateTime LastUsage { get; set; }
        public List<DailyUsageInfo> DailyUsage { get; set; } = new();
    }

    public class DailyUsageInfo
    {
        public DateTime Date { get; set; }
        public int UsageCount { get; set; }
        public decimal ResourceConsumed { get; set; }
    }
}
