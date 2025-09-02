using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class AccountApiUrlDef: BaseApiUrlDef
    {
        protected static string pathController = "/api/Account";

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

        public static string GetAccountByUsername(string username)
        {
            return @$"{pathController}/GetAccountByUsername?username={username}";
        }

        public static string CreateAccount()
        {
            return @$"{pathController}/CreateAccount";
        }

        public static string UpdateAccount()
        {
            return @$"{pathController}/UpdateAccount";
        }

        public static string DeleteAccount(long id)
        {
            return @$"{pathController}/DeleteAccount?id={id}";
        }

        public static string LockAccount()
        {
            return @$"{pathController}/LockAccount";
        }

        public static string UnlockAccount(long id)
        {
            return @$"{pathController}/UnlockAccount?id={id}";
        }

        public static string ActivateAccount(long id)
        {
            return @$"{pathController}/ActivateAccount?id={id}";
        }

        public static string DeactivateAccount(long id)
        {
            return @$"{pathController}/DeactivateAccount?id={id}";
        }

        public static string GetAccountsByLevel(int level)
        {
            return @$"{pathController}/GetAccountsByLevel?level={level}";
        }

        public static string GetExpiredAccounts()
        {
            return @$"{pathController}/GetExpiredAccounts";
        }

        public static string GetExpiringAccounts(int days)
        {
            return @$"{pathController}/GetExpiringAccounts?days={days}";
        }

        public static string ExtendAccount()
        {
            return @$"{pathController}/ExtendAccount";
        }

        public static string ValidateAccount()
        {
            return @$"{pathController}/ValidateAccount";
        }

        public static string UpdateAccountInfo()
        {
            return @$"{pathController}/UpdateAccountInfo";
        }

        public static string UploadAvatar()
        {
            return @$"{pathController}/UploadAvatar";
        }

        // AccountDevice URLs
        public static string GetAllAccountDevices()
        {
            return @$"{pathController}/GetAllAccountDevices";
        }

        public static string GetAccountDevicesByAccountId(long accountId)
        {
            return @$"{pathController}/GetAccountDevicesByAccountId?accountId={accountId}";
        }

        public static string GetAccountDeviceById(long id)
        {
            return @$"{pathController}/GetAccountDeviceById?id={id}";
        }

        public static string RegisterDevice()
        {
            return @$"{pathController}/RegisterDevice";
        }

        public static string UpdateDevice()
        {
            return @$"{pathController}/UpdateDevice";
        }

        public static string DeleteDevice(long id)
        {
            return @$"{pathController}/DeleteDevice?id={id}";
        }

        public static string ActivateDevice(long id)
        {
            return @$"{pathController}/ActivateDevice?id={id}";
        }

        public static string DeactivateDevice(long id)
        {
            return @$"{pathController}/DeactivateDevice?id={id}";
        }

        public static string GetActiveDevices(long accountId)
        {
            return @$"{pathController}/GetActiveDevices?accountId={accountId}";
        }

        public static string GetDevicesByType(string deviceType)
        {
            return @$"{pathController}/GetDevicesByType?deviceType={deviceType}";
        }

        public static string IsDeviceRegistered(string deviceId, long accountId)
        {
            return @$"{pathController}/IsDeviceRegistered?deviceId={deviceId}&accountId={accountId}";
        }
    }
}
