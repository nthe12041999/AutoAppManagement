using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;

namespace AutoAppManagement.Service.Services
{
    public interface ILicenseService : IBaseBusinessService<LicenseDTO>
    {
        Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId);
        Task<LicenseDTO> GetLicenseByKey(string licenseKey);
        
        // License assignment methods
        Task<BaseResponse> AssignLicenseToAccount(AssignLicenseToAccountRequest request);
        Task<BaseResponse> AssignLicenseToUser(AssignLicenseToUserRequest request);
        Task<BaseResponse> UnassignLicenseFromAccount(long accountId);
        Task<List<AccountDTO>> GetUsersAssignedToLicense(long licenseId);
        
        // License management methods
        Task<BaseResponse> RenewLicense(RenewLicenseRequest request);
        Task<BaseResponse> SuspendLicense(long id);
        Task<BaseResponse> ActivateLicense(long id);
        Task<List<LicenseDTO>> GetExpiredLicenses();
        Task<List<LicenseDTO>> GetExpiringLicenses(int days);
        Task<BaseResponse> ExtendLicense(long id, DateTime newExpiryDate);
    }

    public class LicenseService : BaseBusinessService<License, LicenseDTO, ILicenseRepository>, ILicenseService
    {
        // Lazy load specific repositories 
        private ILicenseRepository? _licenseRepository;
        protected ILicenseRepository LicenseRepository
            => _licenseRepository ??= _serviceProvider.GetRequiredService<ILicenseRepository>();

        private IAccountsRepository? _accountRepository;
        protected IAccountsRepository AccountRepository
            => _accountRepository ??= UnitOfWork.AccountsRepository;

        public LicenseService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId)
        {
            try
            {
                // Cách 1: Lấy license trực tiếp từ Account.LicenseId
                var account = await AccountRepository.FirstOrDefault(a => a.ID == accountId);
                if (account?.LicenseId != null)
                {
                    var license = await Repository.FirstOrDefault(l => l.ID == account.LicenseId);
                    if (license != null)
                    {
                        return new List<LicenseDTO> { Mapper.Map<LicenseDTO>(license) };
                    }
                }
                return new List<LicenseDTO>();
            }
            catch (Exception)
            {
                return new List<LicenseDTO>();
            }
        }

        public async Task<LicenseDTO> GetLicenseByKey(string licenseKey)
        {
            var license = await Repository.FirstOrDefault(l => l.LicenseKey == licenseKey);
            return Mapper.Map<LicenseDTO>(license);
        }

        public async Task<BaseResponse> AssignLicenseToAccount(AssignLicenseToAccountRequest request)
        {
            try
            {
                // Kiểm tra license tồn tại và hợp lệ
                var license = await Repository.FirstOrDefault(l => l.ID == request.LicenseId);
                if (license == null)
                {
                    return BaseResponse.Error("License không tồn tại");
                }

                if (license.EndDate < DateTime.UtcNow)
                {
                    return BaseResponse.Error("License đã hết hạn");
                }

                // Kiểm tra account tồn tại
                var account = await AccountRepository.FirstOrDefault(a => a.ID == request.AccountId);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }

                // Gán license cho account
                account.LicenseId = request.LicenseId;
                account.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đã gán license '{license.LicenseName}' cho account '{account.UserName}' thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán license cho account: {ex.Message}");
            }
        }

        public async Task<BaseResponse> AssignLicenseToUser(AssignLicenseToUserRequest request)
        {
            try
            {
                // Kiểm tra account tồn tại  
                var account = await AccountRepository.FirstOrDefault(a => a.ID == request.AccountId);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }
                account.LicenseId = request.LicenseId;

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đã gán license cho user '{account.UserName}' thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán license cho user: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UnassignLicenseFromAccount(long accountId)
        {
            try
            {
                var account = await AccountRepository.FirstOrDefault(a => a.ID == accountId);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }

                account.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đã hủy gán license khỏi account thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi hủy gán license: {ex.Message}");
            }
        }

        public async Task<List<AccountDTO>> GetUsersAssignedToLicense(long licenseId)
        {
            try
            {
                var licenseUsers = await AccountRepository.GetByCondition(lu => lu.LicenseId == licenseId);
                return Mapper.Map<List<AccountDTO>>(licenseUsers.ToList());
            }
            catch (Exception)
            {
                return new List<AccountDTO>();
            }
        }

        public async Task<BaseResponse> RenewLicense(AutoAppManagement.Models.DTO.License.RenewLicenseRequest request)
        {
            try
            {
                var license = await UpdateById(request.LicenseId);

                license.EndDate = request.NewExpiryDate;
                license.Status = Models.Enum.StatusEnum.Active;

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<LicenseDTO>(license), "Gia hạn license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gia hạn license: {ex.Message}");
            }
        }

        public async Task<BaseResponse> SuspendLicense(long id)
        {
            try
            {
                var license = await UpdateById(id);

                license.Status = Models.Enum.StatusEnum.Inactive;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Tạm ngưng license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạm ngưng license: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ActivateLicense(long id)
        {
            try
            {
                var license = await UpdateById(id);

                license.Status = Models.Enum.StatusEnum.Active;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Kích hoạt license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi kích hoạt license: {ex.Message}");
            }
        }

        public async Task<List<LicenseDTO>> GetExpiredLicenses()
        {
            var licenses = await Repository.GetByCondition(l => l.EndDate < DateTime.UtcNow && l.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        public async Task<List<LicenseDTO>> GetExpiringLicenses(int days)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var licenses = await Repository.GetByCondition(l => l.EndDate <= expiryDate && l.EndDate > DateTime.UtcNow && l.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        public async Task<BaseResponse> ExtendLicense(long id, DateTime newExpiryDate)
        {
            try
            {
                var license = await UpdateById(id);

                if (newExpiryDate <= license.EndDate)
                {
                    return BaseResponse.Error("Ngày hết hạn mới phải sau ngày hết hạn hiện tại");
                }

                license.EndDate = newExpiryDate;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<LicenseDTO>(license), "Gia hạn license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gia hạn license: {ex.Message}");
            }
        }
    }
}
