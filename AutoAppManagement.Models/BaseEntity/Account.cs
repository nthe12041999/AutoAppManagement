using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Account: BaseCUEntity
{

    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    public DateTime? RegisterDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public bool IsLocked { get; set; }

    // Optional split fields for display and binding; not required
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    private string _name;
    [NotMapped]
    public string Name
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_name)) return _name;
            var first = FirstName ?? string.Empty;
            var last = LastName ?? string.Empty;
            var full = ($"{first} {last}").Trim();
            return string.IsNullOrWhiteSpace(full) ? null : full;
        }
        set { _name = value; }
    }

    public Gender Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string ImgAvatar { get; set; } = string.Empty;

    public bool IsAutoRenewal { get; set; }

    public long LicenseId { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<AccountDevice> AccountDevices { get; set; } = new List<AccountDevice>();

    [InverseProperty("Account")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [ForeignKey("LicenseId")]
    [InverseProperty("Account")]
    public virtual License License { get; set; }
}
