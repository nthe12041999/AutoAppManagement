using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class NotificationController : BaseBusinessController<INotificationService, NotificationDTO>
    {
        public NotificationController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Trang danh sách thông báo
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Quản lý thông báo";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications page");
                return View("Error");
            }
        }

        /// <summary>
        /// Modal form để thêm/sửa thông báo
        /// </summary>
        /// <returns></returns>
        public IActionResult NotificationForms()
        {
            return View();
        }

        /// <summary>
        /// Lấy danh sách thông báo cho DataGrid
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetNotifications(long? accountId = null)
        {
            try
            {
                if (accountId.HasValue)
                {
                    var notifications = await Service.GetNotificationsByAccountId(accountId.Value);
                    ResOutput.SuccessEventHandler(notifications);
                }
                else
                {
                    ResOutput.ErrorEventHandler("Cần chỉ định AccountId");
                }
                return Json(ResOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                ResOutput.ErrorEventHandler("Có lỗi xảy ra khi lấy danh sách thông báo");
                return Json(ResOutput);
            }
        }

        /// <summary>
        /// Lấy thông báo chưa đọc
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUnreadNotifications(long accountId)
        {
            try
            {
                var notifications = await Service.GetUnreadNotifications(accountId);
                ResOutput.SuccessEventHandler(notifications);
                return Json(ResOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications for account {AccountId}", accountId);
                ResOutput.ErrorEventHandler("Có lỗi xảy ra khi lấy thông báo chưa đọc");
                return Json(ResOutput);
            }
        }

        /// <summary>
        /// Đánh dấu đã đọc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            try
            {
                var result = await Service.MarkAsRead(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read {NotificationId}", id);
                ResOutput.ErrorEventHandler("Có lỗi xảy ra khi đánh dấu đã đọc");
                return Json(ResOutput);
            }
        }

        /// <summary>
        /// Đánh dấu tất cả đã đọc
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead(long accountId)
        {
            try
            {
                var result = await Service.MarkAllAsRead(accountId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for account {AccountId}", accountId);
                ResOutput.ErrorEventHandler("Có lỗi xảy ra khi đánh dấu tất cả đã đọc");
                return Json(ResOutput);
            }
        }

        /// <summary>
        /// Lấy số thông báo chưa đọc
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount(long accountId)
        {
            try
            {
                var count = await Service.GetUnreadCount(accountId);
                ResOutput.SuccessEventHandler(count);
                return Json(ResOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for account {AccountId}", accountId);
                ResOutput.ErrorEventHandler("Có lỗi xảy ra khi lấy số thông báo chưa đọc");
                return Json(ResOutput);
            }
        }

        /// <summary>
        /// Gửi thông báo hàng loạt
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendBulkNotification([FromBody] SendBulkNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler("Dữ liệu không hợp lệ");
                    return Json(ResOutput);
                }

                var result = await Service.SendBulkNotification(request.AccountIds, request.Title, request.Message, request.Type);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk notification");
                ResOutput.ErrorEventHandler("Có lỗi xảy ra khi gửi thông báo hàng loạt");
                return Json(ResOutput);
            }
        }

        /// <summary>
        /// Lấy thông báo theo loại
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNotificationsByType(long accountId, string type)
        {
            try
            {
                var notifications = await Service.GetNotificationsByType(accountId, type);
                ResOutput.SuccessEventHandler(notifications);
                return Json(ResOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by type for account {AccountId}", accountId);
                ResOutput.ErrorEventHandler("Có lỗi xảy ra khi lấy thông báo theo loại");
                return Json(ResOutput);
            }
        }
    }
}
