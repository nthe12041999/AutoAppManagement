using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AccountDevice;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IAccountService : IBaseBusinessService<AccountDTO>
    {
        Task<AccountDTO> GetAccountByUsername(string username);
        Task<bool> ChangePassword(long id, string newPassword);
        Task<bool> LockAccount(long id, string reason = "");
        Task<bool> UnlockAccount(long id);
        Task<bool> ActivateAccount(long id);
        Task<bool> DeactivateAccount(long id);
        Task<List<AccountDTO>> GetAccountsByLevel(int level);
        Task<List<AccountDTO>> GetExpiredAccounts();
        Task<List<AccountDTO>> GetExpiringAccounts(int days);
        Task<bool> ExtendAccount(long id, DateTime newExpiryDate);
        Task<bool> ValidateAccount(string username, string password);
        Task<bool> UpdateAccountInfo(UpdateAccountInfoRequest request);
        Task<bool> UploadAvatar(long id, string avatarPath);
        Task<LoginResponse> Login(LoginRequest request);

        // AccountDevice methods
        Task<List<AccountDeviceDTO>> GetAllAccountDevices();
        Task<List<AccountDeviceDTO>> GetAccountDevicesByAccountId(long accountId);
        Task<AccountDeviceDTO> GetAccountDeviceById(long id);
        Task<bool> RegisterDevice(RegisterDeviceRequest request);
        Task<bool> UpdateDevice(UpdateDeviceRequest request);
        Task<bool> DeleteDevice(long id);
        Task<bool> ActivateDevice(long id);
        Task<bool> DeactivateDevice(long id);
        Task<List<AccountDeviceDTO>> GetActiveDevices(long accountId);
        Task<List<AccountDeviceDTO>> GetDevicesByType(string deviceType);
        Task<bool> IsDeviceRegistered(string deviceId, long accountId);
    }

    public class AccountService : BaseBusinessService<AccountDTO>, IAccountService
    {
        public AccountService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Lấy account theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AccountDTO> GetAccountByUsername(string username)
        {
            return await RequestAuthenGetAsync<AccountDTO>(AccountApiUrlDef.GetAccountByUsername(username));
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> ChangePassword(long id, string newPassword)
        {
            var request = new ChangePasswordRequest { Id = id, NewPassword = newPassword };
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.ChangePassword(), request);
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<bool> LockAccount(long id, string reason = "")
        {
            var request = new LockAccountRequest { Id = id, Reason = reason };
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.LockAccount(), request);
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> UnlockAccount(long id)
        {
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.UnlockAccount(id));
        }

        /// <summary>
        /// Kích hoạt tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ActivateAccount(long id)
        {
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.ActivateAccount(id));
        }

        /// <summary>
        /// Vô hiệu hóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeactivateAccount(long id)
        {
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.DeactivateAccount(id));
        }

        /// <summary>
        /// Lấy accounts theo level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<List<AccountDTO>> GetAccountsByLevel(int level)
        {
            return await RequestAuthenGetAsync<List<AccountDTO>>(AccountApiUrlDef.GetAccountsByLevel(level));
        }

        /// <summary>
        /// Lấy accounts đã hết hạn
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountDTO>> GetExpiredAccounts()
        {
            return await RequestAuthenGetAsync<List<AccountDTO>>(AccountApiUrlDef.GetExpiredAccounts());
        }

        /// <summary>
        /// Lấy accounts sắp hết hạn
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<List<AccountDTO>> GetExpiringAccounts(int days)
        {
            return await RequestAuthenGetAsync<List<AccountDTO>>(AccountApiUrlDef.GetExpiringAccounts(days));
        }

        /// <summary>
        /// Gia hạn account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newExpiryDate"></param>
        /// <returns></returns>
        public async Task<bool> ExtendAccount(long id, DateTime newExpiryDate)
        {
            var request = new ExtendAccountRequest { Id = id, NewExpiryDate = newExpiryDate };
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.ExtendAccount(), request);
        }

        /// <summary>
        /// Kiểm tra tài khoản hợp lệ
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAccount(string username, string password)
        {
            var request = new ValidateAccountRequest { Username = username, Password = password };
            return await RequestPostAsync<bool>(AccountApiUrlDef.ValidateAccount(), request);
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAccountInfo(UpdateAccountInfoRequest request)
        {
            return await RequestAuthenPutAsync<bool>(AccountApiUrlDef.UpdateAccountInfo(), request);
        }

        /// <summary>
        /// Upload avatar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatarPath"></param>
        /// <returns></returns>
        public async Task<bool> UploadAvatar(long id, string avatarPath)
        {
            var request = new UploadAvatarRequest { Id = id, AvatarPath = avatarPath };
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.UploadAvatar(), request);
        }

        /// <summary>
        /// Đăng nhập bằng email/sdt và password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            return await RequestPostAsync<LoginResponse>(AccountApiUrlDef.Login(), request);
        }

        #region AccountDevice Methods

        /// <summary>
        /// Lấy tất cả account devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetAllAccountDevices()
        {
            return await RequestAuthenGetAsync<List<AccountDeviceDTO>>(AccountApiUrlDef.GetAllAccountDevices());
        }

        /// <summary>
        /// Lấy devices theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetAccountDevicesByAccountId(long accountId)
        {
            return await RequestAuthenGetAsync<List<AccountDeviceDTO>>(AccountApiUrlDef.GetAccountDevicesByAccountId(accountId));
        }

        /// <summary>
        /// Lấy device theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountDeviceDTO> GetAccountDeviceById(long id)
        {
            return await RequestAuthenGetAsync<AccountDeviceDTO>(AccountApiUrlDef.GetAccountDeviceById(id));
        }

        /// <summary>
        /// Đăng ký device mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> RegisterDevice(RegisterDeviceRequest request)
        {
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.RegisterDevice(), request);
        }

        /// <summary>
        /// Cập nhật device
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDevice(UpdateDeviceRequest request)
        {
            return await RequestAuthenPutAsync<bool>(AccountApiUrlDef.UpdateDevice(), request);
        }

        /// <summary>
        /// Xóa device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDevice(long id)
        {
            return await RequestAuthenDeleteAsync<bool>(AccountApiUrlDef.DeleteDevice(id));
        }

        /// <summary>
        /// Kích hoạt device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ActivateDevice(long id)
        {
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.ActivateDevice(id));
        }

        /// <summary>
        /// Vô hiệu hóa device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeactivateDevice(long id)
        {
            return await RequestAuthenPostAsync<bool>(AccountApiUrlDef.DeactivateDevice(id));
        }

        /// <summary>
        /// Lấy devices đang hoạt động của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetActiveDevices(long accountId)
        {
            return await RequestAuthenGetAsync<List<AccountDeviceDTO>>(AccountApiUrlDef.GetActiveDevices(accountId));
        }

        /// <summary>
        /// Lấy devices theo loại
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetDevicesByType(string deviceType)
        {
            return await RequestAuthenGetAsync<List<AccountDeviceDTO>>(AccountApiUrlDef.GetDevicesByType(deviceType));
        }

        /// <summary>
        /// Kiểm tra device đã đăng ký chưa
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<bool> IsDeviceRegistered(string deviceId, long accountId)
        {
            return await RequestAuthenGetAsync<bool>(AccountApiUrlDef.IsDeviceRegistered(deviceId, accountId));
        }

        #endregion
    }
}
