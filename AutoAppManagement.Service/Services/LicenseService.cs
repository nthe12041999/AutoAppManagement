using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
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
        Task<BaseResponse> UnassignLicenseFromUser(long licenseUserId);
        Task<List<LicenseUserDTO>> GetUsersAssignedToLicense(long licenseId);
        
        // License management methods
        Task<BaseResponse> RenewLicense(AutoAppManagement.Models.DTO.License.RenewLicenseRequest request);
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

        private IGenericRepository<LicenseUser>? _licenseUserRepository;
        protected IGenericRepository<LicenseUser> LicenseUserRepository
            => _licenseUserRepository ??= UnitOfWork.GetRepository<LicenseUser>();

        public LicenseService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId)
        {
            try
            {
                // Cách 1: Lấy license trực tiếp từ Account.LicenseId
                var account = await AccountRepository.FirstOrDefault(a => a.Id == accountId && !a.IsDeleted);
                if (account?.LicenseId != null)
                {
                    var license = await Repository.FirstOrDefault(l => l.Id == account.LicenseId && !l.IsDeleted);
                    if (license != null)
                    {
                        return new List<LicenseDTO> { Mapper.Map<LicenseDTO>(license) };
                    }
                }

                // Cách 2: Lấy license từ bảng LicenseUser
                var licenseUsers = await LicenseUserRepository.GetByCondition(lu => lu.AccountId == accountId && !lu.IsDeleted);
                var licenseIds = licenseUsers.Select(lu => lu.LicenseId).ToList();
                var licenses = await Repository.GetByCondition(l => licenseIds.Contains(l.Id) && !l.IsDeleted);
                
                return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
            }
            catch (Exception)
            {
                return new List<LicenseDTO>();
            }
        }

        public async Task<LicenseDTO> GetLicenseByKey(string licenseKey)
        {
            var license = await Repository.FirstOrDefault(l => l.LicenseKey == licenseKey && !l.IsDeleted);
            return Mapper.Map<LicenseDTO>(license);
        }

        public async Task<BaseResponse> AssignLicenseToAccount(AssignLicenseToAccountRequest request)
        {
            try
            {
                // Kiểm tra license tồn tại và hợp lệ
                var license = await Repository.FirstOrDefault(l => l.Id == request.LicenseId && !l.IsDeleted);
                if (license == null)
                {
                    return BaseResponse.Error("License không tồn tại");
                }

                if (license.ExpiryDate < DateTime.UtcNow)
                {
                    return BaseResponse.Error("License đã hết hạn");
                }

                // Kiểm tra account tồn tại
                var account = await AccountRepository.FirstOrDefault(a => a.Id == request.AccountId && !a.IsDeleted);
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
                // Kiểm tra license tồn tại
                var license = await Repository.FirstOrDefault(l => l.Id == request.LicenseId && !l.IsDeleted);
                if (license == null)
                {
                    return BaseResponse.Error("License không tồn tại");
                }

                // Kiểm tra account tồn tại  
                var account = await AccountRepository.FirstOrDefault(a => a.Id == request.AccountId && !a.IsDeleted);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }

                // Kiểm tra đã gán chưa
                var existingAssignment = await LicenseUserRepository.FirstOrDefault(
                    lu => lu.LicenseId == request.LicenseId && lu.AccountId == request.AccountId && !lu.IsDeleted);
                
                if (existingAssignment != null)
                {
                    return BaseResponse.Error("License đã được gán cho user này");
                }

                // Tạo record mới trong LicenseUser
                var licenseUser = new LicenseUser
                {
                    LicenseId = request.LicenseId,
                    AccountId = request.AccountId,
                    StartDate = request.StartDate ?? DateTime.UtcNow,
                    EndDate = request.EndDate ?? license.ExpiryDate ?? DateTime.MaxValue,
                    IsActive = true
                };
                licenseUser.SetCreated(GetCurrentUserId());

                await LicenseUserRepository.Insert(licenseUser);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đã gán license '{license.LicenseName}' cho user '{account.UserName}' thành công");
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
                var account = await AccountRepository.FirstOrDefault(a => a.Id == accountId && !a.IsDeleted);
                if (account == null)
                {
                    return BaseResponse.Error("Account không tồn tại");
                }

                if (account.LicenseId == null)
                {
                    return BaseResponse.Error("Account chưa được gán license");
                }

                account.LicenseId = null;
                account.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đã hủy gán license khỏi account thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi hủy gán license: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UnassignLicenseFromUser(long licenseUserId)
        {
            try
            {
                var licenseUser = await LicenseUserRepository.FirstOrDefault(lu => lu.Id == licenseUserId && !lu.IsDeleted);
                if (licenseUser == null)
                {
                    return BaseResponse.Error("License assignment không tồn tại");
                }

                licenseUser.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đã hủy gán license khỏi user thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi hủy gán license: {ex.Message}");
            }
        }

        public async Task<List<LicenseUserDTO>> GetUsersAssignedToLicense(long licenseId)
        {
            try
            {
                var licenseUsers = await LicenseUserRepository.GetByCondition(lu => lu.LicenseId == licenseId && !lu.IsDeleted);
                return Mapper.Map<List<LicenseUserDTO>>(licenseUsers.ToList());
            }
            catch (Exception)
            {
                return new List<LicenseUserDTO>();
            }
        }

        public async Task<BaseResponse> RenewLicense(AutoAppManagement.Models.DTO.License.RenewLicenseRequest request)
        {
            try
            {
                var license = await UpdateById(request.LicenseId);

                license.ExpiryDate = request.NewExpiryDate;
                license.Status = "Active";

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

                license.Status = "Suspended";
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

                license.Status = "Active";
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
            var licenses = await Repository.GetByCondition(l => l.ExpiryDate < DateTime.UtcNow && l.Status == "Active" && !l.IsDeleted);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        public async Task<List<LicenseDTO>> GetExpiringLicenses(int days)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var licenses = await Repository.GetByCondition(l => l.ExpiryDate <= expiryDate && l.ExpiryDate > DateTime.UtcNow && l.Status == "Active" && !l.IsDeleted);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        public async Task<BaseResponse> ExtendLicense(long id, DateTime newExpiryDate)
        {
            try
            {
                var license = await UpdateById(id);

                if (newExpiryDate <= license.ExpiryDate)
                {
                    return BaseResponse.Error("Ngày hết hạn mới phải sau ngày hết hạn hiện tại");
                }

                license.ExpiryDate = newExpiryDate;
                license.SetUpdated(GetCurrentUserId());
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
