using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class LicenseApiUrlDef : BaseApiUrlDef
    {
        public LicenseApiUrlDef() : base("/api/License") { }

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string RenewLicense(long id)
        {
            return $"{_pathController}/{id}/renew";
        }

        /// <summary>
        /// Tạm dừng license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SuspendLicense(long id)
        {
            return $"{_pathController}/{id}/suspend";
        }

        /// <summary>
        /// Kích hoạt license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ActivateLicense(long id)
        {
            return $"{_pathController}/{id}/activate";
        }
        /// <summary>
        /// Lấy thống kê license
        /// </summary>
        /// <returns></returns>
        public string GetLicenseStatistics()
        {
            return $"{_pathController}/statistics";
        }

        /// <summary>
        /// Lấy license theo khách hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public string GetLicensesByCustomer(long customerId)
        {
            return $"{_pathController}/customer/{customerId}";
        }

        /// <summary>
        /// Kiểm tra license sắp hết hạn
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public string GetExpiringLicenses(int days = 30)
        {
            return $"{_pathController}/expiring?days={days}";
        }


        /// <summary>
        /// Lấy lịch sử thay đổi license
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        public string GetLicenseHistory(long licenseId)
        {
            return $"{_pathController}/{licenseId}/history";
        }
    }
}
