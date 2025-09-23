using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AccountDevice;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class AccountController : BaseBusinessController<IAccountService, Account, AccountDTO>
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Lấy account theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("GetAccountByUsername")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountByUsername(string username)
        {
            var account = await Service.GetAccountByUsername(username);
            if (account == null)
            {
                ResOutput.ErrorEventHandler("Account không tồn tại");
                return NotFound(ResOutput);
            }
            ResOutput.SuccessEventHandler(account);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.ChangePassword(request.Id, request.NewPassword);
            return Ok(result);
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("LockAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> LockAccount([FromBody] LockAccountRequest request)
        {
            var result = await Service.LockAccount(request.Id, request.Reason);
            return Ok(result);
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("UnlockAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UnlockAccount(long id)
        {
            var result = await Service.UnlockAccount(id);
            return Ok(result);
        }

        /// <summary>
        /// Kích hoạt tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("ActivateAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> ActivateAccount(long id)
        {
            var result = await Service.ActivateAccount(id);
            return Ok(result);
        }

        /// <summary>
        /// Vô hiệu hóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("DeactivateAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> DeactivateAccount(long id)
        {
            var result = await Service.DeactivateAccount(id);
            return Ok(result);
        }

        /// <summary>
        /// Lấy accounts đã hết hạn
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExpiredAccounts")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetExpiredAccounts()
        {
            var accounts = await Service.GetExpiredAccounts();
            ResOutput.SuccessEventHandler(accounts);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy accounts sắp hết hạn
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        [HttpGet("GetExpiringAccounts")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetExpiringAccounts(int days = 30)
        {
            var accounts = await Service.GetExpiringAccounts(days);
            ResOutput.SuccessEventHandler(accounts);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Gia hạn account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ExtendAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> ExtendAccount([FromBody] ExtendAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.ExtendAccount(request.Id, request.NewExpiryDate);
            return Ok(result);
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Models.DTO.Account.LoginRequest request)
        {
            var result = await Service.Login(request);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("UpdateAccountInfo")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> UpdateAccountInfo([FromBody] UpdateAccountInfoRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.UpdateAccountInfo(request);
            return Ok(result);
        }

        /// <summary>
        /// Upload avatar
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UploadAvatar")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> UploadAvatar([FromBody] UploadAvatarRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.UploadAvatar(request.Id, request.AvatarPath);
            return Ok(result);
        }

        #region AccountDevice Endpoints

        /// <summary>
        /// Lấy tất cả account devices
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllAccountDevices")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAllAccountDevices()
        {
            var devices = await Service.GetAllAccountDevices();
            ResOutput.SuccessEventHandler(devices);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy devices theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetAccountDevicesByAccountId")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountDevicesByAccountId(long accountId)
        {
            var devices = await Service.GetAccountDevicesByAccountId(accountId);
            ResOutput.SuccessEventHandler(devices);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy device theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetAccountDeviceById")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountDeviceById(long id)
        {
            var device = await Service.GetAccountDeviceById(id);
            if (device == null)
            {
                ResOutput.ErrorEventHandler("Device không tồn tại");
                return NotFound(ResOutput);
            }
            ResOutput.SuccessEventHandler(device);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Đăng ký device mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("RegisterDevice")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.RegisterDevice(request);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật device
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("UpdateDevice")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> UpdateDevice([FromBody] UpdateDeviceRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.UpdateDevice(request);
            return Ok(result);
        }

        /// <summary>
        /// Xóa device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteDevice")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> DeleteDevice(long id)
        {
            var result = await Service.DeleteDevice(id);
            return Ok(result);
        }

        /// <summary>
        /// Kích hoạt device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("ActivateDevice")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> ActivateDevice(long id)
        {
            var result = await Service.ActivateDevice(id);
            return Ok(result);
        }

        /// <summary>
        /// Vô hiệu hóa device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("DeactivateDevice")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> DeactivateDevice(long id)
        {
            var result = await Service.DeactivateDevice(id);
            return Ok(result);
        }

        /// <summary>
        /// Lấy devices đang hoạt động của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetActiveDevices")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetActiveDevices(long accountId)
        {
            var devices = await Service.GetActiveDevices(accountId);
            ResOutput.SuccessEventHandler(devices);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy devices theo loại
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        [HttpGet("GetDevicesByType")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetDevicesByType(string deviceType)
        {
            var devices = await Service.GetDevicesByType(deviceType);
            ResOutput.SuccessEventHandler(devices);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Kiểm tra device đã đăng ký chưa
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("IsDeviceRegistered")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> IsDeviceRegistered(string deviceId, long accountId)
        {
            var isRegistered = await Service.IsDeviceRegistered(deviceId, accountId);
            ResOutput.SuccessEventHandler(isRegistered);
            return Ok(ResOutput);
        }

        #endregion
    }
}
