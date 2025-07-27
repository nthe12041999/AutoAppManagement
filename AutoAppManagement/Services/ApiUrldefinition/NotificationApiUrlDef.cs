namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class NotificationApiUrlDef
    {
        private const string pathController = "/api/Notification";

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetCountNotificationUnReadByAcc()
        {
            return @$"{pathController}/GetCountNotificationUnReadByAcc";
        }

        public static string MaskAsRead(long noticeId)
        {
            return @$"{pathController}/MaskAsRead?noticeId={noticeId}";
        }

        public static string GetNoticeByRange(int from, int to)
        {
            return @$"{pathController}/GetNoticeByRange?from={from}&&to={to}";
        }
    }
}
