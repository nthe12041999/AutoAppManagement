using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Dapper;

namespace AutoAppManagement.Service.Services
{
    public interface IAdminAccountService : IBaseBusinessService<AdminAccountDTO>
    {
        Task<AdminAccountDTO?> GetByUserName(string userName);
        Task<ResponseOutput<TokenViewModel>> Login(string username, string password, string? ipAddress = null, string? userAgent = null, bool rememberMe = false);
        Task<ResponseOutput<TokenViewModel>> RefreshTokenAsync(string refreshToken, string? ip = null, string? userAgent = null);
        Task<ResponseOutput<bool>> ChangePassword(long userId, string currentPassword, string newPassword);
    }

    public class AdminAccountService : BaseBusinessService<AdminAccount, AdminAccountDTO, IAdminAccountRepository>, IAdminAccountService
    {
        // Lazy load repositories
        private IAdminAccountRepository _adminAccountRepository;
        protected IAdminAccountRepository AdminAccountRepository
            => _adminAccountRepository ??= _serviceProvider.GetRequiredService<IAdminAccountRepository>();

        private IJwtService _jwtService;
        protected IJwtService JwtService
            => _jwtService ??= _serviceProvider.GetRequiredService<IJwtService>();

        private IRoleAccountRepository _roleAccountRepository;
        protected IRoleAccountRepository RoleAccountRepository
            => _roleAccountRepository ??= _serviceProvider.GetRequiredService<IRoleAccountRepository>();

        private IRoleRepository _roleRepository;
        protected IRoleRepository RoleRepository
            => _roleRepository ??= _serviceProvider.GetRequiredService<IRoleRepository>();

        private IGenericRepository<RefreshTokenAdmin> _refreshTokenRepository;
        protected IGenericRepository<RefreshTokenAdmin> RefreshTokenRepository
            => _refreshTokenRepository ??= UnitOfWork.GetRepository<RefreshTokenAdmin>();

        private IRolePermissionRepository _rolePermissionRepository;
        protected IRolePermissionRepository RolePermissionRepository
            => _rolePermissionRepository ??= _serviceProvider.GetRequiredService<IRolePermissionRepository>();

        public AdminAccountService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        private static DateTime GetRefreshTokenExpiryUtc(bool rememberMe)
        {
            // RememberMe: 30 ngày, ngược lại: 1 ngày
            return DateTime.UtcNow.AddDays(rememberMe ? 30 : 1);
        }

        private static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Override để lấy cả Active và Inactive cho danh sách nhân viên
        /// </summary>
        protected override IQueryable<AdminAccount> GetBaseQuery(IEnumerable<AdminAccount> entities)
        {
            return entities.Where(e => e.Status == StatusEnum.Active || e.Status == StatusEnum.Inactive).AsQueryable();
        }

        public async Task<AdminAccountDTO   > GetByUserName(string userName)
        {
            var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.UserName == userName && a.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        public async Task<ResponseOutput<TokenViewModel>> Login(string username, string password, string? ipAddress = null, string? userAgent = null, bool rememberMe = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập và mật khẩu không được để trống"
                    };
                }

                var passwordHash = HashCodeUlti.EncodePassword(password);
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.Email == username && a.PasswordHash == passwordHash);

                if (adminAccount == null)
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                    };
                }

                if (adminAccount.Status != Models.Enum.StatusEnum.Active)
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "Tài khoản đã bị vô hiệu hóa"
                    };
                }

                if (adminAccount.LockedUntil.HasValue && adminAccount.LockedUntil.Value > DateTime.UtcNow)
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = $"Tài khoản đã bị khóa đến {adminAccount.LockedUntil.Value:dd/MM/yyyy HH:mm:ss}"
                    };
                }

                // Update login info
                adminAccount.LastLoginAt = DateTime.UtcNow;
                adminAccount.LastLoginIp = ipAddress;
                adminAccount.LoginCount++;

                // Load user permissions
                var permissions = await GetUserPermissions(adminAccount.ID);

                // Generate access token with permissions
                var tokenDTO = JwtService.GenerateAdminToken(adminAccount, permissions);

                // Generate refresh token
                var refreshTokenStr = JwtService.GenerateRefreshToken();
                var tokenHash = HashToken(refreshTokenStr);
                var familyId = Guid.NewGuid();

                var refreshToken = new RefreshTokenAdmin
                {
                    AdminAccountId = adminAccount.ID,
                    Token = refreshTokenStr,
                    ExpiryDate = GetRefreshTokenExpiryUtc(rememberMe),
                    IsUsed = false,
                    IsRevoked = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByIp = ipAddress,
                    UserAgent = userAgent,
                    TokenHash = tokenHash,
                    FamilyId = familyId
                };

                adminAccount.RefreshTokens.Add(refreshToken);
                await UnitOfWork.SaveAsync();

                var tokenViewModel = new TokenViewModel
                {
                    AccessToken = tokenDTO.AccessToken,
                    AccessTokenExpired = tokenDTO.AccessTokenExpired,
                    RefreshToken = refreshTokenStr,
                    RefreshTokenExpired = refreshToken.ExpiryDate,
                    AccountInfor = adminAccount,
                    Permissions = permissions
                };

                return new ResponseOutput<TokenViewModel>
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công",
                    Data = tokenViewModel
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<TokenViewModel>
                {
                    IsSuccess = false,
                    Message = $"Đã có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        public async Task<ResponseOutput<TokenViewModel>> RefreshTokenAsync(string refreshToken, string? ip = null, string? userAgent = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "RefreshToken không được để trống"
                    };
                }

                var token = await RefreshTokenRepository.FirstOrDefault(t => 
                    t.Token == refreshToken && 
                    t.Status == StatusEnum.Active);

                if (token == null)
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "RefreshToken không hợp lệ"
                    };
                }

                if (token.IsRevoked)
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "RefreshToken đã bị thu hồi"
                    };
                }

                if (token.IsUsed)
                {
                    // Token reuse detected - có thể bị đánh cắp
                    // Thu hồi toàn bộ family
                    var tokensInFamily = (await RefreshTokenRepository.GetAll())
                        .Where(t => t.FamilyId == token.FamilyId && t.Status == StatusEnum.Active)
                        .ToList();
                    
                    foreach (var t in tokensInFamily)
                    {
                        t.IsRevoked = true;
                        t.RevokedDate = DateTime.UtcNow;
                        t.RevokedByIp = ip;
                    }
                    await UnitOfWork.SaveAsync();

                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "Phát hiện token bị sử dụng lại - vui lòng đăng nhập lại"
                    };
                }

                if (token.ExpiryDate < DateTime.UtcNow)
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "RefreshToken đã hết hạn"
                    };
                }

                // Get admin account
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.ID == token.AdminAccountId);
                if (adminAccount == null || adminAccount.Status != StatusEnum.Active)
                {
                    return new ResponseOutput<TokenViewModel>
                    {
                        IsSuccess = false,
                        Message = "Tài khoản không tồn tại hoặc đã bị vô hiệu hóa"
                    };
                }

                // Mark old token as used
                token.IsUsed = true;
                
                // Load user permissions
                var permissions = await GetUserPermissions(adminAccount.ID);
                
                // Generate new tokens
                var newAccessToken = JwtService.GenerateAdminToken(adminAccount, permissions);
                var newRefreshTokenStr = JwtService.GenerateRefreshToken();
                var newTokenHash = HashToken(newRefreshTokenStr);

                var newRefreshToken = new RefreshTokenAdmin
                {
                    AdminAccountId = adminAccount.ID,
                    Token = newRefreshTokenStr,
                    ExpiryDate = token.ExpiryDate, // Giữ nguyên expiry của token cũ
                    IsUsed = false,
                    IsRevoked = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByIp = ip,
                    UserAgent = userAgent,
                    TokenHash = newTokenHash,
                    FamilyId = token.FamilyId, // Cùng family
                    ReplacedByToken = newRefreshTokenStr
                };

                token.ReplacedByToken = newRefreshTokenStr;
                adminAccount.RefreshTokens.Add(newRefreshToken);
                await UnitOfWork.SaveAsync();

                var tokenViewModel = new TokenViewModel
                {
                    AccessToken = newAccessToken.AccessToken,
                    AccessTokenExpired = newAccessToken.AccessTokenExpired,
                    RefreshToken = newRefreshTokenStr,
                    RefreshTokenExpired = newRefreshToken.ExpiryDate,
                    AccountInfor = adminAccount,
                    Permissions = permissions
                };

                return new ResponseOutput<TokenViewModel>
                {
                    IsSuccess = true,
                    Message = "Làm mới token thành công",
                    Data = tokenViewModel
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<TokenViewModel>
                {
                    IsSuccess = false,
                    Message = $"Đã có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        public async Task<ResponseOutput<bool>> ChangePassword(long userId, string currentPassword, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return new ResponseOutput<bool>
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu không được để trống"
                    };
                }

                if (newPassword.Length < 8)
                {
                    return new ResponseOutput<bool>
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu mới phải có ít nhất 8 ký tự"
                    };
                }

                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.ID == userId && a.Status == Models.Enum.StatusEnum.Active);

                if (adminAccount == null)
                {
                    return new ResponseOutput<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tài khoản"
                    };
                }

                var currentPasswordHash = HashCodeUlti.EncodePassword(currentPassword);
                if (adminAccount.PasswordHash != currentPasswordHash)
                {
                    return new ResponseOutput<bool>
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu hiện tại không đúng"
                    };
                }

                adminAccount.PasswordHash = HashCodeUlti.EncodePassword(newPassword);
                adminAccount.PasswordChangedAt = DateTime.UtcNow;
                await UnitOfWork.SaveAsync();

                return new ResponseOutput<bool>
                {
                    IsSuccess = true,
                    Message = "Đổi mật khẩu thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<bool>
                {
                    IsSuccess = false,
                    Message = $"Đã có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        public override async Task<AdminAccountDTO> GetById(long id)
        {
            try
            {
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.ID == id);
                if (adminAccount == null)
                {
                    return null;
                }

                var dto = Mapper.Map<AdminAccountDTO>(adminAccount);

                // Load roles cho admin account
                var roleAccounts = (await RoleAccountRepository.GetAll())
                    .Where(ra => ra.AccountID == id)
                    .ToList();
                    
                if (roleAccounts != null && roleAccounts.Any())
                {
                    var roleIds = roleAccounts.Select(ra => ra.RoleID).ToList();
                    var roles = (await RoleRepository.GetAll())
                        .Where(r => roleIds.Contains(r.ID))
                        .ToList();
                    
                    if (roles != null && roles.Any())
                    {
                        dto.Roles = roles.Select(r => new { r.ID, r.Name, r.Description }).ToList();
                    }
                }

                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin: {ex.Message}");
                return null;
            }
        }

        public override async Task<BaseResponse> SubmitData(AdminAccountDTO dto)
        {
            try
            {
                // Đặt password mặc định nếu không có
                if (string.IsNullOrEmpty(dto.PasswordHash))
                {
                    dto.PasswordHash = HashCodeUlti.EncodePassword("12345678@Abc");
                }
                else
                {
                    dto.PasswordHash = HashCodeUlti.EncodePassword(dto.PasswordHash);
                }

                // Gọi base SubmitData để lưu AdminAccount
                var result = await base.SubmitData(dto);
                
                if (!result.IsSuccess)
                {
                    return result;
                }

                // Xử lý RoleIds nếu có
                if (dto.RoleIds != null && dto.RoleIds.Any())
                {
                    var accountId = dto.State == AutoAppManagement.Models.Common.EntityState.Add
                        ? (await AdminAccountRepository.FirstOrDefault(a => a.Email == dto.Email))?.ID ?? 0
                        : dto.ID;

                    if (accountId > 0)
                    {
                        // Lấy các RoleAccount hiện tại (Active)
                        var existingRoleAccounts = (await RoleAccountRepository.GetAll())
                            .Where(ra => ra.AccountID == accountId)
                            .ToList();
                        
                        var existingRoleIds = existingRoleAccounts.Select(ra => ra.RoleID).ToList();
                        var newRoleIds = dto.RoleIds.ToList();
                        
                        // Xóa các role không còn trong danh sách mới (DELETE thật)
                        var roleIdsToDelete = existingRoleIds.Except(newRoleIds).ToList();
                        if (roleIdsToDelete.Any())
                        {
                            var roleAccountsToDelete = existingRoleAccounts.Where(ra => roleIdsToDelete.Contains(ra.RoleID)).ToList();
                            RoleAccountRepository.DeleteRange(roleAccountsToDelete);
                        }
                        
                        // Thêm các role mới chưa có
                        var roleIdsToAdd = newRoleIds.Except(existingRoleIds).ToList();
                        foreach (var roleId in roleIdsToAdd)
                        {
                            var roleAccount = new RoleAccount
                            {
                                AccountID = accountId,
                                RoleID = roleId,
                                Status = StatusEnum.Active
                            };
                            await RoleAccountRepository.CreateAsync(roleAccount);
                        }
                        
                        // Roles đã có thì giữ nguyên (không làm gì)
                        
                        await UnitOfWork.SaveAsync();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi lưu: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all permissions for a user based on their assigned roles
        /// </summary>
        private async Task<List<string>> GetUserPermissions(long accountId)
        {
            return await Repository.GetUserPermissions(accountId);
        }
    }
}
