using System.Security.Claims;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, RestOutput res)
            : base(res)
        {
            _logger = logger;
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
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // TODO: Gọi API để xác thực người dùng
                // Tạm thời hardcode để demo
                if (IsValidUser(model.UserName, model.Password))
                {
                    var userRole = GetUserRole(model.UserName);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, userRole),
                        new Claim("UserId", "1"), // TODO: Lấy từ database
                        new Claim("FullName", GetFullName(model.UserName)),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe
                            ? DateTimeOffset.UtcNow.AddDays(30)
                            : DateTimeOffset.UtcNow.AddHours(8),
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    // Redirect URL dựa trên role
                    var redirectUrl = userRole.ToLower() switch
                    {
                        "admin" or "super_admin" => Url.Action("Index", "Home"),
                        "customer" => Url.Action("Dashboard", "Customer"),
                        _ => Url.Action("Index", "Home"),
                    };

                    _res.SuccessEventHandler(
                        new { redirectUrl = redirectUrl, userRole = userRole },
                        "Đăng nhập thành công"
                    );
                }
                else
                {
                    _res.ErrorEventHandler(message: "Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {UserName}", model.UserName);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra trong quá trình đăng nhập");
            }
            return Json(_res);
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
                _res.SuccessEventHandler(true, "Đăng xuất thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi đăng xuất");
            }
            return Json(_res);
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

    #region ViewModels
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class CurrentUserViewModel
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
    #endregion
}
