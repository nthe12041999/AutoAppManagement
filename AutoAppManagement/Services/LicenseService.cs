using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface ILicenseService
    {
        Task<List<LicenseViewModel>> GetLicensesAsync();
        Task<LicenseViewModel> GetLicenseByIdAsync(long id);
        Task<ResponseOutput<LicenseViewModel>> CreateLicenseAsync(CreateLicenseViewModel model);
        Task<ResponseOutput<LicenseViewModel>> UpdateLicenseAsync(long id, UpdateLicenseViewModel model);
        Task<ResponseOutput<bool>> DeleteLicenseAsync(long id);
        Task<ResponseOutput<bool>> RenewLicenseAsync(long id, RenewLicenseViewModel model);
        Task<ResponseOutput<bool>> SuspendLicenseAsync(long id);
        Task<ResponseOutput<bool>> ActivateLicenseAsync(long id);
        Task<PagedResult<LicenseViewModel>> SearchLicensesAsync(string keyword = "", string type = "", string status = "", int pageIndex = 1, int pageSize = 10);
        Task<LicenseStatisticsViewModel> GetLicenseStatisticsAsync();
        Task<List<LicenseViewModel>> GetLicensesByCustomerAsync(long customerId);
        Task<List<LicenseViewModel>> GetExpiringLicensesAsync(int days = 30);
        Task<byte[]> ExportLicensesToExcelAsync();
        Task<List<LicenseHistoryViewModel>> GetLicenseHistoryAsync(long licenseId);
    }

    public class LicenseService : BaseService, ILicenseService
    {
        public LicenseService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, config, httpContextAccessor)
        {
        }

        public async Task<List<LicenseViewModel>> GetLicensesAsync()
        {
            var url = LicenseApiUrlDef.GetLicenses();
            return await RequestAuthenGetAsync<List<LicenseViewModel>>(url) ?? new List<LicenseViewModel>();
        }

        public async Task<LicenseViewModel> GetLicenseByIdAsync(long id)
        {
            var url = LicenseApiUrlDef.GetLicenseById(id);
            return await RequestAuthenGetAsync<LicenseViewModel>(url);
        }

        public async Task<ResponseOutput<LicenseViewModel>> CreateLicenseAsync(CreateLicenseViewModel model)
        {
            var url = LicenseApiUrlDef.CreateLicense();
            return await RequestFullAuthenPostAsync<LicenseViewModel>(url, model);
        }

        public async Task<ResponseOutput<LicenseViewModel>> UpdateLicenseAsync(long id, UpdateLicenseViewModel model)
        {
            var url = LicenseApiUrlDef.UpdateLicense(id);
            return await RequestFullAuthenPostAsync<LicenseViewModel>(url, model);
        }

        public async Task<ResponseOutput<bool>> DeleteLicenseAsync(long id)
        {
            var url = LicenseApiUrlDef.DeleteLicense(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<ResponseOutput<bool>> RenewLicenseAsync(long id, RenewLicenseViewModel model)
        {
            var url = LicenseApiUrlDef.RenewLicense(id);
            return await RequestFullAuthenPostAsync<bool>(url, model);
        }

        public async Task<ResponseOutput<bool>> SuspendLicenseAsync(long id)
        {
            var url = LicenseApiUrlDef.SuspendLicense(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<ResponseOutput<bool>> ActivateLicenseAsync(long id)
        {
            var url = LicenseApiUrlDef.ActivateLicense(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<PagedResult<LicenseViewModel>> SearchLicensesAsync(string keyword = "", string type = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            var url = LicenseApiUrlDef.SearchLicenses(keyword, type, status, pageIndex, pageSize);
            return await RequestAuthenGetAsync<PagedResult<LicenseViewModel>>(url) ?? new PagedResult<LicenseViewModel>();
        }

        public async Task<LicenseStatisticsViewModel> GetLicenseStatisticsAsync()
        {
            var url = LicenseApiUrlDef.GetLicenseStatistics();
            return await RequestAuthenGetAsync<LicenseStatisticsViewModel>(url);
        }

        public async Task<List<LicenseViewModel>> GetLicensesByCustomerAsync(long customerId)
        {
            var url = LicenseApiUrlDef.GetLicensesByCustomer(customerId);
            return await RequestAuthenGetAsync<List<LicenseViewModel>>(url) ?? new List<LicenseViewModel>();
        }

        public async Task<List<LicenseViewModel>> GetExpiringLicensesAsync(int days = 30)
        {
            var url = LicenseApiUrlDef.GetExpiringLicenses(days);
            return await RequestAuthenGetAsync<List<LicenseViewModel>>(url) ?? new List<LicenseViewModel>();
        }

        public async Task<byte[]> ExportLicensesToExcelAsync()
        {
            var url = LicenseApiUrlDef.ExportLicensesToExcel();
            return await RequestAuthenGetFile(url);
        }

        public async Task<List<LicenseHistoryViewModel>> GetLicenseHistoryAsync(long licenseId)
        {
            var url = LicenseApiUrlDef.GetLicenseHistory(licenseId);
            return await RequestAuthenGetAsync<List<LicenseHistoryViewModel>>(url) ?? new List<LicenseHistoryViewModel>();
        }
    }

    #region ViewModels
    public class LicenseViewModel
    {
        public long Id { get; set; }
        public string LicenseKey { get; set; }
        public string LicenseName { get; set; }
        public string LicenseType { get; set; }
        public string Description { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public int CurrentDevices { get; set; }
        public int CurrentUsers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int DaysUntilExpiry { get; set; }
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
    }

    public class CreateLicenseViewModel
    {
        public long CustomerId { get; set; }
        public string LicenseName { get; set; }
        public string LicenseType { get; set; }
        public string Description { get; set; }
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
    }

    public class UpdateLicenseViewModel
    {
        public string LicenseName { get; set; }
        public string Description { get; set; }
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }

    public class RenewLicenseViewModel
    {
        public DateTime NewExpiryDate { get; set; }
        public string Reason { get; set; }
    }

    public class LicenseStatisticsViewModel
    {
        public int TotalLicenses { get; set; }
        public int ActiveLicenses { get; set; }
        public int ExpiredLicenses { get; set; }
        public int SuspendedLicenses { get; set; }
        public int ExpiringSoonLicenses { get; set; }
        public Dictionary<string, int> LicensesByType { get; set; }
        public Dictionary<string, int> LicensesByStatus { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }

    public class LicenseHistoryViewModel
    {
        public long Id { get; set; }
        public long LicenseId { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionBy { get; set; }
    }
    #endregion
}
