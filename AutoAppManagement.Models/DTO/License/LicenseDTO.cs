using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.License
{
    public class LicenseDTO
    {
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
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "License key không được để trống")]
        [StringLength(255, ErrorMessage = "License key không được vượt quá 255 ký tự")]
        public string LicenseKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên license không được để trống")]
        [StringLength(100, ErrorMessage = "Tên license không được vượt quá 100 ký tự")]
        public string LicenseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại license không được để trống")]
        [StringLength(50, ErrorMessage = "Loại license không được vượt quá 50 ký tự")]
        public string LicenseType { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Số thiết bị tối đa phải lớn hơn 0")]
        public int MaxDevices { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Số user tối đa phải lớn hơn 0")]
        public int MaxUsers { get; set; } = 1;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        public DateTime ExpiryDate { get; set; }

        public bool IsAutoRenewal { get; set; } = false;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [StringLength(10, ErrorMessage = "Đơn vị tiền tệ không được vượt quá 10 ký tự")]
        public string Currency { get; set; } = "VND";

        [StringLength(500, ErrorMessage = "Thông tin thanh toán không được vượt quá 500 ký tự")]
        public string PaymentInfo { get; set; } = string.Empty;

        public string AllowedFeatures { get; set; } = string.Empty;

        public string UsageLimits { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateLicenseRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "License key không được để trống")]
        [StringLength(255, ErrorMessage = "License key không được vượt quá 255 ký tự")]
        public string LicenseKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên license không được để trống")]
        [StringLength(100, ErrorMessage = "Tên license không được vượt quá 100 ký tự")]
        public string LicenseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại license không được để trống")]
        [StringLength(50, ErrorMessage = "Loại license không được vượt quá 50 ký tự")]
        public string LicenseType { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Số thiết bị tối đa phải lớn hơn 0")]
        public int MaxDevices { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Số user tối đa phải lớn hơn 0")]
        public int MaxUsers { get; set; } = 1;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        public DateTime ExpiryDate { get; set; }

        public bool IsAutoRenewal { get; set; } = false;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [StringLength(10, ErrorMessage = "Đơn vị tiền tệ không được vượt quá 10 ký tự")]
        public string Currency { get; set; } = "VND";

        [StringLength(500, ErrorMessage = "Thông tin thanh toán không được vượt quá 500 ký tự")]
        public string PaymentInfo { get; set; } = string.Empty;

        public string AllowedFeatures { get; set; } = string.Empty;

        public string UsageLimits { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }

    public class RenewLicenseRequest
    {
        [Required(ErrorMessage = "License ID không được để trống")]
        public long LicenseId { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn mới không được để trống")]
        public DateTime NewExpiryDate { get; set; }
    }
}
