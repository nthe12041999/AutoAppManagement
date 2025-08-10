using AutoAppManagement.Models.BaseEntity;

namespace AutoAppManagement.Repositories
{
    public interface IAdminAccountRepository
    {
        // Basic CRUD Operations
        Task<IEnumerable<AdminAccount>> GetAllAsync();
        Task<AdminAccount?> GetByIdAsync(int id);
        Task<AdminAccount?> GetByEmailAsync(string email);
        Task<AdminAccount?> GetByUserNameAsync(string userName);
        Task<AdminAccount> CreateAsync(AdminAccount adminAccount);
        Task<AdminAccount> UpdateAsync(AdminAccount adminAccount);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Pagination and Filtering
        Task<(IEnumerable<AdminAccount> Items, int TotalCount)> GetPagedAsync(
            int page = 1, 
            int pageSize = 10, 
            string? search = null,
            string? role = null,
            string? status = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null);

        // Authentication and Security
        Task<AdminAccount?> AuthenticateAsync(string userName, string password);
        Task<bool> VerifyPasswordAsync(int adminId, string password);
        Task<bool> ChangePasswordAsync(int adminId, string newPasswordHash);
        Task<bool> LockAccountAsync(int adminId, DateTime lockUntil, string? reason = null);
        Task<bool> UnlockAccountAsync(int adminId);
        Task<bool> IncrementFailedLoginAsync(int adminId);
        Task<bool> ResetFailedLoginAsync(int adminId);

        // Email and Phone Verification
        Task<bool> VerifyEmailAsync(int adminId);
        Task<bool> VerifyPhoneAsync(int adminId);
        Task<bool> UpdateLastLoginAsync(int adminId, string? ipAddress = null, string? userAgent = null);

        // Role and Permission Management
        Task<bool> UpdateRoleAsync(int adminId, string newRole, string? changedBy = null);
        Task<bool> UpdatePermissionsAsync(int adminId, List<string> permissions, string? changedBy = null);
        Task<IEnumerable<string>> GetPermissionsAsync(int adminId);
        Task<bool> HasPermissionAsync(int adminId, string permission);

        // Login History
        Task<IEnumerable<AdminLoginHistory>> GetLoginHistoryAsync(int adminId, int limit = 50);
        Task<AdminLoginHistory> AddLoginHistoryAsync(AdminLoginHistory loginHistory);

        // Permission History
        Task<IEnumerable<AdminPermissionHistory>> GetPermissionHistoryAsync(int adminId, int limit = 50);
        Task<AdminPermissionHistory> AddPermissionHistoryAsync(AdminPermissionHistory permissionHistory);

        // Statistics and Reports
        Task<AdminAccountStats> GetStatsAsync();
        Task<IEnumerable<AdminAccount>> GetRecentlyActiveAsync(int hours = 24);
        Task<IEnumerable<AdminAccount>> GetInactiveAdminsAsync(int daysInactive = 30);
        Task<IEnumerable<AdminAccount>> GetLockedAccountsAsync();

        // Validation
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> IsUserNameUniqueAsync(string userName, int? excludeId = null);

        // Account Management
        Task<bool> ActivateAccountAsync(int adminId);
        Task<bool> DeactivateAccountAsync(int adminId, string? reason = null);
        Task<bool> EnableTwoFactorAsync(int adminId, string secret);
        Task<bool> DisableTwoFactorAsync(int adminId);

        // Security
        Task<bool> UpdateRecoveryTokensAsync(int adminId, List<string> tokens);
        Task<bool> RequestPasswordResetAsync(int adminId);
        Task<bool> IsPasswordResetValidAsync(int adminId, int hoursValid = 24);
    }

    public class AdminAccountStats
    {
        public int TotalAdmins { get; set; }
        public int ActiveAdmins { get; set; }
        public int InactiveAdmins { get; set; }
        public int LockedAdmins { get; set; }
        public int VerifiedAdmins { get; set; }
        public int UnverifiedAdmins { get; set; }
        public int OnlineAdmins { get; set; }
        public int TwoFactorEnabledAdmins { get; set; }
        public Dictionary<string, int> RoleDistribution { get; set; } = new();
        public Dictionary<string, int> LoginsByDay { get; set; } = new();
        public Dictionary<string, int> FailedLoginsByDay { get; set; } = new();
    }
}
