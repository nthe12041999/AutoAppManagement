using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;

namespace AutoAppManagement.Service.Services
{
    public interface IAdminAccountService : IBaseBusinessService<AdminAccountDTO>
    {
        Task<AdminAccountDTO> GetAdminAccountByUsername(string username);
        Task<RestOutput> ChangePassword(long id, string newPassword);
        Task<RestOutput> LockAccount(long id, int minutes = 30);
        Task<RestOutput> UnlockAccount(long id);
        Task<RestOutput> VerifyEmail(long id);
        Task<RestOutput> VerifyPhone(long id);
        Task<RestOutput> EnableTwoFactor(long id);
        Task<RestOutput> DisableTwoFactor(long id);
        Task<TokenViewModel> Login(string username, string password, string? ipAddress = null, string? userAgent = null);
        Task<List<AdminAccountDTO>> GetAdminAccountsByRole(string role);
        Task<RestOutput> UpdatePermissions(long id, string permissions);
    }

    public class AdminAccountService : BaseBusinessService<AdminAccount, AdminAccountDTO, IAdminAccountRepository>, IAdminAccountService
    {
        public AdminAccountService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<AdminAccountDTO> GetAdminAccountByUsername(string username)
        {
            var adminAccount = await Repository.FirstOrDefault(a => a.UserName == username && !a.IsDeleted);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        public async Task<RestOutput> ChangePassword(long id, string newPassword)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                adminAccount.ChangePassword(HashCodeUlti.EncodePassword(newPassword), GetUserAuthen()?.Id);
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }

        public async Task<RestOutput> LockAccount(long id, int minutes = 30)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                adminAccount.LockAccount(minutes, GetUserAuthen()?.Id);
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }

        public async Task<RestOutput> UnlockAccount(long id)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                adminAccount.UnlockAccount(GetUserAuthen()?.Id);
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }

        public async Task<RestOutput> VerifyEmail(long id)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                // TODO: Implement email verification logic
                // adminAccount.VerifyEmail(GetUserAuthen()?.Id);
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }

        public async Task<RestOutput> VerifyPhone(long id)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                // TODO: Implement phone verification logic
                // adminAccount.VerifyPhone(GetUserAuthen()?.Id);
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }

        public async Task<RestOutput> EnableTwoFactor(long id)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                adminAccount.IsTwoFactorEnabled = true;
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }

        public async Task<RestOutput> DisableTwoFactor(long id)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                adminAccount.IsTwoFactorEnabled = false;
                adminAccount.TwoFactorSecret = null;
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }

        public async Task<TokenViewModel> Login(string username, string password, string? ipAddress = null, string? userAgent = null)
        {
            var adminAccount = await Repository.FirstOrDefault(a => a.UserName == username && !a.IsDeleted);

            if (adminAccount == null) throw new Exception("Tài khoản không tồn tại");
            if (adminAccount.IsLocked) throw new Exception("Tài khoản đã bị khóa");

            var passwordHash = HashCodeUlti.EncodePassword(password);
            if (adminAccount.PasswordHash != passwordHash)
            {
                throw new Exception("Mật khẩu không chính xác");
            }

            if (!adminAccount.IsActive) throw new Exception("Tài khoản không hoạt động");

            //adminAccount.RecordLogin(ipAddress, userAgent);
            //await UnitOfWork.SaveAsync();

            var jwtService = _serviceProvider.GetRequiredService<IJwtService>();
            var accountToken = new Account
            {
                Id = adminAccount.Id,
                UserName = adminAccount.UserName,
                Email = adminAccount.Email,
                Phone = adminAccount.PhoneNumber,
                Name = adminAccount.FullName,
            };
            var token = jwtService.GenerateToken(accountToken, null);

            return new TokenViewModel
            {
                AccessToken = token.AccessToken,
                AccessTokenExpired = token.AccessTokenExpired,
                AccountInfor = adminAccount
            };
        }

        public async Task<List<AdminAccountDTO>> GetAdminAccountsByRole(string role)
        {
            var adminAccounts = await Repository.GetByCondition(a => a.Role == role && !a.IsDeleted);
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.ToList());
        }

        public async Task<RestOutput> UpdatePermissions(long id, string permissions)
        {
            var result = new RestOutput();
            try
            {
                var adminAccount = await UpdateById(id);

                // TODO: Implement permissions property or method
                // adminAccount.Permissions = permissions;
                await UnitOfWork.SaveAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }
            return result;
        }
    }
}
