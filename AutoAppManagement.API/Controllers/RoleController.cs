using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseBusinessController<IRoleService, Role, RoleDTO>
    {
        public RoleController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Lấy roles theo account ID
        /// </summary>
        [HttpGet("GetRolesByAccountId/{accountId}")]
        public async Task<IActionResult> GetRolesByAccountId(long accountId)
        {
            try
            {
                var result = await _service.GetRolesByAccountId(accountId);
                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Assign role cho account
        /// </summary>
        [HttpPost("AssignRoleToAccount")]
        public async Task<IActionResult> AssignRoleToAccount([FromBody] AssignRoleRequest request)
        {
            try
            {
                var result = await _service.AssignRoleToAccount(request);
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(result.Data, result.Message);
                    return Ok(ResOutput);
                }
                else
                {
                    ResOutput.ErrorEventHandler(result.Message);
                    return BadRequest(ResOutput);
                }
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }
    }
}
