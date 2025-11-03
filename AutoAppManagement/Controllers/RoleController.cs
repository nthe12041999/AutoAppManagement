using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    //[Authorize]
    public class RoleController : BaseBusinessController<IRoleService, RoleDTO>
    {
        public RoleController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Trang quản lý vai trò
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                ViewData["Title"] = "Quản lý vai trò";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading roles page");
                return View("Error");
            }
        }

        /// <summary>
        /// Modal form để thêm/sửa Role (được gọi từ DataGrid)
        /// </summary>
        /// <returns></returns>
        public IActionResult RoleForms(string mode = "add", string entity = "Role")
        {
            try
            {
                ViewBag.Mode = mode;
                ViewBag.Entity = entity;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading role forms");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Detail view để xem thông tin chi tiết role
        /// </summary>
        [HttpGet]
        public IActionResult DetailRole(int id)
        {
            try
            {
                // Simulate getting role detail
                ViewBag.RoleId = id;
                ViewBag.Mode = "view";
                return PartialView("RoleForms");
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        #region Role APIs

        /// <summary>
        /// API: Gán quyền hạn cho vai trò
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignPermissions(
            long roleId,
            [FromBody] List<long> permissionIds
        )
        {
            try
            {
                var permissionService = _serviceProvider.GetRequiredService<IPermissionService>();
                var result = await permissionService.AssignRolePermissionsAsync(roleId, permissionIds);
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(true, "Gán quyền hạn thành công");
                }
                else
                {
                    ResOutput.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role {RoleId}", roleId);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi gán quyền hạn");
            }
            return Json(ResOutput);
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
                var permissions = await Service.GetById(roleId);
                ResOutput.SuccessEventHandler(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role permissions {RoleId}", roleId);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi tải quyền hạn");
            }
            return Json(ResOutput);
        }
        #endregion

        #region Permission Management APIs
        /// <summary>
        /// API: Lấy tất cả quyền hạn có sẵn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                // Simulate permissions data grouped by module
                var permissions = new
                {
                    users = new[]
                    {
                        new { id = 101, name = "Xem người dùng", code = "users.view", description = "Quyền xem danh sách người dùng" },
                        new { id = 102, name = "Tạo người dùng", code = "users.create", description = "Quyền tạo mới người dùng" },
                        new { id = 103, name = "Chỉnh sửa người dùng", code = "users.edit", description = "Quyền chỉnh sửa thông tin người dùng" },
                        new { id = 104, name = "Xóa người dùng", code = "users.delete", description = "Quyền xóa người dùng" },
                        new { id = 105, name = "Khóa/Mở khóa người dùng", code = "users.lock", description = "Quyền khóa hoặc mở khóa tài khoản người dùng" }
                    },
                    license = new[]
                    {
                        new { id = 201, name = "Xem license", code = "license.view", description = "Quyền xem thông tin license" },
                        new { id = 202, name = "Tạo license", code = "license.create", description = "Quyền tạo mới license" },
                        new { id = 203, name = "Chỉnh sửa license", code = "license.edit", description = "Quyền chỉnh sửa thông tin license" },
                        new { id = 204, name = "Xóa license", code = "license.delete", description = "Quyền xóa license" },
                        new { id = 205, name = "Tải xuống license", code = "license.download", description = "Quyền tải xuống file license" }
                    },
                    reports = new[]
                    {
                        new { id = 301, name = "Xem báo cáo", code = "reports.view", description = "Quyền xem các báo cáo" },
                        new { id = 302, name = "Xuất báo cáo", code = "reports.export", description = "Quyền xuất báo cáo" },
                        new { id = 303, name = "Tạo báo cáo", code = "reports.create", description = "Quyền tạo báo cáo tùy chỉnh" }
                    },
                    settings = new[]
                    {
                        new { id = 401, name = "Xem cài đặt", code = "settings.view", description = "Quyền xem cài đặt hệ thống" },
                        new { id = 402, name = "Chỉnh sửa cài đặt", code = "settings.edit", description = "Quyền thay đổi cài đặt hệ thống" }
                    },
                    orders = new[]
                    {
                        new { id = 501, name = "Xem đơn hàng", code = "orders.view", description = "Quyền xem danh sách đơn hàng" },
                        new { id = 502, name = "Tạo đơn hàng", code = "orders.create", description = "Quyền tạo đơn hàng mới" }
                    },
                    system = new[]
                    {
                        new { id = 801, name = "Quản trị hệ thống", code = "system.admin", description = "Quyền quản trị toàn hệ thống" },
                        new { id = 802, name = "Phát triển hệ thống", code = "system.develop", description = "Quyền phát triển và bảo trì hệ thống" }
                    }
                };

                ResOutput.SuccessEventHandler(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all permissions");
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi tải quyền hạn");
            }
            return Json(ResOutput);
        }

        /// <summary>
        /// Modal form để quản lý quyền cho vai trò
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IActionResult ManagePermissions(long roleId)
        {
            try
            {
                ViewBag.RoleId = roleId;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading manage permissions page");
                return StatusCode(500, "Internal Server Error");
            }
        }
        #endregion
    }
}
