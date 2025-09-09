using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    public interface ILicenseRepository : IBaseRepository<License>
    {
        /// <summary>
        /// Lấy danh sách license theo AccountId
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <returns>Danh sách license</returns>
        Task<IEnumerable<License>> GetLicensesByAccountId(long accountId);

        /// <summary>
        /// Lấy license theo LicenseKey
        /// </summary>
        /// <param name="licenseKey">License Key</param>
        /// <returns>Thông tin license</returns>
        Task<License> GetLicenseByKey(string licenseKey);

        /// <summary>
        /// Lấy license đang hoạt động của tài khoản
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>License đang hoạt động</returns>
        Task<License> GetActiveLicense(long accountId);

        /// <summary>
        /// Lấy danh sách license đang hoạt động
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Danh sách license đang hoạt động</returns>
        Task<IEnumerable<License>> GetActiveLicenses(long accountId);

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
        Task<IEnumerable<License>> GetExpiringLicenses(int days = 30);

        /// <summary>
        /// Lấy danh sách license đã hết hạn
        /// </summary>
        /// <returns>Danh sách license đã hết hạn</returns>
        Task<IEnumerable<License>> GetExpiredLicenses();

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

    public class LicenseRepository
        : BaseRepository<License>,
            ILicenseRepository
    {
        public LicenseRepository(AutoAppManagementContext context)
            : base(context) { }

        public async Task<IEnumerable<License>> GetLicensesByAccountId(long accountId)
        {
            // Tìm license thông qua Account.LicenseId
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account?.LicenseId != null)
            {
                var license = await FirstOrDefault(l => l.Id == account.LicenseId);
                return license != null ? new[] { license } : new License[0];
            }
            return new License[0];
        }

        public async Task<License> GetLicenseByKey(string licenseKey)
        {
            return await FirstOrDefault(l => l.LicenseKey == licenseKey);
        }

        public async Task<License> GetActiveLicense(long accountId)
        {
            // Tìm license active của account thông qua Account.LicenseId
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account?.LicenseId != null)
            {
                return await FirstOrDefault(l =>
                    l.Id == account.LicenseId && l.Status == "Active" && l.ExpiryDate > DateTime.Now
                );
            }
            return null;
        }

        public async Task<IEnumerable<License>> GetActiveLicenses(long accountId)
        {
            var license = await GetActiveLicense(accountId);
            return license != null ? new[] { license } : new License[0];
        }

        public async Task<bool> IsLicenseValid(string licenseKey, long accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account?.LicenseId != null)
            {
                return await CheckExitsByCondition(l =>
                    l.LicenseKey == licenseKey
                    && l.Id == account.LicenseId
                    && l.Status == "Active"
                    && l.ExpiryDate > DateTime.Now
                );
            }
            return false;
        }

        public async Task<IEnumerable<License>> GetExpiringLicenses(int days = 30)
        {
            var expiryThreshold = DateTime.Now.AddDays(days);
            return await FindBy(l =>
                l.Status == "Active"
                && l.ExpiryDate <= expiryThreshold
                && l.ExpiryDate > DateTime.Now
            );
        }

        public async Task<IEnumerable<License>> GetExpiredLicenses()
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
            return licenses.GroupBy(l => l.LicenseType).ToDictionary(g => g.Key.ToString(), g => g.Count());
        }
    }
}
