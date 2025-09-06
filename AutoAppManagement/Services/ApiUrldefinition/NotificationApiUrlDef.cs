using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class NotificationApiUrlDef : BaseApiUrlDef
    {
        public NotificationApiUrlDef() : base("/api/Notification") { }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string GetCountNotificationUnReadByAcc()
        {
            return @$"{_pathController}/GetCountNotificationUnReadByAcc";
        }

        public string MaskAsRead(long noticeId)
        {
            return @$"{_pathController}/MaskAsRead?noticeId={noticeId}";
        }

        public string GetNoticeByRange(int from, int to)
        {
            return @$"{_pathController}/GetNoticeByRange?from={from}&&to={to}";
        }
    }
}
