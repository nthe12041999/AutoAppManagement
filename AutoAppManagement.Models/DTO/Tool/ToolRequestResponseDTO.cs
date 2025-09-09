using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Tool
{
    /// <summary>
    /// Request DTO cho tìm kiếm tools
    /// </summary>
    public class ToolSearchRequestDTO
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? ToolType { get; set; }
        public bool? IsActive { get; set; }
        public bool? RequiresLicense { get; set; }
        public bool? IsPublic { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "ToolName";
        public string? SortDirection { get; set; } = "ASC";
    }

    /// <summary>
    /// Response DTO cho tìm kiếm tools
    /// </summary>
    public class ToolSearchResponseDTO
    {
        public List<ToolDTO> Tools { get; set; } = new List<ToolDTO>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    /// <summary>
    /// Request DTO cho tạo tool
    /// </summary>
    public class ToolCreateRequestDTO
    {
        [Required(ErrorMessage = "Tên tool không được để trống")]
        [StringLength(200, ErrorMessage = "Tên tool không được vượt quá 200 ký tự")]
        public string ToolName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã tool không được để trống")]
        [StringLength(50, ErrorMessage = "Mã tool không được vượt quá 50 ký tự")]
        public string ToolCode { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        public string Category { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Loại tool không được vượt quá 20 ký tự")]
        public string ToolType { get; set; } = "Standard";

        public bool IsActive { get; set; } = true;
        public bool IsPublic { get; set; } = true;
        public bool RequiresLicense { get; set; } = true;

        [StringLength(20, ErrorMessage = "Version không được vượt quá 20 ký tự")]
        public string Version { get; set; } = "1.0.0";

        [StringLength(1000, ErrorMessage = "Download URL không được vượt quá 1000 ký tự")]
        public string? DownloadUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Documentation URL không được vượt quá 1000 ký tự")]
        public string? DocumentationUrl { get; set; }

        [StringLength(200, ErrorMessage = "Supported platforms không được vượt quá 200 ký tự")]
        public string? SupportedPlatforms { get; set; }

        [StringLength(500, ErrorMessage = "Requirements không được vượt quá 500 ký tự")]
        public string? Requirements { get; set; }

        public decimal? FileSize { get; set; }

        [StringLength(2000, ErrorMessage = "Release notes không được vượt quá 2000 ký tự")]
        public string? ReleaseNotes { get; set; }
    }

    /// <summary>
    /// Response DTO cho tạo tool
    /// </summary>
    public class ToolCreateResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ToolDTO? Tool { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Request DTO cho cập nhật tool
    /// </summary>
    public class ToolUpdateRequestDTO : ToolCreateRequestDTO
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [StringLength(20, ErrorMessage = "Status không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Active";

        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// Response DTO cho cập nhật tool
    /// </summary>
    public class ToolUpdateResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ToolDTO? Tool { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Request DTO cho tạo tool version
    /// </summary>
    public class ToolVersionCreateRequestDTO
    {
        [Required(ErrorMessage = "Tool ID không được để trống")]
        public long ToolId { get; set; }

        [Required(ErrorMessage = "Version không được để trống")]
        [StringLength(20, ErrorMessage = "Version không được vượt quá 20 ký tự")]
        public string Version { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Tên version không được vượt quá 500 ký tự")]
        public string? VersionName { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [StringLength(2000, ErrorMessage = "Release notes không được vượt quá 2000 ký tự")]
        public string? ReleaseNotes { get; set; }

        [StringLength(1000, ErrorMessage = "Download URL không được vượt quá 1000 ký tự")]
        public string? DownloadUrl { get; set; }

        [StringLength(500, ErrorMessage = "Requirements không được vượt quá 500 ký tự")]
        public string? Requirements { get; set; }

        public decimal? FileSize { get; set; }

        [StringLength(100, ErrorMessage = "File hash không được vượt quá 100 ký tự")]
        public string? FileHash { get; set; }

        public bool IsStable { get; set; } = true;
        public bool IsSupported { get; set; } = true;

        public DateTime? ReleaseDate { get; set; }
        public DateTime? SupportEndDate { get; set; }

        [StringLength(50, ErrorMessage = "Minimum system version không được vượt quá 50 ký tự")]
        public string? MinimumSystemVersion { get; set; }

        [StringLength(1000, ErrorMessage = "Dependencies không được vượt quá 1000 ký tự")]
        public string? Dependencies { get; set; }
    }

    /// <summary>
    /// Response DTO cho tạo tool version
    /// </summary>
    public class ToolVersionCreateResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ToolVersionDTO? ToolVersion { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Request DTO cho cập nhật tool version
    /// </summary>
    public class ToolVersionUpdateRequestDTO : ToolVersionCreateRequestDTO
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [StringLength(20, ErrorMessage = "Status không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Active";
    }

    /// <summary>
    /// Response DTO cho cập nhật tool version
    /// </summary>
    public class ToolVersionUpdateResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ToolVersionDTO? ToolVersion { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO cho so sánh versions
    /// </summary>
    public class VersionComparisonDTO
    {
        public ToolVersionDTO? Version1 { get; set; }
        public ToolVersionDTO? Version2 { get; set; }
        public List<string> NewFeatures { get; set; } = new List<string>();
        public List<string> RemovedFeatures { get; set; } = new List<string>();
        public List<string> ChangedFeatures { get; set; } = new List<string>();
        public List<string> BugFixes { get; set; } = new List<string>();
        public bool IsCompatible { get; set; }
        public string? CompatibilityNotes { get; set; }
        public string? RecommendationNotes { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê tool category
    /// </summary>
    public class ToolCategoryStatsDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public int TotalTools { get; set; }
        public int ActiveTools { get; set; }
        public int PublicTools { get; set; }
        public int PrivateTools { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalDownloads { get; set; }
        public decimal AverageRating { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<ToolDTO>? TopTools { get; set; }
    }
}
