using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface INotificationService : IBaseService
    {
        Task<int> GetCountNotificationUnReadByAcc();
        Task<bool> MaskAsRead(long noticeId);
        Task<IEnumerable<Notification>> GetNoticeByRange(int from, int to);
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
    }
}
