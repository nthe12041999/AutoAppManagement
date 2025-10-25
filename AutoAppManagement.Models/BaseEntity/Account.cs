using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Account: BaseCUEntity
{

    [StringLength(50)]
    public string UserName { get; set; }

    [StringLength(255)]
    public string Password { get; set; }

    [StringLength(20)]
    public string Phone { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    public DateTime? RegisterDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public bool IsLocked { get; set; }

    public string Name { get; set; }

    public short Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string ImgAvatar { get; set; }

    public bool IsAutoRenewal { get; set; }

    public long LicenseId { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<AccountDevice> AccountDevices { get; set; } = new List<AccountDevice>();

    [InverseProperty("Account")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [ForeignKey("LicenseId")]
    [InverseProperty("Account")]
    public virtual License? License { get; set; }
}
