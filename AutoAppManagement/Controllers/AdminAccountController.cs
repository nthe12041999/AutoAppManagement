using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.ViewModel.AdminAccount;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    //[Authorize]
    public class AdminAccountController : BaseBusinessController<IAdminAccountService, AdminAccountDTO>
    {
        public AdminAccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

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
                Logger.LogError(ex, "Error loading admin accounts page");
                return View("Error");
            }
        }

        /// <summary>
        /// Modal form để thêm/sửa Admin (được gọi từ DataGrid)
        /// </summary>
        /// <returns></returns>
        public IActionResult AdminForms()
        {
            return View();
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
                var result = await Service.ChangeAdminAccountStatusAsync(id, status);
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(true, "Thay đổi trạng thái thành công");
                }
                else
                {
                    ResOutput.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error changing admin account status {Id}", id);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi thay đổi trạng thái");
            }
            return Json(ResOutput);
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
                var result = await Service.AssignPermissionsAsync(id, permissions);
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(true, "Phân quyền thành công");
                }
                else
                {
                    ResOutput.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error assigning permissions to admin account {Id}", id);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi phân quyền");
            }
            return Json(ResOutput);
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
                var statistics = await Service.GetAdminAccountStatisticsAsync();
                ResOutput.SuccessEventHandler(statistics);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting admin account statistics");
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi tải thống kê");
            }
            return Json(ResOutput);
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
                var admins = await Service.GetOnlineAdminsAsync();
                ResOutput.SuccessEventHandler(admins);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting online admins");
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(ResOutput);
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
                    ResOutput.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(ResOutput);
                }

                var result = await Service.ChangePasswordAsync(id, model);
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(true, "Đổi mật khẩu thành công");
                }
                else
                {
                    ResOutput.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error changing password for admin {Id}", id);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi đổi mật khẩu");
            }
            return Json(ResOutput);
        }

        /// <summary>
        /// Trang thêm admin mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Demo form với validation engine
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateWithValidation()
        {
            return View();
        }
    }
}
