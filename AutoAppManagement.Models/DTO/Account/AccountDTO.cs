using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Account
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Level { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Language { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string ImgAvatar { get; set; } = string.Empty;
        public int MaxAccountFb { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class CreateAccountRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Level phải từ 1-10")]
        public int Level { get; set; } = 1;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        public DateTime ExpiredDate { get; set; }

        [StringLength(10, ErrorMessage = "Ngôn ngữ không được vượt quá 10 ký tự")]
        public string Language { get; set; } = "vi";

        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Giới tính không được vượt quá 10 ký tự")]
        public string Gender { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [Range(1, 1000, ErrorMessage = "Số lượng account FB phải từ 1-1000")]
        public int MaxAccountFb { get; set; } = 1;
    }

    public class UpdateAccountRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string UserName { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Level phải từ 1-10")]
        public int Level { get; set; } = 1;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        public DateTime ExpiredDate { get; set; }

        [StringLength(10, ErrorMessage = "Ngôn ngữ không được vượt quá 10 ký tự")]
        public string Language { get; set; } = "vi";

        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Giới tính không được vượt quá 10 ký tự")]
        public string Gender { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [Range(1, 1000, ErrorMessage = "Số lượng account FB phải từ 1-1000")]
        public int MaxAccountFb { get; set; } = 1;
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
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

    public class ValidateAccountRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = string.Empty;
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
    }

    public class LoginResponse
    {
        public AccountDTO Account { get; set; } = new AccountDTO();
        public LicenseInfoDTO? LicenseInfo { get; set; }
        public DateTime LoginTime { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
    }

    public class LicenseInfoDTO
    {
        public long LicenseId { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string LicenseName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public string WarningMessage { get; set; } = string.Empty;
    }

    public class LicenseCheckResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public LicenseInfoDTO? LicenseInfo { get; set; }
    }
}
