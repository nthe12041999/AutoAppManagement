using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.License
{
using AutoAppManagement.Models.Common;

    public class LicenseDTO : BaseEntity.License,IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    public class RenewLicenseRequest
    {
        [Required]
        public long LicenseId { get; set; }

        [Required]
        public DateTime NewExpiryDate { get; set; }
    }
}
