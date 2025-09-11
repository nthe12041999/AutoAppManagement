using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.Models.BaseEntity
{
    /// <summary>
    /// License User - Quản lý quan hệ Account + License
    /// </summary>
    [Table("license_users")]
    public class LicenseUser : BaseEntity
    {
        public long AccountId { get; set; }

        public long LicenseId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsTrial { get; set; } = false;

        public bool AutoRenew { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;

        [ForeignKey("LicenseId")]
        public virtual License License { get; set; } = null!;

        // Helper methods
        public bool IsValidLicense()
        {
            return IsActive && StartDate <= DateTime.UtcNow && EndDate > DateTime.UtcNow;
        }

        public int DaysRemaining()
        {
            return Math.Max(0, (EndDate - DateTime.UtcNow).Days);
        }

        public bool IsExpiringSoon(int days = 30)
        {
            return IsValidLicense() && DaysRemaining() <= days;
        }
    }
}
