using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Account
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự và không quá 255 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request để admin đổi mật khẩu cho user khác
    /// </summary>
    public class AdminChangePasswordRequest
    {
        [Required(ErrorMessage = "ID tài khoản không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự và không quá 255 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool SendEmailNotification { get; set; } = true;
    }

    /// <summary>
    /// Simple request for admin to change password (backward compatibility)
    /// </summary>
    public class SimpleChangePasswordRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
