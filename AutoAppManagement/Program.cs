using AspNetCoreRateLimit;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Common.MiddleWare;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;

#region Config service
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
builder.Services.AddControllersWithViews();

services.AddHttpClient();
services.AddHttpContextAccessor();

#region IConfiguration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

services.AddSingleton(configuration);
#endregion

#region Localize
services.AddLocalization(options => options.ResourcesPath = "Resources");
services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var supportedCultures = new[]
{
    new CultureInfo("vi-VN"),
    new CultureInfo("en-US"),
};
services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("vi-VN");

    // Formatting numbers, dates, etc.
    options.SupportedCultures = supportedCultures;

    // UI strings that we have localized.
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider()
    };
});

#endregion

#region config CORS
// TODO: ntthe => xác định lại cần truy cập từ đâu nữa để config thêm nhé -> hiện tại defined allow all rồi
services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
#endregion

#region Xử lý DDOS
services.AddMemoryCache();
services.Configure<IpRateLimitOptions>(options => configuration.GetSection("IPRateLimiting").Bind(options));
services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
#endregion

#region config Author, Authen
// sử dụng cookie token
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login"; // Đường dẫn trang đăng nhập
    options.LogoutPath = "/Account/Logout"; // Đường dẫn trang đăng xuất
});
;
#endregion

#region Đăng kí lifetime cho các Service

services.AddScoped<IAccountsService, AccountsService>();
services.AddScoped<INotificationService, NotificationsService>();
services.AddScoped<RestOutput>();

#endregion

#endregion

#region Run service pipleline

var app = builder.Build();

#region Localize
var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseMiddleware<LanguageMiddleware>();
#endregion
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

#region CORS
app.UseCors("AllowAnyOrigin");
#endregion

#region DDOS
app.UseIpRateLimiting();
#endregion

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseMiddleware<ErrorHandlingMiddleware>();
app.Run();

#endregion