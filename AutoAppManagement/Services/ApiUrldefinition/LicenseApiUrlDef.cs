namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class LicenseApiUrlDef
    {
        private const string pathController = "/api/License";

        /// <summary>
        /// Lấy danh sách license
        /// </summary>
        /// <returns></returns>
        public static string GetLicenses()
        {
            return $"{pathController}";
        }

        /// <summary>
        /// Lấy license theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetLicenseById(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Tạo license mới
        /// </summary>
        /// <returns></returns>
        public static string CreateLicense()
        {
            return $"{pathController}";
        }

        /// <summary>
        /// Cập nhật license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string UpdateLicense(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Xóa license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeleteLicense(long id)
        {
            return $"{pathController}/{id}";
        }

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
        /// Tìm kiếm license
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string SearchLicenses(string keyword = "", string type = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            return $"{pathController}/search?keyword={keyword}&type={type}&status={status}&pageIndex={pageIndex}&pageSize={pageSize}";
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
        /// Xuất danh sách license ra Excel
        /// </summary>
        /// <returns></returns>
        public static string ExportLicensesToExcel()
        {
            return $"{pathController}/export";
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
