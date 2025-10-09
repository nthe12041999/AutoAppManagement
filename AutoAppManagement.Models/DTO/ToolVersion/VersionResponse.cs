namespace AutoAppManagement.Models.DTO.ToolVersion
{
    /// <summary>
    /// Response model đơn giản cho thông tin phiên bản - tương thích với yêu cầu ban đầu
    /// </summary>
    public class VersionResponse
    {
        /// <summary>
        /// Phiên bản hiện tại
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// URL tải xuống
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;

        /// <summary>
        /// URL changelog (sử dụng ReleaseNotes)
        /// </summary>
        public string ChangelogUrl { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả phiên bản
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Checksum để verify file
        /// </summary>
        public string Checksum { get; set; } = string.Empty;

        /// <summary>
        /// Có bắt buộc cập nhật hay không
        /// </summary>
        public bool Mandatory { get; set; } = false;

        /// <summary>
        /// Kích thước file (bytes)
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Tạo VersionResponse từ ToolVersionDTO
        /// </summary>
        public static VersionResponse FromToolVersionDTO(ToolVersionDTO dto)
        {
            return new VersionResponse
            {
                Version = dto.CurrentVersion,
                DownloadUrl = dto.DownloadUrl ?? string.Empty,
                ChangelogUrl = dto.ReleaseNotes ?? string.Empty, // Sử dụng ReleaseNotes làm ChangelogUrl
                Description = dto.Description ?? string.Empty,
                Checksum = dto.Checksum ?? string.Empty,
                Mandatory = dto.IsRequired,
                FileSize = dto.FileSize
            };
        }
    }

    /// <summary>
    /// Response đơn giản cho CheckVersion API - chỉ trả về VersionResponse nếu có update
    /// </summary>
    public class SimpleCheckVersionResponse
    {
        /// <summary>
        /// Có update mới hay không
        /// </summary>
        public bool UpdateAvailable { get; set; } = false;

        /// <summary>
        /// Có bắt buộc update hay không
        /// </summary>
        public bool UpdateRequired { get; set; } = false;

        /// <summary>
        /// Thông tin phiên bản mới (nếu có update)
        /// </summary>
        public VersionResponse? LatestVersion { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}