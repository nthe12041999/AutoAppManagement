using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace AutoAppManagement.Service.Services
{
    public interface ILicenseService
    {
        Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId);
        Task<LicenseDTO> GetLicenseById(long id);
        Task<LicenseDTO> GetLicenseByKey(string licenseKey);
        Task<RestOutput> CreateLicense(CreateLicenseRequest request);
        Task<RestOutput> UpdateLicense(UpdateLicenseRequest request);
        Task<RestOutput> DeleteLicense(long id);
        Task<RestOutput> RenewLicense(RenewLicenseRequest request);
        Task<RestOutput> SuspendLicense(long id);
        Task<RestOutput> ActivateLicense(long id);
        Task<List<LicenseDTO>> GetExpiredLicenses();
        Task<List<LicenseDTO>> GetExpiringLicenses(int days);
        Task<bool> ValidateLicense(string licenseKey);
        Task<RestOutput> ExtendLicense(long id, DateTime newExpiryDate);
    }

    public class LicenseService : BaseService, ILicenseService
    {
        public LicenseService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache, 
            IUnitOfWork unitOfWork, IMapper mapper, INotificationSocketHub notificationSocketHub) 
            : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
        }

        /// <summary>
        /// Lấy license theo account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<LicenseDTO>> GetLicensesByAccountId(long accountId)
        {
            var licenses = await UnitOfWork.LicenseRepository.GetByCondition(l => l.AccountId == accountId);
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        /// <summary>
        /// Lấy license theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<LicenseDTO> GetLicenseById(long id)
        {
            var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.Id == id);
            return Mapper.Map<LicenseDTO>(license);
        }

        /// <summary>
        /// Lấy license theo key
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <returns></returns>
        public async Task<LicenseDTO> GetLicenseByKey(string licenseKey)
        {
            var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.LicenseKey == licenseKey);
            return Mapper.Map<LicenseDTO>(license);
        }

        /// <summary>
        /// Tạo license mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> CreateLicense(CreateLicenseRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra account tồn tại
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.AccountId);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                // Kiểm tra license key đã tồn tại chưa
                var existingLicense = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.LicenseKey == request.LicenseKey);
                if (existingLicense != null)
                {
                    result.ErrorEventHandler("License key đã tồn tại");
                    return result;
                }

                var license = new License
                {
                    AccountId = request.AccountId,
                    LicenseKey = request.LicenseKey,
                    LicenseName = request.LicenseName,
                    LicenseType = request.LicenseType,
                    Description = request.Description,
                    MaxDevices = request.MaxDevices,
                    MaxUsers = request.MaxUsers,
                    StartDate = request.StartDate,
                    ExpiryDate = request.ExpiryDate,
                    Status = "Active",
                    IsAutoRenewal = request.IsAutoRenewal,
                    Price = request.Price,
                    Currency = request.Currency ?? "VND",
                    PaymentInfo = request.PaymentInfo,
                    AllowedFeatures = request.AllowedFeatures,
                    UsageLimits = request.UsageLimits,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetUserAuthen()?.Id,
                    Notes = request.Notes
                };

                await UnitOfWork.LicenseRepository.CreateAsync(license);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<LicenseDTO>(license));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateLicense(UpdateLicenseRequest request)
        {
            var result = new RestOutput();

            try
            {
                var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.Id == request.Id);
                if (license == null)
                {
                    result.ErrorEventHandler("License không tồn tại");
                    return result;
                }

                // Kiểm tra license key đã tồn tại chưa (trừ license hiện tại)
                var existingLicense = await UnitOfWork.LicenseRepository.FirstOrDefault(l => 
                    l.LicenseKey == request.LicenseKey && l.Id != request.Id);
                if (existingLicense != null)
                {
                    result.ErrorEventHandler("License key đã tồn tại");
                    return result;
                }

                license.LicenseKey = request.LicenseKey;
                license.LicenseName = request.LicenseName;
                license.LicenseType = request.LicenseType;
                license.Description = request.Description;
                license.MaxDevices = request.MaxDevices;
                license.MaxUsers = request.MaxUsers;
                license.StartDate = request.StartDate;
                license.ExpiryDate = request.ExpiryDate;
                license.IsAutoRenewal = request.IsAutoRenewal;
                license.Price = request.Price;
                license.Currency = request.Currency ?? "VND";
                license.PaymentInfo = request.PaymentInfo;
                license.AllowedFeatures = request.AllowedFeatures;
                license.UsageLimits = request.UsageLimits;
                license.UpdatedDate = DateTime.UtcNow;
                license.UpdatedBy = GetUserAuthen()?.Id;
                license.Notes = request.Notes;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<LicenseDTO>(license));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Xóa license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeleteLicense(long id)
        {
            var result = new RestOutput();

            try
            {
                var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.Id == id);
                if (license == null)
                {
                    result.ErrorEventHandler("License không tồn tại");
                    return result;
                }

                UnitOfWork.LicenseRepository.Delete(license);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> RenewLicense(RenewLicenseRequest request)
        {
            var result = new RestOutput();

            try
            {
                var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.Id == request.LicenseId);
                if (license == null)
                {
                    result.ErrorEventHandler("License không tồn tại");
                    return result;
                }

                license.ExpiryDate = request.NewExpiryDate;
                license.Status = "Active";
                license.UpdatedDate = DateTime.UtcNow;
                license.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<LicenseDTO>(license));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Tạm dừng license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> SuspendLicense(long id)
        {
            var result = new RestOutput();

            try
            {
                var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.Id == id);
                if (license == null)
                {
                    result.ErrorEventHandler("License không tồn tại");
                    return result;
                }

                license.Status = "Suspended";
                license.UpdatedDate = DateTime.UtcNow;
                license.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Kích hoạt license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> ActivateLicense(long id)
        {
            var result = new RestOutput();

            try
            {
                var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.Id == id);
                if (license == null)
                {
                    result.ErrorEventHandler("License không tồn tại");
                    return result;
                }

                license.Status = "Active";
                license.UpdatedDate = DateTime.UtcNow;
                license.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Lấy license đã hết hạn
        /// </summary>
        /// <returns></returns>
        public async Task<List<LicenseDTO>> GetExpiredLicenses()
        {
            var licenses = await UnitOfWork.LicenseRepository.GetByCondition(l => 
                l.ExpiryDate < DateTime.UtcNow && l.Status == "Active");
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        /// <summary>
        /// Lấy license sắp hết hạn
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<List<LicenseDTO>> GetExpiringLicenses(int days)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var licenses = await UnitOfWork.LicenseRepository.GetByCondition(l => 
                l.ExpiryDate <= expiryDate && l.ExpiryDate > DateTime.UtcNow && l.Status == "Active");
            return Mapper.Map<List<LicenseDTO>>(licenses.ToList());
        }

        /// <summary>
        /// Kiểm tra license hợp lệ
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <returns></returns>
        public async Task<bool> ValidateLicense(string licenseKey)
        {
            var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.LicenseKey == licenseKey);
            return license != null && license.Status == "Active" && license.ExpiryDate > DateTime.UtcNow;
        }

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newExpiryDate"></param>
        /// <returns></returns>
        public async Task<RestOutput> ExtendLicense(long id, DateTime newExpiryDate)
        {
            var result = new RestOutput();

            try
            {
                var license = await UnitOfWork.LicenseRepository.FirstOrDefault(l => l.Id == id);
                if (license == null)
                {
                    result.ErrorEventHandler("License không tồn tại");
                    return result;
                }

                if (newExpiryDate <= license.ExpiryDate)
                {
                    result.ErrorEventHandler("Ngày hết hạn mới phải sau ngày hết hạn hiện tại");
                    return result;
                }

                license.ExpiryDate = newExpiryDate;
                license.UpdatedDate = DateTime.UtcNow;
                license.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<LicenseDTO>(license));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }
    }
}
