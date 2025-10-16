using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAccountController : BaseBusinessController<IAdminAccountService, AdminAccount, AdminAccountDTO>
    {
        public AdminAccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Đăng nhập admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _service.Login(request.Username, request.Password, 
                HttpContext.Connection.RemoteIpAddress?.ToString(), 
                HttpContext.Request.Headers["User-Agent"].ToString());
            
            if (result == null)
                return BadRequest(new { success = false, message = "Đăng nhập thất bại" });
                
            return Ok(new { success = true, data = result });
        }

        [HttpGet("GetAccountsByRole/{roleName}")]
        public async Task<IActionResult> GetAccountsByRole(string roleName)
        {
            var result = await _service.GetAccountsByRole(roleName);
            return Ok(new { success = true, data = result });
        }
    }

    // DTOs for requests
    public class ChangePasswordRequest
    {
        public long Id { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }

    public class LockAccountRequest  
    {
        public long Id { get; set; }
        public int Minutes { get; set; } = 30;
        public string Reason { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
