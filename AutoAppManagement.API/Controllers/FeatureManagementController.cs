using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Feature;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    /// <summary>
    /// Simple Feature Management API Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeatureManagementController : BaseController
    {
        private readonly IFeatureManagementService _featureManagementService;

        public FeatureManagementController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _featureManagementService = serviceProvider.GetRequiredService<IFeatureManagementService>();
        }

        /// <summary>
        /// L?y danh sách tính n?ng ???c phép cho user hi?n t?i
        /// </summary>
        [HttpGet("my-features")]
        public async Task<IActionResult> GetMyFeatures()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var allowedFeatures = await _featureManagementService.GetAllowedFeaturesAsync(userId);
                
                ResOutput.SuccessEventHandler(new
                {
                    userId,
                    allowedFeatures,
                    totalFeatures = allowedFeatures.Count
                });
                
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting user features: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// L?y chi ti?t tính n?ng cho user hi?n t?i
        /// </summary>
        [HttpGet("my-features/details")]
        public async Task<IActionResult> GetMyFeatureDetails()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var featureDetails = await _featureManagementService.GetFeatureDetailsAsync(userId);
                
                ResOutput.SuccessEventHandler(new
                {
                    userId,
                    features = featureDetails,
                    totalFeatures = featureDetails.Count
                });
                
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting feature details: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Ki?m tra quy?n s? d?ng tính n?ng c? th?
        /// </summary>
        [HttpGet("check-feature/{featureId}")]
        public async Task<IActionResult> CheckFeature(long featureId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isAllowed = await _featureManagementService.IsFeatureAllowedAsync(userId, featureId);
                
                ResOutput.SuccessEventHandler(new
                {
                    userId,
                    featureId,
                    isAllowed,
                    timestamp = DateTime.UtcNow
                });
                
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error checking feature: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Ki?m tra quy?n s? d?ng tính n?ng theo code
        /// </summary>
        [HttpGet("check-feature-code/{featureCode}")]
        public async Task<IActionResult> CheckFeatureByCode(string featureCode)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isAllowed = await _featureManagementService.IsFeatureAllowedAsync(userId, featureCode);
                
                ResOutput.SuccessEventHandler(new
                {
                    userId,
                    featureCode,
                    isAllowed,
                    timestamp = DateTime.UtcNow
                });
                
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error checking feature: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Ghi nh?n vi?c s? d?ng tính n?ng
        /// </summary>
        [HttpPost("record-usage")]
        public async Task<IActionResult> RecordFeatureUsage([FromBody] RecordUsageRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                bool success;
                
                if (request.FeatureId.HasValue)
                {
                    success = await _featureManagementService.RecordFeatureUsageAsync(
                        userId, 
                        request.FeatureId.Value, 
                        request.ResourceAmount, 
                        request.UsageType);
                }
                else if (!string.IsNullOrEmpty(request.FeatureCode))
                {
                    success = await _featureManagementService.RecordFeatureUsageAsync(
                        userId, 
                        request.FeatureCode, 
                        request.ResourceAmount, 
                        request.UsageType);
                }
                else
                {
                    ResOutput.ErrorEventHandler("Ph?i cung c?p FeatureId ho?c FeatureCode");
                    return BadRequest(ResOutput);
                }

                if (success)
                {
                    ResOutput.SuccessEventHandler("Ghi nh?n s? d?ng tính n?ng thành công");
                }
                else
                {
                    ResOutput.ErrorEventHandler("Không th? ghi nh?n s? d?ng tính n?ng");
                }
                
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error recording usage: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// L?y th?ng kê s? d?ng tính n?ng
        /// </summary>
        [HttpGet("usage-stats")]
        public async Task<IActionResult> GetUsageStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                var stats = await _featureManagementService.GetFeatureUsageStatsAsync(userId);
                
                ResOutput.SuccessEventHandler(stats);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting usage stats: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// L?y danh sách categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<IActionResult> GetFeatureCategories()
        {
            try
            {
                var categories = await _featureManagementService.GetFeatureCategoriesAsync();
                
                ResOutput.SuccessEventHandler(categories);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting categories: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Làm m?i cache tính n?ng
        /// </summary>
        [HttpPost("refresh-cache")]
        public async Task<IActionResult> RefreshCache()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _featureManagementService.RefreshFeatureCacheAsync(userId);
                
                ResOutput.SuccessEventHandler("?ã làm m?i cache thành công");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error refreshing cache: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        #region Admin APIs

        /// <summary>
        /// L?y thông tin tính n?ng c?a user b?t k? (Admin only)
        /// </summary>
        [HttpGet("admin/user-features/{userId}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetUserFeatures(long userId)
        {
            try
            {
                var allowedFeatures = await _featureManagementService.GetAllowedFeaturesAsync(userId);
                var featureDetails = await _featureManagementService.GetFeatureDetailsAsync(userId);
                var usageStats = await _featureManagementService.GetFeatureUsageStatsAsync(userId);
                
                ResOutput.SuccessEventHandler(new
                {
                    userId,
                    allowedFeatures,
                    featureDetails,
                    usageStats,
                    totalFeatures = allowedFeatures.Count
                });
                
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting user features: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Ki?m tra nhi?u tính n?ng cùng lúc (Admin only)
        /// </summary>
        [HttpPost("admin/batch-check")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> BatchCheckFeatures([FromBody] BatchFeatureCheckRequest request)
        {
            try
            {
                var response = new BatchFeatureCheckResponse();
                
                foreach (var featureId in request.FeatureIds)
                {
                    var isAllowed = await _featureManagementService.IsFeatureAllowedAsync(request.UserId, featureId);
                    response.Results[featureId] = isAllowed;
                }
                
                ResOutput.SuccessEventHandler(response);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error in batch check: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        #endregion

        #region Helper Methods

        private long GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.FindFirst("AccountId") ?? HttpContext.User.FindFirst("UserId");
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        #endregion
    }

    #region Request DTOs

    /// <summary>
    /// Request ?? ghi nh?n vi?c s? d?ng tính n?ng
    /// </summary>
    public class RecordUsageRequest
    {
        public long? FeatureId { get; set; }
        public string? FeatureCode { get; set; }
        public decimal ResourceAmount { get; set; } = 1;
        public string UsageType { get; set; } = "Access";
    }

    /// <summary>
    /// Request ?? thu h?i license
    /// </summary>
    public class RevokeLicenseRequest
    {
        public long UserId { get; set; }
        public long LicenseId { get; set; }
    }

    #endregion
}
