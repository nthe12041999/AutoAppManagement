namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class AdminAccountApiUrlDef
    {
        private const string pathController = "/api/AdminAccount";

        /// <summary>
        /// Lấy danh sách tài khoản admin
        /// </summary>
        /// <returns></returns>
        public static string GetAdminAccounts()
        {
            return $"{pathController}";
        }

        /// <summary>
        /// Lấy tài khoản admin theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetAdminAccountById(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Tạo tài khoản admin mới
        /// </summary>
        /// <returns></returns>
        public static string CreateAdminAccount()
        {
            return $"{pathController}";
        }

        /// <summary>
        /// Cập nhật tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string UpdateAdminAccount(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Xóa tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeleteAdminAccount(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Tìm kiếm tài khoản admin
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string SearchAdminAccounts(string keyword = "", string role = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            return $"{pathController}/search?keyword={keyword}&role={role}&status={status}&pageIndex={pageIndex}&pageSize={pageSize}";
        }

        /// <summary>
        /// Thay đổi trạng thái tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ChangeAdminAccountStatus(long id)
        {
            return $"{pathController}/{id}/status";
        }

        /// <summary>
        /// Phân quyền cho tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string AssignPermissions(long id)
        {
            return $"{pathController}/{id}/permissions";
        }

        /// <summary>
        /// Lấy quyền hạn của tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetAdminPermissions(long id)
        {
            return $"{pathController}/{id}/permissions";
        }

        /// <summary>
        /// Lấy thống kê tài khoản admin
        /// </summary>
        /// <returns></returns>
        public static string GetAdminAccountStatistics()
        {
            return $"{pathController}/statistics";
        }

        /// <summary>
        /// Lấy danh sách admin đang online
        /// </summary>
        /// <returns></returns>
        public static string GetOnlineAdmins()
        {
            return $"{pathController}/online";
        }

        /// <summary>
        /// Đổi mật khẩu admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ChangePassword(long id)
        {
            return $"{pathController}/{id}/change-password";
        }

        /// <summary>
        /// Reset mật khẩu admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ResetPassword(long id)
        {
            return $"{pathController}/{id}/reset-password";
        }

        /// <summary>
        /// Lấy lịch sử đăng nhập của admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetLoginHistory(long id)
        {
            return $"{pathController}/{id}/login-history";
        }

        /// <summary>
        /// Xuất danh sách admin ra Excel
        /// </summary>
        /// <returns></returns>
        public static string ExportAdminAccountsToExcel()
        {
            return $"{pathController}/export";
        }
    }
}
