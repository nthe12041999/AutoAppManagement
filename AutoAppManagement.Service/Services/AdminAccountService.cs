using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using AutoAppManagement.Service.Common.Ulti;
using Microsoft.Extensions.DependencyInjection;

namespace AutoAppManagement.Service.Services
{
    public interface IAdminAccountService
    {
        Task<List<AdminAccountDTO>> GetAll();
        Task<AdminAccountDTO?> GetById(long id);
        Task<AdminAccountDTO?> GetByUserName(string userName);
        Task<BaseResponse> SubmitData(AdminAccountDTO dto);
        Task<BaseResponse> Delete(long id);
        Task<BaseResponse> ChangePassword(long id, string newPassword);
        Task<BaseResponse> LockAccount(long id, int minutes = 30);
        Task<BaseResponse> UnlockAccount(long id);
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
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.Where(a => !a.IsDeleted).ToList());
        }

        public async Task<AdminAccountDTO?> GetById(long id)
        {
            var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        public async Task<AdminAccountDTO?> GetByUserName(string userName)
        {
            var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.UserName == userName && !a.IsDeleted);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        public async Task<BaseResponse> SubmitData(AdminAccountDTO dto)
        {
            try
            {
                if (dto.State == EntityState.Add)
                {
                    var adminAccount = Mapper.Map<AdminAccount>(dto);
                    adminAccount.SetCreated(1); // Hardcode for now
                    await AdminAccountRepository.CreateAsync(adminAccount);
                }
                else if (dto.State == EntityState.Edit)
                {
                    var existingAccount = await AdminAccountRepository.FirstOrDefault(a => a.Id == dto.Id);
                    if (existingAccount == null)
                        return BaseResponse.Error("Không tìm thấy tài khoản admin");

                    Mapper.Map(dto, existingAccount);
                    existingAccount.SetUpdated(1); // Hardcode for now
                }
                else if (dto.State == EntityState.Remove)
                {
                    var existingAccount = await AdminAccountRepository.FirstOrDefault(a => a.Id == dto.Id);
                    if (existingAccount == null)
                        return BaseResponse.Error("Không tìm thấy tài khoản admin");

                    existingAccount.SetDeleted(1); // Hardcode for now
                }

                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Thực hiện thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<BaseResponse> Delete(long id)
        {
            try
            {
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                    return BaseResponse.Error("Không tìm thấy tài khoản admin");

                adminAccount.SetDeleted(1); // Hardcode for now
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Xóa tài khoản admin thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xóa tài khoản: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ChangePassword(long id, string newPassword)
        {
            try
            {
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.Id == id);
                if (adminAccount == null)
                    return BaseResponse.Error("Không tìm thấy tài khoản admin");

                adminAccount.ChangePassword(HashCodeUlti.EncodePassword(newPassword), 1); // Hardcode for now
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đổi mật khẩu: {ex.Message}");
            }
        }

        public async Task<BaseResponse> LockAccount(long id, int minutes = 30)
        {
            try
            {
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.Id == id);
                if (adminAccount == null)
                    return BaseResponse.Error("Không tìm thấy tài khoản admin");

                adminAccount.LockAccount(minutes, 1); // Hardcode for now
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success($"Khóa tài khoản thành công trong {minutes} phút");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi khóa tài khoản: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UnlockAccount(long id)
        {
            try
            {
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.Id == id);
                if (adminAccount == null)
                    return BaseResponse.Error("Không tìm thấy tài khoản admin");

                adminAccount.UnlockAccount(1); // Hardcode for now
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Mở khóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi mở khóa tài khoản: {ex.Message}");
            }
        }

        public async Task<TokenDTO> Login(string username, string password, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var passwordHash = HashCodeUlti.EncodePassword(password);
                var adminAccount = await AdminAccountRepository.FirstOrDefault(a => a.UserName == username && a.PasswordHash == passwordHash);

                if (adminAccount == null || adminAccount.IsLocked)
                    return null;

                // Update login info
                adminAccount.LastLoginAt = DateTime.UtcNow;
                adminAccount.LastLoginIp = ipAddress;
                adminAccount.Status = "Online";
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
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.Where(a => !a.IsDeleted).ToList());
        }
    }
}
