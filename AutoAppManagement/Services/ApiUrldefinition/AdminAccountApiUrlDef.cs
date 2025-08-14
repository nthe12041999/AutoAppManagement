namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class AdminAccountApiUrlDef
    {
        private const string pathController = "/api/AdminAccount";

        /// <summary>
        /// Đăng nhập admin
        /// </summary>
        /// <returns></returns>
        public static string Login()
        {
            return $"{pathController}/login";
        }

        /// <summary>
        /// Đăng ký admin
        /// </summary>
        /// <returns></returns>
        public static string Register()
        {
            return $"{pathController}/register";
        }

        /// <summary>
        /// Đăng xuất admin
        /// </summary>
        /// <returns></returns>
        public static string Logout()
        {
            return $"{pathController}/logout";
        }

        /// <summary>
        /// Lấy danh sách tài khoản admin
        /// </summary>
        /// <returns></returns>
        public static string GetAllAdmin()
        {
            return $"{pathController}";
        }

        /// <summary>
        /// Lấy tài khoản admin theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetAdminById(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Lấy thông tin admin hiện tại
        /// </summary>
        /// <returns></returns>
        public static string GetAdminInforGeneric()
        {
            return $"{pathController}/profile";
        }

        /// <summary>
        /// Cập nhật thông tin admin
        /// </summary>
        /// <returns></returns>
        public static string UpdateAdminInfor()
        {
            return $"{pathController}/profile";
        }

        /// <summary>
        /// Tạo tài khoản admin mới
        /// </summary>
        /// <returns></returns>
        public static string CreateAdmin()
        {
            return $"{pathController}";
        }

        /// <summary>
        /// Cập nhật tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string UpdateAdmin(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Xóa tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeleteAdmin(long id)
        {
            return $"{pathController}/{id}";
        }

        /// <summary>
        /// Cập nhật trạng thái khóa admin
        /// </summary>
        /// <returns></returns>
        public static string UpdateLockedAdmin()
        {
            return $"{pathController}/lock-status";
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
        public static string SearchAdmins(string keyword = "", string role = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            return $"{pathController}/search?keyword={keyword}&role={role}&status={status}&pageIndex={pageIndex}&pageSize={pageSize}";
        }

        /// <summary>
        /// Thay đổi trạng thái tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ChangeAdminStatus(long id)
        {
            return $"{pathController}/{id}/status";
        }

        /// <summary>
        /// Đổi mật khẩu admin
        /// </summary>
        /// <returns></returns>
        public static string ChangePassword()
        {
            return $"{pathController}/change-password";
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
        public static string GetAdminStatistics()
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
        /// Lấy lịch sử đăng nhập của admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetLoginHistory(long id)
        {
            return $"{pathController}/{id}/login-history";
        }

        /// <summary>
        /// Lấy lịch sử hoạt động của admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetActivityHistory(long id)
        {
            return $"{pathController}/{id}/activity-history";
        }

        /// <summary>
        /// Xuất danh sách admin ra Excel
        /// </summary>
        /// <returns></returns>
        public static string ExportAdminsToExcel()
        {
            return $"{pathController}/export";
        }

        /// <summary>
        /// Gửi thông báo đến admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SendNotification(long id)
        {
            return $"{pathController}/{id}/notification";
        }

        /// <summary>
        /// Gửi thông báo đến tất cả admin
        /// </summary>
        /// <returns></returns>
        public static string BroadcastNotification()
        {
            return $"{pathController}/broadcast-notification";
        }
    }
}
