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

        [StringLength(1000)]
        public string? Permissions { get; set; } // JSON array of permissions

        // Account Status
        public bool IsEmailVerified { get; set; } = false;

        public bool IsPhoneVerified { get; set; } = false;

        public bool IsTwoFactorEnabled { get; set; } = false;

        // Login Information
        public DateTime? LastLoginAt { get; set; }

        public int LoginCount { get; set; } = 0;

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockedUntil { get; set; }

        [StringLength(45)]
        public string? LastLoginIp { get; set; }

        [StringLength(500)]
        public string? LastLoginUserAgent { get; set; }

        // Timestamps (specific to AdminAccount)
        public DateTime? EmailVerifiedAt { get; set; }

        public DateTime? PhoneVerifiedAt { get; set; }

        public DateTime? PasswordChangedAt { get; set; }

        // Additional Information
        [StringLength(255)]
        public string? Avatar { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        // Security
        [StringLength(255)]
        public string? TwoFactorSecret { get; set; }

        [StringLength(500)]
        public string? RecoveryTokens { get; set; } // JSON array of recovery tokens

        public DateTime? LastPasswordChangeRequest { get; set; }

        // Navigation Properties
        public virtual ICollection<AdminLoginHistory>? LoginHistory { get; set; }
        public virtual ICollection<AdminPermissionHistory>? PermissionHistory { get; set; }

        // Computed Properties
        [NotMapped]
        public string AccountStatus
        {
            get
            {
                if (IsDeleted) return "Deleted";
                if (!IsActive) return "Inactive";
                if (LockedUntil.HasValue && LockedUntil > DateTime.UtcNow) return "Locked";
                if (!IsEmailVerified) return "Pending Verification";
                return "Active";
            }
        }

        [NotMapped]
        public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;

        [NotMapped]
        public string OnlineStatus => LastLoginAt.HasValue && LastLoginAt > DateTime.UtcNow.AddMinutes(-30) ? "Online" : "Offline";

        [NotMapped]
        public DateTime? LastLoginDate => LastLoginAt;

        // Methods
        public void LockAccount(int minutes = 30, long? lockedBy = null)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(minutes);
            FailedLoginAttempts = 0;
            SetUpdated(lockedBy);
        }

        public void UnlockAccount(long? unlockedBy = null)
        {
            LockedUntil = null;
            FailedLoginAttempts = 0;
            SetUpdated(unlockedBy);
        }

        public void RecordLogin(string? ipAddress = null, string? userAgent = null)
        {
            LastLoginAt = DateTime.UtcNow;
            LastLoginIp = ipAddress;
            LastLoginUserAgent = userAgent;
            LoginCount++;
            FailedLoginAttempts = 0;
        }

        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;
            
            // Auto-lock after 5 failed attempts
            if (FailedLoginAttempts >= 5)
            {
                LockAccount(30); // Lock for 30 minutes
            }
        }

        public void VerifyEmail(long? verifiedBy = null)
        {
            IsEmailVerified = true;
            EmailVerifiedAt = DateTime.UtcNow;
            SetUpdated(verifiedBy);
        }

        public void VerifyPhone(long? verifiedBy = null)
        {
            IsPhoneVerified = true;
            PhoneVerifiedAt = DateTime.UtcNow;
            SetUpdated(verifiedBy);
        }

        public void ChangePassword(string newPasswordHash, long? changedBy = null)
        {
            PasswordHash = newPasswordHash;
            PasswordChangedAt = DateTime.UtcNow;
            SetUpdated(changedBy);
        }
    }

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

    /// <summary>
    /// AdminPermissionHistory entity for tracking permission changes
    /// </summary>
    [Table("AdminPermissionHistory")]
    public class AdminPermissionHistory : BaseEntity
    {
        public long AdminAccountId { get; set; }

        [StringLength(50)]
        public string Action { get; set; } = string.Empty; // Grant, Revoke, Update

        [StringLength(100)]
        public string Permission { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? OldValue { get; set; }

        [StringLength(1000)]
        public string? NewValue { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual AdminAccount? AdminAccount { get; set; }
    }

    /// <summary>
    /// Enum for admin roles
    /// </summary>
    public enum AdminRole
    {
        Admin,
        Moderator,
        Support,
        Viewer
    }

    /// <summary>
    /// Enum for admin permissions
    /// </summary>
    public enum AdminPermission
    {
        ManageUsers,
        ManageAdmins,
        ManageProducts,
        ManageOrders,
        ManageLicenses,
        ViewReports,
        ManageSettings,
        ManageFiles,
        ViewLogs,
        ManageRoles,
        ManagePermissions
    }
}
