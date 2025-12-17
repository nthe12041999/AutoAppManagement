using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Role: BaseCUEntity
{
    [Column("RoleName")]
    public string Name { get; set; } = string.Empty;

    [Column("RoleDescription")]
    public string? Description { get; set; }

    // Backward compatibility properties
    [NotMapped]
    public string RoleName 
    { 
        get => Name; 
        set => Name = value; 
    }

    [NotMapped]
    public string RoleDescription 
    { 
        get => Description ?? string.Empty; 
        set => Description = value; 
    }

    [InverseProperty("Role")]
    public virtual ICollection<RoleAccount> RoleAccounts { get; set; } = new List<RoleAccount>();

    [InverseProperty("Role")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
