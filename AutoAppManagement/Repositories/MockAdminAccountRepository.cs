using AutoAppManagement.Models.BaseEntity;
using System.Security.Cryptography;
using System.Text;

namespace AutoAppManagement.Repositories
{
    public class MockAdminAccountRepository : IAdminAccountRepository
    {
        // Mock data for demonstration
        private static readonly List<AdminAccount> _mockAdmins = new()
        {
            new AdminAccount
            {
                Id = 1,
                FullName = "Nguyễn Văn Admin",
                UserName = "admin01",
                Email = "admin01@company.com",
                PhoneNumber = "0901234567",
                Role = "Admin",
                PasswordHash = HashPassword("123456"),
                IsActive = true,
                IsEmailVerified = true,
                IsPhoneVerified = true,
                IsTwoFactorEnabled = false,
                Department = "IT",
                Position = "System Administrator",
                CreatedDate = DateTime.Now.AddDays(-30),
                LastLoginAt = DateTime.Now.AddHours(-2),
                LoginCount = 245,
                LastLoginIp = "192.168.1.100"
            },
            new AdminAccount
            {
                Id = 2,
                FullName = "Trần Thị Manager",
                UserName = "manager01",
                Email = "manager01@company.com",
                PhoneNumber = "0912345678",
                Role = "Moderator",
                PasswordHash = HashPassword("123456"),
                IsActive = true,
                IsEmailVerified = true,
                IsPhoneVerified = false,
                IsTwoFactorEnabled = true,
                Department = "Management",
                Position = "Content Manager",
                CreatedDate = DateTime.Now.AddDays(-25),
                LastLoginAt = DateTime.Now.AddHours(-5),
                LoginCount = 156,
                LastLoginIp = "192.168.1.101"
            },
            new AdminAccount
            {
                Id = 3,
                FullName = "Lê Văn Editor",
                UserName = "editor01",
                Email = "editor01@company.com",
                PhoneNumber = "0923456789",
                Role = "Support",
                PasswordHash = HashPassword("123456"),
                IsActive = false,
                IsEmailVerified = true,
                IsPhoneVerified = false,
                IsTwoFactorEnabled = false,
                Department = "Support",
                Position = "Content Editor",
                CreatedDate = DateTime.Now.AddDays(-20),
                LastLoginAt = DateTime.Now.AddDays(-3),
                LoginCount = 89,
                LastLoginIp = "192.168.1.102"
            }
        };

        // Basic CRUD Operations
        public async Task<IEnumerable<AdminAccount>> GetAllAsync()
        {
            await Task.Delay(1);
            return _mockAdmins.OrderByDescending(a => a.CreatedDate);
        }

        public async Task<AdminAccount?> GetByIdAsync(int id)
        {
            await Task.Delay(1);
            return _mockAdmins.FirstOrDefault(a => a.Id == id);
        }

        public async Task<AdminAccount?> GetByEmailAsync(string email)
        {
            await Task.Delay(1);
            return _mockAdmins.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
        }

        public async Task<AdminAccount?> GetByUserNameAsync(string userName)
        {
            await Task.Delay(1);
            return _mockAdmins.FirstOrDefault(a => a.UserName.ToLower() == userName.ToLower());
        }

        public async Task<AdminAccount> CreateAsync(AdminAccount adminAccount)
        {
            await Task.Delay(1);
            adminAccount.Id = _mockAdmins.Max(a => a.Id) + 1;
            adminAccount.CreatedDate = DateTime.UtcNow;
            _mockAdmins.Add(adminAccount);
            return adminAccount;
        }

        public async Task<AdminAccount> UpdateAsync(AdminAccount adminAccount)
        {
            await Task.Delay(1);
            var existingAdmin = _mockAdmins.FirstOrDefault(a => a.Id == adminAccount.Id);
            if (existingAdmin != null)
            {
                var index = _mockAdmins.IndexOf(existingAdmin);
                adminAccount.UpdatedDate = DateTime.UtcNow;
                _mockAdmins[index] = adminAccount;
            }
            return adminAccount;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await Task.Delay(1);
            var admin = _mockAdmins.FirstOrDefault(a => a.Id == id);
            if (admin == null) return false;
            _mockAdmins.Remove(admin);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            await Task.Delay(1);
            return _mockAdmins.Any(a => a.Id == id);
        }

        // Pagination and Filtering
        public async Task<(IEnumerable<AdminAccount> Items, int TotalCount)> GetPagedAsync(
            int page = 1, 
            int pageSize = 10, 
            string? search = null,
            string? role = null,
            string? status = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null)
        {
            await Task.Delay(1);
            
            var query = _mockAdmins.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => 
                    a.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.UserName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(a => a.Role == role);
            }

            var totalCount = query.Count();
            var items = query
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, totalCount);
        }

        // Simple implementations for other required methods
        public async Task<AdminAccount?> AuthenticateAsync(string userName, string password)
        {
            var admin = await GetByUserNameAsync(userName);
            if (admin == null || !VerifyPassword(password, admin.PasswordHash))
                return null;
            if (!admin.IsActive) return null;
            return admin;
        }

        public async Task<AdminAccountStats> GetStatsAsync()
        {
            await Task.Delay(1);
            return new AdminAccountStats
            {
                TotalAdmins = _mockAdmins.Count,
                ActiveAdmins = _mockAdmins.Count(a => a.IsActive),
                VerifiedAdmins = _mockAdmins.Count(a => a.IsEmailVerified),
                OnlineAdmins = _mockAdmins.Count(a => a.LastLoginAt.HasValue && a.LastLoginAt > DateTime.Now.AddMinutes(-5))
            };
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            await Task.Delay(1);
            var query = _mockAdmins.Where(a => a.Email.ToLower() == email.ToLower());
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);
            return !query.Any();
        }

        public async Task<bool> IsUserNameUniqueAsync(string userName, int? excludeId = null)
        {
            await Task.Delay(1);
            var query = _mockAdmins.Where(a => a.UserName.ToLower() == userName.ToLower());
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);
            return !query.Any();
        }

        // Helper method
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "AutoAppSalt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        // Stub implementations for other interface methods
        public Task<bool> VerifyPasswordAsync(int adminId, string password) => Task.FromResult(true);
        public Task<bool> ChangePasswordAsync(int adminId, string newPasswordHash) => Task.FromResult(true);
        public Task<bool> LockAccountAsync(int adminId, DateTime lockUntil, string? reason = null) => Task.FromResult(true);
        public Task<bool> UnlockAccountAsync(int adminId) => Task.FromResult(true);
        public Task<bool> IncrementFailedLoginAsync(int adminId) => Task.FromResult(true);
        public Task<bool> ResetFailedLoginAsync(int adminId) => Task.FromResult(true);
        public Task<bool> VerifyEmailAsync(int adminId) => Task.FromResult(true);
        public Task<bool> VerifyPhoneAsync(int adminId) => Task.FromResult(true);
        public Task<bool> UpdateLastLoginAsync(int adminId, string? ipAddress = null, string? userAgent = null) => Task.FromResult(true);
        public Task<bool> UpdateRoleAsync(int adminId, string newRole, string? changedBy = null) => Task.FromResult(true);
        public Task<bool> UpdatePermissionsAsync(int adminId, List<string> permissions, string? changedBy = null) => Task.FromResult(true);
        public Task<IEnumerable<string>> GetPermissionsAsync(int adminId) => Task.FromResult<IEnumerable<string>>(new List<string>());
        public Task<bool> HasPermissionAsync(int adminId, string permission) => Task.FromResult(true);
        public Task<IEnumerable<AdminLoginHistory>> GetLoginHistoryAsync(int adminId, int limit = 50) => Task.FromResult<IEnumerable<AdminLoginHistory>>(new List<AdminLoginHistory>());
        public Task<AdminLoginHistory> AddLoginHistoryAsync(AdminLoginHistory loginHistory) => Task.FromResult(loginHistory);
        public Task<IEnumerable<AdminPermissionHistory>> GetPermissionHistoryAsync(int adminId, int limit = 50) => Task.FromResult<IEnumerable<AdminPermissionHistory>>(new List<AdminPermissionHistory>());
        public Task<AdminPermissionHistory> AddPermissionHistoryAsync(AdminPermissionHistory permissionHistory) => Task.FromResult(permissionHistory);
        public Task<IEnumerable<AdminAccount>> GetRecentlyActiveAsync(int hours = 24) => Task.FromResult<IEnumerable<AdminAccount>>(new List<AdminAccount>());
        public Task<IEnumerable<AdminAccount>> GetInactiveAdminsAsync(int daysInactive = 30) => Task.FromResult<IEnumerable<AdminAccount>>(new List<AdminAccount>());
        public Task<IEnumerable<AdminAccount>> GetLockedAccountsAsync() => Task.FromResult<IEnumerable<AdminAccount>>(new List<AdminAccount>());
        public Task<bool> ActivateAccountAsync(int adminId) => Task.FromResult(true);
        public Task<bool> DeactivateAccountAsync(int adminId, string? reason = null) => Task.FromResult(true);
        public Task<bool> EnableTwoFactorAsync(int adminId, string secret) => Task.FromResult(true);
        public Task<bool> DisableTwoFactorAsync(int adminId) => Task.FromResult(true);
        public Task<bool> UpdateRecoveryTokensAsync(int adminId, List<string> tokens) => Task.FromResult(true);
        public Task<bool> RequestPasswordResetAsync(int adminId) => Task.FromResult(true);
        public Task<bool> IsPasswordResetValidAsync(int adminId, int hoursValid = 24) => Task.FromResult(true);
    }
}
