using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class AdminAccountController : BaseBusinessController<IAdminAccountService, AdminAccount, AdminAccountDTO>
    {
        public AdminAccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Đăng nhập admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Models.DTO.AdminAccount.LoginRequest request)
        {
            var result = await Service.Login(request.Username, request.Password, 
                HttpContext.Connection.RemoteIpAddress?.ToString(), 
                HttpContext.Request.Headers["User-Agent"].ToString(),
                request.RememberMe);
            
            if (result == null || !result.IsSuccess)
                return BadRequest(result ?? new ResponseOutput<TokenViewModel> { IsSuccess = false, Message = "Đăng nhập thất bại" });

            // Set RefreshToken vào HTTP-only cookie
            if (result.Data?.RefreshToken != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax, // Development: Lax, Production: Strict hoặc None
                    Secure = true, // Production: HTTPS required
                    Path = "/",
                    Expires = result.Data.RefreshTokenExpired
                };
                HttpContext.Response.Cookies.Append("refreshToken", result.Data.RefreshToken, cookieOptions);
                
                // Xóa refreshToken và refreshTokenExpired khỏi response (đã lưu trong cookie)
                result.Data.RefreshToken = null;
                result.Data.RefreshTokenExpired = default;
            }
                
            return Ok(result);
        }

        /// <summary>
        /// Làm mới AccessToken bằng RefreshToken (từ HTTP-only cookie)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            // Debug: Log all cookies
            Console.WriteLine($"🍪 Cookies count: {Request.Cookies.Count}");
            foreach (var cookie in Request.Cookies)
            {
                Console.WriteLine($"   - {cookie.Key}: {cookie.Value.Substring(0, Math.Min(20, cookie.Value.Length))}...");
            }
            
            // Đọc refreshToken từ cookie
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                Console.WriteLine("❌ RefreshToken cookie not found!");
                return Unauthorized(new ResponseOutput<TokenViewModel> 
                { 
                    IsSuccess = false, 
                    Message = "Không tìm thấy refresh token" 
                });
            }

            Console.WriteLine($"✅ RefreshToken found: {refreshToken.Substring(0, Math.Min(20, refreshToken.Length))}...");
            
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers["User-Agent"].ToString();
            var result = await Service.RefreshTokenAsync(refreshToken, ip, ua);
            
            if (result == null || !result.IsSuccess)
                return Unauthorized(result ?? new ResponseOutput<TokenViewModel> 
                { 
                    IsSuccess = false, 
                    Message = "Refresh token không hợp lệ" 
                });

            // Set RefreshToken mới vào HTTP-only cookie
            if (result.Data?.RefreshToken != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = true,
                    Path = "/",
                    Expires = result.Data.RefreshTokenExpired
                };
                HttpContext.Response.Cookies.Append("refreshToken", result.Data.RefreshToken, cookieOptions);
                
                // Xóa refreshToken và refreshTokenExpired khỏi response
                result.Data.RefreshToken = null;
                result.Data.RefreshTokenExpired = default;
            }

            return Ok(result);
        }

        /// <summary>
        /// Đăng xuất - Xóa RefreshToken cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Xóa refreshToken cookie
            Response.Cookies.Delete("refreshToken");
            
            return Ok(new ResponseOutput<bool> 
            { 
                IsSuccess = true, 
                Message = "Đăng xuất thành công",
                Data = true
            });
        }

        /// <summary>
        /// Lấy danh sách permissions của user hiện tại
        /// </summary>
        [HttpGet("GetMyPermissions")]
        public IActionResult GetMyPermissions()
        {
            try
            {
                // Lấy permissions từ claims trong JWT token
                var permissions = User.Claims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .Distinct()
                    .ToList();

                return Ok(new ResponseOutput<List<string>>
                {
                    IsSuccess = true,
                    Message = "Lấy permissions thành công",
                    Data = permissions
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseOutput<List<string>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = new List<string>()
                });
            }
        }
    }
}
