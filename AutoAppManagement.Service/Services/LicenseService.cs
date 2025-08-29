using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;

namespace AutoAppManagement.Service.Services
{
    public interface ILicenseService : IBaseBusinessService<LicenseDTO>
    {
        Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId);
        Task<LicenseDTO> GetLicenseByKey(string licenseKey);
        Task<BaseResponse> RenewLicense(RenewLicenseRequest request);
        Task<BaseResponse> SuspendLicense(long id);
        Task<BaseResponse> ActivateLicense(long id);
        Task<List<LicenseDTO>> GetExpiredLicenses();
        Task<List<LicenseDTO>> GetExpiringLicenses(int days);
        Task<BaseResponse> ExtendLicense(long id, DateTime newExpiryDate);
    }

    public class LicenseService : BaseBusinessService<License, LicenseDTO, ILicenseRepository>, ILicenseService
    {
        public LicenseService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId)
        {
            var licenses = await Repository.GetByCondition(l => l.AccountId == accountId && !l.IsDeleted);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        public async Task<LicenseDTO> GetLicenseByKey(string licenseKey)
        {
            var license = await Repository.FirstOrDefault(l => l.LicenseKey == licenseKey && !l.IsDeleted);
            return Mapper.Map<LicenseDTO>(license);
        }

        public async Task<BaseResponse> RenewLicense(RenewLicenseRequest request)
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
