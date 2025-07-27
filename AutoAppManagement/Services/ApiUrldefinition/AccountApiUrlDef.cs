namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class AccountApiUrlDef
    {
        private const string pathController = "/api/Account";

        /// <summary>
        /// Tạo url đăng nhập
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <returns></returns>
        public static string Login()
        {
            return @$"{pathController}/Login";
        }

        /// <summary>
        /// Tạo url đăng ký
        /// CreatedBy ntthe 04.03.2024
        /// </summary>
        /// <returns></returns>
        public static string Register()
        {
            return @$"{pathController}/Register";
        }

        public static string GetAllAccount()
        {
            return @$"{pathController}/GetAllAccount";
        }

        /// <summary>
        /// Lấy thông tin user
        /// </summary>
        public static string GetUserInforGeneric()
        {
            return @$"{pathController}/GetUserInforGeneric";
        }

        /// <summary>
        /// Lấy thông tin user
        /// </summary>
        public static string UpdateUserInfor()
        {
            return @$"{pathController}/UpdateUserInfor";
        }

        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        public static string ChangePassword()
        {
            return @$"{pathController}/ChangePassword";
        }

        /// <summary>
        /// Đăng ký tác giả
        /// </summary>
        public static string RegisterAuthorAccount()
        {
            return @$"{pathController}/RegisterAuthorAccount";
        }

        /// <summary>
        /// Lấy danh sách user theo role
        /// </summary>
        /// <returns></returns>
        public static string GetRegisterAccountsByRole()
        {
            return @$"{pathController}/GetRegisterAccountsByRole";
        }

        /// <summary>
        /// Lấy toàn bộ danh sách người dùng
        /// </summary>
        /// <returns></returns>
        public static string GetAllAccounts()
        {
            return @$"{pathController}/GetAll";
        }

        /// <summary>
        /// Cập nhật trạng thái khóa tài khoản
        /// </summary>
        /// <returns></returns>
        public static string UpdateLockedAccount()
        {
            return @$"{pathController}/UpdateLockedAccount";
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        public static string Logout()
        {
            return @$"{pathController}/Logout";
        }

        public static string UpdateAccountMaxAcc()
        {
            return @$"{pathController}/UpdateAccountMaxAcc";
        }
    }
}
