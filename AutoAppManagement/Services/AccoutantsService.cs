using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.WebApp.Models;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IAccountsService : IBaseService
    {
        Task<ResponseOutput<LoginResponse>> Login(LoginViewModel loginViewModel);
        Task<ResponseOutput<string>> Register(AccountRegister loginViewModel);
        Task<List<AccountGenericVM>> GetAllAccount();
        Task<ResponseOutput<string>> UpdateLockedAccount(LockedAccountParam param);
        Task<AccountUpdate> GetUserInforGeneric();
        Task<ResponseOutput<UserInforGeneric>> UpdateUserInfor(AccountUpdateFile account);
        Task<ResponseOutput<string>> ChangePassword(ChangPasswordVM password);
        Task<ResponseOutput<string>> Logout();
    }

    public class AccountsService : BaseService, IAccountsService
    {
        public AccountsService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, config, httpContextAccessor)
        {

        }

        /// <summary>
        /// Hàm lấy tất cả api
        /// CreatedBy ntthe 14.09.2024
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountGenericVM>> GetAllAccount()
        {
            return await RequestAuthenGetAsync<List<AccountGenericVM>>(AccountApiUrlDef.GetAllAccount());
        }

        /// <summary>
        /// Hàm cập nhật trạng thái khóa tài khoản
        /// CreatedBy ntthe 14.09.2024
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> UpdateLockedAccount(LockedAccountParam param)
        {
            return await RequestFullAuthenPostAsync<string>(AccountApiUrlDef.UpdateLockedAccount(), param);
        }
        /// <summary>
        /// Hàm xử lý login
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseOutput<LoginResponse>> Login(LoginViewModel loginViewModel)
        {
            return await RequestFullPostAsync<LoginResponse>(AccountApiUrlDef.Login(), loginViewModel);
        }

        /// <summary>
        /// Hàm xử lý đăng ký
        /// CreatedBy ntthe 04.03.2024
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> Register(AccountRegister loginViewModel)
        {
            return await RequestFullPostAsync<string>(AccountApiUrlDef.Register(), loginViewModel);
        }

        /// <summary>
        /// Lấy thông tin user
        /// </summary>
        /// <returns></returns>
        public async Task<AccountUpdate> GetUserInforGeneric()
        {
            return await RequestAuthenGetAsync<AccountUpdate>(AccountApiUrlDef.GetUserInforGeneric());
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<ResponseOutput<UserInforGeneric>> UpdateUserInfor(AccountUpdateFile account)
        {
            if (account != null && account.SelectedImage != null)
            {
                var imgContent = await ConvertFileToBase64(account.SelectedImage);
                if (imgContent.Any())
                {
                    account.ImgContent = imgContent.FirstOrDefault();
                }
            }
            return await RequestFullAuthenPostAsync<UserInforGeneric>(AccountApiUrlDef.UpdateUserInfor(), account);
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> ChangePassword(ChangPasswordVM password)
        {
            return await RequestFullAuthenPostAsync<string>(AccountApiUrlDef.ChangePassword(), password);
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> Logout()
        {
            return await RequestFullPostAsync<string>(AccountApiUrlDef.Logout());
        }
    }
}
