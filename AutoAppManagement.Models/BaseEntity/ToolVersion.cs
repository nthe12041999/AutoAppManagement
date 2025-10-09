using AutoAppManagement.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.BaseEntity;

public partial class ToolVersion : BaseCUEntity
{
    public ToolCode ToolCode { get; set; }

    [StringLength(50)]
    public string CurrentVersion { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime ReleaseDate { get; set; }

    public bool IsRequired { get; set; } = false;

    public decimal FileSize { get; set; }

    public string? Checksum { get; set; }
}
