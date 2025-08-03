using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface ICustomerAccountService
    {
        Task<List<CustomerAccountViewModel>> GetCustomerAccountsAsync();
        Task<CustomerAccountViewModel> GetCustomerAccountByIdAsync(long id);
        Task<ResponseOutput<CustomerAccountViewModel>> CreateCustomerAccountAsync(CreateCustomerAccountViewModel model);
        Task<ResponseOutput<CustomerAccountViewModel>> UpdateCustomerAccountAsync(long id, UpdateCustomerAccountViewModel model);
        Task<ResponseOutput<bool>> DeleteCustomerAccountAsync(long id);
        Task<PagedResult<CustomerAccountViewModel>> SearchCustomerAccountsAsync(string keyword = "", string status = "", string role = "", int pageIndex = 1, int pageSize = 10);
        Task<ResponseOutput<bool>> ChangeCustomerAccountStatusAsync(long id, string status);
        Task<CustomerAccountStatisticsViewModel> GetCustomerAccountStatisticsAsync();
        Task<byte[]> ExportCustomerAccountsToExcelAsync();
    }

    public class CustomerAccountService : BaseService, ICustomerAccountService
    {
        public CustomerAccountService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, config, httpContextAccessor)
        {
        }

        public async Task<List<CustomerAccountViewModel>> GetCustomerAccountsAsync()
        {
            var url = CustomerAccountApiUrlDef.GetCustomerAccounts();
            return await RequestAuthenGetAsync<List<CustomerAccountViewModel>>(url) ?? new List<CustomerAccountViewModel>();
        }

        public async Task<CustomerAccountViewModel> GetCustomerAccountByIdAsync(long id)
        {
            var url = CustomerAccountApiUrlDef.GetCustomerAccountById(id);
            return await RequestAuthenGetAsync<CustomerAccountViewModel>(url);
        }

        public async Task<ResponseOutput<CustomerAccountViewModel>> CreateCustomerAccountAsync(CreateCustomerAccountViewModel model)
        {
            var url = CustomerAccountApiUrlDef.CreateCustomerAccount();
            return await RequestFullAuthenPostAsync<CustomerAccountViewModel>(url, model);
        }

        public async Task<ResponseOutput<CustomerAccountViewModel>> UpdateCustomerAccountAsync(long id, UpdateCustomerAccountViewModel model)
        {
            var url = CustomerAccountApiUrlDef.UpdateCustomerAccount(id);
            return await RequestFullAuthenPostAsync<CustomerAccountViewModel>(url, model);
        }

        public async Task<ResponseOutput<bool>> DeleteCustomerAccountAsync(long id)
        {
            var url = CustomerAccountApiUrlDef.DeleteCustomerAccount(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<PagedResult<CustomerAccountViewModel>> SearchCustomerAccountsAsync(string keyword = "", string status = "", string role = "", int pageIndex = 1, int pageSize = 10)
        {
            var url = CustomerAccountApiUrlDef.SearchCustomerAccounts(keyword, status, role, pageIndex, pageSize);
            return await RequestAuthenGetAsync<PagedResult<CustomerAccountViewModel>>(url) ?? new PagedResult<CustomerAccountViewModel>();
        }

        public async Task<ResponseOutput<bool>> ChangeCustomerAccountStatusAsync(long id, string status)
        {
            var url = CustomerAccountApiUrlDef.ChangeCustomerAccountStatus(id);
            return await RequestFullAuthenPostAsync<bool>(url, new { Status = status });
        }

        public async Task<CustomerAccountStatisticsViewModel> GetCustomerAccountStatisticsAsync()
        {
            var url = CustomerAccountApiUrlDef.GetCustomerAccountStatistics();
            return await RequestAuthenGetAsync<CustomerAccountStatisticsViewModel>(url);
        }

        public async Task<byte[]> ExportCustomerAccountsToExcelAsync()
        {
            var url = CustomerAccountApiUrlDef.ExportCustomerAccountsToExcel();
            return await RequestAuthenGetFile(url);
        }
    }

    #region ViewModels
    public class CustomerAccountViewModel
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Avatar { get; set; }
        public bool IsOnline { get; set; }
    }

    public class CreateCustomerAccountViewModel
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateCustomerAccountViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class CustomerAccountStatisticsViewModel
    {
        public int TotalAccounts { get; set; }
        public int ActiveAccounts { get; set; }
        public int InactiveAccounts { get; set; }
        public int SuspendedAccounts { get; set; }
        public int OnlineAccounts { get; set; }
        public int NewAccountsThisMonth { get; set; }
        public Dictionary<string, int> AccountsByRole { get; set; }
        public Dictionary<string, int> AccountsByStatus { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
    #endregion
}
