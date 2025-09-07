using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Permission entity representing system permissions
    /// </summary>
    [Table("permissions")]
    public class Permission : BaseEntity
    {
        /// <summary>
        /// Resource name (e.g., 'orders', 'accounts', 'licenses')
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column("resource")]
        public string Resource { get; set; } = string.Empty;

        /// <summary>
        /// Action name (e.g., 'view', 'create', 'update', 'delete')
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column("action")]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Permission code (e.g., 'orders.view', 'accounts.create')
        /// </summary>
        [Required]
        [StringLength(200)]
        [Column("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Permission display name
        /// </summary>
        [StringLength(200)]
        [Column("display_name")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Permission description
        /// </summary>
        [StringLength(500)]
        [Column("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Permission category for grouping
        /// </summary>
        [StringLength(100)]
        [Column("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Navigation property to role permissions
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        /// <summary>
        /// Initialize permission code based on resource and action
        /// </summary>
        public void GenerateCode()
        {
            if (!string.IsNullOrEmpty(Resource) && !string.IsNullOrEmpty(Action))
            {
                Code = $"{Resource.ToLower()}.{Action.ToLower()}";
            }
        }

        /// <summary>
        /// Get full permission identifier
        /// </summary>
        public string GetFullIdentifier()
        {
            return $"{Resource}:{Action}";
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
                   Action.Equals(action, StringComparison.OrdinalIgnoreCase);
        }
    }
}
