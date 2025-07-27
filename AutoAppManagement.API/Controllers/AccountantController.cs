using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Service.Services;
using AutoAppManagement.WebApp.Common.Attribute;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountsService _accountsService;

        public AccountController(IRestOutput res, IHttpContextAccessor httpContextAccessor,
                                    IAccountsService accountsService) : base(res, httpContextAccessor)
        {
            _accountsService = accountsService;

        }

        /// <summary>
        /// Hàm xử lý đăng nhập
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="loginInfor"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginInfor)
        {
            try
            {
                var userResult = await _accountsService.Login(loginInfor.UserName, loginInfor.Password);
                if (userResult != null && !string.IsNullOrEmpty(userResult.Token))
                {
                    _res.SuccessEventHandler(userResult);
                }
            }
            catch (Exception ex)
            {
                _res.ErrorEventHandler(null, ex.Message);
            }

            return Ok(_res);
        }

        /// <summary>
        /// Hàm xử lý đăng ký
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(AccountRegister acc)
        {
            if (acc != null)
            {
                var isSuccess = await _accountsService.Register(acc);
                if (isSuccess)
                {
                    _res.SuccessEventHandler("Đăng ký thành công");
                }
                else
                {
                    _res.ErrorEventHandler();
                }

            }
            return Ok(_res);
        }

        /// <summary>
        /// Hàm xử lý lấy thông tin user
        /// CreatedBy ntthe 03.03.2024
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpGet("GetUserInforGeneric")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> GetUserInforGeneric()
        {
            var userData = await _accountsService.GetUserInforGeneric();
            if (userData != null)
            {
                _res.SuccessEventHandler(userData);
            }
            return Ok(_res);
        }

        [HttpGet("GetAllAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAllAccount()
        {
            var userData = await _accountsService.GetAllAccount();
            if (userData != null)
            {
                _res.SuccessEventHandler(userData);
            }
            return Ok(_res);
        }

        /// <summary>
        /// Hàm xử lý cập nhật thông tin user
        /// CreatedBy ntthe 03.03.2024
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpPost("UpdateUserInfor")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> UpdateUserInfor(AccountUpdate acc)
        {
            _res = await _accountsService.UpdateUserInfor(acc);
            return Ok(_res);
        }

        /// <summary>
        /// Hàm xử lý cập nhật password
        /// CreatedBy ntthe 03.03.2024
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> ChangePassword(ChangPasswordVM password)
        {
            _res = await _accountsService.ChangePassword(password.NewPassword, password.OldPassword);
            return Ok(_res);
        }

        /// <summary>
        /// Hàm xử lấy thông tin user
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpPost("GetUserInfor")]
        [Roles(RoleConstant.Customer)]
        public IActionResult GetUserInfor()
        {
            var userInfor = _accountsService.GetUserInfor();
            _res.SuccessEventHandler(userInfor);
            return Ok(_res);
        }

        [HttpPost("UpdateLockedAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UpdateLockedAccount(LockedAccountParam param)
        {
            var result = await _accountsService.UpdateLockedAccount(param);
            if (result)
            {
                _res.SuccessEventHandler("Cập nhật trạng thái khóa tài khoản thành công");
            }
            else
            {
                _res.ErrorEventHandler("Cập nhật trạng thái khóa tài khoản thất bại");
            }

            return Ok(_res);
        }
    }
}
