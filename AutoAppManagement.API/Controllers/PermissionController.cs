using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Permission;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class PermissionController : BaseBusinessController<IPermissionService, Permission, PermissionDTO>
    {
        public PermissionController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Lấy tất cả permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllPermissions")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _service.GetAllPermissions();
            ResOutput.SuccessEventHandler(permissions);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy permissions theo category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("GetPermissionsByCategory/{category}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetPermissionsByCategory(string category)
        {
            var permissions = await _service.GetPermissionsByCategory(category);
            ResOutput.SuccessEventHandler(permissions);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Gán permission cho role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AssignPermissionToRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignPermissionToRole([FromBody] AssignPermissionToRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.AssignPermissionToRole(request.RoleId, request.PermissionId, 
                request.ScopeDefault, request.Priority);
            return Ok(result);
        }

        /// <summary>
        /// Gỡ permission khỏi role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        [HttpDelete("RemovePermissionFromRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> RemovePermissionFromRole(long roleId, long permissionId)
        {
            var result = await _service.RemovePermissionFromRole(roleId, permissionId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy permissions của role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("GetRolePermissions/{roleId}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetRolePermissions(long roleId)
        {
            var permissions = await _service.GetRolePermissions(roleId);
            ResOutput.SuccessEventHandler(permissions);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CheckAccountPermission")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CheckAccountPermission([FromBody] PermissionCheckRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var hasPermission = await _service.CheckAccountHasPermission(request.AccountId, 
                request.Resource, request.Action, request.RequiredScope);
            ResOutput.SuccessEventHandler(hasPermission);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetAccountPermissions/{accountId}")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetAccountPermissions(long accountId)
        {
            var permissions = await _service.GetAccountPermissions(accountId);
            ResOutput.SuccessEventHandler(permissions);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost("AssignRoleToAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignRoleToAccount(long accountId, long roleId)
        {
            var result = await _service.AssignRoleToAccount(accountId, roleId);
            return Ok(result);
        }

        /// <summary>
        /// Gán nhiều permissions cho role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("BulkAssignPermissionsToRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> BulkAssignPermissionsToRole([FromBody] BulkAssignPermissionsRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.BulkAssignPermissionsToRole(request.RoleId, 
                request.PermissionIds, request.DefaultScope);
            return Ok(result);
        }

        /// <summary>
        /// Khởi tạo permissions mặc định
        /// </summary>
        /// <returns></returns>
        [HttpPost("InitializeDefaultPermissions")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> InitializeDefaultPermissions()
        {
            var result = await _service.InitializeDefaultPermissions();
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm permissions
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        [HttpGet("SearchPermissions")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> SearchPermissions(string searchTerm)
        {
            var permissions = await _service.SearchPermissions(searchTerm);
            ResOutput.SuccessEventHandler(permissions);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Lấy permissions theo category (grouped)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPermissionsByCategory")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetPermissionsByCategory()
        {
            var permissionsByCategory = await _service.GetPermissionsByCategory();
            ResOutput.SuccessEventHandler(permissionsByCategory);
            return Ok(ResOutput);
        }

        /// <summary>
        /// Tạo role với permissions cụ thể
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateRoleWithPermissions")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateRoleWithPermissions([FromBody] CreateRoleWithPermissionsRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var permissions = request.Permissions.Select(p => (p.Resource, p.Action, p.Scope)).ToList();
            var result = await _service.CreateRoleWithPermissions(
                request.RoleName, 
                request.RoleDescription, 
                permissions);
            
            return Ok(result);
        }

        /// <summary>
        /// Tạo role với permissions mặc định theo loại role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateRoleWithDefaultPermissions")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateRoleWithDefaultPermissions([FromBody] CreateRoleWithDefaultPermissionsRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.CreateRoleWithDefaultPermissions(
                request.RoleName, 
                request.RoleDescription, 
                request.RoleType);
            
            return Ok(result);
        }

        /// <summary>
        /// Tạo role với permissions và gán cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateRoleAndAssignToAccount")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateRoleAndAssignToAccount([FromBody] CreateRoleAndAssignToAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var permissions = request.Permissions.Select(p => (p.Resource, p.Action, p.Scope)).ToList();
            var result = await _service.CreateRoleAndAssignToAccount(
                request.AccountId,
                request.RoleName, 
                request.RoleDescription, 
                permissions);
            
            return Ok(result);
        }

        /// <summary>
        /// Tạo account mới với role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateAccountWithRole")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> CreateAccountWithRole([FromBody] CreateAccountWithRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                return BadRequest(ResOutput);
            }

            var result = await _service.CreateAccountWithRole(
                request.Email,
                request.FullName, 
                request.RoleName, 
                request.RoleType);
            
            return Ok(result);
        }
    }

    // Request DTOs cho composite operations
    public class CreateRoleWithPermissionsRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public List<PermissionItem> Permissions { get; set; } = new();
    }

    public class CreateRoleWithDefaultPermissionsRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public string RoleType { get; set; } = "user"; // admin, manager, user, viewer
    }

    public class CreateRoleAndAssignToAccountRequest
    {
        public long AccountId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public List<PermissionItem> Permissions { get; set; } = new();
    }

    public class CreateAccountWithRoleRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string RoleType { get; set; } = "user"; // admin, manager, user, viewer
    }

    public class PermissionItem
    {
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Scope { get; set; } = "own";
    }
}
