using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    //[Authorize]
    public class LicenseController : BaseController
    {
        private readonly ILicenseService _licenseService;
        private readonly ILogger<LicenseController> _logger;

        public LicenseController(
            ILicenseService licenseService,
            ILogger<LicenseController> logger,
            RestOutput res
        )
            : base(res)
        {
            _licenseService = licenseService;
            _logger = logger;
        }

        /// <summary>
        /// Trang quản lý license
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
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
        /// API: Lấy danh sách license
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLicenses()
        {
            try
            {
                var licenses = await _licenseService.GetLicensesAsync();
                return Json(new { success = true, data = licenses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting licenses");
                return Json(new { success = false, message = "Lỗi khi lấy danh sách license" });
            }
        }

        /// <summary>
        /// API: Lấy thông tin license theo ID
        /// </summary>
        /// <param name="id">ID của license</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLicense(long id)
        {
            try
            {
                var license = await _licenseService.GetLicenseByIdAsync(id);
                if (license == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy license" });
                }
                return Json(new { success = true, data = license });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting license {Id}", id);
                return Json(new { success = false, message = "Lỗi khi lấy thông tin license" });
            }
        }

        /// <summary>
        /// API: Tạo license mới
        /// </summary>
        /// <param name="model">Thông tin license</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var result = await _licenseService.CreateLicenseAsync(model);
                return Json(new { success = result.IsSuccess, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating license");
                return Json(new { success = false, message = "Lỗi khi tạo license" });
            }
        }

        /// <summary>
        /// API: Cập nhật license
        /// </summary>
        /// <param name="id">ID của license</param>
        /// <param name="model">Thông tin cập nhật</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateLicense(long id, [FromBody] UpdateLicenseViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var result = await _licenseService.UpdateLicenseAsync(id, model);
                return Json(new { success = result.IsSuccess, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating license {Id}", id);
                return Json(new { success = false, message = "Lỗi khi cập nhật license" });
            }
        }

        /// <summary>
        /// API: Xóa license
        /// </summary>
        /// <param name="id">ID của license</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteLicense(long id)
        {
            try
            {
                var result = await _licenseService.DeleteLicenseAsync(id);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting license {Id}", id);
                return Json(new { success = false, message = "Lỗi khi xóa license" });
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

                var result = await _licenseService.RenewLicenseAsync(id, model);
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
                var result = await _licenseService.SuspendLicenseAsync(id);
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
                var result = await _licenseService.ActivateLicenseAsync(id);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating license {Id}", id);
                return Json(new { success = false, message = "Lỗi khi kích hoạt license" });
            }
        }

        /// <summary>
        /// API: Tìm kiếm license
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="type">Loại license</param>
        /// <param name="status">Trạng thái</param>
        /// <param name="pageIndex">Trang hiện tại</param>
        /// <param name="pageSize">Số lượng mỗi trang</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchLicenses(string keyword = "", string type = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var result = await _licenseService.SearchLicensesAsync(keyword, type, status, pageIndex, pageSize);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching licenses");
                return Json(new { success = false, message = "Lỗi khi tìm kiếm license" });
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
                var statistics = await _licenseService.GetLicenseStatisticsAsync();
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
                var licenses = await _licenseService.GetExpiringLicensesAsync(days);
                return Json(new { success = true, data = licenses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring licenses");
                return Json(new { success = false, message = "Lỗi khi lấy license sắp hết hạn" });
            }
        }

        /// <summary>
        /// API: Xuất danh sách license ra Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ExportLicensesToExcel()
        {
            try
            {
                var fileBytes = await _licenseService.ExportLicensesToExcelAsync();
                var fileName = $"Licenses_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting licenses to Excel");
                return Json(new { success = false, message = "Lỗi khi xuất file Excel" });
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
                var history = await _licenseService.GetLicenseHistoryAsync(licenseId);
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
