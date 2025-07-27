using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public int Level { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public DateTime? RegisterDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string Language { get; set; }

    public bool IsLocked { get; set; } = false;

    public string Name { get; set; }

    public GenderType Gender { get; set; } = GenderType.Male;

    public DateTime? DateOfBirth { get; set; }

    public string ImgAvatar { get; set; }
    public int MaxAccountFb { get; set; }

    public virtual ICollection<AccountsFb> AccountsFbs { get; set; } = new List<AccountsFb>();

    public virtual ICollection<RoleAccount> RoleAccountAccounts { get; set; } = new List<RoleAccount>();

    public virtual ICollection<RoleAccount> RoleAccountCreatedByNavigations { get; set; } = new List<RoleAccount>();
    public virtual ICollection<Notification> Notification { get; set; } = new List<Notification>();
}
