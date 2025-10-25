using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Account
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email hoặc số điện thoại không được để trống")]
        [StringLength(100, ErrorMessage = "Email hoặc số điện thoại không được vượt quá 100 ký tự")]
        public string EmailOrPhone { get; set; } = string.Empty;
    }

    public class ForgotPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MaskedEmail { get; set; }
    }
}
