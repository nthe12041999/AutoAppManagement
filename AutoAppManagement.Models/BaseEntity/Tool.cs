using AutoAppManagement.Models.Common;
using System.ComponentModel.DataAnnotations;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Entity quản lý thông tin tool
    /// </summary>
    public class Tool : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string ToolName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ToolCode { get; set; } = string.Empty; // Unique identifier

        [StringLength(500)]         
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // AI, Image, Text, etc.

        /// <summary>
        /// Loại tool
        /// </summary>
        public ToolType ToolType { get; set; } = ToolType.Utility;

        [StringLength(20)]
        public string CurrentVersion { get; set; } = "1.0.0";

        [StringLength(500)]
        public string? IconUrl { get; set; }

        [StringLength(1000)]
        public string? DocumentationUrl { get; set; }

        public bool IsPublic { get; set; } = true; // Công khai hay private

        /// <summary>
        /// Trạng thái tool
        /// </summary>
        public new StatusType Status { get; set; } = StatusType.Active;

        // Navigation properties
        public virtual ICollection<ToolVersion> ToolVersions { get; set; } = new List<ToolVersion>();
        public virtual ICollection<ToolFeature> ToolFeatures { get; set; } = new List<ToolFeature>();
        public virtual ICollection<ToolCategory> ToolCategories { get; set; } = new List<ToolCategory>();
    }

    /// <summary>
    /// Entity quản lý version của tool
    /// </summary>
    public class ToolVersion : BaseEntity
    {
        public long ToolId { get; set; }

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = string.Empty; // 1.0.0, 1.1.0, etc.

        [StringLength(500)]
        public string? VersionName { get; set; } // Codename for version

        [StringLength(2000)]
        public string? ReleaseNotes { get; set; }

        [StringLength(1000)]
        public string? DownloadUrl { get; set; }

        public long? FileSize { get; set; } // Bytes

        [StringLength(100)]
        public string? FileHash { get; set; } // SHA256 hash

        public bool IsStable { get; set; } = true; // Stable, Beta, Alpha

        public bool IsLatest { get; set; } = false;

        public bool IsSupported { get; set; } = true;

        public DateTime ReleaseDate { get; set; }

        public DateTime? SupportEndDate { get; set; }

        [StringLength(50)]
        public string? MinimumSystemVersion { get; set; }

        [StringLength(1000)]
        public string? Dependencies { get; set; } // JSON format

        [StringLength(20)]
        public new StatusType Status { get; set; } = StatusType.Active;

        // Navigation properties
        public virtual Tool Tool { get; set; } = null!;
        public virtual ICollection<ToolFeature> ToolFeatures { get; set; } = new List<ToolFeature>();
    }

    /// <summary>
    /// Entity quản lý category của tool
    /// </summary>
    public class ToolCategory : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string CategoryCode { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? IconUrl { get; set; }

        [StringLength(7)]
        public string? ColorCode { get; set; } // Hex color

        public long? ParentCategoryId { get; set; }

        public int SortOrder { get; set; } = 0;

        public new bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ToolCategory? ParentCategory { get; set; }
        public virtual ICollection<ToolCategory> SubCategories { get; set; } = new List<ToolCategory>();
        public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
    }

    /// <summary>
    /// Entity quản lý compatibility giữa tool versions
    /// </summary>
    public class ToolCompatibility : BaseEntity
    {
        public long ToolVersionId { get; set; }

        public long CompatibleWithVersionId { get; set; }

        [StringLength(20)]
        public string CompatibilityType { get; set; } = "Forward"; // Forward, Backward, Full

        [StringLength(500)]
        public new string? Notes { get; set; }

        public bool IsVerified { get; set; } = false;

        // Navigation properties
        public virtual ToolVersion ToolVersion { get; set; } = null!;
        public virtual ToolVersion CompatibleWithVersion { get; set; } = null!;
    }

    /// <summary>
    /// Entity quản lý dependency giữa các tools
    /// </summary>
    public class ToolDependency : BaseEntity
    {
        public long ToolId { get; set; }

        public long DependentToolId { get; set; }

        [StringLength(20)]
        public string? RequiredVersion { get; set; }

        [StringLength(20)]
        public string? MinimumVersion { get; set; }

        [StringLength(20)]
        public string? MaximumVersion { get; set; }

        [StringLength(20)]
        public string DependencyType { get; set; } = "Required"; // Required, Optional, Recommended

        [StringLength(500)]
        public new string? Notes { get; set; }

        public new bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Tool Tool { get; set; } = null!;
        public virtual Tool DependentTool { get; set; } = null!;
    }
}
