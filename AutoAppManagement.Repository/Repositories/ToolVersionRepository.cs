using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Data.Models;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IToolVersionRepository : IBaseRepository<ToolVersion>
    {
        Task<ToolVersion?> GetByToolCodeAsync(string toolCode, string? platform = null);
        Task<IEnumerable<ToolVersion>> GetByToolCodeHistoryAsync(string toolCode, int limit = 10);
        Task<ToolVersion?> GetActiveVersionAsync(string toolCode, string? platform = null);
        Task<IEnumerable<ToolVersion>> GetAllActiveVersionsAsync();
        Task<bool> IsVersionExistsAsync(string toolCode, string version);
        Task<IEnumerable<ToolVersion>> GetVersionsByCategoryAsync(string category);
        Task<IEnumerable<ToolVersion>> GetVersionsByPlatformAsync(string platform);
        Task<ToolVersion?> GetLatestVersionAsync(string toolCode, string? platform = null);
        Task<IEnumerable<ToolVersion>> GetRequiredUpdatesAsync();
        void Update(ToolVersion entity);
    }

    public class ToolVersionRepository : BaseRepository<ToolVersion>, IToolVersionRepository
    {
        public ToolVersionRepository(AutoAppManagementContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy version hiện tại của tool theo code
        /// </summary>
        public async Task<ToolVersion?> GetByToolCodeAsync(string toolCode, string? platform = null)
        {
            var query = _context.Set<ToolVersion>()
                .Where(tv => tv.ToolCode == toolCode && tv.IsActive);

            if (!string.IsNullOrEmpty(platform))
            {
                query = query.Where(tv => tv.Platform == platform || tv.Platform == null || tv.Platform == "All");
            }

            return await query
                .OrderByDescending(tv => tv.Priority)
                .ThenByDescending(tv => tv.ReleaseDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy lịch sử version của tool
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetByToolCodeHistoryAsync(string toolCode, int limit = 10)
        {
            return await _context.Set<ToolVersion>()
                .Where(tv => tv.ToolCode == toolCode)
                .OrderByDescending(tv => tv.ReleaseDate)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy version đang active của tool
        /// </summary>
        public async Task<ToolVersion?> GetActiveVersionAsync(string toolCode, string? platform = null)
        {
            var query = _context.Set<ToolVersion>()
                .Where(tv => tv.ToolCode == toolCode && tv.IsActive);

            if (!string.IsNullOrEmpty(platform))
            {
                query = query.Where(tv => tv.Platform == platform || tv.Platform == null || tv.Platform == "All");
            }

            return await query
                .OrderByDescending(tv => tv.ReleaseDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy tất cả version đang active
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetAllActiveVersionsAsync()
        {
            return await _context.Set<ToolVersion>()
                .Where(tv => tv.IsActive)
                .OrderBy(tv => tv.ToolName)
                .ThenByDescending(tv => tv.ReleaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Kiểm tra version đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsVersionExistsAsync(string toolCode, string version)
        {
            return await _context.Set<ToolVersion>()
                .AnyAsync(tv => tv.ToolCode == toolCode && tv.CurrentVersion == version);
        }

        /// <summary>
        /// Lấy versions theo category
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetVersionsByCategoryAsync(string category)
        {
            return await _context.Set<ToolVersion>()
                .Where(tv => tv.Category == category && tv.IsActive)
                .OrderBy(tv => tv.ToolName)
                .ThenByDescending(tv => tv.ReleaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy versions theo platform
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetVersionsByPlatformAsync(string platform)
        {
            return await _context.Set<ToolVersion>()
                .Where(tv => (tv.Platform == platform || tv.Platform == null || tv.Platform == "All") && tv.IsActive)
                .OrderBy(tv => tv.ToolName)
                .ThenByDescending(tv => tv.ReleaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy version mới nhất của tool
        /// </summary>
        public async Task<ToolVersion?> GetLatestVersionAsync(string toolCode, string? platform = null)
        {
            var query = _context.Set<ToolVersion>()
                .Where(tv => tv.ToolCode == toolCode);

            if (!string.IsNullOrEmpty(platform))
            {
                query = query.Where(tv => tv.Platform == platform || tv.Platform == null || tv.Platform == "All");
            }

            return await query
                .OrderByDescending(tv => tv.ReleaseDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy danh sách updates bắt buộc
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetRequiredUpdatesAsync()
        {
            return await _context.Set<ToolVersion>()
                .Where(tv => tv.IsActive && tv.IsRequired)
                .OrderByDescending(tv => tv.Priority)
                .ThenBy(tv => tv.ToolName)
                .ToListAsync();
        }

        /// <summary>
        /// Override để thêm sorting mặc định
        /// </summary>
        public override async Task<IEnumerable<ToolVersion>> GetAll()
        {
            return await _context.Set<ToolVersion>()
                .OrderBy(tv => tv.ToolName)
                .ThenByDescending(tv => tv.ReleaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Update tool version entity
        /// </summary>
        /// <param name="entity"></param>
        public void Update(ToolVersion entity)
        {
            _context.Set<ToolVersion>().Update(entity);
        }

    }
}
