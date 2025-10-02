using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.ToolVersion;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    /// <summary>
    /// Tool Version Management API Controller
    /// Provides endpoints for checking and managing tool versions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetCurrentVersion(string toolCode, [FromQuery] string? platform = null)
        {
            try
            {
                var version = await Service.GetCurrentVersionAsync(toolCode, platform);
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
        /// Kiểm tra xem có update mới không (PUBLIC - Dành cho bên thứ 3)
        /// </summary>
        /// <param name="request">Thông tin version hiện tại</param>
        /// <returns>Thông tin về update nếu có</returns>
        [HttpPost("check")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckVersion([FromBody] CheckVersionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler("Invalid request data");
                    return BadRequest(ResOutput);
                }

                var response = await Service.CheckVersionAsync(request);
                ResOutput.SuccessEventHandler(response);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error checking version: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Kiểm tra update nhanh (PUBLIC - Simplified endpoint)
        /// </summary>
        /// <param name="toolCode">Mã tool</param>
        /// <param name="currentVersion">Version hiện tại</param>
        /// <param name="platform">Platform - Optional</param>
        /// <returns>True nếu có update, False nếu không</returns>
        [HttpGet("check-update/{toolCode}/{currentVersion}")]
        [AllowAnonymous]
        public async Task<IActionResult> QuickCheckUpdate(string toolCode, string currentVersion, [FromQuery] string? platform = null)
        {
            try
            {
                var request = new CheckVersionRequest
                {
                    ToolCode = toolCode,
                    CurrentVersion = currentVersion,
                    Platform = platform
                };

                var response = await Service.CheckVersionAsync(request);
                
                // Simplified response for quick check
                var quickResponse = new
                {
                    updateAvailable = response.UpdateAvailable,
                    updateRequired = response.UpdateRequired,
                    latestVersion = response.LatestVersion,
                    downloadUrl = response.DownloadUrl,
                    message = response.Message
                };

                ResOutput.SuccessEventHandler(quickResponse);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error checking update: {ex.Message}");
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
        [AllowAnonymous]
        public async Task<IActionResult> GetVersionHistory(string toolCode, [FromQuery] int limit = 10)
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
        [AllowAnonymous]
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

        /// <summary>
        /// Lấy versions theo platform (PUBLIC)
        /// </summary>
        /// <param name="platform">Platform (Windows, MacOS, Linux, etc.)</param>
        /// <returns>Danh sách versions cho platform</returns>
        [HttpGet("platform/{platform}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVersionsByPlatform(string platform)
        {
            try
            {
                var versions = await Service.GetVersionsByPlatformAsync(platform);
                ResOutput.SuccessEventHandler(versions);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting versions by platform: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        #endregion

        #region Admin Endpoints (Requires Authentication)

        /// <summary>
        /// Tạo version mới (Admin only)
        /// </summary>
        /// <param name="request">Thông tin version mới</param>
        /// <returns>Version đã tạo</returns>
        [HttpPost("create")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateVersion([FromBody] CreateToolVersionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler("Invalid request data");
                    return BadRequest(ResOutput);
                }

                var result = await Service.CreateVersionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error creating version: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Cập nhật version (Admin only)
        /// </summary>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Version đã cập nhật</returns>
        [HttpPut("update")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UpdateVersion([FromBody] UpdateToolVersionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler("Invalid request data");
                    return BadRequest(ResOutput);
                }

                var result = await Service.UpdateVersionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error updating version: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Activate version (Admin only)
        /// </summary>
        /// <param name="id">Version ID</param>
        /// <returns>Success message</returns>
        [HttpPost("activate/{id}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> ActivateVersion(long id)
        {
            try
            {
                var result = await Service.ActivateVersionAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error activating version: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Deactivate version (Admin only)
        /// </summary>
        /// <param name="id">Version ID</param>
        /// <returns>Success message</returns>
        [HttpPost("deactivate/{id}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> DeactivateVersion(long id)
        {
            try
            {
                var result = await Service.DeactivateVersionAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error deactivating version: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Xóa version (Admin only)
        /// </summary>
        /// <param name="id">Version ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Roles(RoleConstant.Admin)]
        public override async Task<IActionResult> Delete(long id)
        {
            return await base.Delete(id);
        }

        /// <summary>
        /// Lấy danh sách updates bắt buộc (Admin only)
        /// </summary>
        /// <returns>Danh sách required updates</returns>
        [HttpGet("required-updates")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRequiredUpdates()
        {
            try
            {
                var updates = await Service.GetRequiredUpdatesAsync();
                ResOutput.SuccessEventHandler(updates);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting required updates: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Lấy versions theo category (Admin only)
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>Danh sách versions</returns>
        [HttpGet("category/{category}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetVersionsByCategory(string category)
        {
            try
            {
                var versions = await Service.GetVersionsByCategoryAsync(category);
                ResOutput.SuccessEventHandler(versions);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting versions by category: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        #endregion

        #region Health Check Endpoint

        /// <summary>
        /// Health check endpoint for monitoring
        /// </summary>
        /// <returns>Service status</returns>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                service = "ToolVersion",
                timestamp = DateTime.UtcNow
            });
        }

        #endregion
    }
}

