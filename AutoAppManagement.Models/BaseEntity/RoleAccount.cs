using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class RoleAccount: BaseCUEntity
{
    public long RoleID { get; set; }

    public long AccountID { get; set; }

    [ForeignKey("AccountID")]
    [InverseProperty("RoleAccounts")]
    public virtual AdminAccount Account { get; set; }

    [ForeignKey("RoleID")]
    [InverseProperty("RoleAccounts")]
    public virtual Role Role { get; set; }
}
