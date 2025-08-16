using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Notification
{
    public class NotificationDTO
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public long AccountId { get; set; }
        public bool IsReaded { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class CreateNotificationRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại thông báo không được để trống")]
        [StringLength(20, ErrorMessage = "Loại thông báo không được vượt quá 20 ký tự")]
        public string Type { get; set; } = "info";

        [StringLength(255, ErrorMessage = "Icon không được vượt quá 255 ký tự")]
        public string Icon { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Hình ảnh không được vượt quá 255 ký tự")]
        public string Image { get; set; } = string.Empty;
    }

    public class UpdateNotificationRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại thông báo không được để trống")]
        [StringLength(20, ErrorMessage = "Loại thông báo không được vượt quá 20 ký tự")]
        public string Type { get; set; } = "info";

        [StringLength(255, ErrorMessage = "Icon không được vượt quá 255 ký tự")]
        public string Icon { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Hình ảnh không được vượt quá 255 ký tự")]
        public string Image { get; set; } = string.Empty;
    }

    public class SendBulkNotificationRequest
    {
        [Required(ErrorMessage = "Danh sách Account ID không được để trống")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 Account ID")]
        public List<long> AccountIds { get; set; } = new List<long>();

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự")]
        public string Message { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Loại thông báo không được vượt quá 20 ký tự")]
        public string Type { get; set; } = "info";
    }
}
