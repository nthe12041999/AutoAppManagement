using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    [Authorize]
    public class RolePermissionController : BaseController
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly ILogger<RolePermissionController> _logger;

        public RolePermissionController(
            IRolePermissionService rolePermissionService,
            ILogger<RolePermissionController> logger,
            RestOutput res
        )
            : base(res)
        {
            _rolePermissionService = rolePermissionService;
            _logger = logger;
        }

        /// <summary>
        /// Trang quản lý quyền hạn
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                ViewData["Title"] = "Quản lý quyền hạn";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading roles page");
                return View("Error");
            }
        }

        #region Role APIs
        /// <summary>
        /// API: Lấy danh sách vai trò
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _rolePermissionService.GetRolesAsync();
                _res.SuccessEventHandler(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy vai trò theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRole(long id)
        {
            try
            {
                var role = await _rolePermissionService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy vai trò");
                }
                else
                {
                    _res.SuccessEventHandler(role);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tạo vai trò mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _rolePermissionService.CreateRoleAsync(model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Tạo vai trò thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tạo vai trò");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Cập nhật vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _rolePermissionService.UpdateRoleAsync(id, model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Cập nhật vai trò thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi cập nhật vai trò");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xóa vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRole(long id)
        {
            try
            {
                var result = await _rolePermissionService.DeleteRoleAsync(id);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Xóa vai trò thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xóa vai trò");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy quyền hạn của vai trò
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(long roleId)
        {
            try
            {
                var permissions = await _rolePermissionService.GetRolePermissionsAsync(roleId);
                _res.SuccessEventHandler(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role permissions {RoleId}", roleId);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải quyền hạn");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Gán quyền hạn cho vai trò
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignRolePermissions(
            long roleId,
            [FromBody] List<long> permissionIds
        )
        {
            try
            {
                var result = await _rolePermissionService.AssignRolePermissionsAsync(
                    roleId,
                    permissionIds
                );
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Gán quyền hạn thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role permissions {RoleId}", roleId);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi gán quyền hạn");
            }
            return Json(_res);
        }
        #endregion

        #region Permission APIs
        /// <summary>
        /// API: Lấy danh sách quyền hạn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var permissions = await _rolePermissionService.GetPermissionsAsync();
                _res.SuccessEventHandler(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy quyền hạn theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPermission(long id)
        {
            try
            {
                var permission = await _rolePermissionService.GetPermissionByIdAsync(id);
                if (permission == null)
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy quyền hạn");
                }
                else
                {
                    _res.SuccessEventHandler(permission);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tạo quyền hạn mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePermission(
            [FromBody] CreatePermissionViewModel model
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _rolePermissionService.CreatePermissionAsync(model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Tạo quyền hạn thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tạo quyền hạn");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Cập nhật quyền hạn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePermission(
            long id,
            [FromBody] UpdatePermissionViewModel model
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _rolePermissionService.UpdatePermissionAsync(id, model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Cập nhật quyền hạn thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi cập nhật quyền hạn");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xóa quyền hạn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeletePermission(long id)
        {
            try
            {
                var result = await _rolePermissionService.DeletePermissionAsync(id);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Xóa quyền hạn thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xóa quyền hạn");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy quyền hạn theo nhóm
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPermissionsByGroup(string group)
        {
            try
            {
                var permissions = await _rolePermissionService.GetPermissionsByGroupAsync(group);
                _res.SuccessEventHandler(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions by group {Group}", group);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }
        #endregion

        #region User Role Assignment APIs
        /// <summary>
        /// API: Lấy danh sách phân quyền người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserRoleAssignments()
        {
            try
            {
                var assignments = await _rolePermissionService.GetUserRoleAssignmentsAsync();
                _res.SuccessEventHandler(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user role assignments");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy vai trò của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserRoles(long userId)
        {
            try
            {
                var roles = await _rolePermissionService.GetUserRolesAsync(userId);
                _res.SuccessEventHandler(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user roles {UserId}", userId);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải vai trò");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Gán vai trò cho người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignUserRole(long userId, [FromBody] long roleId)
        {
            try
            {
                var result = await _rolePermissionService.AssignUserRoleAsync(userId, roleId);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Gán vai trò thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user role {UserId} {RoleId}", userId, roleId);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi gán vai trò");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xóa vai trò của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveUserRole(long userId, long roleId)
        {
            try
            {
                var result = await _rolePermissionService.RemoveUserRoleAsync(userId, roleId);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Xóa vai trò thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user role {UserId} {RoleId}", userId, roleId);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xóa vai trò");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy quyền hạn của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserPermissions(long userId)
        {
            try
            {
                var permissions = await _rolePermissionService.GetUserPermissionsAsync(userId);
                _res.SuccessEventHandler(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user permissions {UserId}", userId);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải quyền hạn");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Kiểm tra quyền hạn của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckUserPermission(long userId, string permission)
        {
            try
            {
                var hasPermission = await _rolePermissionService.CheckUserPermissionAsync(
                    userId,
                    permission
                );
                _res.SuccessEventHandler(hasPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error checking user permission {UserId} {Permission}",
                    userId,
                    permission
                );
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi kiểm tra quyền hạn");
            }
            return Json(_res);
        }
        #endregion

        #region Statistics and Reports APIs
        /// <summary>
        /// API: Lấy thống kê vai trò và quyền hạn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRolePermissionStatistics()
        {
            try
            {
                var statistics = await _rolePermissionService.GetRolePermissionStatisticsAsync();
                _res.SuccessEventHandler(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role permission statistics");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải thống kê");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy báo cáo phân quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPermissionReport()
        {
            try
            {
                var report = await _rolePermissionService.GetPermissionReportAsync();
                _res.SuccessEventHandler(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission report");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải báo cáo");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xuất báo cáo phân quyền ra Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ExportPermissionReport()
        {
            try
            {
                var fileBytes = await _rolePermissionService.ExportPermissionReportAsync();
                var fileName = $"BaoCaoPhanQuyen_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting permission report");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xuất báo cáo");
                return Json(_res);
            }
        }
        #endregion

        #region Search APIs
        /// <summary>
        /// API: Tìm kiếm vai trò
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchRoles(
            string keyword = "",
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            try
            {
                var result = await _rolePermissionService.SearchRolesAsync(
                    keyword,
                    pageIndex,
                    pageSize
                );
                _res.SuccessEventHandler(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching roles");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tìm kiếm");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tìm kiếm quyền hạn
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="group"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchPermissions(
            string keyword = "",
            string group = "",
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            try
            {
                var result = await _rolePermissionService.SearchPermissionsAsync(
                    keyword,
                    group,
                    pageIndex,
                    pageSize
                );
                _res.SuccessEventHandler(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching permissions");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tìm kiếm");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tìm kiếm phân quyền người dùng
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchUserRoleAssignments(
            string keyword = "",
            string role = "",
            string status = "",
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            try
            {
                var result = await _rolePermissionService.SearchUserRoleAssignmentsAsync(
                    keyword,
                    role,
                    status,
                    pageIndex,
                    pageSize
                );
                _res.SuccessEventHandler(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching user role assignments");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tìm kiếm");
            }
            return Json(_res);
        }
        #endregion
    }
}
