using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AutoAppManagement.Models.Enum;

namespace AutoAppManagement.Service.Services
{
    public interface IRefreshTokenService : IBaseBusinessService<RefreshTokenDTO>
    {
        Task<BaseResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress = null, string? userAgent = null);
        Task<BaseResponse> RevokeTokenAsync(RevokeTokenRequest request, string? ipAddress = null);
        Task<BaseResponse> RevokeAllUserTokensAsync(long accountId, string? ipAddress = null);
        Task<RefreshToken> CreateRefreshTokenAsync(long accountId, string refreshToken, string? ipAddress = null, string? userAgent = null);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        Task<int> CleanupExpiredTokensAsync();
    }

    public class RefreshTokenService : BaseBusinessService<RefreshToken, RefreshTokenDTO, IRefreshTokenRepository>, IRefreshTokenService
    {
        private IGenericRepository<Account>? _accountRepository;
        protected IGenericRepository<Account> AccountRepository
            => _accountRepository ??= UnitOfWork.GetRepository<Account>();

        private IJwtService? _jwtService;
        protected IJwtService JwtService
            => _jwtService ??= _serviceProvider.GetRequiredService<IJwtService>();

        private IAccountService? _accountService;
        protected IAccountService AccountService
            => _accountService ??= _serviceProvider.GetRequiredService<IAccountService>();

        public RefreshTokenService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Refresh access token bằng refresh token
        /// </summary>
        public async Task<BaseResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                // Tìm refresh token trong database
                var refreshToken = await Repository.GetByTokenAsync(request.RefreshToken);
                if (refreshToken == null)
                {
                    return BaseResponse.Error("Refresh token không hợp lệ");
                }

                // Kiểm tra refresh token có active không
                if (!refreshToken.IsActive)
                {
                    return BaseResponse.Error("Refresh token đã hết hạn hoặc bị thu hồi");
                }

                // Lấy thông tin account
                var account = await AccountRepository.FirstOrDefault(a => a.ID == refreshToken.AccountId && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Tài khoản không tồn tại hoặc đã bị vô hiệu hóa");
                }

                // Kiểm tra account có bị khóa không
                if (account.IsLocked)
                {
                    await Repository.RevokeAllTokensByAccountIdAsync(account.ID, ipAddress);
                    return BaseResponse.Error("Tài khoản đã bị khóa");
                }

                // Đánh dấu refresh token cũ đã được sử dụng
                refreshToken.IsUsed = true;
                refreshToken.SetUpdated(account.ID);
                await Repository.Update(refreshToken);

                // Tạo token mới
                var newTokens = JwtService.GenerateToken(account);

                // Tạo refresh token mới
                var newRefreshToken = await CreateRefreshTokenAsync(
                    account.ID, 
                    newTokens.RefreshToken!, 
                    ipAddress, 
                    userAgent);

                // Cập nhật thông tin đăng nhập
                account.SetUpdated(account.ID);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(new RefreshTokenResponse
                {
                    AccessToken = newTokens.AccessToken,
                    AccessTokenExpired = newTokens.AccessTokenExpired,
                    RefreshToken = newTokens.RefreshToken!,
                    RefreshTokenExpired = newTokens.RefreshTokenExpired!.Value
                }, "Refresh token thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi refresh token: {ex.Message}");
            }
        }

        /// <summary>
        /// Thu hồi một refresh token cụ thể
        /// </summary>
        public async Task<BaseResponse> RevokeTokenAsync(RevokeTokenRequest request, string? ipAddress = null)
        {
            try
            {
                var result = await Repository.RevokeTokenAsync(request.Token, ipAddress);
                if (!result)
                {
                    return BaseResponse.Error("Token không tồn tại hoặc đã bị thu hồi");
                }

                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Thu hồi token thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi thu hồi token: {ex.Message}");
            }
        }

        /// <summary>
        /// Thu hồi tất cả refresh token của một user
        /// </summary>
        public async Task<BaseResponse> RevokeAllUserTokensAsync(long accountId, string? ipAddress = null)
        {
            try
            {
                var result = await Repository.RevokeAllTokensByAccountIdAsync(accountId, ipAddress);
                if (!result)
                {
                    return BaseResponse.Error("Không có token nào để thu hồi");
                }

                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Thu hồi tất cả token thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi thu hồi token: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo refresh token mới và lưu vào database
        /// </summary>
        public async Task<RefreshToken> CreateRefreshTokenAsync(long accountId, string refreshToken, string? ipAddress = null, string? userAgent = null)
        {
            var token = new RefreshToken
            {
                Token = refreshToken,
                AccountId = accountId,
                ExpiryDate = DateTime.UtcNow.AddDays(7), // 7 ngày
                CreatedByIp = ipAddress,
                UserAgent = userAgent,
                CreatedBy = accountId
            };

            await Repository.Insert(token);
            return token;
        }

        /// <summary>
        /// Validate refresh token
        /// </summary>
        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var token = await Repository.GetByTokenAsync(refreshToken);
                return token?.IsActive == true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Dọn dẹp các token đã hết hạn
        /// </summary>
        public async Task<int> CleanupExpiredTokensAsync()
        {
            try
            {
                var count = await Repository.CleanupExpiredTokensAsync();
                await UnitOfWork.SaveAsync();
                return count;
            }
            catch
            {
                return 0;
            }
        }
    }
}
