using AutoAppManagement.Service.Services;
using System.Security.Claims;

namespace AutoAppManagement.API.Common.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;

        public JwtMiddleware(RequestDelegate next, IJwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var principal = _jwtService.ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                    
                    // Thêm thông tin user vào HttpContext.Items để dễ truy cập
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
                    var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                    
                    context.Items["UserId"] = userId;
                    context.Items["UserName"] = userName;
                    context.Items["Email"] = email;
                }
            }
            catch
            {
                // Token validation failed, do nothing
            }
        }
    }
}
