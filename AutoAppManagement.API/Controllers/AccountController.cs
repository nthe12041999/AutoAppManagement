using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AccountDevice;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IRestOutput res, IHttpContextAccessor httpContextAccessor,
                               IAccountService accountService) : base(res, httpContextAccessor)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Lấy tất cả accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllAccounts")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccounts();
            _res.SuccessEventHandler(accounts);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy account theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetAccountById")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountById(long id)
        {
            var account = await _accountService.GetAccountById(id);
            if (account == null)
            {
                _res.ErrorEventHandler("Account không tồn tại");
                return NotFound(_res);
            }
            _res.SuccessEventHandler(account);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy account theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("GetAccountByUsername")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountByUsername(string username)
        {
            var account = await _accountService.GetAccountByUsername(username);
            if (account == null)
            {
                _res.ErrorEventHandler("Account không tồn tại");
                return NotFound(_res);
            }
            _res.SuccessEventHandler(account);
            return Ok(_res);
        }

        /// <summary>
        /// Tạo account mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.CreateAccount(request);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("UpdateAccount")]
        [Roles(RoleConstant.Customer, RoleConstant.Admin)]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.UpdateAccount(request);
            return Ok(result);
        }

        /// <summary>
        /// Xóa account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> DeleteAccount(long id)
        {
            var result = await _accountService.DeleteAccount(id);
            return Ok(result);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.ChangePassword(request.Id, request.NewPassword);
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
            var result = await _accountService.LockAccount(request.Id, request.Reason);
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
            var result = await _accountService.UnlockAccount(id);
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
            var result = await _accountService.ActivateAccount(id);
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
            var result = await _accountService.DeactivateAccount(id);
            return Ok(result);
        }

        /// <summary>
        /// Lấy accounts theo level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpGet("GetAccountsByLevel")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountsByLevel(int level)
        {
            var accounts = await _accountService.GetAccountsByLevel(level);
            _res.SuccessEventHandler(accounts);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy accounts đã hết hạn
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExpiredAccounts")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetExpiredAccounts()
        {
            var accounts = await _accountService.GetExpiredAccounts();
            _res.SuccessEventHandler(accounts);
            return Ok(_res);
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
            var accounts = await _accountService.GetExpiringAccounts(days);
            _res.SuccessEventHandler(accounts);
            return Ok(_res);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.ExtendAccount(request.Id, request.NewExpiryDate);
            return Ok(result);
        }

        /// <summary>
        /// Kiểm tra tài khoản hợp lệ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ValidateAccount")]
        public async Task<IActionResult> ValidateAccount([FromBody] ValidateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var isValid = await _accountService.ValidateAccount(request.Username, request.Password);
            _res.SuccessEventHandler(isValid);
            return Ok(_res);
        }

        /// <summary>
        /// Đăng nhập bằng email/sdt và password, kiểm tra license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.Login(request);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.UpdateAccountInfo(request);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.UploadAvatar(request.Id, request.AvatarPath);
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
            var devices = await _accountService.GetAllAccountDevices();
            _res.SuccessEventHandler(devices);
            return Ok(_res);
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
            var devices = await _accountService.GetAccountDevicesByAccountId(accountId);
            _res.SuccessEventHandler(devices);
            return Ok(_res);
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
            var device = await _accountService.GetAccountDeviceById(id);
            if (device == null)
            {
                _res.ErrorEventHandler("Device không tồn tại");
                return NotFound(_res);
            }
            _res.SuccessEventHandler(device);
            return Ok(_res);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.RegisterDevice(request);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _accountService.UpdateDevice(request);
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
            var result = await _accountService.DeleteDevice(id);
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
            var result = await _accountService.ActivateDevice(id);
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
            var result = await _accountService.DeactivateDevice(id);
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
            var devices = await _accountService.GetActiveDevices(accountId);
            _res.SuccessEventHandler(devices);
            return Ok(_res);
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
            var devices = await _accountService.GetDevicesByType(deviceType);
            _res.SuccessEventHandler(devices);
            return Ok(_res);
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
            var isRegistered = await _accountService.IsDeviceRegistered(deviceId, accountId);
            _res.SuccessEventHandler(isRegistered);
            return Ok(_res);
        }

        #endregion
    }
}
