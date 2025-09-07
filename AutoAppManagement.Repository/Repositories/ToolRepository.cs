using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Repository.Common.Repository;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IToolRepository : IBaseRepository<Tool>
    {
        Task<Tool?> GetByToolCodeAsync(string toolCode);
        Task<IEnumerable<Tool>> GetByCategoryAsync(string category);
        Task<IEnumerable<Tool>> GetByToolTypeAsync(string toolType);
        Task<bool> IsToolCodeExistsAsync(string toolCode, long? excludeId = null);
        Task<IEnumerable<Tool>> GetPublicToolsAsync();
        Task<IEnumerable<Tool>> SearchToolsAsync(string searchTerm);
        Task<Dictionary<string, int>> GetToolCountByCategoryAsync();
        Task<Dictionary<string, int>> GetToolCountByTypeAsync();
    }

    public class ToolRepository : BaseRepository<Tool>, IToolRepository
    {
        public ToolRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<Tool?> GetByToolCodeAsync(string toolCode)
        {
            return await FirstOrDefault(x => x.ToolCode == toolCode && !x.IsDeleted);
        }

        public async Task<IEnumerable<Tool>> GetByCategoryAsync(string category)
        {
            return await FindBy(x => x.Category == category && !x.IsDeleted);
        }

        public async Task<IEnumerable<Tool>> GetByToolTypeAsync(string toolType)
        {
            return await FindBy(x => x.ToolType == toolType && !x.IsDeleted);
        }

        public async Task<bool> IsToolCodeExistsAsync(string toolCode, long? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await CheckExitsByCondition(x => x.ToolCode == toolCode && x.Id != excludeId.Value && !x.IsDeleted);
            }
            return await CheckExitsByCondition(x => x.ToolCode == toolCode && !x.IsDeleted);
        }

        public async Task<IEnumerable<Tool>> GetPublicToolsAsync()
        {
            return await FindBy(x => x.IsPublic && x.Status == "Active" && !x.IsDeleted);
        }

        public async Task<IEnumerable<Tool>> SearchToolsAsync(string searchTerm)
        {
            return await FindBy(x => 
                (x.ToolName.Contains(searchTerm) || 
                 x.ToolCode.Contains(searchTerm) || 
                 x.Description!.Contains(searchTerm) ||
                 x.Category.Contains(searchTerm)) && 
                !x.IsDeleted);
        }

        public async Task<Dictionary<string, int>> GetToolCountByCategoryAsync()
        {
            return await _context.Set<Tool>()
                .Where(x => !x.IsDeleted)
                .GroupBy(x => x.Category)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetToolCountByTypeAsync()
        {
            return await _context.Set<Tool>()
                .Where(x => !x.IsDeleted)
                .GroupBy(x => x.ToolType)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }

    public interface IToolVersionRepository : IBaseRepository<ToolVersion>
    {
        Task<ToolVersion?> GetByToolAndVersionAsync(long toolId, string version);
        Task<IEnumerable<ToolVersion>> GetByToolIdAsync(long toolId);
        Task<ToolVersion?> GetLatestVersionAsync(long toolId);
        Task<IEnumerable<ToolVersion>> GetStableVersionsAsync(long toolId);
        Task<IEnumerable<ToolVersion>> GetSupportedVersionsAsync(long toolId);
        Task<bool> IsVersionExistsAsync(long toolId, string version, long? excludeId = null);
        Task<IEnumerable<ToolVersion>> GetVersionsReleasedAfterAsync(DateTime date);
        Task<IEnumerable<ToolVersion>> GetVersionsEndingSupportAsync(DateTime beforeDate);
    }

    public class ToolVersionRepository : BaseRepository<ToolVersion>, IToolVersionRepository
    {
        public ToolVersionRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<ToolVersion?> GetByToolAndVersionAsync(long toolId, string version)
        {
            return await FirstOrDefault(x => x.ToolId == toolId && x.Version == version && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolVersion>> GetByToolIdAsync(long toolId)
        {
            return await FindBy(x => x.ToolId == toolId && !x.IsDeleted);
        }

        public async Task<ToolVersion?> GetLatestVersionAsync(long toolId)
        {
            return await FirstOrDefault(x => x.ToolId == toolId && x.IsLatest && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolVersion>> GetStableVersionsAsync(long toolId)
        {
            return await FindBy(x => x.ToolId == toolId && x.IsStable && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolVersion>> GetSupportedVersionsAsync(long toolId)
        {
            return await FindBy(x => x.ToolId == toolId && x.IsSupported && !x.IsDeleted);
        }

        public async Task<bool> IsVersionExistsAsync(long toolId, string version, long? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await CheckExitsByCondition(x => x.ToolId == toolId && x.Version == version && x.Id != excludeId.Value && !x.IsDeleted);
            }
            return await CheckExitsByCondition(x => x.ToolId == toolId && x.Version == version && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolVersion>> GetVersionsReleasedAfterAsync(DateTime date)
        {
            return await FindBy(x => x.ReleaseDate >= date && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolVersion>> GetVersionsEndingSupportAsync(DateTime beforeDate)
        {
            return await FindBy(x => x.SupportEndDate.HasValue && x.SupportEndDate.Value <= beforeDate && !x.IsDeleted);
        }
    }

    public interface IToolCategoryRepository : IBaseRepository<ToolCategory>
    {
        Task<ToolCategory?> GetByCategoryCodeAsync(string categoryCode);
        Task<IEnumerable<ToolCategory>> GetRootCategoriesAsync();
        Task<IEnumerable<ToolCategory>> GetSubCategoriesAsync(long parentId);
        Task<bool> IsCategoryCodeExistsAsync(string categoryCode, long? excludeId = null);
        Task<IEnumerable<ToolCategory>> GetActiveCategoriesAsync();
        Task<Dictionary<long, int>> GetToolCountPerCategoryAsync();
    }

    public class ToolCategoryRepository : BaseRepository<ToolCategory>, IToolCategoryRepository
    {
        public ToolCategoryRepository(AutoAppManagementContext context) : base(context)
        {
        }

        public async Task<ToolCategory?> GetByCategoryCodeAsync(string categoryCode)
        {
            return await FirstOrDefault(x => x.CategoryCode == categoryCode && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolCategory>> GetRootCategoriesAsync()
        {
            return await FindBy(x => !x.ParentCategoryId.HasValue && x.IsActive && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolCategory>> GetSubCategoriesAsync(long parentId)
        {
            return await FindBy(x => x.ParentCategoryId == parentId && x.IsActive && !x.IsDeleted);
        }

        public async Task<bool> IsCategoryCodeExistsAsync(string categoryCode, long? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await CheckExitsByCondition(x => x.CategoryCode == categoryCode && x.Id != excludeId.Value && !x.IsDeleted);
            }
            return await CheckExitsByCondition(x => x.CategoryCode == categoryCode && !x.IsDeleted);
        }

        public async Task<IEnumerable<ToolCategory>> GetActiveCategoriesAsync()
        {
            return await FindBy(x => x.IsActive && !x.IsDeleted);
        }

        public async Task<Dictionary<long, int>> GetToolCountPerCategoryAsync()
        {
            return await _context.Set<Tool>()
                .Join(_context.Set<ToolCategory>(),
                    tool => tool.Category,
                    category => category.CategoryCode,
                    (tool, category) => new { CategoryId = category.Id, ToolId = tool.Id })
                .Where(x => !_context.Set<Tool>().First(t => t.Id == x.ToolId).IsDeleted &&
                           !_context.Set<ToolCategory>().First(c => c.Id == x.CategoryId).IsDeleted)
                .GroupBy(x => x.CategoryId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}
