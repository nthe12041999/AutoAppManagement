using AutoAppManagement.Models.BaseEntity;
using Microsoft.AspNetCore.SignalR;

namespace AutoAppManagement.Service.Common.Socket
{
    public class NotificationHub : Hub
    {

    }

    public interface INotificationSocketHub
    {
        Task SendToUser(long userId, Notification message);
        Task SendToAll(string message);
    }
    public class NotificationSocketHub : INotificationSocketHub
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationSocketHub(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        // Gửi thông báo đến một user cụ thể
        public async Task SendToUser(long userId, Notification message)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }

        // Gửi thông báo đến tất cả user
        public async Task SendToAll(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }

        // Gửi thông báo đến một group cụ thể
        public async Task SendToGroup(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }

        //// Tham gia một group
        //public async Task JoinGroup(string groupName)
        //{
        //    await _hubContext.Clients.AddToGroupAsync(Context.ConnectionId, groupName);
        //}

        //// Rời khỏi một group
        //public async Task LeaveGroup(string groupName)
        //{
        //    await _hubContext.Clients.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        //}
    }
}
