using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// AdminLoginHistory entity for tracking admin login attempts
    /// </summary>
    [Table("AdminLoginHistory")]
    public class AdminLoginHistory : BaseEntity
    {
        [Required]
        public long AdminAccountId { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Required]
        [StringLength(50)]
        public string LoginResult { get; set; } = string.Empty; // Success, Failed, Blocked

        [StringLength(255)]
        public string? FailureReason { get; set; }

        [Required]
        public DateTime LoginAttemptAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AdminAccountId")]
        public virtual AdminAccount AdminAccount { get; set; } = null!;

        // Computed Properties
        [NotMapped]
        public bool IsSuccessful => LoginResult == "Success";

        [NotMapped]
        public string LoginResultDisplay
        {
            get
            {
                return LoginResult switch
                {
                    "Success" => "Thành công",
                    "Failed" => "Thất bại",
                    "Blocked" => "Bị chặn",
                    _ => LoginResult
                };
            }
        }

        [NotMapped]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - LoginAttemptAt;
                if (timeSpan.TotalMinutes < 1)
                    return "Vừa xong";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} phút trước";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} giờ trước";
                return $"{(int)timeSpan.TotalDays} ngày trước";
            }
        }
    }
}
