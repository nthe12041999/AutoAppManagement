using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Repository.Common.Repository;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    public class ToolFeatureRepository : BaseRepository<ToolFeature>, IToolFeatureRepository
    {
        public ToolFeatureRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<ToolFeature?> GetByFeatureCodeAsync(string featureCode)
        {
            return await FirstOrDefault(x => x.FeatureCode == featureCode && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolFeature>> GetActiveFeaturesByCategoryAsync(string category)
        {
            return await FindBy(x => x.Category == category && x.IsActive && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolFeature>> GetFeaturesByTypeAsync(string featureType)
        {
            return await FindBy(x => x.FeatureType == featureType && !x.IsDeleted);
        }

        public async Task<bool> IsFeatureCodeExistsAsync(string featureCode, long? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await CheckExitsByCondition(x => x.FeatureCode == featureCode && x.Id != excludeId.Value && !x.IsDeleted);
            }
            return await CheckExitsByCondition(x => x.FeatureCode == featureCode && !x.IsDeleted);
        }
    }

    public class LicenseFeatureRepository : BaseRepository<LicenseFeature>, ILicenseFeatureRepository
    {
        public LicenseFeatureRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LicenseFeature>> GetFeaturesByLicenseIdAsync(long licenseId)
        {
            return await GetByCondition(x => x.LicenseId == licenseId && !x.IsDeleted);
        }

        public async Task<LicenseFeature?> GetLicenseFeatureAsync(long licenseId, long toolFeatureId)
        {
            return await FirstOrDefault(x => x.LicenseId == licenseId && x.ToolFeatureId == toolFeatureId && !x.IsDeleted);
        }

        public async Task<IEnumerable<LicenseFeature>> GetLicensesByFeatureCodeAsync(string featureCode)
        {
            return await _context.Set<LicenseFeature>()
                .Include(lf => lf.License)
                .Include(lf => lf.ToolFeature)
                .Where(lf => lf.ToolFeature.FeatureCode == featureCode && !lf.IsDeleted && !lf.ToolFeature.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsFeatureEnabledForLicenseAsync(long licenseId, string featureCode)
        {
            return await _context.Set<LicenseFeature>()
                .Include(lf => lf.ToolFeature)
                .AnyAsync(lf => lf.LicenseId == licenseId 
                    && lf.ToolFeature.FeatureCode == featureCode 
                    && lf.IsEnabled 
                    && !lf.IsDeleted 
                    && !lf.ToolFeature.IsDeleted
                    && lf.ToolFeature.IsActive);
        }
    }

    public class FeatureUsageRepository : BaseRepository<FeatureUsage>, IFeatureUsageRepository
    {
        public FeatureUsageRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FeatureUsage>> GetUsageByAccountAsync(long accountId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = await GetByCondition(x => x.AccountId == accountId && !x.IsDeleted);
            
            if (fromDate.HasValue)
                query = query.Where(x => x.UsageDate >= fromDate.Value);
            
            if (toDate.HasValue)
                query = query.Where(x => x.UsageDate <= toDate.Value);
            
            return query.OrderByDescending(x => x.UsageDate);
        }

        public async Task<IEnumerable<FeatureUsage>> GetUsageByLicenseAsync(long licenseId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = await GetByCondition(x => x.LicenseId == licenseId && !x.IsDeleted);
            
            if (fromDate.HasValue)
                query = query.Where(x => x.UsageDate >= fromDate.Value);
            
            if (toDate.HasValue)
                query = query.Where(x => x.UsageDate <= toDate.Value);
            
            return query.OrderByDescending(x => x.UsageDate);
        }

        public async Task<IEnumerable<FeatureUsage>> GetUsageByFeatureAsync(string featureCode, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from fu in _context.Set<FeatureUsage>()
                       join tf in _context.Set<ToolFeature>() on fu.ToolFeatureId equals tf.Id
                       where tf.FeatureCode == featureCode && !fu.IsDeleted
                       select fu;
            
            if (fromDate.HasValue)
                query = query.Where(x => x.UsageDate >= fromDate.Value);
            
            if (toDate.HasValue)
                query = query.Where(x => x.UsageDate <= toDate.Value);
            
            return await query.OrderByDescending(x => x.UsageDate).ToListAsync();
        }

        public async Task<decimal> GetTotalResourceUsageAsync(long accountId, long toolFeatureId, string usageType, DateTime fromDate, DateTime toDate)
        {
            var usages = await GetByCondition(x => x.AccountId == accountId 
                && x.ToolFeatureId == toolFeatureId 
                && x.UsageType == usageType 
                && x.UsageDate >= fromDate 
                && x.UsageDate <= toDate 
                && !x.IsDeleted);
            
            return usages.Sum(x => x.ResourceConsumed);
        }

        public async Task<int> GetUsageCountAsync(long accountId, long toolFeatureId, string usageType, DateTime fromDate, DateTime toDate)
        {
            var usages = await GetByCondition(x => x.AccountId == accountId 
                && x.ToolFeatureId == toolFeatureId 
                && x.UsageType == usageType 
                && x.UsageDate >= fromDate 
                && x.UsageDate <= toDate 
                && !x.IsDeleted);
            
            return usages.Sum(x => x.UsageCount);
        }

        public async Task<FeatureUsage?> GetLatestUsageAsync(long accountId, long toolFeatureId, string usageType)
        {
            var usages = await GetByCondition(x => x.AccountId == accountId 
                && x.ToolFeatureId == toolFeatureId 
                && x.UsageType == usageType 
                && !x.IsDeleted);
            
            return usages.OrderByDescending(x => x.UsageDate).FirstOrDefault();
        }
    }
}
