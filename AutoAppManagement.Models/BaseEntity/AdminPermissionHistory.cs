using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// AdminPermissionHistory entity for tracking permission changes
    /// </summary>
    [Table("AdminPermissionHistory")]
    public class AdminPermissionHistory : BaseEntity
    {
        public long AdminAccountId { get; set; }

        [StringLength(50)]
        public string Action { get; set; } = string.Empty; // Grant, Revoke, Update

        [StringLength(100)]
        public string Permission { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? OldValue { get; set; }

        [StringLength(1000)]
        public string? NewValue { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual AdminAccount? AdminAccount { get; set; }
    }
}
