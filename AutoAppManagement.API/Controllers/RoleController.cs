using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class RoleController : BaseBusinessController<IRoleService, Role, RoleDTO>
    {
        public RoleController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Lấy Role kèm danh sách Permission
        /// </summary>
        [HttpGet("GetWithPermissions/{id}")]
        [RequirePermission(PermissionCodes.ROLE_VIEW)]
        public async Task<IActionResult> GetWithPermissions(long id)
        {
            var result = await Service.GetWithPermissions(id);
            if (result.IsSuccess)
            {
                ResOutput.SuccessEventHandler(result.Data, result.Message);
                return Ok(ResOutput);
            }
            ResOutput.ErrorEventHandler(result.Message);
            return BadRequest(ResOutput);
        }
    }
}
