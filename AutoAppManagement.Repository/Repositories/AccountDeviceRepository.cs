using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    public interface ICustomerDeviceRepository : IBaseRepository<AccountDevice>
    {
        /// <summary>
        /// Lấy danh sách thiết bị theo AccountId
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <returns>Danh sách thiết bị</returns>
        Task<IEnumerable<AccountDevice>> GetDevicesByAccountId(long accountId);

        /// <summary>
        /// Lấy thiết bị theo DeviceId và AccountId
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="accountId">Account ID</param>
        /// <returns>Thông tin thiết bị</returns>
        Task<AccountDevice> GetDeviceByDeviceIdAndAccountId(string deviceId, long accountId);

        /// <summary>
        /// Kiểm tra thiết bị có tồn tại không
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="accountId">Account ID</param>
        /// <returns>True nếu tồn tại</returns>
        Task<bool> IsDeviceExists(string deviceId, long accountId);

        /// <summary>
        /// Lấy thiết bị chính của tài khoản
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Thiết bị chính</returns>
        Task<AccountDevice> GetPrimaryDevice(long accountId);

        /// <summary>
        /// Cập nhật thời gian đăng nhập cuối
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="accountId">Account ID</param>
        /// <returns>Task</returns>
        Task UpdateLastLoginDate(string deviceId, long accountId);

        /// <summary>
        /// Lấy danh sách thiết bị đang hoạt động
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Danh sách thiết bị đang hoạt động</returns>
        Task<IEnumerable<AccountDevice>> GetActiveDevices(long accountId);

        /// <summary>
        /// Đếm số lượng thiết bị của tài khoản
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Số lượng thiết bị</returns>
        Task<int> CountDevicesByAccountId(long accountId);

        /// <summary>
        /// Vô hiệu hóa thiết bị
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="accountId">Account ID</param>
        /// <returns>Task</returns>
        Task DeactivateDevice(string deviceId, long accountId);
    }

    public class AccountDeviceRepository
        : BaseRepository<AccountDevice>,
            ICustomerDeviceRepository
    {
        public AccountDeviceRepository(AutoAppManagementContext context)
            : base(context) { }

        public async Task<IEnumerable<AccountDevice>> GetDevicesByAccountId(long accountId)
        {
            return await FindBy(d => d.AccountId == accountId);
        }

        public async Task<AccountDevice> GetDeviceByDeviceIdAndAccountId(
            string deviceId,
            long accountId
        )
        {
            return await FirstOrDefault(d => d.DeviceId == deviceId && d.AccountId == accountId);
        }

        public async Task<bool> IsDeviceExists(string deviceId, long accountId)
        {
            return await CheckExitsByCondition(d =>
                d.DeviceId == deviceId && d.AccountId == accountId
            );
        }

        public async Task<AccountDevice> GetPrimaryDevice(long accountId)
        {
            return await FirstOrDefault(d => d.AccountId == accountId && d.IsPrimaryDevice);
        }

        public async Task UpdateLastLoginDate(string deviceId, long accountId)
        {
            var device = await GetDeviceByDeviceIdAndAccountId(deviceId, accountId);
            if (device != null)
            {
                device.LastLoginDate = DateTime.Now;
                device.UpdatedDate = DateTime.Now;
                // Entity Framework sẽ tự động track changes
            }
        }

        public async Task<IEnumerable<AccountDevice>> GetActiveDevices(long accountId)
        {
            return await FindBy(d => d.AccountId == accountId && d.Status == "Active");
        }

        public async Task<int> CountDevicesByAccountId(long accountId)
        {
            var devices = await FindBy(d => d.AccountId == accountId);
            return devices.Count();
        }

        public async Task DeactivateDevice(string deviceId, long accountId)
        {
            var device = await GetDeviceByDeviceIdAndAccountId(deviceId, accountId);
            if (device != null)
            {
                device.Status = "Inactive";
                device.UpdatedDate = DateTime.Now;
                // Entity Framework sẽ tự động track changes
            }
        }
    }
}
