using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// Role Permission entity representing the relationship between roles and permissions
    /// </summary>
    [Table("role_permissions")]
    public class RolePermission : BaseEntity
    {
        /// <summary>
        /// Role ID foreign key
        /// </summary>
        [Required]
        [Column("role_id")]
        public long RoleId { get; set; }

        /// <summary>
        /// Permission ID foreign key
        /// </summary>
        [Required]
        [Column("permission_id")]
        public long PermissionId { get; set; }

        /// <summary>
        /// Default scope for this permission (own, team, org, all)
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column("scope_default")]
        public string ScopeDefault { get; set; } = "own";

        /// <summary>
        /// Additional constraints in JSON format
        /// </summary>
        [Column("constraints", TypeName = "nvarchar(max)")]
        public string? Constraints { get; set; }

        /// <summary>
        /// Permission priority/order
        /// </summary>
        [Column("priority")]
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Whether this permission is inherited from parent role
        /// </summary>
        [Column("is_inherited")]
        public bool IsInherited { get; set; } = false;

        /// <summary>
        /// Expiry date for temporary permissions
        /// </summary>
        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Navigation property to Role
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        /// <summary>
        /// Navigation property to Permission
        /// </summary>
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; } = null!;

        /// <summary>
        /// Check if permission is valid and not expired
        /// </summary>
        public bool IsValid()
        {
            return IsActive && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
        }

        /// <summary>
        /// Check if scope is valid
        /// </summary>
        public bool IsValidScope()
        {
            var validScopes = new[] { "own", "team", "org", "all" };
            return validScopes.Contains(ScopeDefault.ToLower());
        }

        /// <summary>
        /// Get scope level as integer for comparison
        /// </summary>
        public int GetScopeLevel()
        {
            return ScopeDefault.ToLower() switch
            {
                "own" => 1,
                "team" => 2,
                "org" => 3,
                "all" => 4,
                _ => 0
            };
        }

        /// <summary>
        /// Check if this permission scope covers the requested scope
        /// </summary>
        /// <param name="requestedScope">Requested scope to check</param>
        /// <returns>True if this permission covers the requested scope</returns>
        public bool CoversScope(string requestedScope)
        {
            var thisLevel = GetScopeLevel();
            var requestedLevel = new RolePermission { ScopeDefault = requestedScope }.GetScopeLevel();
            return thisLevel >= requestedLevel;
        }

        /// <summary>
        /// Set expiry date for temporary permission
        /// </summary>
        /// <param name="duration">Duration from now</param>
        public void SetExpiry(TimeSpan duration)
        {
            ExpiresAt = DateTime.UtcNow.Add(duration);
        }

        /// <summary>
        /// Remove expiry date (make permanent)
        /// </summary>
        public void MakePermanent()
        {
            ExpiresAt = null;
        }
    }
}
