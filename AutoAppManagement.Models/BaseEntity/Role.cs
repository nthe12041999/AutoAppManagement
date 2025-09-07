using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Role : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;

    public string RoleDescription { get; set; } = string.Empty;

    public virtual ICollection<RoleAccount> RoleAccounts { get; set; } = new List<RoleAccount>();
    
    /// <summary>
    /// Navigation property to role permissions
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
