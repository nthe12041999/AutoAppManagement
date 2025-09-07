using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.ToolFeature;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolFeatureController : BaseBusinessController<IToolFeatureService, ToolFeature, ToolFeatureDTO>
    {
        public ToolFeatureController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Tạo tính năng tool mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateToolFeature([FromBody] CreateToolFeatureRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.CreateToolFeatureAsync(request);
            if (result.IsSuccess)
            {
                ResOutput.SuccessEventHandler(result.Data);
            }
            else
            {
                ResOutput.ErrorEventHandler(result.Message);
            }
            return Ok(ResOutput);
        }

        /// <summary>
        /// Cập nhật tính năng tool
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UpdateToolFeature([FromBody] UpdateToolFeatureRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.UpdateToolFeatureAsync(request);
            if (result.IsSuccess)
            {
                ResOutput.SuccessEventHandler(result.Data);
            }
            else
            {
                ResOutput.ErrorEventHandler(result.Message);
            }
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy tính năng theo mã
        /// </summary>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        [HttpGet("GetByCode/{featureCode}")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> GetFeatureByCode(string featureCode)
        {
            var feature = await _service.GetFeatureByCodeAsync(featureCode);
            if (feature != null)
            {
                ResOutput.SuccessEventHandler(feature);
            }
            else
            {
                ResOutput.ErrorEventHandler("Không tìm thấy tính năng");
            }
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy tính năng theo danh mục
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("GetByCategory/{category}")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> GetFeaturesByCategory(string category)
        {
            var features = await _service.GetFeaturesByCategoryAsync(category);
            ResOutput.SuccessEventHandler(features);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy tính năng theo loại
        /// </summary>
        /// <param name="featureType"></param>
        /// <returns></returns>
        [HttpGet("GetByType/{featureType}")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> GetFeaturesByType(string featureType)
        {
            var features = await _service.GetFeaturesByTypeAsync(featureType);
            ResOutput.SuccessEventHandler(features);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Kiểm tra mã tính năng có tồn tại không
        /// </summary>
        /// <param name="featureCode"></param>
        /// <param name="excludeId"></param>
        /// <returns></returns>
        [HttpGet("CheckFeatureCodeExists")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CheckFeatureCodeExists(string featureCode, long? excludeId = null)
        {
            var exists = await _service.IsFeatureCodeExistsAsync(featureCode, excludeId);
            ResOutput.SuccessEventHandler(new { exists });
            return Ok(ResOutput);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LicenseFeatureController : BaseBusinessController<ILicenseFeatureService, LicenseFeature, LicenseFeatureDTO>
    {
        public LicenseFeatureController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Gán tính năng cho license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AssignFeature")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignFeatureToLicense([FromBody] AssignFeatureToLicenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.AssignFeatureToLicenseAsync(request);
            if (result.IsSuccess)
            {
                ResOutput.SuccessEventHandler(result.Data);
            }
            else
            {
                ResOutput.ErrorEventHandler(result.Message);
            }
            return Ok(ResOutput);
        }

        /// <summary>
        /// Xóa tính năng khỏi license
        /// </summary>
        /// <param name="licenseId"></param>
        /// <param name="toolFeatureId"></param>
        /// <returns></returns>
        [HttpDelete("RemoveFeature")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> RemoveFeatureFromLicense(long licenseId, long toolFeatureId)
        {
            var result = await _service.RemoveFeatureFromLicenseAsync(licenseId, toolFeatureId);
            if (result.IsSuccess)
            {
                ResOutput.SuccessEventHandler(result.Message);
            }
            else
            {
                ResOutput.ErrorEventHandler(result.Message);
            }
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy danh sách tính năng của license
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        [HttpGet("GetFeaturesByLicense/{licenseId}")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> GetFeaturesByLicense(long licenseId)
        {
            var features = await _service.GetFeaturesByLicenseAsync(licenseId);
            ResOutput.SuccessEventHandler(features);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy danh sách license có tính năng
        /// </summary>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        [HttpGet("GetLicensesByFeature/{featureCode}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetLicensesByFeature(string featureCode)
        {
            var licenses = await _service.GetLicensesByFeatureAsync(featureCode);
            ResOutput.SuccessEventHandler(licenses);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Kiểm tra tính năng có được bật cho license không
        /// </summary>
        /// <param name="licenseId"></param>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        [HttpGet("CheckFeatureEnabled")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> CheckFeatureEnabled(long licenseId, string featureCode)
        {
            var isEnabled = await _service.IsFeatureEnabledForLicenseAsync(licenseId, featureCode);
            ResOutput.SuccessEventHandler(new { isEnabled });
            return Ok(ResOutput);
        }

        /// <summary>
        /// Cập nhật cấu hình tính năng cho license
        /// </summary>
        /// <param name="licenseId"></param>
        /// <param name="toolFeatureId"></param>
        /// <param name="resourceLimits"></param>
        /// <param name="usageQuota"></param>
        /// <returns></returns>
        [HttpPut("UpdateFeatureConfig")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UpdateLicenseFeature(long licenseId, long toolFeatureId, [FromBody] string? resourceLimits = null, string? usageQuota = null)
        {
            var result = await _service.UpdateLicenseFeatureAsync(licenseId, toolFeatureId, resourceLimits, usageQuota);
            if (result.IsSuccess)
            {
                ResOutput.SuccessEventHandler(result.Data);
            }
            else
            {
                ResOutput.ErrorEventHandler(result.Message);
            }
            return Ok(ResOutput);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FeatureAccessController : ControllerBase
    {
        private readonly IFeatureAccessService _featureAccessService;
        protected ResponseOutput<object> ResOutput = new ResponseOutput<object>();

        public FeatureAccessController(IFeatureAccessService featureAccessService)
        {
            _featureAccessService = featureAccessService;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập tính năng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CheckAccess")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> CheckFeatureAccess([FromBody] CheckFeatureAccessRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _featureAccessService.CheckFeatureAccessAsync(request);
            ResOutput.SuccessEventHandler(result);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Ghi nhận việc sử dụng tính năng
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="licenseKey"></param>
        /// <param name="featureCode"></param>
        /// <param name="usageType"></param>
        /// <param name="resourceAmount"></param>
        /// <param name="usageData"></param>
        /// <returns></returns>
        [HttpPost("RecordUsage")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> RecordFeatureUsage(
            long accountId, 
            string licenseKey, 
            string featureCode, 
            string usageType = "Access", 
            decimal resourceAmount = 1, 
            [FromBody] string? usageData = null)
        {
            var result = await _featureAccessService.RecordFeatureUsageAsync(accountId, licenseKey, featureCode, usageType, resourceAmount, usageData);
            if (result.IsSuccess)
            {
                ResOutput.SuccessEventHandler(result.Message);
            }
            else
            {
                ResOutput.ErrorEventHandler(result.Message);
            }
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy báo cáo sử dụng tính năng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetUsageReport")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetUsageReport([FromBody] FeatureUsageReportRequest request)
        {
            var reports = await _featureAccessService.GetUsageReportAsync(request);
            ResOutput.SuccessEventHandler(reports);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Kiểm tra có vượt quá giới hạn sử dụng không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="licenseId"></param>
        /// <param name="toolFeatureId"></param>
        /// <param name="usageType"></param>
        /// <param name="requestedAmount"></param>
        /// <returns></returns>
        [HttpGet("CheckUsageLimits")]
        [Roles(RoleConstant.Admin, RoleConstant.Customer)]
        public async Task<IActionResult> CheckUsageLimits(long accountId, long licenseId, long toolFeatureId, string usageType, decimal requestedAmount)
        {
            var isWithinLimits = await _featureAccessService.IsWithinUsageLimitsAsync(accountId, licenseId, toolFeatureId, usageType, requestedAmount);
            ResOutput.SuccessEventHandler(new { isWithinLimits });
            return Ok(ResOutput);
        }
    }
}
