using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Data.Models;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    /// <summary>
    /// Repository cho quản lý Features
    /// </summary>
    public interface IFeatureRepository : IBaseRepository<Feature>
    {
        Task<List<Feature>> GetActiveFeatures();
        Task<List<Feature>> GetFeaturesByCategory(string category);
        Task<Feature?> GetByCode(string code);
        Task<List<Feature>> GetFeaturesByIds(List<long> featureIds);
        Task<List<Feature>> GetFeaturesByCodes(List<string> featureCodes);
        Task<List<string>> GetDistinctCategories();
        Task<bool> IsFeatureCodeExists(string code, long? excludeId = null);
    }

    /// <summary>
    /// Implementation cho FeatureRepository
    /// </summary>
    public class FeatureRepository : BaseRepository<Feature>, IFeatureRepository
    {
        public FeatureRepository(AutoAppManagementContext context)
            : base(context) { }

        public async Task<List<Feature>> GetActiveFeatures()
        {
            return await _context.Set<Feature>()
                .Where(f => f.Status == Models.Enum.StatusEnum.Active)
                .OrderBy(f => f.PriorityOrder)
                .ThenBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<Feature?> GetByCode(string code)
        {
            return await _context.Set<Feature>()
                .FirstOrDefaultAsync(f => f.Code == code && f.Status == Models.Enum.StatusEnum.Active);
        }

        public async Task<List<Feature>> GetFeaturesByCategory(string category)
        {
            return await _context.Set<Feature>()
                .Where(f => f.Category == category && f.Status == Models.Enum.StatusEnum.Active)
                .OrderBy(f => f.PriorityOrder)
                .ThenBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<List<Feature>> GetFeaturesByIds(List<long> featureIds)
        {
            return await _context.Set<Feature>()
                .Where(f => featureIds.Contains(f.ID) && f.Status == Models.Enum.StatusEnum.Active)
                .ToListAsync();
        }

        public async Task<List<Feature>> GetFeaturesByCodes(List<string> featureCodes)
        {
            return await _context.Set<Feature>()
                .Where(f => featureCodes.Contains(f.Code) && f.Status == Models.Enum.StatusEnum.Active)
                .ToListAsync();
        }

        public async Task<List<string>> GetDistinctCategories()
        {
            return await _context.Set<Feature>()
                .Where(f => f.Status == Models.Enum.StatusEnum.Active && !string.IsNullOrEmpty(f.Category))
                .Select(f => f.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<bool> IsFeatureCodeExists(string code, long? excludeId = null)
        {
            var query = _context.Set<Feature>()
                .Where(f => f.Code == code && f.Status == Models.Enum.StatusEnum.Active);

            if (excludeId.HasValue)
            {
                query = query.Where(f => f.ID != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}