using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.RoleAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class PermissionController : BaseBusinessController<IPermissionService, RoleAccount, RoleAccountDTO>
    {

        public PermissionController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Lấy role accounts theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetRoleAccountsByAccountId")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRoleAccountsByAccountId(long accountId)
        {
            var roleAccounts = await _service.GetRoleAccountsByAccountId(accountId);
            ResOutput.SuccessEventHandler(roleAccounts);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy role accounts theo role ID
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("GetRoleAccountsByRoleId")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRoleAccountsByRoleId(long roleId)
        {
            var roleAccounts = await _service.GetRoleAccountsByRoleId(roleId);
            ResOutput.SuccessEventHandler(roleAccounts);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AssignRoleToAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignRoleToAccount([FromBody] AssignRoleToAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.AssignRoleToAccount(request);
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
            var result = await _service.RemoveRoleFromAccount(accountId, roleId);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật role account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("UpdateRoleAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> UpdateRoleAccount([FromBody] UpdateRoleAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.UpdateRoleAccount(request);
            return Ok(result);
        }

        /// <summary>
        /// Gán nhiều role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("BulkAssignRoles")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> BulkAssignRoles([FromBody] BulkAssignRolesRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.BulkAssignRoles(request);
            return Ok(result);
        }

        /// <summary>
        /// Gỡ nhiều role khỏi account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("BulkRemoveRoles")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> BulkRemoveRoles([FromBody] BulkRemoveRolesRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.BulkRemoveRoles(request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy accounts với roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAccountsWithRoles")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountsWithRoles()
        {
            var accountsWithRoles = await _service.GetAccountsWithRoles();
            ResOutput.SuccessEventHandler(accountsWithRoles);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRolesWithAccounts")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRolesWithAccounts()
        {
            var rolesWithAccounts = await _service.GetRolesWithAccounts();
            ResOutput.SuccessEventHandler(rolesWithAccounts);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Kiểm tra account có role không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("CheckAccountHasRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CheckAccountHasRole(long accountId, long roleId)
        {
            var hasRole = await _service.CheckAccountHasRole(accountId, roleId);
            ResOutput.SuccessEventHandler(hasRole);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpGet("CheckAccountHasPermission")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CheckAccountHasPermission(long accountId, string permission)
        {
            var hasPermission = await _service.CheckAccountHasPermission(accountId, permission);
            ResOutput.SuccessEventHandler(hasPermission);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy tất cả permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetAccountPermissions")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountPermissions(long accountId)
        {
            var permissions = await _service.GetAccountPermissions(accountId);
            ResOutput.SuccessEventHandler(permissions);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Đồng bộ roles của account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SyncAccountRoles")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> SyncAccountRoles([FromBody] SyncAccountRolesRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.SyncAccountRoles(request.AccountId, request.RoleIds);
            return Ok(result);
        }
    }
}
