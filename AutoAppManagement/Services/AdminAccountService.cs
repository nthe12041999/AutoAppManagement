using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IAdminAccountService
    {
        Task<List<AdminAccountViewModel>> GetAdminAccountsAsync();
        Task<AdminAccountViewModel> GetAdminAccountByIdAsync(long id);
        Task<ResponseOutput<AdminAccountViewModel>> CreateAdminAccountAsync(CreateAdminAccountViewModel model);
        Task<ResponseOutput<AdminAccountViewModel>> UpdateAdminAccountAsync(long id, UpdateAdminAccountViewModel model);
        Task<ResponseOutput<bool>> DeleteAdminAccountAsync(long id);
        Task<PagedResult<AdminAccountViewModel>> SearchAdminAccountsAsync(string keyword = "", string role = "", string status = "", int pageIndex = 1, int pageSize = 10);
        Task<ResponseOutput<bool>> ChangeAdminAccountStatusAsync(long id, string status);
        Task<ResponseOutput<bool>> AssignPermissionsAsync(long id, List<string> permissions);
        Task<List<string>> GetAdminPermissionsAsync(long id);
        Task<AdminAccountStatisticsViewModel> GetAdminAccountStatisticsAsync();
        Task<List<AdminAccountViewModel>> GetOnlineAdminsAsync();
        Task<ResponseOutput<bool>> ChangePasswordAsync(long id, ChangePasswordViewModel model);
        Task<ResponseOutput<bool>> ResetPasswordAsync(long id);
        Task<List<LoginHistoryViewModel>> GetLoginHistoryAsync(long id);
        Task<byte[]> ExportAdminAccountsToExcelAsync();
    }

    public class AdminAccountService : BaseService, IAdminAccountService
    {
        public AdminAccountService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, config, httpContextAccessor)
        {
        }

        public async Task<List<AdminAccountViewModel>> GetAdminAccountsAsync()
        {
            var url = AdminAccountApiUrlDef.GetAdminAccounts();
            return await RequestAuthenGetAsync<List<AdminAccountViewModel>>(url) ?? new List<AdminAccountViewModel>();
        }

        public async Task<AdminAccountViewModel> GetAdminAccountByIdAsync(long id)
        {
            var url = AdminAccountApiUrlDef.GetAdminAccountById(id);
            return await RequestAuthenGetAsync<AdminAccountViewModel>(url);
        }

        public async Task<ResponseOutput<AdminAccountViewModel>> CreateAdminAccountAsync(CreateAdminAccountViewModel model)
        {
            var url = AdminAccountApiUrlDef.CreateAdminAccount();
            return await RequestFullAuthenPostAsync<AdminAccountViewModel>(url, model);
        }

        public async Task<ResponseOutput<AdminAccountViewModel>> UpdateAdminAccountAsync(long id, UpdateAdminAccountViewModel model)
        {
            var url = AdminAccountApiUrlDef.UpdateAdminAccount(id);
            return await RequestFullAuthenPostAsync<AdminAccountViewModel>(url, model);
        }

        public async Task<ResponseOutput<bool>> DeleteAdminAccountAsync(long id)
        {
            var url = AdminAccountApiUrlDef.DeleteAdminAccount(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<PagedResult<AdminAccountViewModel>> SearchAdminAccountsAsync(string keyword = "", string role = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            var url = AdminAccountApiUrlDef.SearchAdminAccounts(keyword, role, status, pageIndex, pageSize);
            return await RequestAuthenGetAsync<PagedResult<AdminAccountViewModel>>(url) ?? new PagedResult<AdminAccountViewModel>();
        }

        public async Task<ResponseOutput<bool>> ChangeAdminAccountStatusAsync(long id, string status)
        {
            var url = AdminAccountApiUrlDef.ChangeAdminAccountStatus(id);
            return await RequestFullAuthenPostAsync<bool>(url, new { Status = status });
        }

        public async Task<ResponseOutput<bool>> AssignPermissionsAsync(long id, List<string> permissions)
        {
            var url = AdminAccountApiUrlDef.AssignPermissions(id);
            return await RequestFullAuthenPostAsync<bool>(url, new { Permissions = permissions });
        }

        public async Task<List<string>> GetAdminPermissionsAsync(long id)
        {
            var url = AdminAccountApiUrlDef.GetAdminPermissions(id);
            return await RequestAuthenGetAsync<List<string>>(url) ?? new List<string>();
        }

        public async Task<AdminAccountStatisticsViewModel> GetAdminAccountStatisticsAsync()
        {
            var url = AdminAccountApiUrlDef.GetAdminAccountStatistics();
            return await RequestAuthenGetAsync<AdminAccountStatisticsViewModel>(url);
        }

        public async Task<List<AdminAccountViewModel>> GetOnlineAdminsAsync()
        {
            var url = AdminAccountApiUrlDef.GetOnlineAdmins();
            return await RequestAuthenGetAsync<List<AdminAccountViewModel>>(url) ?? new List<AdminAccountViewModel>();
        }

        public async Task<ResponseOutput<bool>> ChangePasswordAsync(long id, ChangePasswordViewModel model)
        {
            var url = AdminAccountApiUrlDef.ChangePassword(id);
            return await RequestFullAuthenPostAsync<bool>(url, model);
        }

        public async Task<ResponseOutput<bool>> ResetPasswordAsync(long id)
        {
            var url = AdminAccountApiUrlDef.ResetPassword(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<List<LoginHistoryViewModel>> GetLoginHistoryAsync(long id)
        {
            var url = AdminAccountApiUrlDef.GetLoginHistory(id);
            return await RequestAuthenGetAsync<List<LoginHistoryViewModel>>(url) ?? new List<LoginHistoryViewModel>();
        }

        public async Task<byte[]> ExportAdminAccountsToExcelAsync()
        {
            var url = AdminAccountApiUrlDef.ExportAdminAccountsToExcel();
            return await RequestAuthenGetFile(url);
        }
    }

    #region ViewModels
    public class AdminAccountViewModel
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Avatar { get; set; }
        public bool IsOnline { get; set; }
        public string LastLoginIp { get; set; }
        public int LoginCount { get; set; }
    }

    public class CreateAdminAccountViewModel
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }

    public class UpdateAdminAccountViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }

    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class AdminAccountStatisticsViewModel
    {
        public int TotalAdmins { get; set; }
        public int ActiveAdmins { get; set; }
        public int InactiveAdmins { get; set; }
        public int OnlineAdmins { get; set; }
        public int SuperAdmins { get; set; }
        public Dictionary<string, int> AdminsByRole { get; set; }
        public Dictionary<string, int> AdminsByStatus { get; set; }
        public List<AdminActivityViewModel> RecentActivities { get; set; }
    }

    public class LoginHistoryViewModel
    {
        public long Id { get; set; }
        public long AdminId { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Location { get; set; }
        public bool IsSuccessful { get; set; }
        public string FailureReason { get; set; }
    }

    public class AdminActivityViewModel
    {
        public long Id { get; set; }
        public long AdminId { get; set; }
        public string AdminName { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime ActivityTime { get; set; }
        public string IpAddress { get; set; }
    }
    #endregion
}
