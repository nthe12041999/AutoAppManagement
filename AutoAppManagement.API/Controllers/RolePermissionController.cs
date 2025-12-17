using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IUnitOfWork _unitOfWork;
        private IBaseRepository<AutoAppManagement.Models.BaseEntity.RolePermission> _rolePermissionRepository;
        private IBaseRepository<AutoAppManagement.Models.BaseEntity.RolePermission> RolePermissionRepository
            => _rolePermissionRepository ??= _unitOfWork.GetBaseRepository<AutoAppManagement.Models.BaseEntity.RolePermission>();

        public RolePermissionController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _permissionService = serviceProvider.GetRequiredService<IPermissionService>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        /// <summary>
        /// Lấy danh sách Permission của Role
        /// </summary>
        [HttpGet("GetByRoleId")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetByRoleId([FromQuery] long roleId)
        {
            try
            {
                var rolePermissions = await RolePermissionRepository.GetByCondition(
                    rp => rp.RoleId == roleId && rp.Status == StatusEnum.Active);

                var result = rolePermissions.Select(rp => new
                {
                    id = rp.ID,
                    roleId = rp.RoleId,
                    permissionId = rp.PermissionId,
                    createdDate = rp.CreatedDate
                }).ToList();

                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi lấy Permission của Role: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Gán Permission cho Role
        /// </summary>
        [HttpPost("AssignPermissions")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                    return BadRequest(ResOutput);
                }

                // Xóa tất cả RolePermission hiện tại của Role
                var existingRolePermissions = await RolePermissionRepository.GetByCondition(
                    rp => rp.RoleId == request.RoleId && rp.Status == StatusEnum.Active);

                foreach (var rp in existingRolePermissions)
                {
                    RolePermissionRepository.Delete(rp);
                }

                // Tạo mới các RolePermission
                var currentUserId = GetCurrentUserId();

                foreach (var permissionId in request.PermissionIds.Distinct())
                {
                    var rolePermission = new AutoAppManagement.Models.BaseEntity.RolePermission
                    {
                        RoleId = request.RoleId,
                        PermissionId = permissionId
                    };
                    rolePermission.SetCreated(currentUserId);
                    await RolePermissionRepository.CreateAsync(rolePermission);
                }

                await _unitOfWork.SaveAsync();

                ResOutput.SuccessEventHandler(null, "Gán Permission cho Role thành công");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi gán Permission: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Xóa một Permission khỏi Role
        /// </summary>
        [HttpPost("RemovePermission")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> RemovePermission([FromBody] RemovePermissionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                    return BadRequest(ResOutput);
                }

                var rolePermission = await RolePermissionRepository.FirstOrDefault(
                    rp => rp.RoleId == request.RoleId && 
                          rp.PermissionId == request.PermissionId && 
                          rp.Status == StatusEnum.Active);

                if (rolePermission == null)
                {
                    ResOutput.ErrorEventHandler("RolePermission không tồn tại");
                    return NotFound(ResOutput);
                }

                RolePermissionRepository.Delete(rolePermission);
                await _unitOfWork.SaveAsync();

                ResOutput.SuccessEventHandler(null, "Xóa Permission khỏi Role thành công");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi xóa Permission: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Lấy danh sách Role có Permission cụ thể
        /// </summary>
        [HttpGet("GetByPermissionId")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetByPermissionId([FromQuery] long permissionId)
        {
            try
            {
                var rolePermissions = await RolePermissionRepository.GetByCondition(
                    rp => rp.PermissionId == permissionId && rp.Status == StatusEnum.Active);

                var result = rolePermissions.Select(rp => new
                {
                    id = rp.ID,
                    roleId = rp.RoleId,
                    permissionId = rp.PermissionId,
                    createdDate = rp.CreatedDate
                }).ToList();

                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi lấy Role có Permission: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        private long GetCurrentUserId()
        {
            var userContext = HttpContext.User;
            if (userContext?.Identity != null && userContext.Identity.IsAuthenticated)
            {
                var valueAccId = userContext?.FindFirst("AccountId")?.Value ?? userContext?.FindFirst("UserId")?.Value;
                if (valueAccId != null && long.TryParse(valueAccId, out long userId))
                {
                    return userId;
                }
            }
            return 1; // Default for testing
        }
    }
}

