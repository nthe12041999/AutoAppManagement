using AutoAppManagement.WebApp.Services;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    [Authorize]
    public class CustomerAccountController : BaseController
    {
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ILogger<CustomerAccountController> _logger;

        public CustomerAccountController(
            ICustomerAccountService customerAccountService,
            ILogger<CustomerAccountController> logger,
            RestOutput res) : base(res)
        {
            _customerAccountService = customerAccountService;
            _logger = logger;
        }

        /// <summary>
        /// Trang danh sách tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Accounts()
        {
            try
            {
                ViewData["Title"] = "Danh sách tài khoản khách hàng";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer accounts page");
                return View("Error");
            }
        }

        /// <summary>
        /// API: Lấy danh sách tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerAccounts()
        {
            try
            {
                var accounts = await _customerAccountService.GetCustomerAccountsAsync();
                _res.SuccessEventHandler(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer accounts");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy tài khoản khách hàng theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerAccount(long id)
        {
            try
            {
                var account = await _customerAccountService.GetCustomerAccountByIdAsync(id);
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
                _logger.LogError(ex, "Error getting customer account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tạo tài khoản khách hàng mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAccount([FromBody] CreateCustomerAccountViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _customerAccountService.CreateCustomerAccountAsync(model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Tạo tài khoản thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer account");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tạo tài khoản");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Cập nhật tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCustomerAccount(long id, [FromBody] UpdateCustomerAccountViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _customerAccountService.UpdateCustomerAccountAsync(id, model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Cập nhật tài khoản thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi cập nhật tài khoản");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xóa tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteCustomerAccount(long id)
        {
            try
            {
                var result = await _customerAccountService.DeleteCustomerAccountAsync(id);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Xóa tài khoản thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xóa tài khoản");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tìm kiếm tài khoản khách hàng
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="role"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchCustomerAccounts(string keyword = "", string status = "", string role = "", int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var result = await _customerAccountService.SearchCustomerAccountsAsync(keyword, status, role, pageIndex, pageSize);
                _res.SuccessEventHandler(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customer accounts");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tìm kiếm");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Thay đổi trạng thái tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeCustomerAccountStatus(long id, [FromBody] string status)
        {
            try
            {
                var result = await _customerAccountService.ChangeCustomerAccountStatusAsync(id, status);
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
                _logger.LogError(ex, "Error changing customer account status {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi thay đổi trạng thái");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy thống kê tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerAccountStatistics()
        {
            try
            {
                var statistics = await _customerAccountService.GetCustomerAccountStatisticsAsync();
                _res.SuccessEventHandler(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer account statistics");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải thống kê");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xuất danh sách tài khoản khách hàng ra Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ExportCustomerAccountsToExcel()
        {
            try
            {
                var fileBytes = await _customerAccountService.ExportCustomerAccountsToExcelAsync();
                var fileName = $"DanhSachTaiKhoanKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customer accounts to Excel");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xuất file Excel");
                return Json(_res);
            }
        }
    }
}
