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
                HttpContext.Request.Headers["User-Agent"].ToString());
            
            if (result == null || !result.IsSuccess)
                return BadRequest(result ?? new ResponseOutput<TokenViewModel> { IsSuccess = false, Message = "Đăng nhập thất bại" });
                
            return Ok(result);
        }
    }
}
