using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.Enum;

namespace AutoAppManagement.Models.DTO.ToolVersion
{
    public class ToolVersionDTO : IStatefulDTO
    {
        public long ID { get; set; }
        public AutoAppManagement.Models.Common.EntityState State { get; set; }
        public string ToolCode { get; set; } = string.Empty;
        public string ToolName { get; set; } = string.Empty;
        public string CurrentVersion { get; set; } = string.Empty;
        public string? MinimumVersion { get; set; }
        public string? Description { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ReleaseNotes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsRequired { get; set; }
        public string? Platform { get; set; }
        public long? FileSize { get; set; }
        public string? Checksum { get; set; }
        public List<string>? Features { get; set; }
        public List<string>? BugFixes { get; set; }
        public string? Category { get; set; }
        public int? Priority { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        
        // Additional computed properties  
        public string? FileSizeFormatted => FileSize.HasValue ? FormatFileSize(FileSize.Value) : null;
        public bool IsLatest { get; set; }
        
        // Backward compatibility
        public long Id 
        { 
            get => ID; 
            set => ID = value; 
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }

    public class CreateToolVersionRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public ToolCode ToolCode { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CurrentVersion { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DownloadUrl { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        public bool IsRequired { get; set; } = false;
        public long? FileSize { get; set; }
        public string? Checksum { get; set; }
    }

    public class UpdateToolVersionRequest
    {
        public long Id { get; set; }
        public string? CurrentVersion { get; set; }
        public string? Description { get; set; }
        public string? DownloadUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool? IsRequired { get; set; }
        public long? FileSize { get; set; }
        public string? Checksum { get; set; }
    }

    public class CheckVersionRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public ToolCode ToolCode { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CurrentVersion { get; set; } = string.Empty;
    }

    public class CheckVersionResponse
    {
        public string LastestVersion { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal FileSize { get; set; }
        public string Checksum { get; set; }
    }

    public class VersionHistory
    {
        public string Version { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string? Description { get; set; }
        public decimal? FileSize { get; set; }
        public string? DownloadUrl { get; set; }
    }
}
