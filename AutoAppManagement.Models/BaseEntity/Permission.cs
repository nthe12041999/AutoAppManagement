using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.BaseEntity;

public partial class Permission: BaseCUEntity
{

    [StringLength(100)]
    public string Resource { get; set; }

    public PermissionAction Action { get; set; }

    [StringLength(200)]
    public string Code { get; set; }

    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [InverseProperty("Permission")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Initialize permission code based on resource and action
    /// </summary>
    public void GenerateCode()
    {
        if (!string.IsNullOrEmpty(Resource))
        {
            Code = $"{Resource.ToLower()}.{Action.ToString().ToLower()}";
        }
    }
    /// <summary>
    /// Check if permission matches resource and action
    /// </summary>
    /// <param name="resource">Resource to check</param>
    /// <param name="action">Action to check</param>
    /// <returns>True if matches</returns>
    public bool Matches(string resource, string action)
    {
        return Resource.Equals(resource, StringComparison.OrdinalIgnoreCase) &&
               Action.ToString().Equals(action, StringComparison.OrdinalIgnoreCase);
    }
}
