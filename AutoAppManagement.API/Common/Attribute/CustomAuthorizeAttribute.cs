using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoAppManagement.API.Common.Attribute
{
    public class CustomAuthorizeAttribute : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Kiểm tra xem người dùng đã được xác thực chưa
            if (context?.HttpContext?.User?.Identity == null || context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Nếu không, chuyển hướng đến trang đăng nhập
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
