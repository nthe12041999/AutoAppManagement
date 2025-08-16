using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRestOutput res, IHttpContextAccessor httpContextAccessor,
                            IRoleService roleService) : base(res, httpContextAccessor)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Lấy tất cả roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllRoles")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRoles();
            _res.SuccessEventHandler(roles);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy role theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetRoleById")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRoleById(long id)
        {
            var role = await _roleService.GetRoleById(id);
            if (role == null)
            {
                _res.ErrorEventHandler("Role không tồn tại");
                return NotFound(_res);
            }
            _res.SuccessEventHandler(role);
            return Ok(_res);
        }

        /// <summary>
        /// Tạo role mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _roleService.CreateRole(request);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("UpdateRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _roleService.UpdateRole(request);
            return Ok(result);
        }

        /// <summary>
        /// Xóa role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> DeleteRole(long id)
        {
            var result = await _roleService.DeleteRole(id);
            return Ok(result);
        }

        /// <summary>
        /// Lấy roles của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetRolesByAccountId")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRolesByAccountId(long accountId)
        {
            var roles = await _roleService.GetRolesByAccountId(accountId);
            _res.SuccessEventHandler(roles);
            return Ok(_res);
        }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AssignRoleToAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignRoleToAccount([FromBody] AssignRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _roleService.AssignRoleToAccount(request);
            return Ok(result);
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete("RemoveRoleFromAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> RemoveRoleFromAccount(long accountId, long roleId)
        {
            var result = await _roleService.RemoveRoleFromAccount(accountId, roleId);
            return Ok(result);
        }

        /// <summary>
        /// Kiểm tra role có tồn tại không
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpGet("CheckRoleExists")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CheckRoleExists(string roleName)
        {
            var exists = await _roleService.CheckRoleExists(roleName);
            _res.SuccessEventHandler(exists);
            return Ok(_res);
        }
    }
}
