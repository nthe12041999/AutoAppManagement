using AutoAppManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Notification
{
using AutoAppManagement.Models.Common;

    public class NotificationDTO : IStatefulDTO
    {
        public EntityState State { get; set; }
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
        [Required]
        public long AccountId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [StringLength(50)]
        public string Type { get; set; } = "info";
    }

    public class UpdateNotificationRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public bool IsReaded { get; set; }
    }
}

    public class SendBulkNotificationRequest
    {
        [Required]
        public List<long> AccountIds { get; set; } = new List<long>();

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [StringLength(50)]
        public string Type { get; set; } = "info";
    }

