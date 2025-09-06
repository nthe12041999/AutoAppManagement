using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class AdminAccountApiUrlDef: BaseApiUrlDef
    {

        public AdminAccountApiUrlDef() : base("/api/AdminAccount") { }

        /// <summary>
        /// Đăng nhập admin
        /// </summary>
        /// <returns></returns>
        public string Login()
        {
            return $"{_pathController}/login";
        }

        /// <summary>
        /// Đăng ký admin
        /// </summary>
        /// <returns></returns>
        public string Register()
        {
            return $"{_pathController}/register";
        }

        /// <summary>
        /// Đăng xuất admin
        /// </summary>
        /// <returns></returns>
        public string Logout()
        {
            return $"{_pathController}/logout";
        }

        /// <summary>
        /// Lấy thông tin admin hiện tại
        /// </summary>
        /// <returns></returns>
        public string GetAdminInforGeneric()
        {
            return $"{_pathController}/profile";
        }

        /// <summary>
        /// Cập nhật thông tin admin
        /// </summary>
        /// <returns></returns>
        public string UpdateAdminInfor()
        {
            return $"{_pathController}/profile";
        }

        /// <summary>
        /// Cập nhật trạng thái khóa admin
        /// </summary>
        /// <returns></returns>
        public string UpdateLockedAdmin()
        {
            return $"{_pathController}/lock-status";
        }

        /// <summary>
        /// Thay đổi trạng thái tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ChangeAdminStatus(long id)
        {
            return $"{_pathController}/{id}/status";
        }

        /// <summary>
        /// Đổi mật khẩu admin
        /// </summary>
        /// <returns></returns>
        public string ChangePassword()
        {
            return $"{_pathController}/change-password";
        }

        /// <summary>
        /// Reset mật khẩu admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ResetPassword(long id)
        {
            return $"{_pathController}/{id}/reset-password";
        }

        /// <summary>
        /// Phân quyền cho tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string AssignPermissions(long id)
        {
            return $"{_pathController}/{id}/permissions";
        }

        /// <summary>
        /// Lấy quyền hạn của tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetAdminPermissions(long id)
        {
            return $"{_pathController}/{id}/permissions";
        }

        /// <summary>
        /// Lấy thống kê tài khoản admin
        /// </summary>
        /// <returns></returns>
        public string GetAdminStatistics()
        {
            return $"{_pathController}/statistics";
        }

        /// <summary>
        /// Lấy danh sách admin đang online
        /// </summary>
        /// <returns></returns>
        public string GetOnlineAdmins()
        {
            return $"{_pathController}/online";
        }

        /// <summary>
        /// Lấy lịch sử đăng nhập của admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetLoginHistory(long id)
        {
            return $"{_pathController}/{id}/login-history";
        }

        /// <summary>
        /// Lấy lịch sử hoạt động của admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetActivityHistory(long id)
        {
            return $"{_pathController}/{id}/activity-history";
        }

        /// <summary>
        /// Gửi thông báo đến admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SendNotification(long id)
        {
            return $"{_pathController}/{id}/notification";
        }

        /// <summary>
        /// Gửi thông báo đến tất cả admin
        /// </summary>
        /// <returns></returns>
        public string BroadcastNotification()
        {
            return $"{_pathController}/broadcast-notification";
        }
    }
}
