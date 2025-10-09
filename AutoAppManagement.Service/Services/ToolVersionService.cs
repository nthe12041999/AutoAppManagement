using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.ToolVersion;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.Logging;

namespace AutoAppManagement.Service.Services
{
    public interface IToolVersionService : IBaseBusinessService<ToolVersionDTO>
    {
        Task<CheckVersionResponse?> GetCurrentVersionAsync(ToolCode toolCode);
        Task<IEnumerable<ToolVersionDTO>> GetVersionHistoryAsync(ToolCode toolCode, int limit = 10);
        Task<IEnumerable<ToolVersionDTO>> GetActiveVersionsAsync();
        Task<IEnumerable<ToolVersionDTO>> GetRequiredUpdatesAsync();
        Task<IEnumerable<VersionHistory>> GetFullVersionHistoryAsync(ToolCode toolCode);
    }

    public class ToolVersionService : BaseBusinessService<ToolVersion, ToolVersionDTO, IToolVersionRepository>, IToolVersionService
    {
        private readonly IDistributedCacheCustom _cache;
        private readonly ILogger<ToolVersionService> _logger;
        private const string CACHE_KEY_PREFIX = "ToolVersion:";
        private const int CACHE_EXPIRATION_MINUTES = 30;

        public ToolVersionService(
            IServiceProvider serviceProvider,
            IDistributedCacheCustom cache,
            ILogger<ToolVersionService> logger)
            : base(serviceProvider)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Lấy version hiện tại của tool
        /// </summary>
        public async Task<CheckVersionResponse?> GetCurrentVersionAsync(ToolCode toolCode)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}Current:{toolCode}";
                var cached = await _cache.GetAsync<CheckVersionResponse>(cacheKey);
                
                if (cached != null)
                    return cached;

                var version = await Repository.GetActiveVersionAsync(toolCode);
                
                if (version == null)
                    return null;

                var response = new CheckVersionResponse
                {
                    LastestVersion = version.CurrentVersion,
                    Description = version.Description,
                    ReleaseDate = version.ReleaseDate,
                    FileSize = version.FileSize,
                    Checksum = version.Checksum
                };

                await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES));
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current version for tool: {ToolCode}", toolCode);
                throw;
            }
        }

        /// <summary>
        /// Lấy lịch sử version của tool
        /// </summary>
        public async Task<IEnumerable<ToolVersionDTO>> GetVersionHistoryAsync(ToolCode toolCode, int limit = 10)
        {
            try
            {
                var versions = await Repository.GetByToolCodeHistoryAsync(toolCode, limit);
                var dtos = new List<ToolVersionDTO>();

                foreach (var version in versions)
                {
                    var dto = Mapper.Map<ToolVersionDTO>(version);
                    dtos.Add(dto);
                }

                // Mark the first one as latest
                if (dtos.Any())
                    dtos.First().IsLatest = true;

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version history for tool: {ToolCode}", toolCode);
                throw;
            }
        }

        /// <summary>
        /// Lấy tất cả version đang active
        /// </summary>
        public async Task<IEnumerable<ToolVersionDTO>> GetActiveVersionsAsync()
        {
            try
            {
                var versions = await Repository.GetAllActiveVersionsAsync();
                return versions.Select(v =>
                {
                    var dto = Mapper.Map<ToolVersionDTO>(v);
                    return dto;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active versions");
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách updates bắt buộc
        /// </summary>
        public async Task<IEnumerable<ToolVersionDTO>> GetRequiredUpdatesAsync()
        {
            try
            {
                var versions = await Repository.GetRequiredUpdatesAsync();
                return versions.Select(v =>
                {
                    var dto = Mapper.Map<ToolVersionDTO>(v);
                    return dto;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required updates");
                throw;
            }
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử version
        /// </summary>
        public async Task<IEnumerable<VersionHistory>> GetFullVersionHistoryAsync(ToolCode toolCode)
        {
            try
            {
                var versions = await Repository.GetByCondition(v => v.ToolCode == toolCode);
                return versions
                    .OrderByDescending(v => v.ReleaseDate)
                    .Select(v => new VersionHistory
                    {
                        Version = v.CurrentVersion,
                        ReleaseDate = v.ReleaseDate,
                        Description = v.Description,
                        FileSize = v.FileSize
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting full version history");
                throw;
            }
        }

        #region Helper Methods

        #endregion
    }
}
