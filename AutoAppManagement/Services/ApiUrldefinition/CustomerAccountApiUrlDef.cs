namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class CustomerAccountApiUrlDef
    {
        private const string pathController = "/api/CustomerAccount";

        /// <summary>
        /// Lấy danh sách tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        public static string GetCustomerAccounts()
        {
            return $"{pathController}/accounts";
        }

        /// <summary>
        /// Lấy tài khoản khách hàng theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetCustomerAccountById(long id)
        {
            return $"{pathController}/accounts/{id}";
        }

        /// <summary>
        /// Tạo tài khoản khách hàng mới
        /// </summary>
        /// <returns></returns>
        public static string CreateCustomerAccount()
        {
            return $"{pathController}/accounts";
        }

        /// <summary>
        /// Cập nhật tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string UpdateCustomerAccount(long id)
        {
            return $"{pathController}/accounts/{id}";
        }

        /// <summary>
        /// Xóa tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeleteCustomerAccount(long id)
        {
            return $"{pathController}/accounts/{id}";
        }

        /// <summary>
        /// Tìm kiếm tài khoản khách hàng
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="role"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string SearchCustomerAccounts(string keyword = "", string status = "", string role = "", int pageIndex = 1, int pageSize = 10)
        {
            return $"{pathController}/accounts/search?keyword={keyword}&status={status}&role={role}&pageIndex={pageIndex}&pageSize={pageSize}";
        }

        /// <summary>
        /// Thay đổi trạng thái tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ChangeCustomerAccountStatus(long id)
        {
            return $"{pathController}/accounts/{id}/status";
        }

        /// <summary>
        /// Lấy thống kê tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        public static string GetCustomerAccountStatistics()
        {
            return $"{pathController}/accounts/statistics";
        }

        /// <summary>
        /// Xuất danh sách tài khoản khách hàng ra Excel
        /// </summary>
        /// <returns></returns>
        public static string ExportCustomerAccountsToExcel()
        {
            return $"{pathController}/accounts/export";
        }
    }
}
