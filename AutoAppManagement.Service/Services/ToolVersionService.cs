using AutoMapper;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.ToolVersion;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutoAppManagement.Service.Services
{
    public interface IToolVersionService : IBaseBusinessService<ToolVersionDTO>
    {
        Task<CheckVersionResponse> CheckVersionAsync(CheckVersionRequest request);
        Task<ToolVersionDTO?> GetCurrentVersionAsync(string toolCode, string? platform = null);
        Task<IEnumerable<ToolVersionDTO>> GetVersionHistoryAsync(string toolCode, int limit = 10);
        Task<IEnumerable<ToolVersionDTO>> GetActiveVersionsAsync();
        Task<IEnumerable<ToolVersionDTO>> GetVersionsByCategoryAsync(string category);
        Task<IEnumerable<ToolVersionDTO>> GetVersionsByPlatformAsync(string platform);
        Task<IEnumerable<ToolVersionDTO>> GetRequiredUpdatesAsync();
        Task<RestOutput> CreateVersionAsync(CreateToolVersionRequest request);
        Task<RestOutput> UpdateVersionAsync(UpdateToolVersionRequest request);
        Task<RestOutput> ActivateVersionAsync(long id);
        Task<RestOutput> DeactivateVersionAsync(long id);
        Task<bool> IsUpdateAvailableAsync(string toolCode, string currentVersion, string? platform = null);
        Task<IEnumerable<VersionHistory>> GetFullVersionHistoryAsync(string toolCode);
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
        /// Kiểm tra version và trả về thông tin update nếu có
        /// </summary>
        public async Task<CheckVersionResponse> CheckVersionAsync(CheckVersionRequest request)
        {
            try
            {
                // Try to get from cache first
                var cacheKey = $"{CACHE_KEY_PREFIX}Check:{request.ToolCode}:{request.Platform ?? "All"}";
                var cachedResponse = await _cache.GetAsync<CheckVersionResponse>(cacheKey);
                
                if (cachedResponse != null && !string.IsNullOrEmpty(request.CurrentVersion))
                {
                    // Update the comparison based on the current version
                    cachedResponse.UpdateAvailable = IsNewerVersion(cachedResponse.LatestVersion, request.CurrentVersion);
                    cachedResponse.UpdateRequired = cachedResponse.UpdateRequired && 
                        !MeetsMinimumVersion(request.CurrentVersion, cachedResponse.MinimumVersion);
                    return cachedResponse;
                }

                // Get latest version from database
                var latestVersion = await Repository.GetActiveVersionAsync(request.ToolCode, request.Platform);
                
                if (latestVersion == null)
                {
                    return new CheckVersionResponse
                    {
                        UpdateAvailable = false,
                        UpdateRequired = false,
                        Message = $"No version information available for tool: {request.ToolCode}"
                    };
                }

                var response = new CheckVersionResponse
                {
                    LatestVersion = latestVersion.CurrentVersion,
                    MinimumVersion = latestVersion.MinimumVersion,
                    DownloadUrl = latestVersion.DownloadUrl,
                    ReleaseNotes = latestVersion.ReleaseNotes,
                    ReleaseDate = latestVersion.ReleaseDate,
                    FileSize = latestVersion.FileSize,
                    Checksum = latestVersion.Checksum,
                    Features = latestVersion.GetFeatures(),
                    BugFixes = latestVersion.GetBugFixes(),
                    UpdateAvailable = IsNewerVersion(latestVersion.CurrentVersion, request.CurrentVersion),
                    UpdateRequired = latestVersion.IsRequired && !MeetsMinimumVersion(request.CurrentVersion, latestVersion.MinimumVersion)
                };

                if (response.UpdateAvailable)
                {
                    response.Message = response.UpdateRequired 
                        ? "A required update is available. Please update immediately."
                        : "A new version is available.";
                }
                else
                {
                    response.Message = "You are using the latest version.";
                }

                // Cache the response
                await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking version for tool: {ToolCode}", request.ToolCode);
                throw;
            }
        }

        /// <summary>
        /// Lấy version hiện tại của tool
        /// </summary>
        public async Task<ToolVersionDTO?> GetCurrentVersionAsync(string toolCode, string? platform = null)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}Current:{toolCode}:{platform ?? "All"}";
                var cached = await _cache.GetAsync<ToolVersionDTO>(cacheKey);
                
                if (cached != null)
                    return cached;

                var version = await Repository.GetActiveVersionAsync(toolCode, platform);
                
                if (version == null)
                    return null;

                var dto = Mapper.Map<ToolVersionDTO>(version);
                dto.Features = version.GetFeatures();
                dto.BugFixes = version.GetBugFixes();
                dto.IsLatest = true;

                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES));
                
                return dto;
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
        public async Task<IEnumerable<ToolVersionDTO>> GetVersionHistoryAsync(string toolCode, int limit = 10)
        {
            try
            {
                var versions = await Repository.GetByToolCodeHistoryAsync(toolCode, limit);
                var dtos = new List<ToolVersionDTO>();

                foreach (var version in versions)
                {
                    var dto = Mapper.Map<ToolVersionDTO>(version);
                    dto.Features = version.GetFeatures();
                    dto.BugFixes = version.GetBugFixes();
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
                    dto.Features = v.GetFeatures();
                    dto.BugFixes = v.GetBugFixes();
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
        /// Lấy versions theo category
        /// </summary>
        public async Task<IEnumerable<ToolVersionDTO>> GetVersionsByCategoryAsync(string category)
        {
            try
            {
                var versions = await Repository.GetVersionsByCategoryAsync(category);
                return versions.Select(v =>
                {
                    var dto = Mapper.Map<ToolVersionDTO>(v);
                    dto.Features = v.GetFeatures();
                    dto.BugFixes = v.GetBugFixes();
                    return dto;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting versions by category: {Category}", category);
                throw;
            }
        }

        /// <summary>
        /// Lấy versions theo platform
        /// </summary>
        public async Task<IEnumerable<ToolVersionDTO>> GetVersionsByPlatformAsync(string platform)
        {
            try
            {
                var versions = await Repository.GetVersionsByPlatformAsync(platform);
                return versions.Select(v =>
                {
                    var dto = Mapper.Map<ToolVersionDTO>(v);
                    dto.Features = v.GetFeatures();
                    dto.BugFixes = v.GetBugFixes();
                    return dto;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting versions by platform: {Platform}", platform);
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
                    dto.Features = v.GetFeatures();
                    dto.BugFixes = v.GetBugFixes();
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
        /// Tạo version mới
        /// </summary>
        public async Task<RestOutput> CreateVersionAsync(CreateToolVersionRequest request)
        {
            try
            {
                // Check if version already exists
                var exists = await Repository.IsVersionExistsAsync(request.ToolCode, request.CurrentVersion);
                if (exists)
                {
                ResOutput.ErrorEventHandler(message: $"Version {request.CurrentVersion} already exists for tool {request.ToolCode}");
                return (RestOutput)ResOutput;
                }

                // Deactivate other versions if this is active
                if (request.IsActive)
                {
                        var existingVersions = await Repository.GetByCondition(v => v.ToolCode == request.ToolCode && v.IsActive);
                        foreach (var existing in existingVersions)
                        {
                            existing.IsActive = false;
                            Repository.Update(existing);
                        }
                }

                var entity = new ToolVersion
                {
                    ToolCode = request.ToolCode,
                    ToolName = request.ToolName,
                    CurrentVersion = request.CurrentVersion,
                    MinimumVersion = request.MinimumVersion,
                    Description = request.Description,
                    DownloadUrl = request.DownloadUrl,
                    ReleaseNotes = request.ReleaseNotes,
                    ReleaseDate = request.ReleaseDate,
                    IsActive = request.IsActive,
                    IsRequired = request.IsRequired,
                    Platform = request.Platform,
                    FileSize = request.FileSize,
                    Checksum = request.Checksum,
                    Features = request.Features != null ? JsonSerializer.Serialize(request.Features) : null,
                    BugFixes = request.BugFixes != null ? JsonSerializer.Serialize(request.BugFixes) : null,
                    Category = request.Category,
                    Priority = request.Priority,
                    CreatedDate = DateTime.UtcNow
                };

                await Repository.CreateAsync(entity);
                await UnitOfWork.SaveChangeAsync();

                // Clear cache
                await ClearVersionCache(request.ToolCode);

                var dto = Mapper.Map<ToolVersionDTO>(entity);
                dto.Features = entity.GetFeatures();
                dto.BugFixes = entity.GetBugFixes();

                ResOutput.SuccessEventHandler(dto, "Version created successfully");
                return (RestOutput)ResOutput;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating version");
                ResOutput.ErrorEventHandler(message: "Error creating version: " + ex.Message);
                return (RestOutput)ResOutput;
            }
        }

        /// <summary>
        /// Cập nhật version
        /// </summary>
        public async Task<RestOutput> UpdateVersionAsync(UpdateToolVersionRequest request)
        {
            try
            {
                var entity = await Repository.FirstOrDefault(e => e.ID == request.Id);
                if (entity == null)
                {
                    ResOutput.ErrorEventHandler(message: "Version not found");
                    return (RestOutput)ResOutput;
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.CurrentVersion))
                {
                    // Check if new version already exists
                    if (request.CurrentVersion != entity.CurrentVersion)
                    {
                        var exists = await Repository.IsVersionExistsAsync(entity.ToolCode, request.CurrentVersion);
                        if (exists)
                        {
                            ResOutput.ErrorEventHandler(message: $"Version {request.CurrentVersion} already exists for this tool");
                            return (RestOutput)ResOutput;
                        }
                    }
                    entity.CurrentVersion = request.CurrentVersion;
                }

                if (!string.IsNullOrEmpty(request.MinimumVersion))
                    entity.MinimumVersion = request.MinimumVersion;
                
                if (!string.IsNullOrEmpty(request.Description))
                    entity.Description = request.Description;
                
                if (!string.IsNullOrEmpty(request.DownloadUrl))
                    entity.DownloadUrl = request.DownloadUrl;
                
                if (!string.IsNullOrEmpty(request.ReleaseNotes))
                    entity.ReleaseNotes = request.ReleaseNotes;
                
                if (request.ReleaseDate.HasValue)
                    entity.ReleaseDate = request.ReleaseDate.Value;
                
                if (request.IsActive.HasValue)
                {
                    // If activating, deactivate others
                    if (request.IsActive.Value && !entity.IsActive)
                    {
                        var existingVersions = await Repository.GetByCondition(v => v.ToolCode == entity.ToolCode && v.IsActive && v.ID != entity.ID);
                        foreach (var existing in existingVersions)
                        {
                            existing.IsActive = false;
                            Repository.Update(existing);
                        }
                    }
                    entity.IsActive = request.IsActive.Value;
                }

                if (request.IsRequired.HasValue)
                    entity.IsRequired = request.IsRequired.Value;
                
                if (!string.IsNullOrEmpty(request.Platform))
                    entity.Platform = request.Platform;
                
                if (request.FileSize.HasValue)
                    entity.FileSize = request.FileSize.Value;
                
                if (!string.IsNullOrEmpty(request.Checksum))
                    entity.Checksum = request.Checksum;
                
                if (request.Features != null)
                    entity.Features = JsonSerializer.Serialize(request.Features);
                
                if (request.BugFixes != null)
                    entity.BugFixes = JsonSerializer.Serialize(request.BugFixes);
                
                if (!string.IsNullOrEmpty(request.Category))
                    entity.Category = request.Category;
                
                if (request.Priority.HasValue)
                    entity.Priority = request.Priority.Value;

                entity.UpdatedDate = DateTime.UtcNow;

                Repository.Update(entity);
                await UnitOfWork.SaveChangeAsync();

                // Clear cache
                await ClearVersionCache(entity.ToolCode);

                var dto = Mapper.Map<ToolVersionDTO>(entity);
                dto.Features = entity.GetFeatures();
                dto.BugFixes = entity.GetBugFixes();

                ResOutput.SuccessEventHandler(dto, "Version updated successfully");
                return (RestOutput)ResOutput;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating version");
                ResOutput.ErrorEventHandler(message: "Error updating version: " + ex.Message);
                return (RestOutput)ResOutput;
            }
        }

        /// <summary>
        /// Activate version
        /// </summary>
        public async Task<RestOutput> ActivateVersionAsync(long id)
        {
            try
            {
                var entity = await Repository.FirstOrDefault(e => e.ID == id);
                if (entity == null)
                {
                    ResOutput.ErrorEventHandler(message: "Version not found");
                    return (RestOutput)ResOutput;
                }

                // Deactivate other versions
                var existingVersions = await Repository.GetByCondition(v => v.ToolCode == entity.ToolCode && v.IsActive && v.ID != entity.ID);
                foreach (var existing in existingVersions)
                {
                    existing.IsActive = false;
                    Repository.Update(existing);
                }

                entity.IsActive = true;
                entity.UpdatedDate = DateTime.UtcNow;

                Repository.Update(entity);
                await UnitOfWork.SaveChangeAsync();

                // Clear cache
                await ClearVersionCache(entity.ToolCode);

                ResOutput.SuccessEventHandler(message: "Version activated successfully");
                return (RestOutput)ResOutput;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating version");
                ResOutput.ErrorEventHandler(message: "Error activating version: " + ex.Message);
                return (RestOutput)ResOutput;
            }
        }

        /// <summary>
        /// Deactivate version
        /// </summary>
        public async Task<RestOutput> DeactivateVersionAsync(long id)
        {
            try
            {
                var entity = await Repository.FirstOrDefault(e => e.ID == id);
                if (entity == null)
                {
                    ResOutput.ErrorEventHandler(message: "Version not found");
                    return (RestOutput)ResOutput;
                }

                entity.IsActive = false;
                entity.UpdatedDate = DateTime.UtcNow;

                Repository.Update(entity);
                await UnitOfWork.SaveChangeAsync();

                // Clear cache
                await ClearVersionCache(entity.ToolCode);

                ResOutput.SuccessEventHandler(message: "Version deactivated successfully");
                return (RestOutput)ResOutput;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating version");
                ResOutput.ErrorEventHandler(message: "Error deactivating version: " + ex.Message);
                return (RestOutput)ResOutput;
            }
        }

        /// <summary>
        /// Kiểm tra có update mới không
        /// </summary>
        public async Task<bool> IsUpdateAvailableAsync(string toolCode, string currentVersion, string? platform = null)
        {
            try
            {
                var latestVersion = await Repository.GetActiveVersionAsync(toolCode, platform);
                if (latestVersion == null)
                    return false;

                return IsNewerVersion(latestVersion.CurrentVersion, currentVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking update availability");
                return false;
            }
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử version
        /// </summary>
        public async Task<IEnumerable<VersionHistory>> GetFullVersionHistoryAsync(string toolCode)
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
                        Features = v.GetFeatures(),
                        BugFixes = v.GetBugFixes(),
                        FileSize = v.FileSize,
                        DownloadUrl = v.DownloadUrl
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting full version history");
                throw;
            }
        }

        #region Helper Methods

        private bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            try
            {
                var latest = new Version(latestVersion);
                var current = new Version(currentVersion);
                return latest > current;
            }
            catch
            {
                // If version parsing fails, do string comparison
                return string.Compare(latestVersion, currentVersion, StringComparison.OrdinalIgnoreCase) > 0;
            }
        }

        private bool MeetsMinimumVersion(string currentVersion, string? minimumVersion)
        {
            if (string.IsNullOrEmpty(minimumVersion))
                return true;

            try
            {
                var current = new Version(currentVersion);
                var minimum = new Version(minimumVersion);
                return current >= minimum;
            }
            catch
            {
                // If version parsing fails, do string comparison
                return string.Compare(currentVersion, minimumVersion, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private async Task ClearVersionCache(string toolCode)
        {
            try
            {
                var keys = new[]
                {
                    $"{CACHE_KEY_PREFIX}Current:{toolCode}:All",
                    $"{CACHE_KEY_PREFIX}Current:{toolCode}:Windows",
                    $"{CACHE_KEY_PREFIX}Current:{toolCode}:MacOS",
                    $"{CACHE_KEY_PREFIX}Current:{toolCode}:Linux",
                    $"{CACHE_KEY_PREFIX}Check:{toolCode}:All",
                    $"{CACHE_KEY_PREFIX}Check:{toolCode}:Windows",
                    $"{CACHE_KEY_PREFIX}Check:{toolCode}:MacOS",
                    $"{CACHE_KEY_PREFIX}Check:{toolCode}:Linux"
                };

                foreach (var key in keys)
                {
                    await _cache.RemoveAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error clearing cache for tool: {ToolCode}", toolCode);
            }
        }

        #endregion
    }
}
