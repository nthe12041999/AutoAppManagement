using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class LicenseController : BaseController
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(IRestOutput res, IHttpContextAccessor httpContextAccessor,
                               ILicenseService licenseService) : base(res, httpContextAccessor)
        {
            _licenseService = licenseService;
        }

        /// <summary>
        /// Lấy license theo account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetLicensesByAccountId")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetLicensesByAccountId(long accountId)
        {
            var licenses = await _licenseService.GetLicensesByAccountId(accountId);
            _res.SuccessEventHandler(licenses);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy license theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetLicenseById")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetLicenseById(long id)
        {
            var license = await _licenseService.GetLicenseById(id);
            if (license == null)
            {
                _res.ErrorEventHandler("License không tồn tại");
                return NotFound(_res);
            }
            _res.SuccessEventHandler(license);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy license theo key
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <returns></returns>
        [HttpGet("GetLicenseByKey")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetLicenseByKey(string licenseKey)
        {
            var license = await _licenseService.GetLicenseByKey(licenseKey);
            if (license == null)
            {
                _res.ErrorEventHandler("License không tồn tại");
                return NotFound(_res);
            }
            _res.SuccessEventHandler(license);
            return Ok(_res);
        }

        /// <summary>
        /// Tạo license mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateLicense")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _licenseService.CreateLicense(request);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("UpdateLicense")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UpdateLicense([FromBody] UpdateLicenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _licenseService.UpdateLicense(request);
            return Ok(result);
        }

        /// <summary>
        /// Xóa license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteLicense")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> DeleteLicense(long id)
        {
            var result = await _licenseService.DeleteLicense(id);
            return Ok(result);
        }

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("RenewLicense")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> RenewLicense([FromBody] RenewLicenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _licenseService.RenewLicense(request);
            return Ok(result);
        }

        /// <summary>
        /// Tạm dừng license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("SuspendLicense")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> SuspendLicense(long id)
        {
            var result = await _licenseService.SuspendLicense(id);
            return Ok(result);
        }

        /// <summary>
        /// Kích hoạt license
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("ActivateLicense")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> ActivateLicense(long id)
        {
            var result = await _licenseService.ActivateLicense(id);
            return Ok(result);
        }

        /// <summary>
        /// Lấy license đã hết hạn
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExpiredLicenses")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetExpiredLicenses()
        {
            var licenses = await _licenseService.GetExpiredLicenses();
            _res.SuccessEventHandler(licenses);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy license sắp hết hạn
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        [HttpGet("GetExpiringLicenses")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetExpiringLicenses(int days = 30)
        {
            var licenses = await _licenseService.GetExpiringLicenses(days);
            _res.SuccessEventHandler(licenses);
            return Ok(_res);
        }

        /// <summary>
        /// Kiểm tra license hợp lệ
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <returns></returns>
        [HttpGet("ValidateLicense")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> ValidateLicense(string licenseKey)
        {
            var isValid = await _licenseService.ValidateLicense(licenseKey);
            _res.SuccessEventHandler(isValid);
            return Ok(_res);
        }

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newExpiryDate"></param>
        /// <returns></returns>
        [HttpPost("ExtendLicense")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> ExtendLicense(long id, DateTime newExpiryDate)
        {
            var result = await _licenseService.ExtendLicense(id, newExpiryDate);
            return Ok(result);
        }
    }
}
