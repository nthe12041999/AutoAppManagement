using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class AccountController : BaseBusinessController<AccountService, AccountDTO>
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Trang danh sách tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Danh sách tài khoản khách hàng";
                return View();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading customer accounts page");
                return View("Error");
            }
        }

        /// <summary>
        /// Modal form để thêm/sửa (được gọi từ DataGrid)
        /// </summary>
        /// <returns></returns>
        public IActionResult CustomerForms(string mode = "add", string entity = "Customer")
        {
            ViewBag.Mode = mode;
            ViewBag.Entity = entity;
            return PartialView();
        }

        /// <summary>
        /// API: Thay đổi trạng thái tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ToggleCustomerStatus(long id)
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                var result = await _customerAccountService.ChangeCustomerAccountStatusAsync(id, status);
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(true, "Thay đổi trạng thái thành công");
                }
                else
                {
                    ResOutput.ErrorEventHandler(message: result.Message);
                }
                */

                // TEMPORARY: Mock status toggle
                // Simulate customer not found
                if (id == 999)
                {
                    ResOutput.ErrorEventHandler(message: "Không tìm thấy khách hàng");
                    return Json(ResOutput);
                }

                // Mock successful status toggle
                var newStatus = id == 5 ? "Active" : "Locked"; // Toggle status
                var message = newStatus == "Locked" ? "Đã khóa tài khoản khách hàng" : "Đã mở khóa tài khoản khách hàng";

                ResOutput.SuccessEventHandler(new { Id = id, Status = newStatus }, message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error toggling customer status {Id}", id);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi thay đổi trạng thái");
            }
            return Json(ResOutput);
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
                // TODO: Uncomment when service is ready
                /*
                var statistics = await _customerAccountService.GetCustomerAccountStatisticsAsync();
                ResOutput.SuccessEventHandler(statistics);
                */

                // TEMPORARY: Mock statistics data
                var statistics = new
                {
                    TotalCustomers = 5,
                    ActiveCustomers = 3,
                    PremiumCustomers = 2,
                    OnlineCustomers = 2,
                    LockedCustomers = 1,
                    ExpiredCustomers = 1,
                    NewCustomersThisMonth = 2,
                    TotalRevenue = 15000000, // 15 triệu VND
                    AverageLoginPerDay = 25,
                    TopCustomersByLevel = new[]
                    {
                        new { Level = "VIP", Count = 1, Percentage = 20 },
                        new { Level = "Premium", Count = 1, Percentage = 20 },
                        new { Level = "Customer", Count = 3, Percentage = 60 }
                    },
                    RecentActivities = new[]
                    {
                        new { Action = "Login", CustomerName = "Nguyễn Văn Khách", Time = DateTime.Now.AddMinutes(-15) },
                        new { Action = "Register", CustomerName = "Trần Thị Premium", Time = DateTime.Now.AddHours(-2) },
                        new { Action = "Upgrade", CustomerName = "Lê Văn VIP", Time = DateTime.Now.AddHours(-5) }
                    }
                };

                ResOutput.SuccessEventHandler(statistics);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting customer account statistics");
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi tải thống kê");
            }
            return Json(ResOutput);
        }
    }
}
