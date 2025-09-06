using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    //[Authorize]
    public class LicenseController : BaseBusinessController<ILicenseService, LicenseDTO>
    {
        public LicenseController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Trang quản lý license
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                ViewData["Title"] = "Quản lý License";
                ViewData["PageName"] = "license";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading license page");
                return View("Error");
            }
        }

        /// <summary>
        /// Modal form để thêm/sửa License (được gọi từ DataGrid)
        /// </summary>
        /// <returns></returns>
        public IActionResult LicenseForms()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading license forms");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// API: Gia hạn license
        /// </summary>
        /// <param name="id">ID của license</param>
        /// <param name="model">Thông tin gia hạn</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RenewLicense(long id, [FromBody] RenewLicenseViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var result = await Service.RenewLicenseAsync(id, model);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing license {Id}", id);
                return Json(new { success = false, message = "Lỗi khi gia hạn license" });
            }
        }

        /// <summary>
        /// API: Tạm dừng license
        /// </summary>
        /// <param name="id">ID của license</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SuspendLicense(long id)
        {
            try
            {
                var result = await Service.SuspendLicenseAsync(id);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending license {Id}", id);
                return Json(new { success = false, message = "Lỗi khi tạm dừng license" });
            }
        }

        /// <summary>
        /// API: Kích hoạt license
        /// </summary>
        /// <param name="id">ID của license</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ActivateLicense(long id)
        {
            try
            {
                var result = await Service.ActivateLicenseAsync(id);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating license {Id}", id);
                return Json(new { success = false, message = "Lỗi khi kích hoạt license" });
            }
        }

        /// <summary>
        /// API: Lấy thống kê license
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLicenseStatistics()
        {
            try
            {
                var statistics = await Service.GetLicenseStatisticsAsync();
                return Json(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting license statistics");
                return Json(new { success = false, message = "Lỗi khi lấy thống kê license" });
            }
        }

        /// <summary>
        /// API: Lấy license sắp hết hạn
        /// </summary>
        /// <param name="days">Số ngày trước khi hết hạn</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetExpiringLicenses(int days = 30)
        {
            try
            {
                var licenses = await Service.GetExpiringLicensesAsync(days);
                return Json(new { success = true, data = licenses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring licenses");
                return Json(new { success = false, message = "Lỗi khi lấy license sắp hết hạn" });
            }
        }

        /// <summary>
        /// API: Lấy lịch sử license
        /// </summary>
        /// <param name="licenseId">ID của license</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLicenseHistory(long licenseId)
        {
            try
            {
                var history = await Service.GetLicenseHistoryAsync(licenseId);
                return Json(new { success = true, data = history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting license history for {LicenseId}", licenseId);
                return Json(new { success = false, message = "Lỗi khi lấy lịch sử license" });
            }
        }
    }
}
