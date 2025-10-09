using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Data.Models;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IToolVersionRepository : IBaseRepository<ToolVersion>
    {
        Task<ToolVersion?> GetByToolCodeAsync(ToolCode toolCode);
        Task<IEnumerable<ToolVersion>> GetByToolCodeHistoryAsync(ToolCode toolCode, int limit = 10);
        Task<ToolVersion?> GetActiveVersionAsync(ToolCode toolCode);
        Task<IEnumerable<ToolVersion>> GetAllActiveVersionsAsync();
        Task<bool> IsVersionExistsAsync(ToolCode toolCode, string version);
        Task<ToolVersion?> GetLatestVersionAsync(ToolCode toolCode);
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
        public async Task<ToolVersion?> GetByToolCodeAsync(ToolCode toolCode)
        {
            var query = _context.ToolVersions
                .Where(tv => tv.ToolCode == toolCode);

            return await query
                .OrderByDescending(tv => tv.ReleaseDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy lịch sử version của tool
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetByToolCodeHistoryAsync(ToolCode toolCode, int limit = 10)
        {
            return await _context.ToolVersions
                .Where(tv => tv.ToolCode == toolCode)
                .OrderByDescending(tv => tv.ReleaseDate)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy version đang active của tool
        /// </summary>
        public async Task<ToolVersion?> GetActiveVersionAsync(ToolCode toolCode)
        {
            var query = await GetByCondition(tv => tv.ToolCode == toolCode && tv.Status == StatusEnum.Active);

            return query.OrderByDescending(tv => tv.ReleaseDate).FirstOrDefault();
        }

        /// <summary>
        /// Lấy tất cả version đang active
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetAllActiveVersionsAsync()
        {
            return await _context.ToolVersions
                .Where(tv => tv.Status == StatusEnum.Active)
                .OrderBy(tv => tv.ToolCode)
                .ThenByDescending(tv => tv.ReleaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Kiểm tra version đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsVersionExistsAsync(ToolCode toolCode, string version)
        {
            return await _context.ToolVersions
                .AnyAsync(tv => tv.ToolCode == toolCode && tv.CurrentVersion == version);
        }

        /// <summary>
        /// Lấy version mới nhất của tool
        /// </summary>
        public async Task<ToolVersion?> GetLatestVersionAsync(ToolCode toolCode)
        {
            var query = _context.ToolVersions
                .Where(tv => tv.ToolCode == toolCode && tv.Status == StatusEnum.Active);

            return await query
                .OrderByDescending(tv => tv.ReleaseDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy danh sách updates bắt buộc
        /// </summary>
        public async Task<IEnumerable<ToolVersion>> GetRequiredUpdatesAsync()
        {
            return await _context.ToolVersions
                .Where(tv => tv.Status == StatusEnum.Active && tv.IsRequired)
                .OrderByDescending(tv => tv.ToolCode)
                .ToListAsync();
        }

        /// <summary>
        /// Override để thêm sorting mặc định
        /// </summary>
        public override async Task<IEnumerable<ToolVersion>> GetAll()
        {
            return await _context.ToolVersions
                .OrderBy(tv => tv.ToolCode)
                .ThenByDescending(tv => tv.ReleaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Update tool version entity
        /// </summary>
        /// <param name="entity"></param>
        public void Update(ToolVersion entity)
        {
            _context.ToolVersions.Update(entity);
        }
    }
}
