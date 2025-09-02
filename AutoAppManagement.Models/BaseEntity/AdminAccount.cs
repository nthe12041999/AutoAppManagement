using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// AdminAccount entity for managing administrator accounts
    /// </summary>
    [Table("AdminAccounts")]
    public class AdminAccount : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty; // admin, moderator, support

        public bool IsTwoFactorEnabled { get; set; } = false;

        // Login Information
        public DateTime? LastLoginAt { get; set; }

        public DateTime? LockedUntil { get; set; }

        [StringLength(45)]
        public string? LastLoginIp { get; set; }

        [StringLength(500)]
        public string? LastLoginUserAgent { get; set; }

        public DateTime? PasswordChangedAt { get; set; }

        // Additional Information
        [StringLength(255)]
        public string? Avatar { get; set; }

        // Security
        [StringLength(255)]
        public string? TwoFactorSecret { get; set; }

        public DateTime? LastPasswordChangeRequest { get; set; }

        public bool IsLocked { get; set; }

        // Methods
        public void LockAccount(int minutes = 30, long? lockedBy = null)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(minutes);
            SetUpdated(lockedBy);
        }

        public void UnlockAccount(long? unlockedBy = null)
        {
            LockedUntil = null;
            SetUpdated(unlockedBy);
        }

        public void ChangePassword(string newPasswordHash, long? changedBy = null)
        {
            PasswordHash = newPasswordHash;
            PasswordChangedAt = DateTime.UtcNow;
            SetUpdated(changedBy);
        }
    }
}
