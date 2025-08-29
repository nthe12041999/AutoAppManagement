using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoAppManagement.API.Common.Attribute
{
    public class CustomAuthorizeAttribute : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Kiểm tra xem người dùng đã được xác thực chưa
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                // Nếu không được xác thực, trả về lỗi 401 Unauthorized
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
