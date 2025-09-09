using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Tool;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using static AutoAppManagement.Models.Enum.DataModelType;
using Microsoft.Extensions.Logging;

namespace AutoAppManagement.Service.Services
{
    public interface IToolService : IBaseBusinessService<ToolDTO>
    {
        Task<ToolDTO?> GetByToolCodeAsync(string toolCode);
        Task<IEnumerable<ToolDTO>> GetByCategoryAsync(string category);
        Task<IEnumerable<ToolDTO>> GetByToolTypeAsync(string toolType);
        Task<bool> IsToolCodeExistsAsync(string toolCode, long? excludeId = null);
        Task<IEnumerable<ToolDTO>> GetPublicToolsAsync();
        Task<PagingResultDTO<ToolDTO>> SearchToolsAsync(ToolSearchRequest request);
        Task<ToolStatisticsDTO> GetToolStatisticsAsync();
        Task<ToolDTO> CreateToolAsync(ToolDTO request);
        Task<ToolDTO> UpdateToolAsync(ToolDTO request);
        Task<bool> DeleteToolAsync(long id);
    }

    public class ToolService : BaseBusinessService<Tool, ToolDTO, IToolRepository>, IToolService
    {
        private readonly IToolRepository _toolRepository;
        private readonly IToolVersionRepository _toolVersionRepository;
        private readonly ILogger<ToolService> _logger;

        public ToolService(
            IServiceProvider serviceProvider,
            IToolRepository toolRepository,
            IToolVersionRepository toolVersionRepository,
            ILogger<ToolService> logger) : base(serviceProvider)
        {
            _toolRepository = toolRepository;
            _toolVersionRepository = toolVersionRepository;
            _logger = logger;
        }

        protected override IToolRepository GetRepository()
        {
            return _toolRepository;
        }

        public async Task<ToolDTO?> GetByToolCodeAsync(string toolCode)
        {
            try
            {
                var tool = await _toolRepository.GetByToolCodeAsync(toolCode);
                return Mapper.Map<ToolDTO>(tool);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool by code: {ToolCode}", toolCode);
                throw;
            }
        }

        public async Task<IEnumerable<ToolDTO>> GetByCategoryAsync(string category)
        {
            try
            {
                var tools = await _toolRepository.GetByCategoryAsync(category);
                return Mapper.Map<IEnumerable<ToolDTO>>(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tools by category: {Category}", category);
                throw;
            }
        }

        public async Task<IEnumerable<ToolDTO>> GetByToolTypeAsync(string toolType)
        {
            try
            {
                var toolTypeEnum = Enum.Parse<ToolType>(toolType, true);
                var tools = await _toolRepository.GetByToolTypeAsync(toolTypeEnum);
                return Mapper.Map<IEnumerable<ToolDTO>>(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tools by type: {ToolType}", toolType);
                throw;
            }
        }

        public async Task<bool> IsToolCodeExistsAsync(string toolCode, long? excludeId = null)
        {
            try
            {
                return await _toolRepository.IsToolCodeExistsAsync(toolCode, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking tool code exists: {ToolCode}", toolCode);
                throw;
            }
        }

        public async Task<IEnumerable<ToolDTO>> GetPublicToolsAsync()
        {
            try
            {
                var tools = await _toolRepository.GetPublicToolsAsync();
                return Mapper.Map<IEnumerable<ToolDTO>>(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public tools");
                throw;
            }
        }

        public async Task<PagingResultDTO<ToolDTO>> SearchToolsAsync(ToolSearchRequest request)
        {
            try
            {
                var allTools = await _toolRepository.SearchToolsAsync(request.SearchTerm ?? "");
                
                // Apply filters
                if (!string.IsNullOrEmpty(request.Category))
                    allTools = allTools.Where(x => x.Category == request.Category);
                
                if (!string.IsNullOrEmpty(request.ToolType))
                {
                    var toolTypeEnum = Enum.Parse<ToolType>(request.ToolType, true);
                    allTools = allTools.Where(x => x.ToolType == toolTypeEnum);
                }
                
                if (!string.IsNullOrEmpty(request.Status))
                {
                    var statusEnum = Enum.Parse<StatusType>(request.Status, true);
                    allTools = allTools.Where(x => x.Status == statusEnum);
                }
                
                if (request.IsPublic.HasValue)
                    allTools = allTools.Where(x => x.IsPublic == request.IsPublic.Value);

                var totalCount = allTools.Count();
                
                // Apply pagination
                var pagedTools = allTools
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new PagingResultDTO<ToolDTO>
                {
                    Data = Mapper.Map<List<ToolDTO>>(pagedTools),
                    TotalItems = totalCount,
                    PageIndex = request.Page,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tools");
                throw;
            }
        }

        public async Task<ToolStatisticsDTO> GetToolStatisticsAsync()
        {
            try
            {
                var allTools = await _toolRepository.GetAll();
                var categoryStats = await _toolRepository.GetToolCountByCategoryAsync();
                var typeStats = await _toolRepository.GetToolCountByTypeAsync();

                return new ToolStatisticsDTO
                {
                    TotalTools = allTools.Count(),
                    ActiveTools = allTools.Count(x => x.Status == StatusType.Active),
                    PublicTools = allTools.Count(x => x.IsPublic),
                    PrivateTools = allTools.Count(x => !x.IsPublic),
                    ToolsByCategory = categoryStats,
                    ToolsByType = typeStats
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool statistics");
                throw;
            }
        }

        public async Task<ToolDTO> CreateToolAsync(ToolDTO request)
        {
            try
            {
                // Check if tool code exists
                if (await IsToolCodeExistsAsync(request.ToolCode))
                {
                    throw new InvalidOperationException("Tool code already exists");
                }

                var tool = Mapper.Map<Tool>(request);
                tool.CreatedDate = DateTime.UtcNow;
                tool.Status = StatusType.Active;

                await _toolRepository.CreateAsync(tool);
                await UnitOfWork.SaveAsync();
                
                return Mapper.Map<ToolDTO>(tool);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tool");
                throw;
            }
        }

        public async Task<ToolDTO> UpdateToolAsync(ToolDTO request)
        {
            try
            {
                var existingTool = await _toolRepository.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (existingTool == null)
                {
                    throw new InvalidOperationException("Tool not found");
                }

                // Check if tool code exists (excluding current tool)
                if (await IsToolCodeExistsAsync(request.ToolCode, request.Id))
                {
                    throw new InvalidOperationException("Tool code already exists");
                }

                Mapper.Map(request, existingTool);
                existingTool.UpdatedDate = DateTime.UtcNow;

                await UnitOfWork.SaveAsync();

                return Mapper.Map<ToolDTO>(existingTool);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tool");
                throw;
            }
        }

        public async Task<bool> DeleteToolAsync(long id)
        {
            try
            {
                var tool = await _toolRepository.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
                if (tool == null)
                    return false;

                tool.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tool: {ToolId}", id);
                throw;
            }
        }
    }

    public interface IToolVersionService : IBaseBusinessService<ToolVersionDTO>
    {
        Task<ToolVersionDTO?> GetByToolAndVersionAsync(long toolId, string version);
        Task<IEnumerable<ToolVersionDTO>> GetByToolIdAsync(long toolId);
        Task<ToolVersionDTO?> GetLatestVersionAsync(long toolId);
        Task<IEnumerable<ToolVersionDTO>> GetStableVersionsAsync(long toolId);
        Task<IEnumerable<ToolVersionDTO>> GetSupportedVersionsAsync(long toolId);
        Task<bool> IsVersionExistsAsync(long toolId, string version, long? excludeId = null);
        Task<ToolVersionComparisonDTO> CompareVersionsAsync(long version1Id, long version2Id);
        Task<ToolVersionDTO> CreateVersionAsync(ToolVersionDTO request);
        Task<ToolVersionDTO> UpdateVersionAsync(ToolVersionDTO request);
    }

    public class ToolVersionService : BaseBusinessService<ToolVersion, ToolVersionDTO, IToolVersionRepository>, IToolVersionService
    {
        private readonly IToolVersionRepository _toolVersionRepository;
        private readonly ILogger<ToolVersionService> _logger;

        public ToolVersionService(
            IServiceProvider serviceProvider,
            IToolVersionRepository toolVersionRepository,
            ILogger<ToolVersionService> logger) : base(serviceProvider)
        {
            _toolVersionRepository = toolVersionRepository;
            _logger = logger;
        }

        protected override IToolVersionRepository GetRepository()
        {
            return _toolVersionRepository;
        }

        public async Task<ToolVersionDTO?> GetByToolAndVersionAsync(long toolId, string version)
        {
            try
            {
                var toolVersion = await _toolVersionRepository.GetByToolAndVersionAsync(toolId, version);
                return Mapper.Map<ToolVersionDTO>(toolVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool version: {ToolId}, {Version}", toolId, version);
                throw;
            }
        }

        public async Task<IEnumerable<ToolVersionDTO>> GetByToolIdAsync(long toolId)
        {
            try
            {
                var versions = await _toolVersionRepository.GetByToolIdAsync(toolId);
                return Mapper.Map<IEnumerable<ToolVersionDTO>>(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool versions: {ToolId}", toolId);
                throw;
            }
        }

        public async Task<ToolVersionDTO?> GetLatestVersionAsync(long toolId)
        {
            try
            {
                var version = await _toolVersionRepository.GetLatestVersionAsync(toolId);
                return Mapper.Map<ToolVersionDTO>(version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest version: {ToolId}", toolId);
                throw;
            }
        }

        public async Task<IEnumerable<ToolVersionDTO>> GetStableVersionsAsync(long toolId)
        {
            try
            {
                var versions = await _toolVersionRepository.GetStableVersionsAsync(toolId);
                return Mapper.Map<IEnumerable<ToolVersionDTO>>(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stable versions: {ToolId}", toolId);
                throw;
            }
        }

        public async Task<IEnumerable<ToolVersionDTO>> GetSupportedVersionsAsync(long toolId)
        {
            try
            {
                var versions = await _toolVersionRepository.GetSupportedVersionsAsync(toolId);
                return Mapper.Map<IEnumerable<ToolVersionDTO>>(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supported versions: {ToolId}", toolId);
                throw;
            }
        }

        public async Task<bool> IsVersionExistsAsync(long toolId, string version, long? excludeId = null)
        {
            try
            {
                return await _toolVersionRepository.IsVersionExistsAsync(toolId, version, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking version exists: {ToolId}, {Version}", toolId, version);
                throw;
            }
        }

        public async Task<ToolVersionComparisonDTO> CompareVersionsAsync(long version1Id, long version2Id)
        {
            try
            {
                var version1 = await _toolVersionRepository.FirstOrDefault(x => x.Id == version1Id && !x.IsDeleted);
                var version2 = await _toolVersionRepository.FirstOrDefault(x => x.Id == version2Id && !x.IsDeleted);

                if (version1 == null || version2 == null)
                    throw new ArgumentException("One or both versions not found");

                return new ToolVersionComparisonDTO
                {
                    CurrentVersion = Mapper.Map<ToolVersionDTO>(version1),
                    CompareVersion = Mapper.Map<ToolVersionDTO>(version2),
                    CompatibilityNotes = GenerateComparisonNotes(version1, version2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing versions: {Version1Id}, {Version2Id}", version1Id, version2Id);
                throw;
            }
        }

        private string GenerateComparisonNotes(ToolVersion version1, ToolVersion version2)
        {
            var notes = new List<string>();
            
            if (version1.ReleaseDate > version2.ReleaseDate)
                notes.Add($"Version {version1.Version} is newer than {version2.Version}");
            else
                notes.Add($"Version {version2.Version} is newer than {version1.Version}");
            
            if (version1.IsStable != version2.IsStable)
                notes.Add($"Stability differs - {version1.Version}: {(version1.IsStable ? "Stable" : "Unstable")}, {version2.Version}: {(version2.IsStable ? "Stable" : "Unstable")}");
            
            return string.Join("; ", notes);
        }

        public async Task<ToolVersionDTO> CreateVersionAsync(ToolVersionDTO request)
        {
            try
            {
                // Check if version exists
                if (await IsVersionExistsAsync(request.ToolId, request.Version))
                {
                    throw new InvalidOperationException("Version already exists for this tool");
                }

                var toolVersion = Mapper.Map<ToolVersion>(request);
                toolVersion.CreatedDate = DateTime.UtcNow;

                await _toolVersionRepository.CreateAsync(toolVersion);
                await UnitOfWork.SaveAsync();
                
                return Mapper.Map<ToolVersionDTO>(toolVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tool version");
                throw;
            }
        }

        public async Task<ToolVersionDTO> UpdateVersionAsync(ToolVersionDTO request)
        {
            try
            {
                var existingVersion = await _toolVersionRepository.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (existingVersion == null)
                {
                    throw new InvalidOperationException("Tool version not found");
                }

                // Check if version exists (excluding current version)
                if (await IsVersionExistsAsync(existingVersion.ToolId, request.Version, request.Id))
                {
                    throw new InvalidOperationException("Version already exists for this tool");
                }

                Mapper.Map(request, existingVersion);
                existingVersion.UpdatedDate = DateTime.UtcNow;

                await UnitOfWork.SaveAsync();

                return Mapper.Map<ToolVersionDTO>(existingVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tool version");
                throw;
            }
        }
    }

    public interface IToolCategoryService : IBaseBusinessService<ToolCategoryDTO>
    {
        Task<ToolCategoryDTO?> GetByCategoryCodeAsync(string categoryCode);
        Task<IEnumerable<ToolCategoryDTO>> GetRootCategoriesAsync();
        Task<IEnumerable<ToolCategoryDTO>> GetSubCategoriesAsync(long parentId);
        Task<bool> IsCategoryCodeExistsAsync(string categoryCode, long? excludeId = null);
        Task<IEnumerable<ToolCategoryDTO>> GetActiveCategoriesAsync();
        Task<ToolStatisticsDTO> GetCategoryStatisticsAsync();
    }

    public class ToolCategoryService : BaseBusinessService<ToolCategory, ToolCategoryDTO, IToolCategoryRepository>, IToolCategoryService
    {
        private readonly IToolCategoryRepository _toolCategoryRepository;
        private readonly ILogger<ToolCategoryService> _logger;

        public ToolCategoryService(
            IServiceProvider serviceProvider,
            IToolCategoryRepository toolCategoryRepository,
            ILogger<ToolCategoryService> logger) : base(serviceProvider)
        {
            _toolCategoryRepository = toolCategoryRepository;
            _logger = logger;
        }

        protected override IToolCategoryRepository GetRepository()
        {
            return _toolCategoryRepository;
        }

        public async Task<ToolCategoryDTO?> GetByCategoryCodeAsync(string categoryCode)
        {
            try
            {
                var category = await _toolCategoryRepository.GetByCategoryCodeAsync(categoryCode);
                return Mapper.Map<ToolCategoryDTO>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by code: {CategoryCode}", categoryCode);
                throw;
            }
        }

        public async Task<IEnumerable<ToolCategoryDTO>> GetRootCategoriesAsync()
        {
            try
            {
                var categories = await _toolCategoryRepository.GetRootCategoriesAsync();
                return Mapper.Map<IEnumerable<ToolCategoryDTO>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting root categories");
                throw;
            }
        }

        public async Task<IEnumerable<ToolCategoryDTO>> GetSubCategoriesAsync(long parentId)
        {
            try
            {
                var categories = await _toolCategoryRepository.GetSubCategoriesAsync(parentId);
                return Mapper.Map<IEnumerable<ToolCategoryDTO>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sub categories: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<bool> IsCategoryCodeExistsAsync(string categoryCode, long? excludeId = null)
        {
            try
            {
                return await _toolCategoryRepository.IsCategoryCodeExistsAsync(categoryCode, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking category code exists: {CategoryCode}", categoryCode);
                throw;
            }
        }

        public async Task<IEnumerable<ToolCategoryDTO>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _toolCategoryRepository.GetActiveCategoriesAsync();
                return Mapper.Map<IEnumerable<ToolCategoryDTO>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active categories");
                throw;
            }
        }

        public async Task<ToolStatisticsDTO> GetCategoryStatisticsAsync()
        {
            try
            {
                var allCategories = await _toolCategoryRepository.GetAll();
                var toolCountPerCategory = await _toolCategoryRepository.GetToolCountPerCategoryAsync();

                return new ToolStatisticsDTO
                {
                    TotalTools = toolCountPerCategory.Values.Sum(),
                    ToolsByCategory = toolCountPerCategory.ToDictionary(x => $"Category_{x.Key}", x => x.Value)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category statistics");
                throw;
            }
        }
    }
}
