using System.ComponentModel.DataAnnotations;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.ToolFeature;

namespace AutoAppManagement.Models.DTO.Tool
{
    /// <summary>
    /// DTO cho Tool
    /// </summary>
    public class ToolDTO : IStatefulDTO
    {
        public long Id { get; set; }
        
        [Required(ErrorMessage = "Tên tool không được để trống")]
        [StringLength(100, ErrorMessage = "Tên tool không được vượt quá 100 ký tự")]
        public string ToolName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã tool không được để trống")]
        [StringLength(50, ErrorMessage = "Mã tool không được vượt quá 50 ký tự")]
        public string ToolCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category không được để trống")]
        [StringLength(50, ErrorMessage = "Category không được vượt quá 50 ký tự")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại tool không được để trống")]
        [StringLength(20, ErrorMessage = "Loại tool không được vượt quá 20 ký tự")]
        public string ToolType { get; set; } = "Standard";

        [StringLength(20, ErrorMessage = "Version không được vượt quá 20 ký tự")]
        public string CurrentVersion { get; set; } = "1.0.0";

        [StringLength(500, ErrorMessage = "Icon URL không được vượt quá 500 ký tự")]
        public string? IconUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Documentation URL không được vượt quá 1000 ký tự")]
        public string? DocumentationUrl { get; set; }

        public bool IsPublic { get; set; } = true;
        public bool RequiresLicense { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal? PricePerMonth { get; set; }

        [StringLength(10, ErrorMessage = "Currency không được vượt quá 10 ký tự")]
        public string? Currency { get; set; } = "USD";

        [StringLength(20, ErrorMessage = "Status không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Active";

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }

        public EntityState State { get; set; }

        // Related data
        public List<ToolVersionDTO>? Versions { get; set; }
        public List<ToolFeatureDTO>? Features { get; set; }
        public ToolCategoryDTO? CategoryInfo { get; set; }
    }

    /// <summary>
    /// DTO cho Tool Version
    /// </summary>
    public class ToolVersionDTO : IStatefulDTO
    {
        public long Id { get; set; }
        public long ToolId { get; set; }

        [Required(ErrorMessage = "Version không được để trống")]
        [StringLength(20, ErrorMessage = "Version không được vượt quá 20 ký tự")]
        public string Version { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Tên version không được vượt quá 500 ký tự")]
        public string? VersionName { get; set; }

        [StringLength(2000, ErrorMessage = "Release notes không được vượt quá 2000 ký tự")]
        public string? ReleaseNotes { get; set; }

        [StringLength(1000, ErrorMessage = "Download URL không được vượt quá 1000 ký tự")]
        public string? DownloadUrl { get; set; }

        public long? FileSize { get; set; }

        [StringLength(100, ErrorMessage = "File hash không được vượt quá 100 ký tự")]
        public string? FileHash { get; set; }

        public bool IsStable { get; set; } = true;
        public bool IsLatest { get; set; } = false;
        public bool IsSupported { get; set; } = true;

        public DateTime ReleaseDate { get; set; }
        public DateTime? SupportEndDate { get; set; }

        [StringLength(50, ErrorMessage = "Minimum system version không được vượt quá 50 ký tự")]
        public string? MinimumSystemVersion { get; set; }

        [StringLength(1000, ErrorMessage = "Dependencies không được vượt quá 1000 ký tự")]
        public string? Dependencies { get; set; }

        [StringLength(20, ErrorMessage = "Status không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Active";

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }

        public EntityState State { get; set; }

        // Related data
        public ToolDTO? Tool { get; set; }
        public List<ToolFeatureDTO>? Features { get; set; }
    }

    /// <summary>
    /// DTO cho Tool Category
    /// </summary>
    public class ToolCategoryDTO : IStatefulDTO
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Tên category không được để trống")]
        [StringLength(100, ErrorMessage = "Tên category không được vượt quá 100 ký tự")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã category không được để trống")]
        [StringLength(50, ErrorMessage = "Mã category không được vượt quá 50 ký tự")]
        public string CategoryCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Icon URL không được vượt quá 500 ký tự")]
        public string? IconUrl { get; set; }

        [StringLength(7, ErrorMessage = "Color code phải là mã hex 7 ký tự")]
        public string? ColorCode { get; set; }

        public long? ParentCategoryId { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }

        public EntityState State { get; set; }

        // Related data
        public ToolCategoryDTO? ParentCategory { get; set; }
        public List<ToolCategoryDTO>? SubCategories { get; set; }
        public List<ToolDTO>? Tools { get; set; }
        public int ToolCount { get; set; }
    }

    // Request/Response DTOs
    public class CreateToolRequest
    {
        [Required(ErrorMessage = "Tên tool không được để trống")]
        [StringLength(100, ErrorMessage = "Tên tool không được vượt quá 100 ký tự")]
        public string ToolName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã tool không được để trống")]
        [StringLength(50, ErrorMessage = "Mã tool không được vượt quá 50 ký tự")]
        public string ToolCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category không được để trống")]
        [StringLength(50, ErrorMessage = "Category không được vượt quá 50 ký tự")]
        public string Category { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Loại tool không được vượt quá 20 ký tự")]
        public string ToolType { get; set; } = "Standard";

        [StringLength(500, ErrorMessage = "Icon URL không được vượt quá 500 ký tự")]
        public string? IconUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Documentation URL không được vượt quá 1000 ký tự")]
        public string? DocumentationUrl { get; set; }

        public bool IsPublic { get; set; } = true;
        public bool RequiresLicense { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal? PricePerMonth { get; set; }

        [StringLength(10, ErrorMessage = "Currency không được vượt quá 10 ký tự")]
        public string? Currency { get; set; } = "USD";
    }

    public class UpdateToolRequest : CreateToolRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [StringLength(20, ErrorMessage = "Status không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Active";

        public int SortOrder { get; set; } = 0;
    }

    public class CreateToolVersionRequest
    {
        [Required(ErrorMessage = "Tool ID không được để trống")]
        public long ToolId { get; set; }

        [Required(ErrorMessage = "Version không được để trống")]
        [StringLength(20, ErrorMessage = "Version không được vượt quá 20 ký tự")]
        public string Version { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Tên version không được vượt quá 500 ký tự")]
        public string? VersionName { get; set; }

        [StringLength(2000, ErrorMessage = "Release notes không được vượt quá 2000 ký tự")]
        public string? ReleaseNotes { get; set; }

        [StringLength(1000, ErrorMessage = "Download URL không được vượt quá 1000 ký tự")]
        public string? DownloadUrl { get; set; }

        public long? FileSize { get; set; }

        [StringLength(100, ErrorMessage = "File hash không được vượt quá 100 ký tự")]
        public string? FileHash { get; set; }

        public bool IsStable { get; set; } = true;
        public bool IsLatest { get; set; } = false;

        public DateTime? ReleaseDate { get; set; }
        public DateTime? SupportEndDate { get; set; }

        [StringLength(50, ErrorMessage = "Minimum system version không được vượt quá 50 ký tự")]
        public string? MinimumSystemVersion { get; set; }

        [StringLength(1000, ErrorMessage = "Dependencies không được vượt quá 1000 ký tự")]
        public string? Dependencies { get; set; }
    }

    public class CreateToolCategoryRequest
    {
        [Required(ErrorMessage = "Tên category không được để trống")]
        [StringLength(100, ErrorMessage = "Tên category không được vượt quá 100 ký tự")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã category không được để trống")]
        [StringLength(50, ErrorMessage = "Mã category không được vượt quá 50 ký tự")]
        public string CategoryCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Icon URL không được vượt quá 500 ký tự")]
        public string? IconUrl { get; set; }

        [StringLength(7, ErrorMessage = "Color code phải là mã hex 7 ký tự")]
        public string? ColorCode { get; set; }

        public long? ParentCategoryId { get; set; }
        public int SortOrder { get; set; } = 0;
    }

    public class ToolSearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? ToolType { get; set; }
        public bool? RequiresLicense { get; set; }
        public bool? IsPublic { get; set; }
        public string? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "ToolName";
        public string? SortDirection { get; set; } = "ASC";
    }

    public class ToolVersionSearchRequest
    {
        public long? ToolId { get; set; }
        public string? Version { get; set; }
        public bool? IsStable { get; set; }
        public bool? IsLatest { get; set; }
        public bool? IsSupported { get; set; }
        public string? Status { get; set; }
        public DateTime? ReleasedAfter { get; set; }
        public DateTime? ReleasedBefore { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "ReleaseDate";
        public string? SortDirection { get; set; } = "DESC";
    }

    public class ToolStatisticsDTO
    {
        public int TotalTools { get; set; }
        public int PublicTools { get; set; }
        public int PrivateTools { get; set; }
        public int ActiveTools { get; set; }
        public int DeprecatedTools { get; set; }
        public Dictionary<string, int> ToolsByCategory { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ToolsByType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByTool { get; set; } = new Dictionary<string, decimal>();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class ToolVersionComparisonDTO
    {
        public ToolVersionDTO? CurrentVersion { get; set; }
        public ToolVersionDTO? CompareVersion { get; set; }
        public List<string> NewFeatures { get; set; } = new List<string>();
        public List<string> RemovedFeatures { get; set; } = new List<string>();
        public List<string> ChangedFeatures { get; set; } = new List<string>();
        public List<string> BugFixes { get; set; } = new List<string>();
        public bool IsCompatible { get; set; }
        public string? CompatibilityNotes { get; set; }
    }
}
