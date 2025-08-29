using AutoAppManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.License
{
using AutoAppManagement.Models.Common;

    public class LicenseDTO : IStatefulDTO
    {
        public EntityState State { get; set; }
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string LicenseName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsAutoRenewal { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentInfo { get; set; } = string.Empty;
        public string AllowedFeatures { get; set; } = string.Empty;
        public string UsageLimits { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class CreateLicenseRequest
    {
        [Required]
        public long AccountId { get; set; }

        [Required]
        [StringLength(255)]
        public string LicenseKey { get; set; }

        [Required]
        [StringLength(255)]
        public string LicenseName { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseType { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int MaxDevices { get; set; } = 1;
    }

    public class UpdateLicenseRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string LicenseKey { get; set; }

        [Required]
        [StringLength(255)]
        public string LicenseName { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseType { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public string Status { get; set; }

        public int MaxDevices { get; set; } = 1;
    }

    public class RenewLicenseRequest
    {
        [Required]
        public long LicenseId { get; set; }

        [Required]
        public DateTime NewExpiryDate { get; set; }
    }
}
