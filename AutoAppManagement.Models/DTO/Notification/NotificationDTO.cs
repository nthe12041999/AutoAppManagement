using AutoAppManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Notification
{
using AutoAppManagement.Models.Common;

    public class NotificationDTO : BaseEntity.Notification,IStatefulDTO
    {
        public EntityState State { get; set; }
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
}

    

