using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Account : BaseEntity
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int Level { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime? RegisterDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string Language { get; set; } = "vi-VN";

    public bool IsLocked { get; set; } = false;

    public string Name { get; set; } = string.Empty;

    public GenderType Gender { get; set; } = GenderType.Male;

    public DateTime? DateOfBirth { get; set; }

    public string ImgAvatar { get; set; } = "";

    /// <summary>
    /// Có tự động gia hạn không
    /// </summary>
    public bool IsAutoRenewal { get; set; } = false;

    /// <summary>
    /// ID của License được gán cho tài khoản này (một tài khoản chỉ có một license)
    /// </summary>
    public long? LicenseId { get; set; }

    // Navigation properties
    public virtual License? LicenseNavigation { get; set; }

    public virtual ICollection<RoleAccount> RoleAccounts { get; set; } = new List<RoleAccount>();
    public virtual ICollection<RoleAccount> RoleAccountCreatedByNavigations { get; set; } = new List<RoleAccount>();
    public virtual ICollection<Notification> Notification { get; set; } = new List<Notification>();

    // Navigation properties cho Customer Device
    public virtual ICollection<AccountDevice> CustomerDevices { get; set; } = new List<AccountDevice>();
}
