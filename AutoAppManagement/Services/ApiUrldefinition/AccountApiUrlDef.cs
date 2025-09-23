using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class AccountApiUrlDef: BaseApiUrlDef
    {
        public AccountApiUrlDef():base("/api/Account") { }

        /// <summary>
        /// Tạo url đăng nhập
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <returns></returns>
        public string Login()
        {
            return @$"{_pathController}/Login";
        }

        /// <summary>
        /// Tạo url đăng ký
        /// CreatedBy ntthe 04.03.2024
        /// </summary>
        /// <returns></returns>
        public string Register()
        {
            return @$"{_pathController}/Register";
        }

        /// <summary>
        /// Lấy thông tin user
        /// </summary>
        public string GetUserInforGeneric()
        {
            return @$"{_pathController}/GetUserInforGeneric";
        }

        /// <summary>
        /// Lấy thông tin user
        /// </summary>
        public string UpdateUserInfor()
        {
            return @$"{_pathController}/UpdateUserInfor";
        }

        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        public string ChangePassword()
        {
            return @$"{_pathController}/ChangePassword";
        }

        /// <summary>
        /// Đăng ký tác giả
        /// </summary>
        public string RegisterAuthorAccount()
        {
            return @$"{_pathController}/RegisterAuthorAccount";
        }

        /// <summary>
        /// Lấy danh sách user theo role
        /// </summary>
        /// <returns></returns>
        public string GetRegisterAccountsByRole()
        {
            return @$"{_pathController}/GetRegisterAccountsByRole";
        }

        /// <summary>
        /// Cập nhật trạng thái khóa tài khoản
        /// </summary>
        /// <returns></returns>
        public string UpdateLockedAccount()
        {
            return @$"{_pathController}/UpdateLockedAccount";
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        public string Logout()
        {
            return @$"{_pathController}/Logout";
        }

        public string UpdateAccountMaxAcc()
        {
            return @$"{_pathController}/UpdateAccountMaxAcc";
        }

        public string GetAccountByUsername(string username)
        {
            return @$"{_pathController}/GetAccountByUsername?username={username}";
        }

        public string CreateAccount()
        {
            return @$"{_pathController}/CreateAccount";
        }

        public string UpdateAccount()
        {
            return @$"{_pathController}/UpdateAccount";
        }

        public string DeleteAccount(long id)
        {
            return @$"{_pathController}/DeleteAccount?id={id}";
        }

        public string LockAccount()
        {
            return @$"{_pathController}/LockAccount";
        }

        public string UnlockAccount(long id)
        {
            return @$"{_pathController}/UnlockAccount?id={id}";
        }

        public string ActivateAccount(long id)
        {
            return @$"{_pathController}/ActivateAccount?id={id}";
        }

        public string DeactivateAccount(long id)
        {
            return @$"{_pathController}/DeactivateAccount?id={id}";
        }

        public string GetExpiredAccounts()
        {
            return @$"{_pathController}/GetExpiredAccounts";
        }

        public string GetExpiringAccounts(int days)
        {
            return @$"{_pathController}/GetExpiringAccounts?days={days}";
        }

        public string ExtendAccount()
        {
            return @$"{_pathController}/ExtendAccount";
        }

        public string ValidateAccount()
        {
            return @$"{_pathController}/ValidateAccount";
        }

        public string UpdateAccountInfo()
        {
            return @$"{_pathController}/UpdateAccountInfo";
        }

        public string UploadAvatar()
        {
            return @$"{_pathController}/UploadAvatar";
        }

        // AccountDevice URLs
        public string GetAllAccountDevices()
        {
            return @$"{_pathController}/GetAllAccountDevices";
        }

        public string GetAccountDevicesByAccountId(long accountId)
        {
            return @$"{_pathController}/GetAccountDevicesByAccountId?accountId={accountId}";
        }

        public string GetAccountDeviceById(long id)
        {
            return @$"{_pathController}/GetAccountDeviceById?id={id}";
        }

        public string RegisterDevice()
        {
            return @$"{_pathController}/RegisterDevice";
        }

        public string UpdateDevice()
        {
            return @$"{_pathController}/UpdateDevice";
        }

        public string DeleteDevice(long id)
        {
            return @$"{_pathController}/DeleteDevice?id={id}";
        }

        public string ActivateDevice(long id)
        {
            return @$"{_pathController}/ActivateDevice?id={id}";
        }

        public string DeactivateDevice(long id)
        {
            return @$"{_pathController}/DeactivateDevice?id={id}";
        }

        public string GetActiveDevices(long accountId)
        {
            return @$"{_pathController}/GetActiveDevices?accountId={accountId}";
        }

        public string GetDevicesByType(string deviceType)
        {
            return @$"{_pathController}/GetDevicesByType?deviceType={deviceType}";
        }

        public string IsDeviceRegistered(string deviceId, long accountId)
        {
            return @$"{_pathController}/IsDeviceRegistered?deviceId={deviceId}&accountId={accountId}";
        }
    }
}
