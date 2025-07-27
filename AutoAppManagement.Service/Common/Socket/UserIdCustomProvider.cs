using AutoAppManagement.Models.Constant;
using Microsoft.AspNetCore.SignalR;

namespace AutoAppManagement.Service.Common.Socket
{
    public class UserIdCustomProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(JwtRegisteredClaimsNamesConstant.AccId)?.Value;
        }
    }
}
