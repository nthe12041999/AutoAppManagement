using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.Models.Constant;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers.Examples
{
    /// <summary>
    /// Ví dụ về cách sử dụng Feature Access Management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleFeatureController : ControllerBase
    {
        /// <summary>
        /// Ví dụ 1: Kiểm tra quyền truy cập tính năng Export PDF
        /// </summary>
        /// <returns></returns>
        [HttpPost("ExportPdf")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        [FeatureAccess("EXPORT_PDF", "Export", 1)]
        public async Task<IActionResult> ExportPdfReport()
        {
            // Logic xuất PDF ở đây
            // Feature access đã được kiểm tra bởi FeatureAccessAttribute
            
            // Có thể lấy thông tin access result từ HttpContext
            var accessResult = HttpContext.Items["FeatureAccessResult"];
            
            await Task.Delay(1000); // Simulate PDF generation
            
            return Ok(new { 
                message = "PDF exported successfully",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Ví dụ 2: Kiểm tra quota trước khi sử dụng tài nguyên
        /// </summary>
        /// <param name="dataSize">Kích thước dữ liệu cần upload (MB)</param>
        /// <returns></returns>
        [HttpPost("UploadData")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        [ResourceQuotaCheck("CLOUD_STORAGE", "Storage", "dataSize")]
        public async Task<IActionResult> UploadDataToCloud(decimal dataSize)
        {
            // Logic upload dữ liệu ở đây
            // Quota đã được kiểm tra bởi ResourceQuotaCheckAttribute
            
            await Task.Delay(2000); // Simulate upload
            
            return Ok(new { 
                message = $"Successfully uploaded {dataSize}MB to cloud storage",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Ví dụ 3: API với giới hạn số lần gọi
        /// </summary>
        /// <returns></returns>
        [HttpGet("AdvancedAnalytics")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        [FeatureAccess("ADVANCED_ANALYTICS", "API_Call", 1)]
        public async Task<IActionResult> GetAdvancedAnalytics()
        {
            // Logic phân tích nâng cao ở đây
            
            await Task.Delay(3000); // Simulate heavy computation
            
            return Ok(new { 
                analytics = new {
                    totalUsers = 1500,
                    activeUsers = 1200,
                    growthRate = 15.5,
                    predictions = new[] { 1800, 2000, 2200 }
                },
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Ví dụ 4: Tính năng không yêu cầu license
        /// </summary>
        /// <returns></returns>
        [HttpGet("BasicInfo")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        [FeatureAccess("BASIC_INFO", "Access", 0)]
        public IActionResult GetBasicInfo()
        {
            // Tính năng cơ bản không yêu cầu license
            return Ok(new { 
                message = "Basic information access",
                version = "1.0.0",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Ví dụ 5: Batch operation với multiple resource consumption
        /// </summary>
        /// <param name="batchSize">Số lượng items trong batch</param>
        /// <returns></returns>
        [HttpPost("BatchProcess")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        [ResourceQuotaCheck("API_ACCESS", "Batch_Process", "batchSize")]
        public async Task<IActionResult> ProcessBatch(int batchSize)
        {
            // Xử lý batch với resource consumption = batchSize
            
            var results = new List<object>();
            for (int i = 1; i <= batchSize; i++)
            {
                await Task.Delay(100); // Simulate processing each item
                results.Add(new { id = i, processed = true });
            }
            
            return Ok(new { 
                message = $"Successfully processed {batchSize} items",
                results = results,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Controller demo về cách kiểm tra license features thủ công
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManualFeatureCheckController : ControllerBase
    {
        private readonly IFeatureAccessService _featureAccessService;

        public ManualFeatureCheckController(IFeatureAccessService featureAccessService)
        {
            _featureAccessService = featureAccessService;
        }

        /// <summary>
        /// Ví dụ kiểm tra quyền truy cập thủ công
        /// </summary>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        [HttpPost("CheckFeature")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> CheckFeatureManually(string featureCode)
        {
            // Lấy accountId từ claims
            var accountIdClaim = HttpContext.User.FindFirst("AccountId");
            if (accountIdClaim == null || !long.TryParse(accountIdClaim.Value, out long accountId))
            {
                return Unauthorized(new { message = "Invalid user context" });
            }

            // Lấy license key từ header
            var licenseKey = Request.Headers["License-Key"].FirstOrDefault();

            // Kiểm tra quyền truy cập
            var checkRequest = new CheckFeatureAccessRequest
            {
                AccountId = accountId,
                FeatureCode = featureCode,
                LicenseKey = licenseKey,
                UsageType = "Manual_Check",
                ResourceAmount = 0
            };

            var result = await _featureAccessService.CheckFeatureAccessAsync(checkRequest);

            if (result.HasAccess)
            {
                return Ok(new 
                { 
                    hasAccess = true,
                    message = "Feature access granted",
                    limitInfo = result.LimitInfo,
                    licenseFeature = result.LicenseFeature
                });
            }
            else
            {
                return Forbid(new 
                { 
                    hasAccess = false,
                    reason = result.Reason,
                    isLicenseValid = result.IsLicenseValid,
                    isFeatureEnabled = result.IsFeatureEnabled
                }.ToString());
            }
        }

        /// <summary>
        /// Ví dụ ghi nhận usage thủ công
        /// </summary>
        /// <param name="featureCode"></param>
        /// <param name="usageType"></param>
        /// <param name="resourceAmount"></param>
        /// <returns></returns>
        [HttpPost("RecordUsage")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> RecordUsageManually(string featureCode, string usageType = "Manual", decimal resourceAmount = 1)
        {
            var accountIdClaim = HttpContext.User.FindFirst("AccountId");
            if (accountIdClaim == null || !long.TryParse(accountIdClaim.Value, out long accountId))
            {
                return Unauthorized();
            }

            var licenseKey = Request.Headers["License-Key"].FirstOrDefault() ?? "";

            var result = await _featureAccessService.RecordFeatureUsageAsync(
                accountId, 
                licenseKey, 
                featureCode, 
                usageType, 
                resourceAmount,
                System.Text.Json.JsonSerializer.Serialize(new { 
                    manual = true, 
                    timestamp = DateTime.UtcNow,
                    ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                })
            );

            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }
            else
            {
                return BadRequest(new { message = result.Message });
            }
        }
    }
}
