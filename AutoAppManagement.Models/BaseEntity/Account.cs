using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Account : BaseEntity
{
    public string UserName { get; set; }

    public string Password { get; set; }

    public int Level { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public DateTime? RegisterDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string Language { get; set; } = "vi-VN";

    public bool IsLocked { get; set; } = false;

    public string Name { get; set; }

    public GenderType Gender { get; set; } = GenderType.Male;

    public DateTime? DateOfBirth { get; set; }

    public string ImgAvatar { get; set; } = "";

    public virtual ICollection<RoleAccount> RoleAccountAccounts { get; set; } =
        new List<RoleAccount>();

    public virtual ICollection<RoleAccount> RoleAccountCreatedByNavigations { get; set; } =
        new List<RoleAccount>();
    public virtual ICollection<Notification> Notification { get; set; } = new List<Notification>();

    // Navigation properties cho Customer Device và License
    public virtual ICollection<AccountDevice> CustomerDevices { get; set; } =
        new List<AccountDevice>();
    public virtual ICollection<License> Licenses { get; set; } =
        new List<License>();
    public virtual ICollection<License> CreatedLicenses { get; set; } =
        new List<License>();
    public virtual ICollection<License> UpdatedLicenses { get; set; } =
        new List<License>();
}
