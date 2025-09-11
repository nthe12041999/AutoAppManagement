using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class LicenseController : BaseBusinessController<ILicenseService, License, LicenseDTO>
    {
        public LicenseController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Lấy license theo account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetLicensesByAccountId")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetLicensesByAccountId(long accountId)
        {
            var licenses = await _service.GetLicensesByAccountId(accountId);
            ResOutput.SuccessEventHandler(licenses);
            return Ok(ResOutput);
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
            var license = await _service.GetLicenseByKey(licenseKey);
            if (license == null)
            {
                ResOutput.ErrorEventHandler("License không tồn tại");
                return NotFound(ResOutput);
            }
            ResOutput.SuccessEventHandler(license);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Gán license cho user/account (cách 1: sử dụng Account.LicenseId)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AssignLicenseToAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignLicenseToAccount([FromBody] AssignLicenseToAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.AssignLicenseToAccount(request);
            return Ok(result);
        }

        /// <summary>
        /// Gán license cho user (cách 2: sử dụng bảng LicenseUser)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AssignLicenseToUser")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignLicenseToUser([FromBody] AssignLicenseToUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.AssignLicenseToUser(request);
            return Ok(result);
        }

        /// <summary>
        /// Hủy gán license khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPost("UnassignLicenseFromAccount/{accountId}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UnassignLicenseFromAccount(long accountId)
        {
            var result = await _service.UnassignLicenseFromAccount(accountId);
            return Ok(result);
        }

        /// <summary>
        /// Hủy gán license khỏi user (LicenseUser table)
        /// </summary>
        /// <param name="licenseUserId"></param>
        /// <returns></returns>
        [HttpDelete("UnassignLicenseFromUser/{licenseUserId}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UnassignLicenseFromUser(long licenseUserId)
        {
            var result = await _service.UnassignLicenseFromUser(licenseUserId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách user được gán license
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        [HttpGet("GetUsersAssignedToLicense/{licenseId}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetUsersAssignedToLicense(long licenseId)
        {
            var users = await _service.GetUsersAssignedToLicense(licenseId);
            ResOutput.SuccessEventHandler(users);
            return Ok(ResOutput);
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
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.RenewLicense(request);
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
            var result = await _service.SuspendLicense(id);
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
            var result = await _service.ActivateLicense(id);
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
            var licenses = await _service.GetExpiredLicenses();
            ResOutput.SuccessEventHandler(licenses);
            return Ok(ResOutput);
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
            var licenses = await _service.GetExpiringLicenses(days);
            ResOutput.SuccessEventHandler(licenses);
            return Ok(ResOutput);
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
            var result = await _service.ExtendLicense(id, newExpiryDate);
            return Ok(result);
        }
    }
}
