using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(
            RestOutput res,
            INotificationService notificationService,
            ILogger<NotificationController> logger,
            IHttpContextAccessor httpContextAccessor
        )
            : base(res)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

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
                    var notifications = await _notificationService.GetNotificationsByAccountId(accountId.Value);
                    _res.SuccessEventHandler(notifications);
                }
                else
                {
                    _res.ErrorEventHandler("Cần chỉ định AccountId");
                }
                return Json(_res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                _res.ErrorEventHandler("Có lỗi xảy ra khi lấy danh sách thông báo");
                return Json(_res);
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
                var notifications = await _notificationService.GetUnreadNotifications(accountId);
                _res.SuccessEventHandler(notifications);
                return Json(_res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications for account {AccountId}", accountId);
                _res.ErrorEventHandler("Có lỗi xảy ra khi lấy thông báo chưa đọc");
                return Json(_res);
            }
        }

        /// <summary>
        /// Lấy thông tin thông báo theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNotification(long id)
        {
            try
            {
                var notification = await _notificationService.GetNotificationById(id);
                if (notification == null)
                {
                    _res.ErrorEventHandler("Thông báo không tồn tại");
                    return Json(_res);
                }

                _res.SuccessEventHandler(notification);
                return Json(_res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {NotificationId}", id);
                _res.ErrorEventHandler("Có lỗi xảy ra khi lấy thông tin thông báo");
                return Json(_res);
            }
        }

        /// <summary>
        /// Tạo thông báo mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _notificationService.CreateNotification(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                _res.ErrorEventHandler("Có lỗi xảy ra khi tạo thông báo");
                return Json(_res);
            }
        }

        /// <summary>
        /// Cập nhật thông báo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateNotification([FromBody] UpdateNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _notificationService.UpdateNotification(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification {NotificationId}", request.Id);
                _res.ErrorEventHandler("Có lỗi xảy ra khi cập nhật thông báo");
                return Json(_res);
            }
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteNotification(long id)
        {
            try
            {
                var result = await _notificationService.DeleteNotification(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
                _res.ErrorEventHandler("Có lỗi xảy ra khi xóa thông báo");
                return Json(_res);
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
                var result = await _notificationService.MarkAsRead(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read {NotificationId}", id);
                _res.ErrorEventHandler("Có lỗi xảy ra khi đánh dấu đã đọc");
                return Json(_res);
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
                var result = await _notificationService.MarkAllAsRead(accountId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for account {AccountId}", accountId);
                _res.ErrorEventHandler("Có lỗi xảy ra khi đánh dấu tất cả đã đọc");
                return Json(_res);
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
                var count = await _notificationService.GetUnreadCount(accountId);
                _res.SuccessEventHandler(count);
                return Json(_res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for account {AccountId}", accountId);
                _res.ErrorEventHandler("Có lỗi xảy ra khi lấy số thông báo chưa đọc");
                return Json(_res);
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
                    _res.ErrorEventHandler("Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _notificationService.SendBulkNotification(request.AccountIds, request.Title, request.Message, request.Type);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk notification");
                _res.ErrorEventHandler("Có lỗi xảy ra khi gửi thông báo hàng loạt");
                return Json(_res);
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
                var notifications = await _notificationService.GetNotificationsByType(accountId, type);
                _res.SuccessEventHandler(notifications);
                return Json(_res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by type for account {AccountId}", accountId);
                _res.ErrorEventHandler("Có lỗi xảy ra khi lấy thông báo theo loại");
                return Json(_res);
            }
        }
    }
}
