using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;

namespace AutoAppManagement.Service.Services
{
    public interface IAdminAccountService: IBaseBusinessService<AdminAccountDTO>
    {
        Task<AdminAccountDTO?> GetByUserName(string userName);
        Task<TokenDTO> Login(string username, string password, string? ipAddress = null, string? userAgent = null);
        Task<List<AdminAccountDTO>> GetAccountsByRole(string roleName);
    }

    public class AdminAccountService : BaseBusinessService<AdminAccount, AdminAccountDTO, IAdminAccountRepository>, IAdminAccountService
    {
        public AdminAccountService(IServiceProvider serviceProvider) : base(serviceProvider) { }


        public async Task<AdminAccountDTO?> GetByUserName(string userName)
        {
            var adminAccount = await Repository.FirstOrDefault(a => a.UserName == userName && a.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        public async Task<TokenDTO> Login(string username, string password, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var passwordHash = HashCodeUlti.EncodePassword(password);
                var adminAccount = await Repository.FirstOrDefault(a => a.UserName == username && a.PasswordHash == passwordHash);

                if (adminAccount == null)
                    return null;

                // Update login info
                adminAccount.LastLoginAt = DateTime.UtcNow;
                adminAccount.LastLoginIp = ipAddress;
                await UnitOfWork.SaveAsync();

                return new TokenDTO
                {
                    AccessToken = "jwt_token_here", // TODO: Implement JWT
                    AccessTokenExpired = DateTime.UtcNow.AddHours(24)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<AdminAccountDTO>> GetAccountsByRole(string roleName)
        {
            // TODO: Implement role-based filtering
            var adminAccounts = await Repository.GetAll();
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.Where(a => a.Status != Models.Enum.StatusEnum.Active).ToList());
        }
    }
}
