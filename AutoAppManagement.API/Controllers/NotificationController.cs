using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Service.Services;

namespace AutoAppManagement.API.Controllers
{
    public class NotificationController : BaseBusinessController<INotificationService, Notification, NotificationDTO>
    {
        public NotificationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
