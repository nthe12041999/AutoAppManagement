using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Account
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email hoặc số điện thoại không được để trống")]
        [StringLength(100, ErrorMessage = "Email hoặc số điện thoại không được vượt quá 100 ký tự")]
        public string EmailOrPhone { get; set; } = string.Empty;
    }

    public class ConfirmOtpRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP không được để trống")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "Mã OTP phải từ 6-10 ký tự")]
        public string Otp { get; set; } = string.Empty;
    }

    public class ForgotPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MaskedEmail { get; set; }
    }
}
