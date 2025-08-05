using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.Requests
{
    /// <summary>
    /// Request model for creating admin account
    /// </summary>
    public class CreateAdminRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3-50 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-20 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ chứa chữ, số và dấu gạch dưới")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-50 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public string Role { get; set; } = string.Empty;

        public List<string>? Permissions { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string? Bio { get; set; }
    }
}
