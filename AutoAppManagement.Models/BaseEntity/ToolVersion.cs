using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class ToolVersion : BaseCUEntity
{
    [StringLength(100)]
    public string ToolCode { get; set; } = string.Empty;

    [StringLength(200)]
    public string ToolName { get; set; } = string.Empty;

    [StringLength(50)]
    public string CurrentVersion { get; set; } = string.Empty;

    [StringLength(50)]
    public string? MinimumVersion { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? DownloadUrl { get; set; }

    [Column(TypeName = "ntext")]
    public string? ReleaseNotes { get; set; }

    public DateTime ReleaseDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsRequired { get; set; } = false;

    [StringLength(50)]
    public string? Platform { get; set; }

    public long? FileSize { get; set; }

    [StringLength(255)]
    public string? Checksum { get; set; }

    [Column(TypeName = "ntext")]
    public string? Features { get; set; }

    [Column(TypeName = "ntext")]
    public string? BugFixes { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    public int? Priority { get; set; }

    #region Helper Methods

    /// <summary>
    /// Kiểm tra xem version hiện tại có mới hơn version được truyền vào không
    /// </summary>
    public bool IsNewerThan(string version)
    {
        try
        {
            var current = new Version(CurrentVersion);
            var compare = new Version(version);
            return current > compare;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra xem version được truyền vào có đáp ứng minimum requirement không
    /// </summary>
    public bool MeetsMinimumRequirement(string version)
    {
        if (string.IsNullOrEmpty(MinimumVersion))
            return true;

        try
        {
            var minimum = new Version(MinimumVersion);
            var compare = new Version(version);
            return compare >= minimum;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy danh sách features từ JSON
    /// </summary>
    public List<string> GetFeatures()
    {
        if (string.IsNullOrEmpty(Features))
            return new List<string>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(Features) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Lấy danh sách bug fixes từ JSON
    /// </summary>
    public List<string> GetBugFixes()
    {
        if (string.IsNullOrEmpty(BugFixes))
            return new List<string>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(BugFixes) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    #endregion
}
