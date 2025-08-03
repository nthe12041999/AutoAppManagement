using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    public interface ICustomerLicenseRepository : IBaseRepository<CustomerLicense>
    {
        /// <summary>
        /// Lấy danh sách license theo AccountId
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <returns>Danh sách license</returns>
        Task<IEnumerable<CustomerLicense>> GetLicensesByAccountId(long accountId);

        /// <summary>
        /// Lấy license theo LicenseKey
        /// </summary>
        /// <param name="licenseKey">License Key</param>
        /// <returns>Thông tin license</returns>
        Task<CustomerLicense> GetLicenseByKey(string licenseKey);

        /// <summary>
        /// Lấy license đang hoạt động của tài khoản
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>License đang hoạt động</returns>
        Task<CustomerLicense> GetActiveLicense(long accountId);

        /// <summary>
        /// Lấy danh sách license đang hoạt động
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Danh sách license đang hoạt động</returns>
        Task<IEnumerable<CustomerLicense>> GetActiveLicenses(long accountId);

        /// <summary>
        /// Kiểm tra license có hợp lệ không
        /// </summary>
        /// <param name="licenseKey">License Key</param>
        /// <param name="accountId">Account ID</param>
        /// <returns>True nếu hợp lệ</returns>
        Task<bool> IsLicenseValid(string licenseKey, long accountId);

        /// <summary>
        /// Lấy danh sách license sắp hết hạn
        /// </summary>
        /// <param name="days">Số ngày trước khi hết hạn</param>
        /// <returns>Danh sách license sắp hết hạn</returns>
        Task<IEnumerable<CustomerLicense>> GetExpiringLicenses(int days = 30);

        /// <summary>
        /// Lấy danh sách license đã hết hạn
        /// </summary>
        /// <returns>Danh sách license đã hết hạn</returns>
        Task<IEnumerable<CustomerLicense>> GetExpiredLicenses();

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="licenseKey">License Key</param>
        /// <param name="newExpiryDate">Ngày hết hạn mới</param>
        /// <param name="updatedBy">Người cập nhật</param>
        /// <returns>Task</returns>
        Task RenewLicense(string licenseKey, DateTime newExpiryDate, long updatedBy);

        /// <summary>
        /// Vô hiệu hóa license
        /// </summary>
        /// <param name="licenseKey">License Key</param>
        /// <param name="updatedBy">Người cập nhật</param>
        /// <returns>Task</returns>
        Task DeactivateLicense(string licenseKey, long updatedBy);

        /// <summary>
        /// Lấy thống kê license theo loại
        /// </summary>
        /// <returns>Dictionary với key là loại license, value là số lượng</returns>
        Task<Dictionary<string, int>> GetLicenseStatistics();
    }

    public class CustomerLicenseRepository
        : BaseRepository<CustomerLicense>,
            ICustomerLicenseRepository
    {
        public CustomerLicenseRepository(AutoAppManagementContext context)
            : base(context) { }

        public async Task<IEnumerable<CustomerLicense>> GetLicensesByAccountId(long accountId)
        {
            return await FindBy(l => l.AccountId == accountId);
        }

        public async Task<CustomerLicense> GetLicenseByKey(string licenseKey)
        {
            return await FirstOrDefault(l => l.LicenseKey == licenseKey);
        }

        public async Task<CustomerLicense> GetActiveLicense(long accountId)
        {
            return await FirstOrDefault(l =>
                l.AccountId == accountId && l.Status == "Active" && l.ExpiryDate > DateTime.Now
            );
        }

        public async Task<IEnumerable<CustomerLicense>> GetActiveLicenses(long accountId)
        {
            return await FindBy(l =>
                l.AccountId == accountId && l.Status == "Active" && l.ExpiryDate > DateTime.Now
            );
        }

        public async Task<bool> IsLicenseValid(string licenseKey, long accountId)
        {
            return await CheckExitsByCondition(l =>
                l.LicenseKey == licenseKey
                && l.AccountId == accountId
                && l.Status == "Active"
                && l.ExpiryDate > DateTime.Now
            );
        }

        public async Task<IEnumerable<CustomerLicense>> GetExpiringLicenses(int days = 30)
        {
            var expiryThreshold = DateTime.Now.AddDays(days);
            return await FindBy(l =>
                l.Status == "Active"
                && l.ExpiryDate <= expiryThreshold
                && l.ExpiryDate > DateTime.Now
            );
        }

        public async Task<IEnumerable<CustomerLicense>> GetExpiredLicenses()
        {
            return await FindBy(l => l.ExpiryDate <= DateTime.Now && l.Status == "Active");
        }

        public async Task RenewLicense(string licenseKey, DateTime newExpiryDate, long updatedBy)
        {
            var license = await GetLicenseByKey(licenseKey);
            if (license != null)
            {
                license.ExpiryDate = newExpiryDate;
                license.UpdatedDate = DateTime.Now;
                license.UpdatedBy = updatedBy;
                // Entity Framework sẽ tự động track changes
            }
        }

        public async Task DeactivateLicense(string licenseKey, long updatedBy)
        {
            var license = await GetLicenseByKey(licenseKey);
            if (license != null)
            {
                license.Status = "Suspended";
                license.UpdatedDate = DateTime.Now;
                license.UpdatedBy = updatedBy;
                // Entity Framework sẽ tự động track changes
            }
        }

        public async Task<Dictionary<string, int>> GetLicenseStatistics()
        {
            var licenses = await GetAll();
            return licenses.GroupBy(l => l.LicenseType).ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
