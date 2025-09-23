using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Feature;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Data.Models;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    /// <summary>
    /// Interface cho FeatureUsageTracking Repository
    /// </summary>
    public interface IFeatureUsageTrackingRepository : IBaseRepository<FeatureUsageTracking>
    {
        Task<bool> RecordUsage(long userId, long featureId, decimal resourceAmount = 1, string usageType = "Access", string? metadata = null);
        Task<int> GetMonthlyUsage(long userId, long featureId, DateTime? month = null);
        Task<int> GetDailyUsage(long userId, long featureId, DateTime? date = null);
        Task<List<FeatureUsageSummary>> GetUsageSummary(long userId, DateTime startDate, DateTime endDate);
        Task<int> GetUsageCount(long userId, long featureId, string usageType, DateTime startDate, DateTime endDate);
        Task<FeatureUsageTracking?> GetLatestUsage(long userId, long featureId, string usageType);
        Task<decimal> GetTotalResourceUsage(long userId, long featureId, string usageType, DateTime startDate, DateTime endDate);
        Task<List<FeatureUsageTracking>> GetUsageByAccount(long userId, DateTime startDate, DateTime endDate);
    }

    /// <summary>
    /// Implementation cho FeatureUsageTrackingRepository
    /// </summary>
    public class FeatureUsageTrackingRepository : BaseRepository<FeatureUsageTracking>, IFeatureUsageTrackingRepository
    {
        public FeatureUsageTrackingRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<bool> RecordUsage(long userId, long featureId, decimal resourceAmount = 1, string usageType = "Access", string? metadata = null)
        {
            try
            {
                var usage = new FeatureUsageTracking
                {
                    UserId = userId,
                    FeatureId = featureId,
                    ResourceAmount = resourceAmount,
                    UsageType = usageType ?? "Access",
                    Metadata = metadata,
                    UsageDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                await _context.Set<FeatureUsageTracking>().AddAsync(usage);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> GetMonthlyUsage(long userId, long featureId, DateTime? month = null)
        {
            var targetMonth = month?.Date ?? DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            return await _context.Set<FeatureUsageTracking>()
                .Where(fut => fut.UserId == userId && 
                             fut.FeatureId == featureId &&
                             fut.Status == Models.Enum.StatusEnum.Active &&
                             fut.UsageDate >= startOfMonth && 
                             fut.UsageDate < startOfNextMonth)
                .SumAsync(fut => fut.UsageCount);
        }

        public async Task<int> GetDailyUsage(long userId, long featureId, DateTime? date = null)
        {
            var targetDate = date?.Date ?? DateTime.UtcNow.Date;
            var nextDay = targetDate.AddDays(1);

            return await _context.Set<FeatureUsageTracking>()
                .Where(fut => fut.UserId == userId && 
                             fut.FeatureId == featureId &&
                             fut.Status == Models.Enum.StatusEnum.Active &&
                             fut.UsageDate >= targetDate && 
                             fut.UsageDate < nextDay)
                .SumAsync(fut => fut.UsageCount);
        }

        public async Task<List<FeatureUsageSummary>> GetUsageSummary(long userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<FeatureUsageTracking>()
                .Include(fut => fut.Feature)
                .Where(fut => fut.UserId == userId &&
                             fut.Status == Models.Enum.StatusEnum.Active &&
                             fut.UsageDate >= startDate &&
                             fut.UsageDate <= endDate.AddDays(1))
                .GroupBy(fut => new { fut.FeatureId, fut.Feature.Code, fut.Feature.Name, fut.Feature.Category })
                .Select(g => new FeatureUsageSummary
                {
                    UserId = userId,
                    FeatureId = g.Key.FeatureId,
                    FeatureCode = g.Key.Code,
                    FeatureName = g.Key.Name,
                    Category = g.Key.Category ?? "Unknown",
                    TotalUsage = g.Sum(x => x.UsageCount),
                    TotalResourceConsumed = g.Sum(x => x.ResourceAmount),
                    FirstUsed = g.Min(fut => fut.UsageDate),
                    LastUsed = g.Max(fut => fut.UsageDate),
                    UsageDays = g.Select(x => x.UsageDate.Date).Distinct().Count()
                })
                .OrderByDescending(fus => fus.TotalUsage)
                .ToListAsync();
        }

        public async Task<int> GetUsageCount(long userId, long featureId, string usageType, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<FeatureUsageTracking>()
                .Where(fut => fut.UserId == userId && 
                             fut.FeatureId == featureId &&
                             fut.UsageType == usageType &&
                             fut.Status == Models.Enum.StatusEnum.Active &&
                             fut.UsageDate >= startDate && 
                             fut.UsageDate <= endDate)
                .SumAsync(fut => fut.UsageCount);
        }

        public async Task<FeatureUsageTracking?> GetLatestUsage(long userId, long featureId, string usageType)
        {
            return await _context.Set<FeatureUsageTracking>()
                .Where(fut => fut.UserId == userId && 
                             fut.FeatureId == featureId &&
                             fut.UsageType == usageType &&
                             fut.Status == Models.Enum.StatusEnum.Active)
                .OrderByDescending(fut => fut.UsageDate)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetTotalResourceUsage(long userId, long featureId, string usageType, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<FeatureUsageTracking>()
                .Where(fut => fut.UserId == userId && 
                             fut.FeatureId == featureId &&
                             fut.UsageType == usageType &&
                             fut.Status == Models.Enum.StatusEnum.Active &&
                             fut.UsageDate >= startDate && 
                             fut.UsageDate <= endDate)
                .SumAsync(fut => fut.ResourceAmount);
        }

        public async Task<List<FeatureUsageTracking>> GetUsageByAccount(long userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<FeatureUsageTracking>()
                .Include(fut => fut.Feature)
                .Where(fut => fut.UserId == userId &&
                             fut.Status == Models.Enum.StatusEnum.Active &&
                             fut.UsageDate >= startDate &&
                             fut.UsageDate <= endDate)
                .OrderByDescending(fut => fut.UsageDate)
                .ToListAsync();
        }
    }
}