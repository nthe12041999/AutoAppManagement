using AutoAppManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Feature
{
    /// <summary>
    /// Feature DTO cho Simple Feature Management
    /// </summary>
    public class FeatureDTO : BaseEntity.Feature, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    /// <summary>
    /// Request để tạo Feature mới
    /// </summary>
    public class CreateFeatureRequest
    {
        [Required(ErrorMessage = "Mã feature là bắt buộc")]
        [StringLength(100, ErrorMessage = "Mã feature không được vượt quá 100 ký tự")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên feature là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên feature không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        public string? Category { get; set; }

        [StringLength(200, ErrorMessage = "Icon không được vượt quá 200 ký tự")]
        public string? Icon { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsBeta { get; set; } = false;
        public int PriorityOrder { get; set; } = 0;
        
        [StringLength(100, ErrorMessage = "Resource Type không được vượt quá 100 ký tự")]
        public string? ResourceType { get; set; }
        
        public int? DefaultLimit { get; set; }
    }

    /// <summary>
    /// Request để cập nhật Feature
    /// </summary>
    public class UpdateFeatureRequest
    {
        [Required(ErrorMessage = "ID là bắt buộc")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Tên feature là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên feature không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        public string? Category { get; set; }

        [StringLength(200, ErrorMessage = "Icon không được vượt quá 200 ký tự")]
        public string? Icon { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsBeta { get; set; } = false;
        public int PriorityOrder { get; set; } = 0;
        
        [StringLength(100, ErrorMessage = "Resource Type không được vượt quá 100 ký tự")]
        public string? ResourceType { get; set; }
        
        public int? DefaultLimit { get; set; }
    }

    /// <summary>
    /// Response cho danh sách Feature
    /// </summary>
    public class FeatureListResponse
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
        public bool IsBeta { get; set; }
        public int PriorityOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UsageCount { get; set; } // Số lần được sử dụng
    }

    /// <summary>
    /// Response chi tiết Feature
    /// </summary>
    public class FeatureDetailResponse : FeatureListResponse
    {
        public string? Description { get; set; }
        public string? ResourceType { get; set; }
        public int? DefaultLimit { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        
        /// <summary>
        /// Thống kê sử dụng gần đây
        /// </summary>
        public List<FeatureUsageStatistic> RecentUsage { get; set; } = new();
    }

    /// <summary>
    /// Thống kê sử dụng Feature
    /// </summary>
    public class FeatureUsageStatistic
    {
        public DateTime Date { get; set; }
        public int UsageCount { get; set; }
        public int UniqueUsers { get; set; }
        public decimal TotalResourceConsumed { get; set; }
    }

    /// <summary>
    /// Request tìm kiếm Feature
    /// </summary>
    public class FeatureSearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBeta { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "PriorityOrder";
        public string SortOrder { get; set; } = "ASC";
    }

    /// <summary>
    /// Request gán Feature cho License
    /// </summary>
    public class AssignFeatureToLicenseRequest
    {
        [Required(ErrorMessage = "License ID là bắt buộc")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Danh sách Feature là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một Feature")]
        public List<long> FeatureIds { get; set; } = new();

        /// <summary>
        /// Giới hạn sử dụng cho từng feature (JSON format)
        /// Ví dụ: {"1": {"daily": 100, "monthly": 3000}, "2": {"monthly": 1000}}
        /// </summary>
        public Dictionary<long, Dictionary<string, int>>? FeatureLimits { get; set; }
    }

    /// <summary>
    /// Request cập nhật giới hạn Feature cho License
    /// </summary>
    public class UpdateFeatureLimitsRequest
    {
        [Required(ErrorMessage = "License ID là bắt buộc")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Feature Code là bắt buộc")]
        public string FeatureCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giới hạn là bắt buộc")]
        public Dictionary<string, int> Limits { get; set; } = new();
    }

    #region Simple DTOs cho FeatureManagementService

    /// <summary>
    /// Thông tin chi tiết về một feature
    /// </summary>
    public class FeatureInfo
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
        public bool IsBeta { get; set; }
        public string? ResourceType { get; set; }
        
        /// <summary>
        /// Giới hạn sử dụng hàng ngày
        /// </summary>
        public int? DailyLimit { get; set; }
        
        /// <summary>
        /// Giới hạn sử dụng hàng tháng
        /// </summary>
        public int? MonthlyLimit { get; set; }
        
        /// <summary>
        /// Số lần đã sử dụng hôm nay
        /// </summary>
        public int DailyUsage { get; set; }
        
        /// <summary>
        /// Số lần đã sử dụng tháng này
        /// </summary>
        public int MonthlyUsage { get; set; }
        
        /// <summary>
        /// Có được phép sử dụng không
        /// </summary>
        public bool IsAllowed { get; set; } = true;
        
        /// <summary>
        /// Lý do không được phép (nếu IsAllowed = false)
        /// </summary>
        public string? ReasonNotAllowed { get; set; }

        // Calculated properties
        public double UsagePercentage => MonthlyLimit.HasValue && MonthlyLimit > 0
            ? Math.Round((double)MonthlyUsage / MonthlyLimit.Value * 100, 2)
            : 0;

        public int? RemainingUsage => MonthlyLimit.HasValue
            ? Math.Max(0, MonthlyLimit.Value - MonthlyUsage)
            : null;

        public bool IsOverLimit => MonthlyLimit.HasValue && MonthlyUsage >= MonthlyLimit.Value;
    }

    /// <summary>
    /// Thống kê sử dụng feature
    /// </summary>
    public class FeatureUsageStats
    {
        public string FeatureCode { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int TotalUsage { get; set; }
        public decimal TotalResourceConsumed { get; set; }
        public DateTime FirstUsed { get; set; }
        public DateTime LastUsed { get; set; }
        public int UsageDays { get; set; }
        
        /// <summary>
        /// Thống kê theo ngày (7 ngày gần nhất)
        /// </summary>
        public List<DailyUsage> DailyStats { get; set; } = new();
    }

    /// <summary>
    /// Sử dụng theo ngày
    /// </summary>
    public class DailyUsage
    {
        public DateTime Date { get; set; }
        public int Usage { get; set; }
        public decimal ResourceConsumed { get; set; }
    }

    /// <summary>
    /// Category thông tin
    /// </summary>
    public class FeatureCategory
    {
        public string Name { get; set; } = string.Empty;
        public int FeatureCount { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request gán license cho user
    /// </summary>
    public class AssignLicenseRequest
    {
        [Required(ErrorMessage = "User ID là bắt buộc")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "License ID là bắt buộc")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }

        public bool IsTrial { get; set; } = false;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request kiểm tra nhiều feature cùng lúc
    /// </summary>
    public class BatchFeatureCheckRequest
    {
        [Required(ErrorMessage = "User ID là bắt buộc")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "Danh sách Feature ID là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một Feature ID")]
        public List<long> FeatureIds { get; set; } = new();
    }

    /// <summary>
    /// Response cho batch feature check
    /// </summary>
    public class BatchFeatureCheckResponse
    {
        /// <summary>
        /// Key: FeatureId, Value: IsAllowed
        /// </summary>
        public Dictionary<long, bool> Results { get; set; } = new();
        
        public int TotalChecked => Results.Count;
        public int AllowedCount => Results.Count(r => r.Value);
        public int DeniedCount => Results.Count(r => !r.Value);
        public List<long> AllowedFeatures => Results.Where(x => x.Value).Select(x => x.Key).ToList();
        public List<long> DeniedFeatures => Results.Where(x => !x.Value).Select(x => x.Key).ToList();
        public double AllowedPercentage => TotalChecked > 0
            ? Math.Round((double)AllowedCount / TotalChecked * 100, 2)
            : 0;
    }

    /// <summary>
    /// Request gia hạn license
    /// </summary>
    public class FeatureRenewLicenseRequest
    {
        [Required(ErrorMessage = "User ID là bắt buộc")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "License ID là bắt buộc")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc mới là bắt buộc")]
        public DateTime NewEndDate { get; set; }

        [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        public string? Reason { get; set; }
    }

    #endregion
}