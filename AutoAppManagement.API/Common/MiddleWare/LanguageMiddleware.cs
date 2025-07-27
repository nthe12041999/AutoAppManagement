using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace AutoAppManagement.API.Common.MiddleWare
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            // Đọc ngôn ngữ đã chọn từ cookie hoặc session => lấy từ language của account
            // nếu cookie không có lấy mặc định vi
            var language = httpContext.Request.Cookies["Language"] ?? "vi-VN";

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("vi-VN"),
                new CultureInfo("en-US")
            };

            var culture = supportedCultures.FirstOrDefault(c => c.Name == language) ?? new CultureInfo(language);
            var requestCulture = new RequestCulture(culture);

            // Thiết lập toàn bộ ngôn ngữ cho app
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LanguageMiddlewareExtensions
    {
        public static IApplicationBuilder UseLanguageMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LanguageMiddleware>();
        }
    }
}
