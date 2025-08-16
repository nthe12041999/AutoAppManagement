using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.RoleAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class PermissionController : BaseController
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IRestOutput res, IHttpContextAccessor httpContextAccessor,
                                   IPermissionService permissionService) : base(res, httpContextAccessor)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Lấy tất cả role accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllRoleAccounts")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAllRoleAccounts()
        {
            var roleAccounts = await _permissionService.GetAllRoleAccounts();
            _res.SuccessEventHandler(roleAccounts);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy role accounts theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetRoleAccountsByAccountId")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRoleAccountsByAccountId(long accountId)
        {
            var roleAccounts = await _permissionService.GetRoleAccountsByAccountId(accountId);
            _res.SuccessEventHandler(roleAccounts);
            return Ok(_res);
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
            var roleAccounts = await _permissionService.GetRoleAccountsByRoleId(roleId);
            _res.SuccessEventHandler(roleAccounts);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy role account theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetRoleAccountById")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRoleAccountById(long id)
        {
            var roleAccount = await _permissionService.GetRoleAccountById(id);
            if (roleAccount == null)
            {
                _res.ErrorEventHandler("Role account không tồn tại");
                return NotFound(_res);
            }
            _res.SuccessEventHandler(roleAccount);
            return Ok(_res);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _permissionService.AssignRoleToAccount(request);
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
            var result = await _permissionService.RemoveRoleFromAccount(accountId, roleId);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _permissionService.UpdateRoleAccount(request);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _permissionService.BulkAssignRoles(request);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _permissionService.BulkRemoveRoles(request);
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
            var accountsWithRoles = await _permissionService.GetAccountsWithRoles();
            _res.SuccessEventHandler(accountsWithRoles);
            return Ok(_res);
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRolesWithAccounts")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRolesWithAccounts()
        {
            var rolesWithAccounts = await _permissionService.GetRolesWithAccounts();
            _res.SuccessEventHandler(rolesWithAccounts);
            return Ok(_res);
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
            var hasRole = await _permissionService.CheckAccountHasRole(accountId, roleId);
            _res.SuccessEventHandler(hasRole);
            return Ok(_res);
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
            var hasPermission = await _permissionService.CheckAccountHasPermission(accountId, permission);
            _res.SuccessEventHandler(hasPermission);
            return Ok(_res);
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
            var permissions = await _permissionService.GetAccountPermissions(accountId);
            _res.SuccessEventHandler(permissions);
            return Ok(_res);
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
                _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(_res);
            }

            var result = await _permissionService.SyncAccountRoles(request.AccountId, request.RoleIds);
            return Ok(result);
        }
    }
}
