using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AutoAppManagement.Models.BaseEntity;

public partial class AdminAccount: BaseCUEntity
{
    [StringLength(100)]
    public string FullName { get; set; }

    [StringLength(255)]
    public string Email { get; set; }

    [StringLength(20)]
    public string PhoneNumber { get; set; }

    [StringLength(50)]
    public string UserName { get; set; }

    [StringLength(255)]
    public string PasswordHash { get; set; }

    [StringLength(50)]
    public string Role { get; set; }

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public bool IsTwoFactorEnabled { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public int LoginCount { get; set; }

    public DateTime? LockedUntil { get; set; }

    [StringLength(45)]
    public string LastLoginIp { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public DateTime? PhoneVerifiedAt { get; set; }

    public DateTime? PasswordChangedAt { get; set; }

    [StringLength(255)]
    public string Avatar { get; set; }

    [StringLength(255)]
    public string TwoFactorSecret { get; set; }

    [StringLength(500)]
    public string RecoveryTokens { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("Account")]
    public virtual ICollection<RoleAccount> RoleAccounts { get; set; } = new List<RoleAccount>();

    [InverseProperty("AdminAccount")]
    [JsonIgnore]
    public virtual ICollection<RefreshTokenAdmin> RefreshTokens { get; set; } = new List<RefreshTokenAdmin>();
}
