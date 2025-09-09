using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Service.Services
{
    public interface INotificationService : IBaseBusinessService<NotificationDTO>
    {
        Task<List<NotificationDTO>> GetNotificationsByAccountId(long accountId);
        Task<List<NotificationDTO>> GetUnreadNotifications(long accountId);
        Task<BaseResponse> MarkAsRead(long id);
        Task<BaseResponse> MarkAsUnread(long id);
        Task<BaseResponse> MarkAllAsRead(long accountId);
        Task<int> GetUnreadCount(long accountId);
        Task<BaseResponse> SendNotificationToAccount(long accountId, string title, string message, string type = "info");
        Task<BaseResponse> SendBulkNotification(List<long> accountIds, string title, string message, string type = "info");
        Task<List<NotificationDTO>> GetNotificationsByType(long accountId, string type);
    }

    public class NotificationService : BaseBusinessService<Notification, NotificationDTO, INotificationsRepository>, INotificationService
    {
        public NotificationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override async Task<BaseResponse> SubmitData(NotificationDTO dto)
        {
            var result = await base.SubmitData(dto);

            // If creation was successful, send a notification via SignalR
            if (result.IsSuccess && dto.State == EntityState.Add)
            {
                await NotificationSocketHub.SendNotificationToUser(dto.AccountId, dto);
            }

            return result;
        }

        public async Task<List<NotificationDTO>> GetNotificationsByAccountId(long accountId)
        {
            var notifications = await Repository.GetByCondition(n => n.AccountId == accountId && !n.IsDeleted);
            return Mapper.Map<List<NotificationDTO>>(notifications.OrderByDescending(n => n.CreatedDate).ToList());
        }

        public async Task<List<NotificationDTO>> GetUnreadNotifications(long accountId)
        {
            var notifications = await Repository.GetByCondition(n => n.AccountId == accountId && !n.IsReaded && !n.IsDeleted);
            return Mapper.Map<List<NotificationDTO>>(notifications.OrderByDescending(n => n.CreatedDate).ToList());
        }

        public async Task<BaseResponse> MarkAsRead(long id)
        {
            try
            {
                var notification = await UpdateById(id);

                notification.IsReaded = true;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đánh dấu đã đọc thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đánh dấu đã đọc: {ex.Message}");
            }
        }

        public async Task<BaseResponse> MarkAsUnread(long id)
        {
            try
            {
                var notification = await UpdateById(id);

                notification.IsReaded = false;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đánh dấu chưa đọc thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đánh dấu chưa đọc: {ex.Message}");
            }
        }

        public async Task<BaseResponse> MarkAllAsRead(long accountId)
        {
            try
            {
                var notifications = await Repository.GetByCondition(n => n.AccountId == accountId && !n.IsReaded && !n.IsDeleted);
                foreach (var notification in notifications)
                {
                    notification.IsReaded = true;
                    notification.SetUpdated(GetCurrentUserId());
                }

                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Đánh dấu tất cả đã đọc thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đánh dấu tất cả đã đọc: {ex.Message}");
            }
        }

        public async Task<int> GetUnreadCount(long accountId)
        {
            return await Repository.CountByCondition(n => n.AccountId == accountId && !n.IsReaded && !n.IsDeleted);
        }

        public async Task<BaseResponse> SendNotificationToAccount(long accountId, string title, string message, string type = "info")
        {
            var dto = new NotificationDTO
            {
                AccountId = accountId,
                Title = title,
                Message = message,
                Type = Enum.Parse<NotificationType>(type, true),
                State = EntityState.Add
            };
            return await SubmitData(dto);
        }

        public async Task<BaseResponse> SendBulkNotification(List<long> accountIds, string title, string message, string type = "info")
        {
            try
            {
                var notifications = new List<Notification>();
                foreach (var accountId in accountIds)
                {
                    var notification = new Notification
                    {
                        AccountId = accountId,
                        Title = title,
                        Message = message,
                        Type = Enum.Parse<NotificationType>(type, true),
                        CreatedBy = GetCurrentUserId()
                    };
                    notifications.Add(notification);
                }

                await Insert(notifications); // Use helper method from BaseBusinessService
                await UnitOfWork.SaveAsync();

                foreach (var notification in notifications)
                {
                    var dto = Mapper.Map<NotificationDTO>(notification);
                    await NotificationSocketHub.SendNotificationToUser(notification.AccountId, dto);
                }

                return BaseResponse.Success($"Đã gửi {notifications.Count} thông báo");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gửi thông báo hàng loạt: {ex.Message}");
            }
        }

        public async Task<List<NotificationDTO>> GetNotificationsByType(long accountId, string type)
        {
            var notificationType = Enum.Parse<NotificationType>(type, true);
            var notifications = await Repository.GetByCondition(n => n.AccountId == accountId && n.Type == notificationType && !n.IsDeleted);
            return Mapper.Map<List<NotificationDTO>>(notifications.OrderByDescending(n => n.CreatedDate).ToList());
        }
    }
}
