using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Tool;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToolSimpleController : ControllerBase
    {
        private readonly IToolService _toolService;
        private readonly IToolVersionService _toolVersionService;
        private readonly IToolCategoryService _toolCategoryService;

        public ToolSimpleController(
            IToolService toolService,
            IToolVersionService toolVersionService,
            IToolCategoryService toolCategoryService)
        {
            _toolService = toolService;
            _toolVersionService = toolVersionService;
            _toolCategoryService = toolCategoryService;
        }

        #region Tool Management

        [HttpGet("list")]
        public async Task<ActionResult<PagingResultDTO<ToolDTO>>> GetAllTools([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _toolService.GetAllPagingAsync(pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolDTO>> GetToolById(long id)
        {
            try
            {
                var result = await _toolService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagingResultDTO<ToolDTO>>> SearchTools([FromBody] ToolSearchRequest request)
        {
            try
            {
                var result = await _toolService.SearchToolsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<ToolStatisticsDTO>> GetToolStatistics()
        {
            try
            {
                var result = await _toolService.GetToolStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ToolDTO>> CreateTool([FromBody] ToolDTO request)
        {
            try
            {
                var result = await _toolService.CreateToolAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToolDTO>> UpdateTool(long id, [FromBody] ToolDTO request)
        {
            try
            {
                request.Id = id;
                var result = await _toolService.UpdateToolAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTool(long id)
        {
            try
            {
                await _toolService.DeleteAsync(id);
                return Ok(new { message = "Tool deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Tool Version Management

        [HttpGet("{toolId}/versions")]
        public async Task<ActionResult<IEnumerable<ToolVersionDTO>>> GetToolVersions(long toolId)
        {
            try
            {
                var result = await _toolVersionService.GetByConditionAsync(x => x.ToolId == toolId && !x.IsDeleted);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("versions/{id}")]
        public async Task<ActionResult<ToolVersionDTO>> GetVersionById(long id)
        {
            try
            {
                var result = await _toolVersionService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("versions")]
        public async Task<ActionResult<ToolVersionDTO>> CreateVersion([FromBody] ToolVersionDTO request)
        {
            try
            {
                var result = await _toolVersionService.CreateVersionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("versions/{id}")]
        public async Task<ActionResult<ToolVersionDTO>> UpdateVersion(long id, [FromBody] ToolVersionDTO request)
        {
            try
            {
                request.Id = id;
                var result = await _toolVersionService.UpdateVersionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("versions/compare/{version1Id}/{version2Id}")]
        public async Task<ActionResult<ToolVersionComparisonDTO>> CompareVersions(long version1Id, long version2Id)
        {
            try
            {
                var result = await _toolVersionService.CompareVersionsAsync(version1Id, version2Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Tool Category Management

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ToolCategoryDTO>>> GetAllCategories()
        {
            try
            {
                var result = await _toolCategoryService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("categories/{id}")]
        public async Task<ActionResult<ToolCategoryDTO>> GetCategoryById(long id)
        {
            try
            {
                var result = await _toolCategoryService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("categories/statistics")]
        public async Task<ActionResult<ToolStatisticsDTO>> GetCategoryStatistics()
        {
            try
            {
                var result = await _toolCategoryService.GetCategoryStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("categories")]
        public async Task<ActionResult<ToolCategoryDTO>> CreateCategory([FromBody] ToolCategoryDTO request)
        {
            try
            {
                var result = await _toolCategoryService.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult<ToolCategoryDTO>> UpdateCategory(long id, [FromBody] ToolCategoryDTO request)
        {
            try
            {
                request.Id = id;
                var result = await _toolCategoryService.UpdateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(long id)
        {
            try
            {
                await _toolCategoryService.DeleteAsync(id);
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}
