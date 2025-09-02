using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Models.ViewModel.AdminAccount;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IAdminAccountService : IBaseBusinessService<AdminAccountDTO>
    {
        Task<ResponseOutput<bool>> ChangeAdminAccountStatusAsync(long id, string status);
        Task<ResponseOutput<bool>> AssignPermissionsAsync(long id, List<string> permissions);
        Task<AdminAccountStatisticsViewModel> GetAdminAccountStatisticsAsync();
        Task<List<AdminAccountViewModel>> GetOnlineAdminsAsync();
        Task<ResponseOutput<bool>> ChangePasswordAsync(long id, ChangePasswordViewModel model);
        Task<ResponseOutput<bool>> ResetPasswordAsync(long id);
        Task<List<LoginHistoryViewModel>> GetLoginHistoryAsync(long id);

        Task<TokenViewModel> Login(LoginViewModel loginData);
    }

    public class AdminAccountService : BaseBusinessService<AdminAccountDTO>, IAdminAccountService
    {
        public AdminAccountService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<ResponseOutput<bool>> ChangeAdminAccountStatusAsync(long id, string status)
        {
            // Mock implementation
            return new ResponseOutput<bool>
            {
                IsSuccess = true,
                Data = true
            };
        }

        public async Task<TokenViewModel> Login(LoginViewModel loginData)
        {
            return await RequestPostAsync<TokenViewModel>(AdminAccountApiUrlDef.Login(), loginData);
        }

        public async Task<ResponseOutput<bool>> AssignPermissionsAsync(long id, List<string> permissions)
        {
            // Mock implementation
            return new ResponseOutput<bool>
            {
                IsSuccess = true,
                Data = true
            };
        }

        public async Task<AdminAccountStatisticsViewModel> GetAdminAccountStatisticsAsync()
        {
            // Mock implementation
            return new AdminAccountStatisticsViewModel();
        }

        public async Task<List<AdminAccountViewModel>> GetOnlineAdminsAsync()
        {
            // Mock implementation
            return new List<AdminAccountViewModel>();
        }

        public async Task<ResponseOutput<bool>> ChangePasswordAsync(long id, ChangePasswordViewModel model)
        {
            // Mock implementation
            return new ResponseOutput<bool>
            {
                IsSuccess = true,
                Data = true
            };
        }

        public async Task<ResponseOutput<bool>> ResetPasswordAsync(long id)
        {
            // Mock implementation
            return new ResponseOutput<bool>
            {
                IsSuccess = true,
                Data = true
            };
        }

        public async Task<List<LoginHistoryViewModel>> GetLoginHistoryAsync(long id)
        {
            // Mock implementation
            return new List<LoginHistoryViewModel>();
        }
    }
}
