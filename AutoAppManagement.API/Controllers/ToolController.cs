using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Tool;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace AutoAppManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToolsController : ControllerBase
    {
        private readonly IToolService _toolService;
        private readonly IToolVersionService _toolVersionService;
        private readonly ILogger<ToolsController> _logger;
        private readonly IMapper _mapper;

        public ToolsController(IToolService toolService, IToolVersionService toolVersionService, ILogger<ToolsController> logger, IMapper mapper)
        {
            _toolService = toolService;
            _toolVersionService = toolVersionService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetAllTools()
        {
            try
            {
                var tools = await _toolService.GetAll();
                return Ok(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tools");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<object>> GetAllToolsPaging([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Use existing pagination method if available, otherwise implement simple paging
                var allTools = await _toolService.GetAll();
                var totalCount = allTools.Count();
                var pagedTools = allTools.Skip(pageIndex * pageSize).Take(pageSize);
                
                var result = new 
                {
                    Data = pagedTools,
                    TotalCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tools with paging");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolDTO>> GetTool(long id)
        {
            try
            {
                var tool = await _toolService.GetById(id);
                if (tool == null)
                    return NotFound();

                return Ok(tool);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool: {ToolId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-code/{toolCode}")]
        public async Task<ActionResult<ToolDTO>> GetToolByCode(string toolCode)
        {
            try
            {
                var tool = await _toolService.GetByToolCodeAsync(toolCode);
                if (tool == null)
                    return NotFound();

                return Ok(tool);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool by code: {ToolCode}", toolCode);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetToolsByCategory(string category)
        {
            try
            {
                var tools = await _toolService.GetByCategoryAsync(category);
                return Ok(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tools by category: {Category}", category);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-type/{toolType}")]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetToolsByType(string toolType)
        {
            try
            {
                var tools = await _toolService.GetByToolTypeAsync(toolType);
                return Ok(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tools by type: {ToolType}", toolType);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetPublicTools()
        {
            try
            {
                var tools = await _toolService.GetPublicToolsAsync();
                return Ok(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public tools");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ToolSearchResponseDTO>> SearchTools([FromBody] ToolSearchRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Search request is required");

                // Map DTO to domain model
                var searchRequest = _mapper.Map<ToolSearchRequest>(request);
                var result = await _toolService.SearchToolsAsync(searchRequest);
                
                // Map result to response DTO
                var response = new ToolSearchResponseDTO
                {
                    Tools = _mapper.Map<List<ToolDTO>>(result.Data),
                    TotalCount = result.TotalItems,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalPages = result.TotalPages,
                    HasNextPage = request.PageIndex < result.TotalPages - 1,
                    HasPreviousPage = request.PageIndex > 0
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tools");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<ToolStatisticsDTO>> GetToolStatistics()
        {
            try
            {
                var stats = await _toolService.GetToolStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ToolCreateResponseDTO>> CreateTool([FromBody] ToolCreateRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Tool data is required");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Map request DTO to service DTO
                var toolDto = _mapper.Map<ToolDTO>(request);
                var result = await _toolService.CreateToolAsync(toolDto);
                
                // Map result to response DTO
                var response = new ToolCreateResponseDTO
                {
                    Success = true,
                    Message = "Tool created successfully",
                    Tool = result
                };
                
                return CreatedAtAction(nameof(GetTool), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tool");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToolUpdateResponseDTO>> UpdateTool(long id, [FromBody] ToolUpdateRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Tool data is required");

                if (id != request.Id)
                    return BadRequest("Tool ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Map request DTO to service DTO
                var toolDto = _mapper.Map<ToolDTO>(request);
                var result = await _toolService.UpdateToolAsync(toolDto);
                
                // Map result to response DTO
                var response = new ToolUpdateResponseDTO
                {
                    Success = true,
                    Message = "Tool updated successfully",
                    Tool = result
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tool: {ToolId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTool(long id)
        {
            try
            {
                var result = await _toolService.DeleteToolAsync(id);
                
                if (result)
                    return NoContent();
                
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tool: {ToolId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/versions")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetToolVersions(long id)
        {
            try
            {
                var versions = await _toolVersionService.GetByToolIdAsync(id);
                return Ok(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting versions for tool: {ToolId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/check-code/{toolCode}")]
        public async Task<ActionResult<bool>> CheckToolCodeExists(long id, string toolCode)
        {
            try
            {
                var exists = await _toolService.IsToolCodeExistsAsync(toolCode, id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking tool code exists: {ToolCode}", toolCode);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToolVersionsController : ControllerBase
    {
        private readonly IToolVersionService _toolVersionService;
        private readonly IMapper _mapper;
        private readonly ILogger<ToolVersionsController> _logger;

        public ToolVersionsController(IToolVersionService toolVersionService, IMapper mapper, ILogger<ToolVersionsController> logger)
        {
            _toolVersionService = toolVersionService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetAllVersions()
        {
            try
            {
                var versions = await _toolVersionService.GetAll();
                return Ok(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tool versions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolVersionDTO>> GetVersion(long id)
        {
            try
            {
                var version = await _toolVersionService.GetById(id);
                if (version == null)
                    return NotFound();

                return Ok(version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool version: {VersionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tool/{toolId}")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetVersionsByTool(long toolId)
        {
            try
            {
                var versions = await _toolVersionService.GetByToolIdAsync(toolId);
                return Ok(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting versions for tool: {ToolId}", toolId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tool/{toolId}/version/{version}")]
        public async Task<ActionResult<ToolVersionDTO>> GetVersionByToolAndVersion(long toolId, string version)
        {
            try
            {
                var toolVersion = await _toolVersionService.GetByToolAndVersionAsync(toolId, version);
                if (toolVersion == null)
                    return NotFound();

                return Ok(toolVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool version: {ToolId}, {Version}", toolId, version);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tool/{toolId}/latest")]
        public async Task<ActionResult<ToolVersionDTO>> GetLatestVersion(long toolId)
        {
            try
            {
                var version = await _toolVersionService.GetLatestVersionAsync(toolId);
                if (version == null)
                    return NotFound();

                return Ok(version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest version for tool: {ToolId}", toolId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tool/{toolId}/stable")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetStableVersions(long toolId)
        {
            try
            {
                var versions = await _toolVersionService.GetStableVersionsAsync(toolId);
                return Ok(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stable versions for tool: {ToolId}", toolId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tool/{toolId}/supported")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetSupportedVersions(long toolId)
        {
            try
            {
                var versions = await _toolVersionService.GetSupportedVersionsAsync(toolId);
                return Ok(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supported versions for tool: {ToolId}", toolId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("compare/{version1Id}/{version2Id}")]
        public async Task<ActionResult<VersionComparisonDTO>> CompareVersions(long version1Id, long version2Id)
        {
            try
            {
                var comparison = await _toolVersionService.CompareVersionsAsync(version1Id, version2Id);
                return Ok(comparison);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing versions: {Version1Id}, {Version2Id}", version1Id, version2Id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ToolVersionCreateResponseDTO>> CreateVersion([FromBody] ToolVersionCreateRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Version data is required");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Map request DTO to service DTO
                var versionDto = _mapper.Map<ToolVersionDTO>(request);
                var result = await _toolVersionService.CreateVersionAsync(versionDto);
                
                // Map result to response DTO
                var response = new ToolVersionCreateResponseDTO
                {
                    Success = true,
                    Message = "Tool version created successfully",
                    ToolVersion = result
                };
                
                return CreatedAtAction(nameof(GetVersion), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tool version");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToolVersionUpdateResponseDTO>> UpdateVersion(long id, [FromBody] ToolVersionUpdateRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Version data is required");

                if (id != request.Id)
                    return BadRequest("Version ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Map request DTO to service DTO
                var versionDto = _mapper.Map<ToolVersionDTO>(request);
                var result = await _toolVersionService.UpdateVersionAsync(versionDto);
                
                // Map result to response DTO
                var response = new ToolVersionUpdateResponseDTO
                {
                    Success = true,
                    Message = "Tool version updated successfully",
                    ToolVersion = result
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tool version: {VersionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVersion(long id)
        {
            try
            {
                var result = await _toolVersionService.Delete(id);
                
                if (result.IsSuccess)
                    return NoContent();
                
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tool version: {VersionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToolCategoriesController : ControllerBase
    {
        private readonly IToolCategoryService _toolCategoryService;
        private readonly ILogger<ToolCategoriesController> _logger;

        public ToolCategoriesController(IToolCategoryService toolCategoryService, ILogger<ToolCategoriesController> logger)
        {
            _toolCategoryService = toolCategoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToolCategoryDTO>>> GetAllCategories()
        {
            try
            {
                var categories = await _toolCategoryService.GetAll();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tool categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolCategoryDTO>> GetCategory(long id)
        {
            try
            {
                var category = await _toolCategoryService.GetById(id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tool category: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-code/{categoryCode}")]
        public async Task<ActionResult<ToolCategoryDTO>> GetCategoryByCode(string categoryCode)
        {
            try
            {
                var category = await _toolCategoryService.GetByCategoryCodeAsync(categoryCode);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by code: {CategoryCode}", categoryCode);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("root")]
        public async Task<ActionResult<IEnumerable<ToolCategoryDTO>>> GetRootCategories()
        {
            try
            {
                var categories = await _toolCategoryService.GetRootCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting root categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{parentId}/sub-categories")]
        public async Task<ActionResult<IEnumerable<ToolCategoryDTO>>> GetSubCategories(long parentId)
        {
            try
            {
                var categories = await _toolCategoryService.GetSubCategoriesAsync(parentId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sub categories: {ParentId}", parentId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ToolCategoryDTO>>> GetActiveCategories()
        {
            try
            {
                var categories = await _toolCategoryService.GetActiveCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<ToolCategoryStatsDTO>> GetCategoryStatistics()
        {
            try
            {
                var stats = await _toolCategoryService.GetCategoryStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ToolCategoryDTO>> CreateCategory([FromBody] ToolCategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto == null)
                    return BadRequest("Category data is required");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if category code exists
                if (await _toolCategoryService.IsCategoryCodeExistsAsync(categoryDto.CategoryCode))
                    return BadRequest("Category code already exists");

                // Set state for creation
                categoryDto.State = AutoAppManagement.Models.Common.EntityState.Add;
                var result = await _toolCategoryService.SubmitData(categoryDto);
                
                if (result.IsSuccess)
                    return CreatedAtAction(nameof(GetCategory), new { id = ((ToolCategoryDTO)result.Data)?.Id }, result.Data);
                
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tool category");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToolCategoryDTO>> UpdateCategory(long id, [FromBody] ToolCategoryDTO categoryDto)
        {
            try
            {
                if (categoryDto == null)
                    return BadRequest("Category data is required");

                if (id != categoryDto.Id)
                    return BadRequest("Category ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if category code exists (excluding current category)
                if (await _toolCategoryService.IsCategoryCodeExistsAsync(categoryDto.CategoryCode, id))
                    return BadRequest("Category code already exists");

                // Set state for update
                categoryDto.State = AutoAppManagement.Models.Common.EntityState.Edit;
                var result = await _toolCategoryService.SubmitData(categoryDto);
                
                if (result.IsSuccess)
                    return Ok(result.Data);
                
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tool category: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(long id)
        {
            try
            {
                var result = await _toolCategoryService.Delete(id);
                
                if (result.IsSuccess)
                    return NoContent();
                
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tool category: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
