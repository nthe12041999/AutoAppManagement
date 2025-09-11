using System.ComponentModel.DataAnnotations;
using AutoAppManagement.Models.Common;
using static AutoAppManagement.Models.Enum.DataModelType;

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
    /// DTO thông tin LicenseUser
    /// </summary>
    public class LicenseUserDTO : BaseEntity.LicenseUser, IStatefulDTO
    {
        public EntityState State { get; set; }

        // Additional properties for display
        public string? LicenseName { get; set; }
        public string? LicenseKey { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
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
    /// DTO t?o license m?i
    /// </summary>
    public class CreateLicenseRequest
    {
        [Required(ErrorMessage = "License key không ???c ?? tr?ng")]
        [StringLength(255, ErrorMessage = "License key không ???c v??t quá 255 ký t?")]
        public string LicenseKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên license không ???c ?? tr?ng")]
        [StringLength(100, ErrorMessage = "Tên license không ???c v??t quá 100 ký t?")]
        public string LicenseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lo?i license không ???c ?? tr?ng")]
        public LicenseTypeEnum LicenseType { get; set; }

        [StringLength(1000, ErrorMessage = "Mô t? không ???c v??t quá 1000 ký t?")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "S? thi?t b? t?i ?a ph?i l?n h?n 0")]
        public int MaxDevices { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "S? user t?i ?a ph?i l?n h?n 0")]
        public int MaxUsers { get; set; } = 1;

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá ph?i l?n h?n ho?c b?ng 0")]
        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        [StringLength(10, ErrorMessage = "??n v? ti?n t? không ???c v??t quá 10 ký t?")]
        public string Currency { get; set; } = "VND";

        [StringLength(500, ErrorMessage = "Thông tin thanh toán không ???c v??t quá 500 ký t?")]
        public string? PaymentInfo { get; set; }

        public string? Features { get; set; }

        public string? FeatureLimits { get; set; }
    }

    /// <summary>
    /// DTO c?p nh?t license
    /// </summary>
    public class UpdateLicenseRequest
    {
        [Required(ErrorMessage = "License ID không ???c ?? tr?ng")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Tên license không ???c ?? tr?ng")]
        [StringLength(100, ErrorMessage = "Tên license không ???c v??t quá 100 ký t?")]
        public string LicenseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lo?i license không ???c ?? tr?ng")]
        public LicenseTypeEnum LicenseType { get; set; }

        [StringLength(1000, ErrorMessage = "Mô t? không ???c v??t quá 1000 ký t?")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "S? thi?t b? t?i ?a ph?i l?n h?n 0")]
        public int MaxDevices { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "S? user t?i ?a ph?i l?n h?n 0")]
        public int MaxUsers { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá ph?i l?n h?n ho?c b?ng 0")]
        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        [StringLength(500, ErrorMessage = "Thông tin thanh toán không ???c v??t quá 500 ký t?")]
        public string? PaymentInfo { get; set; }

        public string? Features { get; set; }

        public string? FeatureLimits { get; set; }
    }

    /// <summary>
    /// DTO th?ng kê license
    /// </summary>
    public class LicenseStatisticsDTO
    {
        public int TotalLicenses { get; set; }
        public int ActiveLicenses { get; set; }
        public int ExpiredLicenses { get; set; }
        public int SuspendedLicenses { get; set; }
        public int ExpiringSoonLicenses { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }

        public Dictionary<string, int> LicensesByType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> LicensesByStatus { get; set; } = new Dictionary<string, int>();

        public List<LicenseUsageStatisticsDTO> UsageStatistics { get; set; } = new List<LicenseUsageStatisticsDTO>();
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

    /// <summary>
    /// DTO tìm ki?m license
    /// </summary>
    public class LicenseSearchRequest
    {
        public string? Keyword { get; set; }
        public LicenseTypeEnum? LicenseType { get; set; }
        public string? Status { get; set; }
        public DateTime? ExpiryFrom { get; set; }
        public DateTime? ExpiryTo { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public bool? IsExpired { get; set; }
        public bool? IsExpiringSoon { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedDate";
        public string SortDirection { get; set; } = "DESC";
    }

    /// <summary>
    /// DTO bulk operations cho license
    /// </summary>
    public class BulkLicenseOperationRequest
    {
        [Required(ErrorMessage = "Danh sách License ID không ???c ?? tr?ng")]
        public List<long> LicenseIds { get; set; } = new List<long>();

        [Required(ErrorMessage = "Lo?i thao tác không ???c ?? tr?ng")]
        public string Operation { get; set; } = string.Empty; // "Activate", "Suspend", "Delete", "Extend"

        public DateTime? NewExpiryDate { get; set; } // For extend operation
        public string? Reason { get; set; }
    }
}
