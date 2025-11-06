using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Account
{
    public class AccountDTO : BaseEntity.Account, IStatefulDTO
    {
        public EntityState State { get; set; }

        /// <summary>
        /// Cờ đánh dấu có gửi email chào mừng cho khách hàng mới hay không
        /// </summary>
        public bool SendWelcomeEmail { get; set; } = false;

        /// <summary>
        /// Tên License được join từ bảng License
        /// </summary>
        public string LicenseName { get; set; } = string.Empty;

        /// <summary>
        /// Tên trạng thái dễ đọc (được convert từ Status enum)
        /// </summary>
        public string StatusName { get; set; } = string.Empty;
    }

    public class LockAccountRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        public string Reason { get; set; } = string.Empty;
    }

    public class ExtendAccountRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn mới không được để trống")]
        public DateTime NewExpiryDate { get; set; }
    }

    public class UpdateAccountInfoRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Giới tính không được vượt quá 10 ký tự")]
        public string Gender { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10, ErrorMessage = "Ngôn ngữ không được vượt quá 10 ký tự")]
        public string Language { get; set; } = "vi";
    }

    public class UploadAvatarRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn avatar không được để trống")]
        [StringLength(500, ErrorMessage = "Đường dẫn avatar không được vượt quá 500 ký tự")]
        public string AvatarPath { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email hoặc số điện thoại không được để trống")]
        [StringLength(255, ErrorMessage = "Email hoặc số điện thoại không được vượt quá 255 ký tự")]
        public string EmailOrPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string Password { get; set; } = string.Empty;

        // Optional client metadata for per-device refresh
        public string DeviceId { get; set; }
        public string Fingerprint { get; set; }
    }

    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public LoginWithResourcesResponse Data { get; set; }
    }

    public class LicenseInfoDTO
    {
        public long LicenseId { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string LicenseName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public StatusEnum Status { get; set; }
        public int DaysRemaining { get; set; }
        public string WarningMessage { get; set; } = string.Empty;
    }

    public class LoginWithResourcesResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime TokenExpiry { get; set; }
        public LicenseInfoDTO LicenseInfo { get; set; }
        public List<ToolResourceDTO> AvailableResources { get; set; } = new List<ToolResourceDTO>();
        public List<string> AllowedFeatures { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpired { get; set; }
    }

    public class ToolResourceDTO
    {
        public long FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureCode { get; set; } = string.Empty;
        public string ToolName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int RemainingCount { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string LimitType { get; set; } = string.Empty; // "daily", "monthly", "total"
        public string Status { get; set; } = string.Empty; // "available", "limited", "exhausted"
        public string WarningMessage { get; set; } = string.Empty;
    }

    public class ValidateAccountRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO JWT Token
    /// </summary>
    public class TokenDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpired { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpired { get; set; }
    }
}
