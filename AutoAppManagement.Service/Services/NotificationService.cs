using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace AutoAppManagement.Service.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDTO>> GetNotificationsByAccountId(long accountId);
        Task<List<NotificationDTO>> GetUnreadNotifications(long accountId);
        Task<NotificationDTO> GetNotificationById(long id);
        Task<RestOutput> CreateNotification(CreateNotificationRequest request);
        Task<RestOutput> UpdateNotification(UpdateNotificationRequest request);
        Task<RestOutput> DeleteNotification(long id);
        Task<RestOutput> MarkAsRead(long id);
        Task<RestOutput> MarkAsUnread(long id);
        Task<RestOutput> MarkAllAsRead(long accountId);
        Task<int> GetUnreadCount(long accountId);
        Task<RestOutput> SendNotificationToAccount(long accountId, string title, string message, string type = "info");
        Task<RestOutput> SendBulkNotification(List<long> accountIds, string title, string message, string type = "info");
        Task<List<NotificationDTO>> GetNotificationsByType(long accountId, string type);
        Task<RestOutput> DeleteOldNotifications(int daysOld);
    }

    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache, 
            IUnitOfWork unitOfWork, IMapper mapper, INotificationSocketHub notificationSocketHub) 
            : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
        }

        /// <summary>
        /// Lấy thông báo theo account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<NotificationDTO>> GetNotificationsByAccountId(long accountId)
        {
            var notifications = await UnitOfWork.NotificationsRepository.GetByCondition(n => n.AccountId == accountId);
            return Mapper.Map<List<NotificationDTO>>(notifications.OrderByDescending(n => n.CreatedDate).ToList());
        }

        /// <summary>
        /// Lấy thông báo chưa đọc
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<NotificationDTO>> GetUnreadNotifications(long accountId)
        {
            var notifications = await UnitOfWork.NotificationsRepository.GetByCondition(n => 
                n.AccountId == accountId && !n.IsReaded);
            return Mapper.Map<List<NotificationDTO>>(notifications.OrderByDescending(n => n.CreatedDate).ToList());
        }

        /// <summary>
        /// Lấy thông báo theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NotificationDTO> GetNotificationById(long id)
        {
            var notification = await UnitOfWork.NotificationsRepository.FirstOrDefault(n => n.Id == id);
            return Mapper.Map<NotificationDTO>(notification);
        }

        /// <summary>
        /// Tạo thông báo mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> CreateNotification(CreateNotificationRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra account tồn tại
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.AccountId);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                var notification = new Notification
                {
                    Title = request.Title,
                    Message = request.Message,
                    Type = request.Type,
                    Icon = request.Icon ?? "",
                    Image = request.Image ?? "",
                    AccountId = request.AccountId,
                    IsReaded = false,
                    CreatedDate = DateTime.UtcNow
                };

                await UnitOfWork.NotificationsRepository.CreateAsync(notification);
                await UnitOfWork.CommitAsync();

                // Gửi thông báo real-time qua SignalR
                await NotificationSocketHub.SendNotificationToUser(request.AccountId.ToString(), notification);

                result.SuccessEventHandler(Mapper.Map<NotificationDTO>(notification));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật thông báo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateNotification(UpdateNotificationRequest request)
        {
            var result = new RestOutput();

            try
            {
                var notification = await UnitOfWork.NotificationsRepository.FirstOrDefault(n => n.Id == request.Id);
                if (notification == null)
                {
                    result.ErrorEventHandler("Thông báo không tồn tại");
                    return result;
                }

                notification.Title = request.Title;
                notification.Message = request.Message;
                notification.Type = request.Type;
                notification.Icon = request.Icon ?? notification.Icon;
                notification.Image = request.Image ?? notification.Image;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<NotificationDTO>(notification));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeleteNotification(long id)
        {
            var result = new RestOutput();

            try
            {
                var notification = await UnitOfWork.NotificationsRepository.FirstOrDefault(n => n.Id == id);
                if (notification == null)
                {
                    result.ErrorEventHandler("Thông báo không tồn tại");
                    return result;
                }

                UnitOfWork.NotificationsRepository.Delete(notification);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Đánh dấu đã đọc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> MarkAsRead(long id)
        {
            var result = new RestOutput();

            try
            {
                var notification = await UnitOfWork.NotificationsRepository.FirstOrDefault(n => n.Id == id);
                if (notification == null)
                {
                    result.ErrorEventHandler("Thông báo không tồn tại");
                    return result;
                }

                notification.IsReaded = true;
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Đánh dấu chưa đọc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> MarkAsUnread(long id)
        {
            var result = new RestOutput();

            try
            {
                var notification = await UnitOfWork.NotificationsRepository.FirstOrDefault(n => n.Id == id);
                if (notification == null)
                {
                    result.ErrorEventHandler("Thông báo không tồn tại");
                    return result;
                }

                notification.IsReaded = false;
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Đánh dấu tất cả đã đọc
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<RestOutput> MarkAllAsRead(long accountId)
        {
            var result = new RestOutput();

            try
            {
                var notifications = await UnitOfWork.NotificationsRepository.GetByCondition(n => 
                    n.AccountId == accountId && !n.IsReaded);

                foreach (var notification in notifications)
                {
                    notification.IsReaded = true;
                }

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Đếm số thông báo chưa đọc
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<int> GetUnreadCount(long accountId)
        {
            return await UnitOfWork.NotificationsRepository.CountByCondition(n => 
                n.AccountId == accountId && !n.IsReaded);
        }

        /// <summary>
        /// Gửi thông báo đến account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<RestOutput> SendNotificationToAccount(long accountId, string title, string message, string type = "info")
        {
            var request = new CreateNotificationRequest
            {
                AccountId = accountId,
                Title = title,
                Message = message,
                Type = type
            };

            return await CreateNotification(request);
        }

        /// <summary>
        /// Gửi thông báo hàng loạt
        /// </summary>
        /// <param name="accountIds"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<RestOutput> SendBulkNotification(List<long> accountIds, string title, string message, string type = "info")
        {
            var result = new RestOutput();

            try
            {
                var notifications = new List<Notification>();

                foreach (var accountId in accountIds)
                {
                    var notification = new Notification
                    {
                        Title = title,
                        Message = message,
                        Type = type,
                        Icon = "",
                        Image = "",
                        AccountId = accountId,
                        IsReaded = false,
                        CreatedDate = DateTime.UtcNow
                    };

                    notifications.Add(notification);
                    await UnitOfWork.NotificationsRepository.CreateAsync(notification);

                    // Gửi thông báo real-time
                    await NotificationSocketHub.SendNotificationToUser(accountId.ToString(), notification);
                }

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler($"Đã gửi {notifications.Count} thông báo");
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Lấy thông báo theo loại
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<NotificationDTO>> GetNotificationsByType(long accountId, string type)
        {
            var notifications = await UnitOfWork.NotificationsRepository.GetByCondition(n => 
                n.AccountId == accountId && n.Type == type);
            return Mapper.Map<List<NotificationDTO>>(notifications.OrderByDescending(n => n.CreatedDate).ToList());
        }

        /// <summary>
        /// Xóa thông báo cũ
        /// </summary>
        /// <param name="daysOld"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeleteOldNotifications(int daysOld)
        {
            var result = new RestOutput();

            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                var oldNotifications = await UnitOfWork.NotificationsRepository.GetByCondition(n => 
                    n.CreatedDate < cutoffDate);

                foreach (var notification in oldNotifications)
                {
                    UnitOfWork.NotificationsRepository.Delete(notification);
                }

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler($"Đã xóa {oldNotifications.Count()} thông báo cũ");
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }
    }
}
