using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AutoAppManagement.WebApp.Common.MiddleWare;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            // ntthe nếu không pass validate thì trả 400 và message, không cần log làm gì
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            // Bắt các lỗi HTTP như 401 Unauthorized
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Chuyển hướng sang trang Home/Index khi gặp lỗi 401
                context.Response.Cookies.Delete(CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme);
                context.Response.Redirect("/Home/Index");
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("Đã có lỗi xảy ra. Vui lòng liên hệ admin để xử lý");
            }
        }
        catch (Exception ex)
        {
            // nếu có lỗi đặc biệt thì log ra rồi trả 500
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("Đã có lỗi xảy ra. Vui lòng liên hệ admin để xử lý");
        }
    }

    private string GetEndpointInfo(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            return $"{endpoint.DisplayName}";
        }
        else
        {
            return "Unknown endpoint";
        }
    }
}