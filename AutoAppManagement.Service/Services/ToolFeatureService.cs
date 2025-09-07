using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.ToolFeature;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AutoAppManagement.Service.Services
{
    public interface IToolFeatureService : IBaseBusinessService<ToolFeatureDTO>
    {
        Task<BaseResponse> CreateToolFeatureAsync(CreateToolFeatureRequest request);
        Task<BaseResponse> UpdateToolFeatureAsync(UpdateToolFeatureRequest request);
        Task<List<ToolFeatureDTO>> GetFeaturesByCategoryAsync(string category);
        Task<List<ToolFeatureDTO>> GetFeaturesByTypeAsync(string featureType);
        Task<ToolFeatureDTO?> GetFeatureByCodeAsync(string featureCode);
        Task<bool> IsFeatureCodeExistsAsync(string featureCode, long? excludeId = null);
        Task<ToolFeatureDTO?> GetByIdAsync(long id);
    }

    public interface ILicenseFeatureService : IBaseBusinessService<LicenseFeatureDTO>
    {
        Task<BaseResponse> AssignFeatureToLicenseAsync(AssignFeatureToLicenseRequest request);
        Task<BaseResponse> RemoveFeatureFromLicenseAsync(long licenseId, long toolFeatureId);
        Task<BaseResponse> UpdateLicenseFeatureAsync(long licenseId, long toolFeatureId, string? resourceLimits, string? usageQuota);
        Task<List<LicenseFeatureDTO>> GetFeaturesByLicenseAsync(long licenseId);
        Task<List<LicenseFeatureDTO>> GetLicensesByFeatureAsync(string featureCode);
        Task<bool> IsFeatureEnabledForLicenseAsync(long licenseId, string featureCode);
        Task<List<LicenseFeatureDTO>> GetByLicenseIdAsync(long licenseId);
    }

    public interface IFeatureAccessService
    {
        Task<FeatureAccessCheckResult> CheckFeatureAccessAsync(CheckFeatureAccessRequest request);
        Task<BaseResponse> RecordFeatureUsageAsync(long accountId, string licenseKey, string featureCode, string usageType = "Access", decimal resourceAmount = 1, string? usageData = null);
        Task<List<FeatureUsageReport>> GetUsageReportAsync(FeatureUsageReportRequest request);
        Task<bool> IsWithinUsageLimitsAsync(long accountId, long licenseId, long toolFeatureId, string usageType, decimal requestedAmount);
    }

    public class ToolFeatureService : BaseBusinessService<ToolFeature, ToolFeatureDTO, IToolFeatureRepository>, IToolFeatureService
    {
        public ToolFeatureService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<BaseResponse> CreateToolFeatureAsync(CreateToolFeatureRequest request)
        {
            try
            {
                // Check if feature code already exists
                if (await Repository.IsFeatureCodeExistsAsync(request.FeatureCode))
                {
                    return BaseResponse.Error("Mã tính năng đã tồn tại");
                }

                var entity = new ToolFeature
                {
                    FeatureCode = request.FeatureCode,
                    FeatureName = request.FeatureName,
                    Description = request.Description,
                    Category = request.Category,
                    FeatureType = request.FeatureType,
                    RequiresLicense = request.RequiresLicense,
                    DefaultLimits = request.DefaultLimits
                };

                entity.SetCreated(GetCurrentUserId());
                await Repository.CreateAsync(entity);
                await UnitOfWork.SaveAsync();

                var dto = Mapper.Map<ToolFeatureDTO>(entity);
                return BaseResponse.Success(dto, "Tạo tính năng thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạo tính năng: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UpdateToolFeatureAsync(UpdateToolFeatureRequest request)
        {
            try
            {
                var entity = await UpdateById(request.Id);
                
                if (!string.IsNullOrEmpty(request.FeatureName))
                    entity.FeatureName = request.FeatureName;
                
                if (!string.IsNullOrEmpty(request.Description))
                    entity.Description = request.Description;
                
                if (!string.IsNullOrEmpty(request.Category))
                    entity.Category = request.Category;
                
                if (request.IsActive.HasValue)
                    entity.IsActive = request.IsActive.Value;
                
                if (request.RequiresLicense.HasValue)
                    entity.RequiresLicense = request.RequiresLicense.Value;
                
                if (!string.IsNullOrEmpty(request.DefaultLimits))
                    entity.DefaultLimits = request.DefaultLimits;
                
                if (!string.IsNullOrEmpty(request.Status))
                    entity.Status = request.Status;

                entity.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                var dto = Mapper.Map<ToolFeatureDTO>(entity);
                return BaseResponse.Success(dto, "Cập nhật tính năng thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật tính năng: {ex.Message}");
            }
        }

        public async Task<List<ToolFeatureDTO>> GetFeaturesByCategoryAsync(string category)
        {
            var features = await Repository.GetActiveFeaturesByCategoryAsync(category);
            return Mapper.Map<List<ToolFeatureDTO>>(features.ToList());
        }

        public async Task<List<ToolFeatureDTO>> GetFeaturesByTypeAsync(string featureType)
        {
            var features = await Repository.GetFeaturesByTypeAsync(featureType);
            return Mapper.Map<List<ToolFeatureDTO>>(features.ToList());
        }

        public async Task<ToolFeatureDTO?> GetFeatureByCodeAsync(string featureCode)
        {
            var feature = await Repository.GetByFeatureCodeAsync(featureCode);
            return feature != null ? Mapper.Map<ToolFeatureDTO>(feature) : null;
        }

        public async Task<bool> IsFeatureCodeExistsAsync(string featureCode, long? excludeId = null)
        {
            return await Repository.IsFeatureCodeExistsAsync(featureCode, excludeId);
        }

        public async Task<ToolFeatureDTO?> GetByIdAsync(long id)
        {
            return await GetById(id);
        }
    }

    public class LicenseFeatureService : BaseBusinessService<LicenseFeature, LicenseFeatureDTO, ILicenseFeatureRepository>, ILicenseFeatureService
    {
        private readonly IToolFeatureRepository _toolFeatureRepository;
        private readonly ILicenseRepository _licenseRepository;

        public LicenseFeatureService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _toolFeatureRepository = serviceProvider.GetRequiredService<IToolFeatureRepository>();
            _licenseRepository = serviceProvider.GetRequiredService<ILicenseRepository>();
        }

        public async Task<BaseResponse> AssignFeatureToLicenseAsync(AssignFeatureToLicenseRequest request)
        {
            try
            {
                // Check if license exists
                var license = await _licenseRepository.FirstOrDefault(l => l.Id == request.LicenseId && !l.IsDeleted);
                if (license == null)
                {
                    return BaseResponse.Error("License không tồn tại");
                }

                // Check if feature exists
                var feature = await _toolFeatureRepository.FirstOrDefault(f => f.Id == request.ToolFeatureId && !f.IsDeleted);
                if (feature == null)
                {
                    return BaseResponse.Error("Tính năng không tồn tại");
                }

                // Check if already assigned
                var existing = await Repository.GetLicenseFeatureAsync(request.LicenseId, request.ToolFeatureId);
                if (existing != null)
                {
                    return BaseResponse.Error("Tính năng đã được gán cho license này");
                }

                var entity = new LicenseFeature
                {
                    LicenseId = request.LicenseId,
                    ToolFeatureId = request.ToolFeatureId,
                    IsEnabled = request.IsEnabled,
                    ResourceLimits = request.ResourceLimits,
                    UsageQuota = request.UsageQuota,
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = request.EffectiveTo
                };

                entity.SetCreated(GetCurrentUserId());
                await Repository.CreateAsync(entity);
                await UnitOfWork.SaveAsync();

                var dto = Mapper.Map<LicenseFeatureDTO>(entity);
                return BaseResponse.Success(dto, "Gán tính năng cho license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán tính năng: {ex.Message}");
            }
        }

        public async Task<BaseResponse> RemoveFeatureFromLicenseAsync(long licenseId, long toolFeatureId)
        {
            try
            {
                var entity = await Repository.GetLicenseFeatureAsync(licenseId, toolFeatureId);
                if (entity == null)
                {
                    return BaseResponse.Error("Không tìm thấy liên kết tính năng-license");
                }

                entity.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Xóa tính năng khỏi license thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xóa tính năng: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UpdateLicenseFeatureAsync(long licenseId, long toolFeatureId, string? resourceLimits, string? usageQuota)
        {
            try
            {
                var entity = await Repository.GetLicenseFeatureAsync(licenseId, toolFeatureId);
                if (entity == null)
                {
                    return BaseResponse.Error("Không tìm thấy liên kết tính năng-license");
                }

                if (!string.IsNullOrEmpty(resourceLimits))
                    entity.ResourceLimits = resourceLimits;
                
                if (!string.IsNullOrEmpty(usageQuota))
                    entity.UsageQuota = usageQuota;

                entity.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                var dto = Mapper.Map<LicenseFeatureDTO>(entity);
                return BaseResponse.Success(dto, "Cập nhật cấu hình tính năng thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật cấu hình: {ex.Message}");
            }
        }

        public async Task<List<LicenseFeatureDTO>> GetFeaturesByLicenseAsync(long licenseId)
        {
            var features = await Repository.GetFeaturesByLicenseIdAsync(licenseId);
            return Mapper.Map<List<LicenseFeatureDTO>>(features.ToList());
        }

        public async Task<List<LicenseFeatureDTO>> GetLicensesByFeatureAsync(string featureCode)
        {
            var licenses = await Repository.GetLicensesByFeatureCodeAsync(featureCode);
            return Mapper.Map<List<LicenseFeatureDTO>>(licenses.ToList());
        }

        public async Task<bool> IsFeatureEnabledForLicenseAsync(long licenseId, string featureCode)
        {
            return await Repository.IsFeatureEnabledForLicenseAsync(licenseId, featureCode);
        }

        public async Task<List<LicenseFeatureDTO>> GetByLicenseIdAsync(long licenseId)
        {
            var entities = await Repository.GetByCondition(lf => lf.LicenseId == licenseId && !lf.IsDeleted);
            return Mapper.Map<List<LicenseFeatureDTO>>(entities.ToList());
        }
    }
}
