using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Data.Models;
using AutoAppManagement.Repository.Repositories.Base;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetActiveTokensByAccountIdAsync(long accountId);
        Task<bool> RevokeTokenAsync(string token, string? revokedByIp = null);
        Task<bool> RevokeAllTokensByAccountIdAsync(long accountId, string? revokedByIp = null);
        Task<int> CleanupExpiredTokensAsync();
    }

    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await FirstOrDefault(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByAccountIdAsync(long accountId)
        {
            var tokens = await GetByCondition(rt => 
                rt.AccountId == accountId && 
                !rt.IsRevoked && 
                !rt.IsUsed && 
                rt.ExpiryDate > DateTime.UtcNow &&
                rt.Status == Models.Enum.StatusEnum.Active);
            
            return tokens.ToList();
        }

        public async Task<bool> RevokeTokenAsync(string token, string? revokedByIp = null)
        {
            try
            {
                var refreshToken = await GetByTokenAsync(token);
                if (refreshToken == null || refreshToken.IsRevoked)
                    return false;

                refreshToken.IsRevoked = true;
                refreshToken.RevokedDate = DateTime.UtcNow;
                refreshToken.RevokedByIp = revokedByIp;
                refreshToken.SetUpdated(1); // System user

                _dbset.Update(refreshToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RevokeAllTokensByAccountIdAsync(long accountId, string? revokedByIp = null)
        {
            try
            {
                var activeTokens = await GetActiveTokensByAccountIdAsync(accountId);
                
                foreach (var token in activeTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedDate = DateTime.UtcNow;
                    token.RevokedByIp = revokedByIp;
                    token.SetUpdated(1); // System user
                    
                    _dbset.Update(token);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> CleanupExpiredTokensAsync()
        {
            try
            {
                var expiredTokens = await GetByCondition(rt => 
                    rt.ExpiryDate <= DateTime.UtcNow || 
                    rt.IsUsed || 
                    rt.IsRevoked);

                var count = expiredTokens.Count();
                
                foreach (var token in expiredTokens)
                {
                    token.Status = Models.Enum.StatusEnum.Inactive;
                    token.SetUpdated(1); // System user
                    _dbset.Update(token);
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }
    }
}
