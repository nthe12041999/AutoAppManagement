using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.WebApp.Models;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IAdminService : IBaseService
    {
        Task<ResponseOutput<LoginResponse>> Login(LoginViewModel loginViewModel);
        Task<ResponseOutput<string>> Register(AdminRegister adminRegister);
        Task<List<AdminGenericVM>> GetAllAdmin();
        Task<ResponseOutput<string>> UpdateLockedAdmin(LockedAdminParam param);
        Task<AdminUpdate> GetAdminInforGeneric();
        Task<ResponseOutput<AdminInforGeneric>> UpdateAdminInfor(AdminUpdateFile admin);
        Task<ResponseOutput<string>> ChangePassword(ChangPasswordVM password);
        Task<ResponseOutput<string>> Logout();
    }

    public class AdminService : BaseService, IAdminService
    {
        public AdminService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, config, httpContextAccessor)
        {

        }

        /// <summary>
        /// Hàm lấy tất cả admin
        /// CreatedBy ntthe 14.09.2024
        /// </summary>
        /// <returns></returns>
        public async Task<List<AdminGenericVM>> GetAllAdmin()
        {
            return await RequestAuthenGetAsync<List<AdminGenericVM>>(AdminAccountApiUrlDef.GetAllAdmin());
        }

        /// <summary>
        /// Hàm cập nhật trạng thái khóa admin
        /// CreatedBy ntthe 14.09.2024
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> UpdateLockedAdmin(LockedAdminParam param)
        {
            return await RequestFullAuthenPostAsync<string>(AdminAccountApiUrlDef.UpdateLockedAdmin(), param);
        }
        
        /// <summary>
        /// Hàm xử lý login admin
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public async Task<ResponseOutput<LoginResponse>> Login(LoginViewModel loginViewModel)
        {
            return await RequestFullPostAsync<LoginResponse>(AdminAccountApiUrlDef.Login(), loginViewModel);
        }

        /// <summary>
        /// Hàm xử lý đăng ký admin
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="adminRegister"></param>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> Register(AdminRegister adminRegister)
        {
            return await RequestFullPostAsync<string>(AdminAccountApiUrlDef.Register(), adminRegister);
        }

        /// <summary>
        /// Hàm lấy thông tin admin
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <returns></returns>
        public async Task<AdminUpdate> GetAdminInforGeneric()
        {
            return await RequestAuthenGetAsync<AdminUpdate>(AdminAccountApiUrlDef.GetAdminInforGeneric());
        }

        /// <summary>
        /// Hàm cập nhật thông tin admin
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public async Task<ResponseOutput<AdminInforGeneric>> UpdateAdminInfor(AdminUpdateFile admin)
        {
            // TODO: Implement RequestFullAuthenPostFileAsync
            return new ResponseOutput<AdminInforGeneric> { IsSuccess = true };
        }

        /// <summary>
        /// Hàm đổi mật khẩu admin
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> ChangePassword(ChangPasswordVM password)
        {
            return await RequestFullAuthenPostAsync<string>(AdminAccountApiUrlDef.ChangePassword(), password);
        }

        /// <summary>
        /// Hàm logout admin
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseOutput<string>> Logout()
        {
            return await RequestFullAuthenPostAsync<string>(AdminAccountApiUrlDef.Logout(), null);
        }
    }

    // ViewModels for Admin
    public class AdminRegister
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }

    public class AdminGenericVM
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsLocked { get; set; }
    }

    public class LockedAdminParam
    {
        public long AdminId { get; set; }
        public bool IsLocked { get; set; }
        public string Reason { get; set; }
    }

    public class AdminUpdate
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Role { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class AdminUpdateFile
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile AvatarFile { get; set; }
        public string Role { get; set; }
    }

    public class AdminInforGeneric
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Role { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
