using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoAppManagement.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountsService _accountsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(RestOutput res, IAccountsService accountsService, IHttpContextAccessor httpContextAccessor) : base(res)
        {
            _accountsService = accountsService;
            _httpContextAccessor = httpContextAccessor;
        }
        #region Login 
        public IActionResult Login(string ReturnUrl = "/Home/Index")
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login(LoginViewModel loginViewModel)
        {
            var account = await _accountsService.Login(loginViewModel);
            if (account.IsSuccess && !string.IsNullOrEmpty(account?.Data?.Token))
            {
                var data = new AccountGenericDTO
                {
                    UserName = loginViewModel.UserName,
                    ImgAvatar = account?.Data?.ImgAvatar,
                    Email = account?.Data?.Email,
                    Token = account?.Data?.Token,
                    RoleList = account?.Data?.RoleList
                };
                await HandleSignInWithCookieSign(data, account.Data.Token);
                _res.SuccessEventHandler(data);
            }
            else
            {
                _res.ErrorEventHandler(message: account?.Message);
            }
            return Json(_res);
        }

        //[Authorize]
        //[HttpPost]
        //public JsonResult GetInforUser()
        //{
        //    var userName = HttpContext.User.FindFirst("UserName")?.Value;
        //    if (userName != null)
        //    {
        //        var acc = _accountsService.GetUserInforGeneric(userName);
        //        if (acc != null)
        //        {
        //            _res.Data = new AccountGenericDTO
        //            {
        //                UserName = userName,
        //                Coin = acc.Coin
        //            };
        //        }
        //        else
        //        {
        //            _res.ErrorEventHandler();
        //        }
        //    }
        //    else
        //    {
        //        _res.ErrorEventHandler();
        //    }

        //    return Json(_res);
        //}

        #endregion

        #region Logout

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // SignOut
            await _httpContextAccessor?.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _res.SuccessEventHandler("Đăng xuất thành công");
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Register

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Register(AccountRegister accRegister)
        {
            if (accRegister != null)
            {
                var dataRes = await _accountsService.Register(accRegister);
                if (dataRes?.Data != null)
                {
                    // đăng ký thành công thì gọi đăng nhập luôn
                    var loginViewModel = new LoginViewModel
                    {
                        UserName = accRegister.UserName,
                        Password = accRegister.Password
                    };
                    return await Login(loginViewModel);
                }
            }
            return Json(_res);
        }
        #endregion

        #region Language

        [HttpPost]
        public JsonResult SelectLanguage(string languageCode)
        {
            // TODO: tí xử lý check đăng nhập ở đây trước xong insert, chưa đăng nhập thì lưu luôn cookie
            Response.Cookies.Append("Language", languageCode, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1), // Cookie hết hạn sau 1 năm
            });
            _res.SuccessEventHandler();
            return Json(_res);
        }

        #endregion

        #region Private Mehthod

        #region Luồng đăng nhập với cookie
        /// <summary>
        /// Hàm xử lý sign cho đăng nhập với loại đăng nhập là cookie
        /// </summary>
        /// <param name="account"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task HandleSignInWithCookieSign(AccountGenericDTO account, string token)
        {
            var authenObject = SignClaimWriteToken(account, token);
            if (authenObject != null)
            {
                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                            new ClaimsPrincipal(authenObject.ClaimsIdentity),
                                                            authenObject.AuthProperties);
            }
        }

        /// <summary>
        /// Hàm ghi thông tin vào claim
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private AccountAuthenDTO SignClaimWriteToken(AccountGenericDTO account, string token)
        {
            var listClaim = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimsNamesConstant.Sid, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimsNamesConstant.Sub, account.UserName ?? ""),
                    new Claim(JwtRegisteredClaimsNamesConstant.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimsNamesConstant.Token, token),
                    new Claim(JwtRegisteredClaimsNamesConstant.Avatar, account.ImgAvatar ?? ""),
                    new Claim(JwtRegisteredClaimsNamesConstant.Email, account.Email ?? ""),
                };

            if (account.RoleList.Any())
            {
                for (int i = 0; i < account.RoleList.Count; i++)
                {
                    listClaim.Add(new Claim(ClaimTypes.Role, account.RoleList[i]));
                }
            }

            var claimsIdentity = new ClaimsIdentity(listClaim, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                // Thông tin cấu hình cookie, ví dụ như thời gian hết hạn
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                IsPersistent = false, // Đặt true nếu bạn muốn cookie "nhớ đăng nhập" qua các session
                AllowRefresh = false
            };

            return new AccountAuthenDTO
            {
                ClaimsIdentity = claimsIdentity,
                AuthProperties = authProperties,
                UserName = account.UserName,
            };
        }


        #endregion

        #endregion
    }
}