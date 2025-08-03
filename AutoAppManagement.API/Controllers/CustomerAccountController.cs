using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.DTO.CustomerAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAccountController : BaseController
    {
        private readonly ICustomerAccountService _customerAccountService;

        public CustomerAccountController(
            ICustomerAccountService customerAccountService,
            IRestOutput res,
            IHttpContextAccessor httpContextAccessor
        )
            : base(res, httpContextAccessor)
        {
            _customerAccountService = customerAccountService;
        }

        /// <summary>
        /// Đăng nhập với thông tin device
        /// </summary>
        /// <param name="loginDto">Thông tin đăng nhập</param>
        /// <returns>Kết quả đăng nhập</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ResponseOutput<string>
                        {
                            IsSuccess = false,
                            Message = "Dữ liệu không hợp lệ",
                            Data = string.Join(
                                ", ",
                                ModelState
                                    .Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                            ),
                        }
                    );
                }

                var deviceInfo = new DeviceInfo
                {
                    DeviceName = loginDto.DeviceInfo.DeviceName,
                    DeviceType = loginDto.DeviceInfo.DeviceType,
                    OperatingSystem = loginDto.DeviceInfo.OperatingSystem,
                    OSVersion = loginDto.DeviceInfo.OSVersion,
                    BrowserInfo = loginDto.DeviceInfo.BrowserInfo,
                };

                var result = await _customerAccountService.LoginWithDevice(
                    loginDto.UserName,
                    loginDto.Password,
                    deviceInfo
                );

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Đăng ký thiết bị mới
        /// </summary>
        /// <param name="registerDto">Thông tin đăng ký thiết bị</param>
        /// <returns>Kết quả đăng ký</returns>
        [HttpPost("register-device")]
        [Authorize]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ResponseOutput<string>
                        {
                            IsSuccess = false,
                            Message = "Dữ liệu không hợp lệ",
                        }
                    );
                }

                var deviceInfo = new DeviceInfo
                {
                    DeviceName = registerDto.DeviceInfo.DeviceName,
                    DeviceType = registerDto.DeviceInfo.DeviceType,
                    OperatingSystem = registerDto.DeviceInfo.OperatingSystem,
                    OSVersion = registerDto.DeviceInfo.OSVersion,
                    BrowserInfo = registerDto.DeviceInfo.BrowserInfo,
                };

                var result = await _customerAccountService.RegisterDevice(
                    registerDto.AccountId,
                    deviceInfo
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Lấy danh sách thiết bị của tài khoản
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <returns>Danh sách thiết bị</returns>
        [HttpGet("devices/{accountId}")]
        [Authorize]
        public async Task<IActionResult> GetAccountDevices(long accountId)
        {
            try
            {
                var result = await _customerAccountService.GetAccountDevices(accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Xóa thiết bị
        /// </summary>
        /// <param name="removeDto">Thông tin thiết bị cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("remove-device")]
        [Authorize]
        public async Task<IActionResult> RemoveDevice([FromBody] RemoveDeviceDTO removeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ResponseOutput<string>
                        {
                            IsSuccess = false,
                            Message = "Dữ liệu không hợp lệ",
                        }
                    );
                }

                var result = await _customerAccountService.RemoveDevice(
                    removeDto.DeviceId,
                    removeDto.AccountId
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Kiểm tra license của tài khoản
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <returns>Thông tin license</returns>
        [HttpGet("license/{accountId}")]
        [Authorize]
        public async Task<IActionResult> CheckAccountLicense(long accountId)
        {
            try
            {
                var result = await _customerAccountService.CheckAccountLicense(accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Tạo license mới
        /// </summary>
        /// <param name="createDto">Thông tin license</param>
        /// <returns>Kết quả tạo license</returns>
        [HttpPost("create-license")]
        [Authorize]
        public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ResponseOutput<string>
                        {
                            IsSuccess = false,
                            Message = "Dữ liệu không hợp lệ",
                        }
                    );
                }

                var licenseRequest = new CreateLicenseRequest
                {
                    LicenseName = createDto.LicenseName,
                    LicenseType = createDto.LicenseType,
                    Description = createDto.Description,
                    MaxDevices = createDto.MaxDevices,
                    MaxUsers = createDto.MaxUsers,
                    StartDate = createDto.StartDate,
                    ExpiryDate = createDto.ExpiryDate,
                    Price = createDto.Price,
                    Currency = createDto.Currency,
                    AllowedFeatures = createDto.AllowedFeatures,
                    UsageLimits = createDto.UsageLimits,
                };

                // Lấy ID người tạo từ token (giả sử có trong claims)
                var createdBy = GetCurrentUserId(); // Cần implement method này

                var result = await _customerAccountService.CreateLicense(
                    createDto.AccountId,
                    licenseRequest,
                    createdBy
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="renewDto">Thông tin gia hạn</param>
        /// <returns>Kết quả gia hạn</returns>
        [HttpPut("renew-license")]
        [Authorize]
        public async Task<IActionResult> RenewLicense([FromBody] RenewLicenseDTO renewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ResponseOutput<string>
                        {
                            IsSuccess = false,
                            Message = "Dữ liệu không hợp lệ",
                        }
                    );
                }

                var updatedBy = GetCurrentUserId(); // Cần implement method này
                var result = await _customerAccountService.RenewLicense(
                    renewDto.LicenseKey,
                    renewDto.NewExpiryDate,
                    updatedBy
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Kiểm tra quyền truy cập
        /// </summary>
        /// <param name="validateDto">Thông tin kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        [HttpPost("validate-access")]
        [Authorize]
        public async Task<IActionResult> ValidateAccess([FromBody] ValidateAccessDTO validateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ResponseOutput<string>
                        {
                            IsSuccess = false,
                            Message = "Dữ liệu không hợp lệ",
                        }
                    );
                }

                var result = await _customerAccountService.ValidateAccess(
                    validateDto.AccountId,
                    validateDto.DeviceId
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi server: {ex.Message}",
                    }
                );
            }
        }

        /// <summary>
        /// Lấy ID người dùng hiện tại từ token
        /// </summary>
        /// <returns>User ID</returns>
        private long GetCurrentUserId()
        {
            // TODO: Implement logic để lấy user ID từ JWT token
            // Ví dụ: return long.Parse(User.FindFirst("UserId")?.Value ?? "0");
            return 1; // Tạm thời return 1 cho test
        }
    }
}
