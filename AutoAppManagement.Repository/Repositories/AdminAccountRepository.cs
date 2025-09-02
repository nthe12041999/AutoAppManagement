using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Common.Ulti;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IAdminAccountRepository : IBaseRepository<AdminAccount>
    {
        /// <summary>
        /// Lấy admin theo username và password
        /// </summary>
        /// <param name="userName">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Thông tin admin</returns>
        Task<AdminAccount> GetAdminByUserNameAndPass(string userName, string password);

        /// <summary>
        /// Lấy danh sách role của admin
        /// </summary>
        /// <param name="adminId">ID admin</param>
        /// <returns>Danh sách role</returns>
        Task<IEnumerable<Role>> GetListRoleByAdminId(long adminId);

        /// <summary>
        /// Lấy danh sách admin theo role
        /// </summary>
        /// <param name="roleName">Tên role</param>
        /// <returns>Danh sách admin</returns>
        Task<IEnumerable<AdminAccount>> GetAdminsByRole(string roleName);

        /// <summary>
        /// Lấy danh sách admin theo department
        /// </summary>
        /// <param name="department">Phòng ban</param>
        /// <returns>Danh sách admin</returns>
        Task<IEnumerable<AdminAccount>> GetAdminsByDepartment(string department);

        /// <summary>
        /// Lấy danh sách admin đang online
        /// </summary>
        /// <returns>Danh sách admin online</returns>
        Task<IEnumerable<AdminAccount>> GetOnlineAdmins();

        /// <summary>
        /// Kiểm tra admin có online không
        /// </summary>
        /// <param name="adminId">ID admin</param>
        /// <returns>True nếu online</returns>
        Task<bool> IsAdminOnline(long adminId);

        /// <summary>
        /// Cập nhật thời gian đăng nhập cuối
        /// </summary>
        /// <param name="adminId">ID admin</param>
        /// <param name="loginTime">Thời gian đăng nhập</param>
        /// <param name="ipAddress">Địa chỉ IP</param>
        /// <returns>Task</returns>
        Task UpdateLastLoginTime(long adminId, DateTime loginTime, string ipAddress);

        /// <summary>
        /// Cập nhật thời gian hoạt động cuối
        /// </summary>
        /// <param name="adminId">ID admin</param>
        /// <param name="activityTime">Thời gian hoạt động</param>
        /// <returns>Task</returns>
        Task UpdateLastActivityTime(long adminId, DateTime activityTime);

        /// <summary>
        /// Khóa/mở khóa admin
        /// </summary>
        /// <param name="adminId">ID admin</param>
        /// <param name="isLocked">Trạng thái khóa</param>
        /// <param name="reason">Lý do</param>
        /// <param name="lockedBy">Người khóa</param>
        /// <returns>Task</returns>
        Task LockUnlockAdmin(long adminId, bool isLocked, string reason, long? lockedBy);

        /// <summary>
        /// Đổi mật khẩu admin
        /// </summary>
        /// <param name="adminId">ID admin</param>
        /// <param name="newPassword">Mật khẩu mới</param>
        /// <param name="changedBy">Người đổi</param>
        /// <returns>Task</returns>
        Task ChangePassword(long adminId, string newPassword, long? changedBy);

        /// <summary>
        /// Kiểm tra mật khẩu hiện tại
        /// </summary>
        /// <param name="adminId">ID admin</param>
        /// <param name="currentPassword">Mật khẩu hiện tại</param>
        /// <returns>True nếu đúng</returns>
        Task<bool> VerifyCurrentPassword(long adminId, string currentPassword);

        /// <summary>
        /// Lấy thống kê admin
        /// </summary>
        /// <returns>Thống kê</returns>
        Task<AdminStatistics> GetAdminStatistics();

        /// <summary>
        /// Tìm kiếm admin
        /// </summary>
        /// <param name="keyword">Từ khóa</param>
        /// <param name="role">Role</param>
        /// <param name="status">Trạng thái</param>
        /// <param name="department">Phòng ban</param>
        /// <param name="pageIndex">Trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>Kết quả tìm kiếm</returns>
        Task<(IEnumerable<AdminAccount> admins, int totalCount)> SearchAdmins(
            string keyword, string role, string status, string department, 
            int pageIndex, int pageSize);
    }

    public class AdminAccountRepository : BaseRepository<AdminAccount>, IAdminAccountRepository
    {
        public AdminAccountRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<AdminAccount> GetAdminByUserNameAndPass(string userName, string password)
        {
            var passwordEncode = HashCodeUlti.EncodePassword(password);
            var admin = await FindBy(a => a.UserName == userName && a.PasswordHash == passwordEncode);
            return admin.FirstOrDefault();
        }

        public async Task<IEnumerable<Role>> GetListRoleByAdminId(long adminId)
        {
            var roles = await _context.RoleAccounts
                .Where(ra => ra.AccountId == adminId)
                .Include(ra => ra.Role)
                .Select(ra => ra.Role)
                .ToListAsync();
            return roles;
        }

        public async Task<IEnumerable<AdminAccount>> GetAdminsByRole(string roleName)
        {
            var admins = await _context.RoleAccounts
                .Where(ra => ra.Role.RoleName == roleName)
                .Include(ra => ra.Account)
                .Select(ra => ra.Account)
                .Cast<AdminAccount>()
                .ToListAsync();
            return admins;
        }

        public async Task<IEnumerable<AdminAccount>> GetAdminsByDepartment(string department)
        {
            // TODO: Department property not exists in AdminAccount
            // return await FindBy(a => a.Department == department);
            return await Task.FromResult(new List<AdminAccount>());
        }

        public async Task<IEnumerable<AdminAccount>> GetOnlineAdmins()
        {
            var thirtyMinutesAgo = DateTime.Now.AddMinutes(-30);
            return await FindBy(a => a.LastLoginAt >= thirtyMinutesAgo && a.IsActive);
        }

        public async Task<bool> IsAdminOnline(long adminId)
        {
            var thirtyMinutesAgo = DateTime.Now.AddMinutes(-30);
            return await CheckExitsByCondition(a =>
                a.Id == adminId &&
                a.LastLoginAt >= thirtyMinutesAgo &&
                a.IsActive);
        }

        public async Task UpdateLastLoginTime(long adminId, DateTime loginTime, string ipAddress)
        {
            var admin = await FirstOrDefault(a => a.Id == adminId);
            if (admin != null)
            {
                admin.LastLoginAt = loginTime;
                admin.LastLoginIp = ipAddress;
                // Entity Framework sẽ tự động track changes
            }
        }

        public async Task UpdateLastActivityTime(long adminId, DateTime activityTime)
        {
            var admin = await FirstOrDefault(a => a.Id == adminId);
            if (admin != null)
            {
                admin.LastLoginAt = activityTime;
                // Entity Framework sẽ tự động track changes
            }
        }

        public async Task LockUnlockAdmin(long adminId, bool isLocked, string reason, long? lockedBy)
        {
            var admin = await FirstOrDefault(a => a.Id == adminId);
            if (admin != null)
            {
                if (isLocked)
                {
                    admin.LockAccount(30, lockedBy); // Lock for 30 minutes
                }
                else
                {
                    admin.UnlockAccount(lockedBy);
                }
                // Entity Framework sẽ tự động track changes
            }
        }

        public async Task ChangePassword(long adminId, string newPassword, long? changedBy)
        {
            var admin = await FirstOrDefault(a => a.Id == adminId);
            if (admin != null)
            {
                var newPasswordHash = HashCodeUlti.EncodePassword(newPassword);
                admin.ChangePassword(newPasswordHash, changedBy);
                // Entity Framework sẽ tự động track changes
            }
        }

        public async Task<bool> VerifyCurrentPassword(long adminId, string currentPassword)
        {
            var passwordEncode = HashCodeUlti.EncodePassword(currentPassword);
            return await CheckExitsByCondition(a => a.Id == adminId && a.PasswordHash == passwordEncode);
        }

        public async Task<AdminStatistics> GetAdminStatistics()
        {
            var totalAdmins = await _context.AdminAccounts.CountAsync();
            var activeAdmins = await _context.AdminAccounts.CountAsync(a => a.IsActive);
            var inactiveAdmins = await _context.AdminAccounts.CountAsync(a => !a.IsActive);
            var lockedAdmins = await _context.AdminAccounts.CountAsync(a => a.IsLocked);

            var thirtyMinutesAgo = DateTime.Now.AddMinutes(-30);
            var onlineAdmins = await _context.AdminAccounts.CountAsync(a => a.LastLoginAt >= thirtyMinutesAgo && a.IsActive);

            var thisMonth = DateTime.Now.AddDays(-30);
            var newAdminsThisMonth = await _context.AdminAccounts.CountAsync(a => a.CreatedDate >= thisMonth);

            return new AdminStatistics
            {
                TotalAdmins = totalAdmins,
                ActiveAdmins = activeAdmins,
                InactiveAdmins = inactiveAdmins,
                LockedAdmins = lockedAdmins,
                OnlineAdmins = onlineAdmins,
                NewAdminsThisMonth = newAdminsThisMonth
            };
        }

        public async Task<(IEnumerable<AdminAccount> admins, int totalCount)> SearchAdmins(
            string keyword, string role, string status, string department, 
            int pageIndex, int pageSize)
        {
            var query = _context.AdminAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => 
                    a.UserName.Contains(keyword) || 
                    a.FullName.Contains(keyword) || 
                    a.Email.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (!string.IsNullOrEmpty(department))
            {
                // TODO: Department property not exists in AdminAccount
                // query = query.Where(a => a.Department == department);
            }

            var totalCount = await query.CountAsync();
            var admins = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (admins, totalCount);
        }
    }

    /// <summary>
    /// Class thống kê admin
    /// </summary>
    public class AdminStatistics
    {
        public int TotalAdmins { get; set; }
        public int ActiveAdmins { get; set; }
        public int InactiveAdmins { get; set; }
        public int LockedAdmins { get; set; }
        public int OnlineAdmins { get; set; }
        public int NewAdminsThisMonth { get; set; }
    }
}
