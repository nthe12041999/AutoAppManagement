using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class AccountController : BaseBusinessController<IAccountService, Account, AccountDTO>
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        #region Quản lý thông tin cho admin

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("LockAccount")]
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
        public async Task<IActionResult> UnlockAccount(long id)
        {
            var result = await Service.UnlockAccount(id);
            return Ok(result);
        }

        #endregion

        #region Đăng nhập (quản lý token) - Phần dành cho call từ bên tool khách hàng

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
        /// Làm mới AccessToken bằng RefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers["User-Agent"].ToString();
            var result = await Service.RefreshTokenAsync(refreshToken, ip, ua);
            return Ok(result);
        }

        /// <summary>
        /// Thu hồi tất cả refresh token của 1 tài khoản (dùng khi đổi/đến hạn license)
        /// </summary>
        [HttpPost("RevokeAllRefreshTokens")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> RevokeAllRefreshTokens(long accountId)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await Service.RevokeAllTokensForAccount(accountId, ip);
            return Ok(result);
        }

        /// <summary>
        /// Thu hồi refresh token của device hiện tại (lấy thông tin từ token authentication)
        /// </summary>
        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            var result = await Service.RevokeToken();
            return Ok(result);
        }

        #endregion

        #region Mật khẩu (lấy, đổi) - Phần dành cho call từ bên tool khách hàng

        /// <summary>
        /// Gửi mã OTP cho việc đổi mật khẩu (lấy accountId từ token)
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendOtpForChangePassword")]
        public async Task<IActionResult> SendOtpForChangePassword()
        {
            var result = await Service.SendOtpForChangePassword();
            return Ok(result);
        }

        /// <summary>
        /// Đổi mật khẩu với xác thực OTP (verify OTP + đổi mật khẩu trong 1 API)
        /// </summary>
        /// <param name="request">Bao gồm: AccountId, OldPassword, NewPassword, Otp</param>
        /// <returns></returns>
        [HttpPost("ChangePasswordWithOtp")]
        public async Task<IActionResult> ChangePasswordWithOtp([FromBody] Models.DTO.Verification.ChangePasswordWithOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.ChangePasswordWithOtp(request);
            return Ok(result);
        }

        /// <summary>
        /// Quên mật khẩu - Gửi OTP đến email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.ForgotPassword(request.EmailOrPhone);
            return Ok(result);
        }

        /// <summary>
        /// Xác nhận OTP và reset mật khẩu - Gửi mật khẩu mới qua email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ConfirmOtpResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmOtpResetPassword([FromBody] ConfirmOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.ConfirmOtpResetPassword(request.Email, request.Otp);
            return Ok(result);
        }

        /// <summary>
        /// Gửi lại mã OTP cho reset password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ResendOtpForResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtpForResetPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await Service.ResendOtp(request.EmailOrPhone);
            return Ok(result);
        }

        /// <summary>
        /// Gửi lại mã OTP cho change password (lấy accountId từ token)
        /// </summary>
        /// <returns></returns>
        [HttpPost("ResendOtpForChangePassword")]
        public async Task<IActionResult> ResendOtpForChangePassword()
        {
            var result = await Service.ResendOtpForChangePassword();
            return Ok(result);
        }

        #endregion
    }
}
