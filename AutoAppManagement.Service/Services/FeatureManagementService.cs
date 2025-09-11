using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Feature;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;

namespace AutoAppManagement.Service.Services
{
    /// <summary>
    /// Interface cho Simple Feature Management Service
    /// </summary>
    public interface IFeatureManagementService
    {
        #region User Feature Access Methods

        /// <summary>
        /// Lấy danh sách tất cả features được phép sử dụng của user
        /// </summary>
        Task<List<string>> GetAllowedFeaturesAsync(long userId);

        /// <summary>
        /// Lấy thông tin chi tiết về các features được phép sử dụng
        /// </summary>
        Task<List<FeatureInfo>> GetFeatureDetailsAsync(long userId);

        /// <summary>
        /// Kiểm tra user có được phép sử dụng feature không (theo ID)
        /// </summary>
        Task<bool> IsFeatureAllowedAsync(long userId, long featureId);

        /// <summary>
        /// Kiểm tra user có được phép sử dụng feature không (theo Code)
        /// </summary>
        Task<bool> IsFeatureAllowedAsync(long userId, string featureCode);

        #endregion

        #region Usage Tracking Methods

        /// <summary>
        /// Ghi nhận việc sử dụng feature (theo ID)
        /// </summary>
        Task<bool> RecordFeatureUsageAsync(long userId, long featureId, decimal resourceAmount = 1, string usageType = "Access");

        /// <summary>
        /// Ghi nhận việc sử dụng feature (theo Code)
        /// </summary>
        Task<bool> RecordFeatureUsageAsync(long userId, string featureCode, decimal resourceAmount = 1, string usageType = "Access");

        /// <summary>
        /// Lấy thống kê sử dụng features của user
        /// </summary>
        Task<List<FeatureUsageStats>> GetFeatureUsageStatsAsync(long userId);

        #endregion

        #region Feature Information Methods

        /// <summary>
        /// Lấy danh sách categories có sẵn
        /// </summary>
        Task<List<string>> GetFeatureCategoriesAsync();

        /// <summary>
        /// Làm mới cache feature của user (nếu có)
        /// </summary>
        Task RefreshFeatureCacheAsync(long userId);

        #endregion

        #region Admin Methods

        /// <summary>
        /// Gán license cho user (Admin only)
        /// </summary>
        Task<bool> AssignLicenseToUserAsync(long userId, long licenseId, DateTime startDate, DateTime endDate, bool isTrial = false);

        /// <summary>
        /// Thu hồi license từ user (Admin only)
        /// </summary>
        Task<bool> RevokeLicenseFromUserAsync(long userId, long licenseId);

        /// <summary>
        /// Gia hạn license cho user (Admin only)
        /// </summary>
        Task<bool> RenewUserLicenseAsync(long userId, long licenseId, DateTime newEndDate);

        #endregion
    }

    public class FeatureManagementService : BaseService, IFeatureManagementService
    {
        // Lazy load repositories
        private IFeatureRepository? _featureRepository;
        protected IFeatureRepository FeatureRepository
            => _featureRepository ??= _serviceProvider.GetRequiredService<IFeatureRepository>();

        private ILicenseUserRepository? _licenseUserRepository;
        protected ILicenseUserRepository LicenseUserRepository
            => _licenseUserRepository ??= _serviceProvider.GetRequiredService<ILicenseUserRepository>();

        private IFeatureUsageTrackingRepository? _featureUsageTrackingRepository;
        protected IFeatureUsageTrackingRepository FeatureUsageTrackingRepository
            => _featureUsageTrackingRepository ??= _serviceProvider.GetRequiredService<IFeatureUsageTrackingRepository>();

        private ILicenseRepository? _licenseRepository;
        protected ILicenseRepository LicenseRepository
            => _licenseRepository ??= _serviceProvider.GetRequiredService<ILicenseRepository>();

        public FeatureManagementService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<string>> GetAllowedFeaturesAsync(long userId)
        {
            try
            {
                // Lấy active license của user
                var activeLicense = await LicenseUserRepository.GetActiveLicenseByUserId(userId);
                if (activeLicense == null)
                {
                    return new List<string>(); // Không có license thì không có feature nào
                }

                // Lấy thông tin license để biết features được phép
                var license = await LicenseRepository.FirstOrDefault(l => l.Id == activeLicense.LicenseId && !l.IsDeleted);
                if (license == null)
                {
                    return new List<string>();
                }

                // Lấy danh sách feature codes từ JSON trong license
                var allowedFeatureCodes = license.GetFeatureCodes();
                
                return allowedFeatureCodes;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<FeatureInfo>> GetFeatureDetailsAsync(long userId)
        {
            try
            {
                var allowedFeatureCodes = await GetAllowedFeaturesAsync(userId);
                if (!allowedFeatureCodes.Any())
                {
                    return new List<FeatureInfo>();
                }

                // Lấy chi tiết features từ database
                var features = await FeatureRepository.GetFeaturesByCodes(allowedFeatureCodes);
                
                var featureInfos = new List<FeatureInfo>();
                
                foreach (var feature in features)
                {
                    var featureInfo = new FeatureInfo
                    {
                        Id = feature.Id,
                        Code = feature.Code,
                        Name = feature.Name,
                        Description = feature.Description,
                        Category = feature.Category,
                        Icon = feature.Icon,
                        IsActive = feature.IsActive,
                        IsBeta = feature.IsBeta,
                        ResourceType = feature.ResourceType,
                        IsAllowed = true
                    };

                    // Lấy thông tin giới hạn từ license
                    var activeLicense = await LicenseUserRepository.GetActiveLicenseByUserId(userId);
                    if (activeLicense != null)
                    {
                        var license = await LicenseRepository.FirstOrDefault(l => l.Id == activeLicense.LicenseId && !l.IsDeleted);
                        var limits = license?.GetFeatureLimit(feature.Code);
                        
                        if (limits != null)
                        {
                            featureInfo.DailyLimit = limits.ContainsKey("daily") ? limits["daily"] : null;
                            featureInfo.MonthlyLimit = limits.ContainsKey("monthly") ? limits["monthly"] : null;
                        }

                        // Lấy usage hiện tại
                        featureInfo.DailyUsage = await FeatureUsageTrackingRepository.GetDailyUsage(userId, feature.Id);
                        featureInfo.MonthlyUsage = await FeatureUsageTrackingRepository.GetMonthlyUsage(userId, feature.Id);
                    }

                    featureInfos.Add(featureInfo);
                }

                return featureInfos;
            }
            catch (Exception)
            {
                return new List<FeatureInfo>();
            }
        }

        public async Task<bool> IsFeatureAllowedAsync(long userId, long featureId)
        {
            try
            {
                var feature = await FeatureRepository.FirstOrDefault(f => f.Id == featureId && !f.IsDeleted);
                if (feature == null) return false;

                return await IsFeatureAllowedAsync(userId, feature.Code);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsFeatureAllowedAsync(long userId, string featureCode)
        {
            try
            {
                var allowedFeatures = await GetAllowedFeaturesAsync(userId);
                return allowedFeatures.Contains(featureCode);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RecordFeatureUsageAsync(long userId, long featureId, decimal resourceAmount = 1, string usageType = "Access")
        {
            try
            {
                // Kiểm tra quyền trước khi ghi nhận
                if (!await IsFeatureAllowedAsync(userId, featureId))
                {
                    return false;
                }

                // Kiểm tra giới hạn sử dụng
                var feature = await FeatureRepository.FirstOrDefault(f => f.Id == featureId && !f.IsDeleted);
                if (feature == null) return false;

                var activeLicense = await LicenseUserRepository.GetActiveLicenseByUserId(userId);
                if (activeLicense == null) return false;

                var license = await LicenseRepository.FirstOrDefault(l => l.Id == activeLicense.LicenseId && !l.IsDeleted);
                var limits = license?.GetFeatureLimit(feature.Code);

                if (limits != null)
                {
                    // Kiểm tra giới hạn hàng ngày
                    if (limits.ContainsKey("daily"))
                    {
                        var dailyUsage = await FeatureUsageTrackingRepository.GetDailyUsage(userId, featureId);
                        if (dailyUsage >= limits["daily"])
                        {
                            return false; // Đã vượt giới hạn hàng ngày
                        }
                    }

                    // Kiểm tra giới hạn hàng tháng
                    if (limits.ContainsKey("monthly"))
                    {
                        var monthlyUsage = await FeatureUsageTrackingRepository.GetMonthlyUsage(userId, featureId);
                        if (monthlyUsage >= limits["monthly"])
                        {
                            return false; // Đã vượt giới hạn hàng tháng
                        }
                    }
                }

                // Ghi nhận sử dụng
                return await FeatureUsageTrackingRepository.RecordUsage(userId, featureId, resourceAmount, usageType);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RecordFeatureUsageAsync(long userId, string featureCode, decimal resourceAmount = 1, string usageType = "Access")
        {
            try
            {
                var feature = await FeatureRepository.GetByCode(featureCode);
                if (feature == null) return false;

                return await RecordFeatureUsageAsync(userId, feature.Id, resourceAmount, usageType);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<FeatureUsageStats>> GetFeatureUsageStatsAsync(long userId)
        {
            try
            {
                var startDate = DateTime.UtcNow.Date.AddDays(-30); // 30 ngày gần nhất
                var endDate = DateTime.UtcNow.Date.AddDays(1);

                var usageSummary = await FeatureUsageTrackingRepository.GetUsageSummary(userId, startDate, endDate);

                return usageSummary.Select(us => new FeatureUsageStats
                {
                    FeatureCode = us.FeatureCode,
                    FeatureName = us.FeatureName,
                    Category = us.Category,
                    TotalUsage = us.TotalUsage,
                    TotalResourceConsumed = us.TotalResourceConsumed,
                    FirstUsed = us.FirstUsed,
                    LastUsed = us.LastUsed,
                    UsageDays = us.UsageDays
                }).ToList();
            }
            catch (Exception)
            {
                return new List<FeatureUsageStats>();
            }
        }

        public async Task<List<string>> GetFeatureCategoriesAsync()
        {
            try
            {
                return await FeatureRepository.GetDistinctCategories();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task RefreshFeatureCacheAsync(long userId)
        {
            // Simple implementation - in real scenario might involve Redis cache
            // For now, just a placeholder as data is fetched fresh each time
            await Task.CompletedTask;
        }

        #region Admin Methods

        public async Task<bool> AssignLicenseToUserAsync(long userId, long licenseId, DateTime startDate, DateTime endDate, bool isTrial = false)
        {
            try
            {
                return await LicenseUserRepository.AssignLicenseToUser(userId, licenseId, startDate, endDate, isTrial);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RevokeLicenseFromUserAsync(long userId, long licenseId)
        {
            try
            {
                return await LicenseUserRepository.RevokeLicenseFromUser(userId, licenseId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RenewUserLicenseAsync(long userId, long licenseId, DateTime newEndDate)
        {
            try
            {
                return await LicenseUserRepository.RenewUserLicense(userId, licenseId, newEndDate);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}