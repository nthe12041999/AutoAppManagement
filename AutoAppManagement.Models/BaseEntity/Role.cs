using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Role: BaseCUEntity
{
    public string RoleName { get; set; }

    public string RoleDescription { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<RoleAccount> RoleAccounts { get; set; } = new List<RoleAccount>();

    [InverseProperty("Role")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
