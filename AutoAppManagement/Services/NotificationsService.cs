using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface INotificationService : IBaseService
    {
        Task<int> GetCountNotificationUnReadByAcc();
        Task<bool> MaskAsRead(long noticeId);
        Task<IEnumerable<Notification>> GetNoticeByRange(int from, int to);

        // New methods for NotificationController
        Task<List<NotificationDTO>> GetNotificationsByAccountId(long accountId);
        Task<List<NotificationDTO>> GetUnreadNotifications(long accountId);
        Task<NotificationDTO> GetNotificationById(long id);
        Task<bool> CreateNotification(CreateNotificationRequest request);
        Task<bool> UpdateNotification(UpdateNotificationRequest request);
        Task<bool> DeleteNotification(long id);
        Task<bool> MarkAsRead(long id);
        Task<bool> MarkAllAsRead(long accountId);
        Task<int> GetUnreadCount(long accountId);
        Task<bool> SendBulkNotification(List<long> accountIds, string title, string message, string type);
        Task<List<NotificationDTO>> GetNotificationsByType(long accountId, string type);
    }

    public class NotificationsService : BaseService, INotificationService
    {
        public NotificationsService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, config, httpContextAccessor)
        {

        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCountNotificationUnReadByAcc()
        {
            return await RequestAuthenGetAsync<int>(NotificationApiUrlDef.GetCountNotificationUnReadByAcc());
        }

        public async Task<bool> MaskAsRead(long noticeId)
        {
            return await RequestAuthenPostAsync<bool>(NotificationApiUrlDef.MaskAsRead(noticeId));
        }

        public async Task<IEnumerable<Notification>> GetNoticeByRange(int from, int to)
        {
            return await RequestAuthenGetAsync<IEnumerable<Notification>>(NotificationApiUrlDef.GetNoticeByRange(from, to));
        }

        // Implementation for new methods
        public async Task<List<NotificationDTO>> GetNotificationsByAccountId(long accountId)
        {
            return await RequestAuthenGetAsync<List<NotificationDTO>>($"/api/Notification/GetNotificationsByAccountId?accountId={accountId}");
        }

        public async Task<List<NotificationDTO>> GetUnreadNotifications(long accountId)
        {
            return await RequestAuthenGetAsync<List<NotificationDTO>>($"/api/Notification/GetUnreadNotifications?accountId={accountId}");
        }

        public async Task<NotificationDTO> GetNotificationById(long id)
        {
            return await RequestAuthenGetAsync<NotificationDTO>($"/api/Notification/GetNotificationById?id={id}");
        }

        public async Task<bool> CreateNotification(CreateNotificationRequest request)
        {
            return await RequestAuthenPostAsync<bool>("/api/Notification/CreateNotification", request);
        }

        public async Task<bool> UpdateNotification(UpdateNotificationRequest request)
        {
            return await RequestAuthenPutAsync<bool>("/api/Notification/UpdateNotification", request);
        }

        public async Task<bool> DeleteNotification(long id)
        {
            return await RequestAuthenDeleteAsync<bool>($"/api/Notification/DeleteNotification?id={id}");
        }

        public async Task<bool> MarkAsRead(long id)
        {
            return await RequestAuthenPostAsync<bool>($"/api/Notification/MarkAsRead?id={id}");
        }

        public async Task<bool> MarkAllAsRead(long accountId)
        {
            return await RequestAuthenPostAsync<bool>($"/api/Notification/MarkAllAsRead?accountId={accountId}");
        }

        public async Task<int> GetUnreadCount(long accountId)
        {
            return await RequestAuthenGetAsync<int>($"/api/Notification/GetUnreadCount?accountId={accountId}");
        }

        public async Task<bool> SendBulkNotification(List<long> accountIds, string title, string message, string type)
        {
            var request = new SendBulkNotificationRequest
            {
                AccountIds = accountIds,
                Title = title,
                Message = message,
                Type = type
            };
            return await RequestAuthenPostAsync<bool>("/api/Notification/SendBulkNotification", request);
        }

        public async Task<List<NotificationDTO>> GetNotificationsByType(long accountId, string type)
        {
            return await RequestAuthenGetAsync<List<NotificationDTO>>($"/api/Notification/GetNotificationsByType?accountId={accountId}&type={type}");
        }
    }
}
