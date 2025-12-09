using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.ToolVersion;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    /// <summary>
    /// Tool Version Management API Controller
    /// Provides endpoints for checking and managing tool versions
    /// </summary>
    public class ToolVersionController : BaseBusinessController<IToolVersionService, ToolVersion, ToolVersionDTO>
    {
        public ToolVersionController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        #region Public Endpoints (No Authentication Required)

        /// <summary>
        /// Kiểm tra version hiện tại của tool (PUBLIC - Dành cho bên thứ 3)
        /// </summary>
        /// <param name="toolCode">Mã tool cần kiểm tra</param>
        /// <param name="platform">Platform (Windows, MacOS, Linux, etc.) - Optional</param>
        /// <returns>Thông tin version hiện tại</returns>
        [HttpGet("current/{toolCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentVersion(ToolCode toolCode)
        {
            try
            {
                var version = await Service.GetCurrentVersionAsync(toolCode);
                if (version == null)
                {
                    ResOutput.ErrorEventHandler($"No version information found for tool: {toolCode}");
                    return NotFound(ResOutput);
                }

                ResOutput.SuccessEventHandler(version);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting version: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Lấy lịch sử version của tool (PUBLIC)
        /// </summary>
        /// <param name="toolCode">Mã tool</param>
        /// <param name="limit">Số lượng version muốn lấy (default: 10)</param>
        /// <returns>Danh sách version history</returns>
        [HttpGet("history/{toolCode}")]
        public async Task<IActionResult> GetVersionHistory(ToolCode toolCode, [FromQuery] int limit = 10)
        {
            try
            {
                var history = await Service.GetVersionHistoryAsync(toolCode, limit);
                ResOutput.SuccessEventHandler(history);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting version history: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả tool versions đang active (PUBLIC)
        /// </summary>
        /// <returns>Danh sách tool versions</returns>
        [HttpGet("all-active")]
        public async Task<IActionResult> GetAllActiveVersions()
        {
            try
            {
                var versions = await Service.GetActiveVersionsAsync();
                ResOutput.SuccessEventHandler(versions);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting active versions: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        #endregion

        #region Admin Endpoints (Requires Authentication)

        #endregion
    }
}

