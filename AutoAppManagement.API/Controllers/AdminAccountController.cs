using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class AdminAccountController : BaseBusinessController<IAdminAccountService, AdminAccount, AdminAccountDTO>
    {
        public AdminAccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Đăng nhập bằng email/sdt và password, kiểm tra license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel request)
        {
            var result = await Service.Login(request.UserName, request.Password);
            return Ok(result);
        }

    }
}
