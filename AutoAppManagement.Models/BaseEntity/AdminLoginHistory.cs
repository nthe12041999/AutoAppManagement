using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// AdminLoginHistory entity for tracking admin login history
    /// </summary>
    [Table("AdminLoginHistory")]
    public class AdminLoginHistory : BaseEntity
    {
        public long AdminAccountId { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string LoginResult { get; set; } = string.Empty; // Success, Failed, Blocked

        [StringLength(255)]
        public string? FailureReason { get; set; }

        public DateTime LoginAttemptAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual AdminAccount? AdminAccount { get; set; }
    }
}
