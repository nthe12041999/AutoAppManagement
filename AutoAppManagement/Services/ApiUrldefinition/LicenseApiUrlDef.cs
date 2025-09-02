using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class LicenseApiUrlDef : BaseApiUrlDef
    {
        protected static string pathController = "/api/License";

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string RenewLicense(long id)
        {
            return $"{pathController}/{id}/renew";
        }

        /// <summary>
        /// Tạm dừng license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SuspendLicense(long id)
        {
            return $"{pathController}/{id}/suspend";
        }

        /// <summary>
        /// Kích hoạt license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ActivateLicense(long id)
        {
            return $"{pathController}/{id}/activate";
        }
        /// <summary>
        /// Lấy thống kê license
        /// </summary>
        /// <returns></returns>
        public static string GetLicenseStatistics()
        {
            return $"{pathController}/statistics";
        }

        /// <summary>
        /// Lấy license theo khách hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public static string GetLicensesByCustomer(long customerId)
        {
            return $"{pathController}/customer/{customerId}";
        }

        /// <summary>
        /// Kiểm tra license sắp hết hạn
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static string GetExpiringLicenses(int days = 30)
        {
            return $"{pathController}/expiring?days={days}";
        }


        /// <summary>
        /// Lấy lịch sử thay đổi license
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        public static string GetLicenseHistory(long licenseId)
        {
            return $"{pathController}/{licenseId}/history";
        }
    }
}
