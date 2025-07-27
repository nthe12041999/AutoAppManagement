namespace AutoAppManagement.API.Common.MiddleWare;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context) // , ILogEntryService logEntryService)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            // ntthe nếu không pass validate thì trả 400 và message, không cần log làm gì
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(ex.Message);
        }
        catch (Exception ex)
        {
            // nếu có lỗi đặc biệt thì log ra rồi trả 500
            //logEntryService.LogError(ex.ToString(), GetEndpointInfo(context));
            context.Response.StatusCode = 500;
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