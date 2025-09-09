using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Tool;
using AutoAppManagement.Service.Services;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ToolController : BaseController
    {
        private readonly AutoAppManagement.Service.Services.IToolService _toolService;
        private readonly AutoAppManagement.Service.Services.IToolVersionService _toolVersionService;
        private readonly AutoAppManagement.Service.Services.IToolCategoryService _toolCategoryService;

        public ToolController(
            IServiceProvider serviceProvider,
            AutoAppManagement.Service.Services.IToolService toolService,
            AutoAppManagement.Service.Services.IToolVersionService toolVersionService,
            AutoAppManagement.Service.Services.IToolCategoryService toolCategoryService) : base(serviceProvider)
        {
            _toolService = toolService;
            _toolVersionService = toolVersionService;
            _toolCategoryService = toolCategoryService;
        }

        #region Tool Management - Base Methods

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetTools()
        {
            try
            {
                var tools = await _toolService.GetAll();
                ResOutput.SuccessEventHandler(tools);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolDTO>> GetTool(long id)
        {
            try
            {
                var tool = await _toolService.GetById(id);
                if (tool == null)
                {
                    ResOutput.ErrorEventHandler("Tool not found");
                    return NotFound(ResOutput);
                }

                ResOutput.SuccessEventHandler(tool);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateTool([FromBody] ToolDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler(ModelState);
                    return BadRequest(ResOutput);
                }

                request.CreatedBy = GetCurrentUserId();
                request.State = AutoAppManagement.Models.Common.EntityState.Add;
                var result = await _toolService.SubmitData(request);
                
                ResOutput.SuccessEventHandler(result, "Tool created successfully");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateTool([FromBody] ToolDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler(ModelState);
                    return BadRequest(ResOutput);
                }

                request.UpdatedBy = GetCurrentUserId();
                request.State = AutoAppManagement.Models.Common.EntityState.Edit;
                var result = await _toolService.SubmitData(request);
                
                ResOutput.SuccessEventHandler(result, "Tool updated successfully");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTool(long id)
        {
            try
            {
                var result = await _toolService.Delete(id);
                if (!result.IsSuccess)
                {
                    ResOutput.ErrorEventHandler(result.Message);
                    return BadRequest(ResOutput);
                }

                ResOutput.SuccessEventHandler(result.Data ?? new object(), "Tool deleted successfully");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        #endregion

        #region Tool Management - Extended Methods

        [HttpGet("code/{toolCode}")]
        public async Task<ActionResult<ToolDTO>> GetByToolCode(string toolCode)
        {
            try
            {
                var tool = await _toolService.GetByToolCodeAsync(toolCode);
                if (tool == null)
                {
                    ResOutput.ErrorEventHandler("Tool not found");
                    return NotFound(ResOutput);
                }

                ResOutput.SuccessEventHandler(tool);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetByCategory(string category)
        {
            try
            {
                var tools = await _toolService.GetByCategoryAsync(category);
                ResOutput.SuccessEventHandler(tools);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("type/{toolType}")]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetByType(string toolType)
        {
            try
            {
                var tools = await _toolService.GetByToolTypeAsync(toolType);
                ResOutput.SuccessEventHandler(tools);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ToolDTO>>> GetPublicTools()
        {
            try
            {
                var tools = await _toolService.GetPublicToolsAsync();
                ResOutput.SuccessEventHandler(tools);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult> SearchTools([FromBody] ToolSearchRequestDTO request)
        {
            try
            {
                // Map DTO to domain request
                var searchRequest = new ToolSearchRequest
                {
                    SearchTerm = request.SearchTerm,
                    Category = request.Category,
                    ToolType = request.ToolType,
                    IsPublic = request.IsPublic,
                    Page = request.PageIndex,
                    PageSize = request.PageSize,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection
                };
                
                var result = await _toolService.SearchToolsAsync(searchRequest);
                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _toolService.GetToolStatisticsAsync();
                ResOutput.SuccessEventHandler(statistics);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("{id}/check-code")]
        public async Task<ActionResult<bool>> CheckToolCodeExists(long id, [FromQuery] string toolCode)
        {
            try
            {
                var exists = await _toolService.IsToolCodeExistsAsync(toolCode, id);
                ResOutput.SuccessEventHandler(new { exists });
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        #endregion

        #region Tool Version Management

        [HttpGet("{toolId}/versions")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetToolVersions(long toolId)
        {
            try
            {
                var versions = await _toolVersionService.GetByToolIdAsync(toolId);
                ResOutput.SuccessEventHandler(versions);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("{toolId}/versions/latest")]
        public async Task<ActionResult<ToolVersionDTO>> GetLatestVersion(long toolId)
        {
            try
            {
                var version = await _toolVersionService.GetLatestVersionAsync(toolId);
                if (version == null)
                {
                    ResOutput.ErrorEventHandler("No version found");
                    return NotFound(ResOutput);
                }

                ResOutput.SuccessEventHandler(version);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("{toolId}/versions/stable")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetStableVersions(long toolId)
        {
            try
            {
                var versions = await _toolVersionService.GetStableVersionsAsync(toolId);
                ResOutput.SuccessEventHandler(versions);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("{toolId}/versions/supported")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetSupportedVersions(long toolId)
        {
            try
            {
                var versions = await _toolVersionService.GetSupportedVersionsAsync(toolId);
                ResOutput.SuccessEventHandler(versions);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("{toolId}/versions/{version}")]
        public async Task<ActionResult<ToolVersionDTO>> GetByVersion(long toolId, string version)
        {
            try
            {
                var toolVersion = await _toolVersionService.GetByToolAndVersionAsync(toolId, version);
                if (toolVersion == null)
                {
                    ResOutput.ErrorEventHandler("Version not found");
                    return NotFound(ResOutput);
                }

                ResOutput.SuccessEventHandler(toolVersion);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpPost("{toolId}/versions")]
        public async Task<ActionResult> CreateVersion(long toolId, [FromBody] ToolVersionDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler(ModelState);
                    return BadRequest(ResOutput);
                }

                if (toolId != request.ToolId)
                {
                    ResOutput.ErrorEventHandler("Tool ID mismatch");
                    return BadRequest(ResOutput);
                }

                request.CreatedBy = GetCurrentUserId();
                request.State = AutoAppManagement.Models.Common.EntityState.Add;
                var result = await _toolVersionService.SubmitData(request);
                
                ResOutput.SuccessEventHandler(result, "Version created successfully");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpPut("versions/{versionId}")]
        public async Task<ActionResult> UpdateVersion(long versionId, [FromBody] ToolVersionDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler(ModelState);
                    return BadRequest(ResOutput);
                }

                if (versionId != request.Id)
                {
                    ResOutput.ErrorEventHandler("Version ID mismatch");
                    return BadRequest(ResOutput);
                }

                request.UpdatedBy = GetCurrentUserId();
                request.State = AutoAppManagement.Models.Common.EntityState.Edit;
                var result = await _toolVersionService.SubmitData(request);
                
                ResOutput.SuccessEventHandler(result, "Version updated successfully");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpPost("versions/{version1Id}/compare/{version2Id}")]
        public async Task<ActionResult> CompareVersions(long version1Id, long version2Id)
        {
            try
            {
                var comparison = await _toolVersionService.CompareVersionsAsync(version1Id, version2Id);
                ResOutput.SuccessEventHandler(comparison);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("{toolId}/versions/{version}/check")]
        public async Task<ActionResult<bool>> CheckVersionExists(long toolId, string version, [FromQuery] long? excludeId = null)
        {
            try
            {
                var exists = await _toolVersionService.IsVersionExistsAsync(toolId, version, excludeId);
                ResOutput.SuccessEventHandler(new { exists });
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        #endregion
    }
}
