using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    //[Authorize]
    public class RoleController : BaseController
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            IRolePermissionService rolePermissionService,
            ILogger<RoleController> logger,
            RestOutput res
        )
            : base(res)
        {
            _rolePermissionService = rolePermissionService;
            _logger = logger;
        }

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
        public IActionResult RoleForms()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading role forms");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// API để DataGrid lấy dữ liệu role
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRoles(int page = 1, int pageSize = 10, string status = "", string group = "", string search = "")
        {
            try
            {
                // Simulate data for DataGrid (chỉ roles)
                var allRoles = new List<object>
                {
                    new { 
                        id = 1, 
                        name = "Quản trị viên", 
                        code = "admin", 
                        description = "Quyền cao nhất trong hệ thống", 
                        group = "admin", 
                        status = "active",
                        userCount = 5,
                        permissionCount = 15,
                        createdDate = DateTime.Now.AddDays(-30)
                    },
                    new { 
                        id = 2, 
                        name = "Quản lý cấp cao", 
                        code = "senior_manager", 
                        description = "Quản lý cấp cao có quyền ra quyết định", 
                        group = "management", 
                        status = "active",
                        userCount = 8,
                        permissionCount = 12,
                        createdDate = DateTime.Now.AddDays(-28)
                    },
                    new { 
                        id = 3, 
                        name = "Quản lý", 
                        code = "manager", 
                        description = "Quản lý các hoạt động chính", 
                        group = "management", 
                        status = "active",
                        userCount = 15,
                        permissionCount = 8,
                        createdDate = DateTime.Now.AddDays(-25)
                    },
                    new { 
                        id = 4, 
                        name = "Trưởng phòng", 
                        code = "department_head", 
                        description = "Trưởng phòng quản lý bộ phận", 
                        group = "management", 
                        status = "active",
                        userCount = 12,
                        permissionCount = 10,
                        createdDate = DateTime.Now.AddDays(-22)
                    },
                    new { 
                        id = 5, 
                        name = "Nhân viên kinh doanh", 
                        code = "sales_staff", 
                        description = "Nhân viên phụ trách bán hàng", 
                        group = "staff", 
                        status = "active",
                        userCount = 25,
                        permissionCount = 6,
                        createdDate = DateTime.Now.AddDays(-20)
                    },
                    new { 
                        id = 6, 
                        name = "Nhân viên kỹ thuật", 
                        code = "technical_staff", 
                        description = "Nhân viên kỹ thuật hỗ trợ", 
                        group = "staff", 
                        status = "active",
                        userCount = 18,
                        permissionCount = 9,
                        createdDate = DateTime.Now.AddDays(-18)
                    },
                    new { 
                        id = 7, 
                        name = "Nhân viên", 
                        code = "staff", 
                        description = "Nhân viên thường", 
                        group = "staff", 
                        status = "active",
                        userCount = 45,
                        permissionCount = 4,
                        createdDate = DateTime.Now.AddDays(-15)
                    },
                    new { 
                        id = 8, 
                        name = "Thực tập sinh", 
                        code = "intern", 
                        description = "Thực tập sinh có quyền hạn chế", 
                        group = "staff", 
                        status = "pending",
                        userCount = 8,
                        permissionCount = 2,
                        createdDate = DateTime.Now.AddDays(-12)
                    },
                    new { 
                        id = 9, 
                        name = "Khách hàng Premium", 
                        code = "premium_customer", 
                        description = "Khách hàng VIP có ưu đãi đặc biệt", 
                        group = "customer", 
                        status = "active",
                        userCount = 35,
                        permissionCount = 5,
                        createdDate = DateTime.Now.AddDays(-10)
                    },
                    new { 
                        id = 10, 
                        name = "Khách hàng", 
                        code = "customer", 
                        description = "Khách hàng sử dụng dịch vụ", 
                        group = "customer", 
                        status = "active",
                        userCount = 120,
                        permissionCount = 3,
                        createdDate = DateTime.Now.AddDays(-8)
                    },
                    new { 
                        id = 11, 
                        name = "Khách hàng dùng thử", 
                        code = "trial_customer", 
                        description = "Khách hàng sử dụng bản dùng thử", 
                        group = "customer", 
                        status = "active",
                        userCount = 85,
                        permissionCount = 2,
                        createdDate = DateTime.Now.AddDays(-5)
                    },
                    new { 
                        id = 12, 
                        name = "Nhà phát triển", 
                        code = "developer", 
                        description = "Nhà phát triển phần mềm", 
                        group = "staff", 
                        status = "active",
                        userCount = 6,
                        permissionCount = 13,
                        createdDate = DateTime.Now.AddDays(-3)
                    },
                    new { 
                        id = 13, 
                        name = "Kế toán", 
                        code = "accountant", 
                        description = "Nhân viên kế toán", 
                        group = "staff", 
                        status = "active",
                        userCount = 4,
                        permissionCount = 7,
                        createdDate = DateTime.Now.AddDays(-1)
                    }
                };

                // Apply filters
                var filteredData = allRoles.ToList();

                if (!string.IsNullOrEmpty(status))
                    filteredData = filteredData.Where(x => ((dynamic)x).status == status).ToList();

                if (!string.IsNullOrEmpty(group))
                    filteredData = filteredData.Where(x => ((dynamic)x).group == group).ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    filteredData = filteredData.Where(x => 
                        ((dynamic)x).name.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        ((dynamic)x).code.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        ((dynamic)x).description.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                var totalRecords = filteredData.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var pagedData = filteredData
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = pagedData,
                    totalRecords = totalRecords,
                    totalPages = totalPages,
                    currentPage = page,
                    pageSize = pageSize
                });
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
                return View("RoleForms");
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
                _logger.LogError(ex, "Error assigning permissions to role {RoleId}", roleId);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi gán quyền hạn");
            }
            return Json(_res);
        }
        #endregion
    }
}
