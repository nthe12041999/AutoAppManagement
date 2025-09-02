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
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(IServiceProvider serviceProvider): base(serviceProvider)
        {
            _logger = serviceProvider
                .GetRequiredService<ILogger<AuthController>>();
        }

        /// <summary>
        /// Trang đăng nhập
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập thì redirect về trang chủ
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "Đăng nhập";
            return View();
        }

        /// <summary>
        /// API: Xử lý đăng nhập
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, string callbackUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isSuccess = false, message = "Dữ liệu không hợp lệ" });
                }

                var adminAccountService = _serviceProvider.GetRequiredService<IAdminAccountService>();
                var tokenInfor = await adminAccountService.Login(model);

                if (tokenInfor != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, tokenInfor.AccountInfor.Id.ToString()),
                        new Claim(ClaimTypes.Name, tokenInfor.AccountInfor.UserName ?? ""),
                        new Claim(ClaimTypes.Email, tokenInfor.AccountInfor.Email ?? ""),
                        new Claim("phone", tokenInfor.AccountInfor.PhoneNumber ?? ""),
                        new Claim("fullName", tokenInfor.AccountInfor.FullName ?? ""),
                        new Claim("loginTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = tokenInfor.AccessTokenExpired,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    ResOutput.SuccessEventHandler(
                        new { tokenInfor.AccountInfor },
                        "Đăng nhập thành công"
                    );
                }
                else
                {
                    ResOutput.ErrorEventHandler(message: "Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {UserName}", model.UserName);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra trong quá trình đăng nhập");
            }
            return Json(ResOutput);
        }

        /// <summary>
        /// API: Đăng xuất
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                ResOutput.SuccessEventHandler(true, "Đăng xuất thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi đăng xuất");
            }
            return Json(ResOutput);
        }

        /// <summary>
        /// Trang Access Denied
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Không có quyền truy cập";
            return View();
        }

        /// <summary>
        /// API: Lấy thông tin người dùng hiện tại
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var user = new
                {
                    UserName = User.Identity.Name,
                    UserId = User.FindFirst("UserId")?.Value,
                    FullName = User.FindFirst("FullName")?.Value,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value,
                    IsAuthenticated = User.Identity.IsAuthenticated,
                };

                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return Json(
                    new { success = false, message = "Có lỗi xảy ra khi lấy thông tin người dùng" }
                );
            }
        }

        /// <summary>
        /// API: Kiểm tra quyền truy cập
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult CheckPermission(string permission)
        {
            try
            {
                // TODO: Implement permission checking logic
                var hasPermission = HasPermission(User, permission);
                return Json(new { success = true, data = hasPermission });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission}", permission);
                return Json(new { success = false, message = "Có lỗi xảy ra khi kiểm tra quyền" });
            }
        }

        #region Private Methods
        /// <summary>
        /// Kiểm tra thông tin đăng nhập (tạm thời hardcode)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool IsValidUser(string userName, string password)
        {
            // TODO: Thay thế bằng logic xác thực thực tế
            var validUsers = new Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "superadmin", "super123" },
                { "customer1", "123456" },
                { "customer2", "123456" },
                { "support", "support123" },
            };

            return validUsers.ContainsKey(userName.ToLower())
                && validUsers[userName.ToLower()] == password;
        }

        /// <summary>
        /// Lấy role của user (tạm thời hardcode)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string GetUserRole(string userName)
        {
            // TODO: Thay thế bằng logic lấy role từ database
            return userName.ToLower() switch
            {
                "superadmin" => "super_admin",
                "admin" => "admin",
                "support" => "support",
                _ => "customer",
            };
        }

        /// <summary>
        /// Lấy tên đầy đủ của user (tạm thời hardcode)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string GetFullName(string userName)
        {
            // TODO: Thay thế bằng logic lấy tên từ database
            return userName.ToLower() switch
            {
                "superadmin" => "Super Administrator",
                "admin" => "Administrator",
                "support" => "Support Staff",
                "customer1" => "Nguyễn Văn An",
                "customer2" => "Trần Thị Bình",
                _ => userName,
            };
        }

        /// <summary>
        /// Kiểm tra quyền hạn (tạm thời hardcode)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private bool HasPermission(ClaimsPrincipal user, string permission)
        {
            // TODO: Implement real permission checking
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            return role switch
            {
                "super_admin" => true, // Super admin có tất cả quyền
                "admin" => !permission.StartsWith("super_"), // Admin có hầu hết quyền trừ super
                "support" => permission.StartsWith("view_") || permission.StartsWith("support_"),
                "customer" => permission.StartsWith("customer_"),
                _ => false,
            };
        }
        #endregion
    }
}
