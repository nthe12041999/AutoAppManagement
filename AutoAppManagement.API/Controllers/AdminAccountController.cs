using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAccountController : ControllerBase
    {
        private readonly IAdminAccountService _adminAccountService;

        public AdminAccountController(IAdminAccountService adminAccountService)
        {
            _adminAccountService = adminAccountService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _adminAccountService.GetAll();
            return Ok(new { success = true, data = result });
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _adminAccountService.GetById(id);
            if (result == null)
                return NotFound(new { success = false, message = "Không tìm thấy admin account" });
            
            return Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Đăng nhập admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _adminAccountService.Login(request.Username, request.Password, 
                HttpContext.Connection.RemoteIpAddress?.ToString(), 
                HttpContext.Request.Headers["User-Agent"].ToString());
            
            if (result == null)
                return BadRequest(new { success = false, message = "Đăng nhập thất bại" });
                
            return Ok(new { success = true, data = result });
        }

        [HttpGet("GetAccountsByRole/{roleName}")]
        public async Task<IActionResult> GetAccountsByRole(string roleName)
        {
            var result = await _adminAccountService.GetAccountsByRole(roleName);
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
