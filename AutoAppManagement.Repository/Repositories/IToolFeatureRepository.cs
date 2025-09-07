using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Repositories.Base;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IToolFeatureRepository : IBaseRepository<ToolFeature>
    {
        Task<ToolFeature?> GetByFeatureCodeAsync(string featureCode);
        Task<IEnumerable<ToolFeature>> GetActiveFeaturesByCategoryAsync(string category);
        Task<IEnumerable<ToolFeature>> GetFeaturesByTypeAsync(string featureType);
        Task<bool> IsFeatureCodeExistsAsync(string featureCode, long? excludeId = null);
    }

    public interface ILicenseFeatureRepository : IBaseRepository<LicenseFeature>
    {
        Task<IEnumerable<LicenseFeature>> GetFeaturesByLicenseIdAsync(long licenseId);
        Task<LicenseFeature?> GetLicenseFeatureAsync(long licenseId, long toolFeatureId);
        Task<IEnumerable<LicenseFeature>> GetLicensesByFeatureCodeAsync(string featureCode);
        Task<bool> IsFeatureEnabledForLicenseAsync(long licenseId, string featureCode);
    }

    public interface IFeatureUsageRepository : IBaseRepository<FeatureUsage>
    {
        Task<IEnumerable<FeatureUsage>> GetUsageByAccountAsync(long accountId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<FeatureUsage>> GetUsageByLicenseAsync(long licenseId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<FeatureUsage>> GetUsageByFeatureAsync(string featureCode, DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetTotalResourceUsageAsync(long accountId, long toolFeatureId, string usageType, DateTime fromDate, DateTime toDate);
        Task<int> GetUsageCountAsync(long accountId, long toolFeatureId, string usageType, DateTime fromDate, DateTime toDate);
        Task<FeatureUsage?> GetLatestUsageAsync(long accountId, long toolFeatureId, string usageType);
    }
}
