using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    [Authorize]
    public class AdminAccountController : BaseController
    {
        private readonly IAdminAccountService _adminAccountService;
        private readonly ILogger<AdminAccountController> _logger;

        public AdminAccountController(
            IAdminAccountService adminAccountService,
            ILogger<AdminAccountController> logger,
            RestOutput res
        )
            : base(res)
        {
            _adminAccountService = adminAccountService;
            _logger = logger;
        }

        /// <summary>
        /// Trang quản lý tài khoản Admin
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Accounts()
        {
            try
            {
                ViewData["Title"] = "Quản lý tài khoản Admin";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin accounts page");
                return View("Error");
            }
        }

        /// <summary>
        /// API: Lấy danh sách tài khoản admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAdminAccounts()
        {
            try
            {
                var accounts = await _adminAccountService.GetAdminAccountsAsync();
                _res.SuccessEventHandler(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin accounts");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy tài khoản admin theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAdminAccount(long id)
        {
            try
            {
                var account = await _adminAccountService.GetAdminAccountByIdAsync(id);
                if (account == null)
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy tài khoản");
                }
                else
                {
                    _res.SuccessEventHandler(account);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tạo tài khoản admin mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAdminAccount([FromBody] CreateAdminAccountViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _adminAccountService.CreateAdminAccountAsync(model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Tạo tài khoản admin thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin account");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tạo tài khoản");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Cập nhật tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAdminAccount(long id, [FromBody] UpdateAdminAccountViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _adminAccountService.UpdateAdminAccountAsync(id, model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Cập nhật tài khoản admin thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi cập nhật tài khoản");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xóa tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAdminAccount(long id)
        {
            try
            {
                var result = await _adminAccountService.DeleteAdminAccountAsync(id);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Xóa tài khoản admin thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xóa tài khoản");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tìm kiếm tài khoản admin
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchAdminAccounts(string keyword = "", string role = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var result = await _adminAccountService.SearchAdminAccountsAsync(keyword, role, status, pageIndex, pageSize);
                _res.SuccessEventHandler(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching admin accounts");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tìm kiếm");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Thay đổi trạng thái tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeAdminAccountStatus(long id, [FromBody] string status)
        {
            try
            {
                var result = await _adminAccountService.ChangeAdminAccountStatusAsync(id, status);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Thay đổi trạng thái thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing admin account status {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi thay đổi trạng thái");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Phân quyền cho tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AssignPermissions(long id, [FromBody] List<string> permissions)
        {
            try
            {
                var result = await _adminAccountService.AssignPermissionsAsync(id, permissions);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Phân quyền thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to admin account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi phân quyền");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy quyền hạn của tài khoản admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAdminPermissions(long id)
        {
            try
            {
                var permissions = await _adminAccountService.GetAdminPermissionsAsync(id);
                _res.SuccessEventHandler(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin permissions {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải quyền hạn");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy thống kê tài khoản admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAdminAccountStatistics()
        {
            try
            {
                var statistics = await _adminAccountService.GetAdminAccountStatisticsAsync();
                _res.SuccessEventHandler(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin account statistics");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải thống kê");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy danh sách admin đang online
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOnlineAdmins()
        {
            try
            {
                var admins = await _adminAccountService.GetOnlineAdminsAsync();
                _res.SuccessEventHandler(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online admins");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Đổi mật khẩu admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _adminAccountService.ChangePasswordAsync(id, model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Đổi mật khẩu thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for admin {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi đổi mật khẩu");
            }
            return Json(_res);
        }
    }
}
