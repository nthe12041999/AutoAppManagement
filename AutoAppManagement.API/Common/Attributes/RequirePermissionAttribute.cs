using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AutoAppManagement.API.Common.Attribute
{
    /// <summary>
    /// Attribute để check quyền truy cập API
    /// Sử dụng: [RequirePermission("PERMISSION_VIEW", "PERMISSION_EDIT")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : System.Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public RequirePermissionAttribute(params string[] permissions)
        {
            _permissions = permissions ?? Array.Empty<string>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Nếu không yêu cầu permission nào, cho phép truy cập
            if (_permissions.Length == 0)
            {
                return;
            }

            var user = context.HttpContext.User;

            // Kiểm tra đã authenticated chưa
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    isSuccess = false,
                    message = "Bạn chưa đăng nhập"
                });
                return;
            }

            // Lấy userId từ claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    isSuccess = false,
                    message = "Không tìm thấy thông tin user"
                });
                return;
            }

            // Lấy permissions từ claims (được add khi login)
            var userPermissions = user.FindAll("permission").Select(c => c.Value).ToList();

            // Check xem user có ít nhất 1 trong các permissions yêu cầu không
            var hasPermission = _permissions.Any(p => userPermissions.Contains(p));

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new ObjectResult(new
                {
                    isSuccess = false,
                    message = $"Bạn không có quyền truy cập. Yêu cầu: {string.Join(" hoặc ", _permissions)}"
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
