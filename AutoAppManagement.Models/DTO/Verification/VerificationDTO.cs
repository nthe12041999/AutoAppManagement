using System.ComponentModel.DataAnnotations;
using AutoAppManagement.Models.BaseEntity;

namespace AutoAppManagement.Models.DTO.Verification
{
    public class SendOtpRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại OTP không được để trống")]
        public VerificationType Type { get; set; }
    }

    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP không được để trống")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "Mã OTP phải từ 6-10 ký tự")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại OTP không được để trống")]
        public VerificationType Type { get; set; }
    }

    public class VerifyOtpResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; } // Token để thực hiện action tiếp theo (đổi mật khẩu, etc)
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Token không được để trống")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordWithOtpRequest
    {

        [Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP không được để trống")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "Mã OTP phải từ 6-10 ký tự")]
        public string Otp { get; set; } = string.Empty;
    }
}
