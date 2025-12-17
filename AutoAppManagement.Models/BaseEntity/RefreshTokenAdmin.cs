using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AutoAppManagement.Models.BaseEntity
{
    public class RefreshTokenAdmin : BaseCUEntity
    {
        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public long AdminAccountId { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public bool IsUsed { get; set; } = false;

        [Required]
        public bool IsRevoked { get; set; } = false;

        [StringLength(500)]
        public string ReplacedByToken { get; set; }

        [StringLength(45)]
        public string CreatedByIp { get; set; }

        [StringLength(45)]
        public string RevokedByIp { get; set; }

        public DateTime? RevokedDate { get; set; }

        [StringLength(255)]
        public string DeviceInfo { get; set; }

        [StringLength(255)]
        public string UserAgent { get; set; }

        // Security enhancements
        [StringLength(88)]
        public string TokenHash { get; set; } // Base64(SHA256(token))

        public Guid? FamilyId { get; set; }

        [StringLength(88)]
        public string FingerprintHash { get; set; }

        // Navigation properties
        [ForeignKey("AdminAccountId")]
        [InverseProperty("RefreshTokens")]
        [JsonIgnore]
        public virtual AdminAccount AdminAccount { get; set; } = null!;

        // Helper properties
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

        [NotMapped]
        public bool IsActive => !IsRevoked && !IsUsed && !IsExpired;
    }
}
