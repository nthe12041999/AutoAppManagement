using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Role: BaseCUEntity
{
    [Column("RoleName")]
    public string Name { get; set; } = string.Empty;

    [Column("RoleDescription")]
    public string? Description { get; set; }

    [StringLength(100)]
    [Required]
    [NotMapped] // Database không có cột Code, đánh dấu NotMapped để tránh lỗi SQL
    public string Code { get; set; } = string.Empty;

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
