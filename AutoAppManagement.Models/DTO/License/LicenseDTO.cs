using AutoAppManagement.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.License
{
    public class LicenseDTO : BaseEntity.License, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    /// <summary>
    /// DTO gán license cho account (1-1 relationship)
    /// </summary>
    public class AssignLicenseToAccountRequest
    {
        [Required(ErrorMessage = "License ID không ???c ?? tr?ng")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Account ID không ???c ?? tr?ng")]
        public long AccountId { get; set; }

        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO gán license cho user (Many-Many relationship)
    /// </summary>
    public class AssignLicenseToUserRequest
    {
        [Required(ErrorMessage = "License ID không ???c ?? tr?ng")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Account ID không ???c ?? tr?ng")]
        public long AccountId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO gia h?n license
    /// </summary>
    public class RenewLicenseRequest
    {
        [Required(ErrorMessage = "License ID không ???c ?? tr?ng")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Ngày h?t h?n m?i không ???c ?? tr?ng")]
        public DateTime NewExpiryDate { get; set; }

        public string? Reason { get; set; }
    }

    /// <summary>
    /// DTO th?ng kê s? d?ng license
    /// </summary>
    public class LicenseUsageStatisticsDTO
    {
        public long LicenseId { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string LicenseName { get; set; } = string.Empty;
        public int AssignedUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int MaxUsers { get; set; }
        public double UtilizationRate { get; set; }
        public int DaysUntilExpiry { get; set; }
    }
}
