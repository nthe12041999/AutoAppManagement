using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;

namespace AutoAppManagement.Service.Services
{
    public interface IAdminAccountService
    {
        Task<List<AdminAccountDTO>> GetAll();
        Task<AdminAccountDTO?> GetById(long id);
        Task<AdminAccountDTO?> GetByUserName(string userName);
        Task<TokenDTO> Login(string username, string password, string? ipAddress = null, string? userAgent = null);
        Task<List<AdminAccountDTO>> GetAccountsByRole(string roleName);
    }

    public class AdminAccountService : BaseService, IAdminAccountService
    {
        // Lazy load repositories
        private IAdminAccountRepository? _adminAccountRepository;
        protected IAdminAccountRepository AdminAccountRepository
            => _adminAccountRepository ??= _serviceProvider.GetRequiredService<IAdminAccountRepository>();

        public AdminAccountService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<List<AdminAccountDTO>> GetAll()
        {
            var adminAccounts = await AdminAccountRepository.GetAll();
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.Where(a => a.Status == Models.Enum.StatusEnum.Active).ToList());
        }

        public async Task<AdminAccountDTO?> GetById(long id)
        {
            var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.ID == id && a.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        public async Task<AdminAccountDTO?> GetByUserName(string userName)
        {
            var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.UserName == userName && a.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        public async Task<TokenDTO> Login(string username, string password, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var passwordHash = HashCodeUlti.EncodePassword(password);
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.UserName == username && a.PasswordHash == passwordHash);

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
            var adminAccounts = await AdminAccountRepository.GetAll();
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.Where(a => a.Status != Models.Enum.StatusEnum.Active).ToList());
        }
    }
}
