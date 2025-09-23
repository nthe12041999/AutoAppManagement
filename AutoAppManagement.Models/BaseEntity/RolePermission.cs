using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class RolePermission: BaseCUEntity
{
    public long RoleId { get; set; }

    public long PermissionId { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("RolePermissions")]
    public virtual Permission? Permission { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("RolePermissions")]
    public virtual Role? Role { get; set; }
}
