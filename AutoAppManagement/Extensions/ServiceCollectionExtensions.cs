using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Services;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace AutoAppManagement.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Đăng ký các services cho ứng dụng
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Đăng ký DbContext
            services.AddDbContext<AutoAppManagementContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("AutoAppManagement.API")
                );
            });

            // Đăng ký HttpClient
            services.AddHttpClient();

            // Đăng ký HttpContextAccessor
            services.AddHttpContextAccessor();

            // Đăng ký RestOutput
            services.AddScoped<RestOutput>();

            // Đăng ký các services
            // services.AddScoped<ICustomerAccountService, CustomerAccountService>(); // Removed
            services.AddScoped<AutoAppManagement.WebApp.Services.ILicenseService, AutoAppManagement.WebApp.Services.LicenseService>();
            services.AddScoped<AutoAppManagement.WebApp.Services.IAdminAccountService, AutoAppManagement.WebApp.Services.AdminAccountService>();
            
            // Tool Management Services
            services.AddScoped<AutoAppManagement.Service.Services.IToolService, AutoAppManagement.Service.Services.ToolService>();
            services.AddScoped<AutoAppManagement.Service.Services.IToolVersionService, AutoAppManagement.Service.Services.ToolVersionService>();
            services.AddScoped<AutoAppManagement.Service.Services.IToolCategoryService, AutoAppManagement.Service.Services.ToolCategoryService>();
            services.AddScoped<AutoAppManagement.Service.Services.IToolFeatureService, AutoAppManagement.Service.Services.ToolFeatureService>();
            services.AddScoped<AutoAppManagement.Service.Services.IFeatureAccessService, AutoAppManagement.Service.Services.FeatureAccessService>();
            services.AddScoped<AutoAppManagement.Service.Services.IAccountResourceService, AutoAppManagement.Service.Services.AccountResourceService>();

            // Đăng ký các repositories
            services.AddScoped<IAdminAccountRepository, AdminAccountRepository>();
            services.AddScoped<IRoleAccountRepository, RoleAccountRepository>();
            
            // Tool Management Repositories
            services.AddScoped<IToolRepository, ToolRepository>();
            services.AddScoped<IToolVersionRepository, ToolVersionRepository>();
            services.AddScoped<IToolCategoryRepository, ToolCategoryRepository>();
            services.AddScoped<IToolFeatureRepository, ToolFeatureRepository>();

            return services;
        }

        /// <summary>
        /// Cấu hình Authentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                    options.Cookie.Name = "AutoAppManagement.Auth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });

            services.AddAuthorization(options =>
            {
                // Định nghĩa các policy
                options.AddPolicy(
                    "AdminOnly",
                    policy => policy.RequireRole("admin", "super_admin")
                );

                options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("super_admin"));

                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("customer"));

                options.AddPolicy("SupportOnly", policy => policy.RequireRole("support"));

                // Policy cho các chức năng cụ thể
                options.AddPolicy(
                    "ManageUsers",
                    policy => policy.RequireRole("admin", "super_admin")
                );

                options.AddPolicy(
                    "ManageLicenses",
                    policy => policy.RequireRole("admin", "super_admin")
                );

                options.AddPolicy("ManageRoles", policy => policy.RequireRole("super_admin"));

                options.AddPolicy(
                    "ViewReports",
                    policy => policy.RequireRole("admin", "super_admin", "support")
                );
            });

            return services;
        }

        /// <summary>
        /// Cấu hình CORS
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCorsServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "DefaultPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );

                options.AddPolicy(
                    "RestrictedPolicy",
                    builder =>
                    {
                        builder
                            .WithOrigins("https://localhost:7001", "https://autoapp.local")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                );
            });

            return services;
        }

        /// <summary>
        /// Cấu hình Session
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSessionServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "AutoAppManagement.Session";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            return services;
        }

        /// <summary>
        /// Cấu hình Logging
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddLoggingServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.AddDebug();

                // Có thể thêm các provider khác như Serilog, NLog, etc.
                if (configuration.GetValue<bool>("Logging:EnableFileLogging"))
                {
                    // TODO: Cấu hình file logging
                }
            });

            return services;
        }

        /// <summary>
        /// Cấu hình các services khác
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOtherServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Cấu hình JSON options
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = null; // Giữ nguyên tên property
                options.SerializerOptions.WriteIndented = true;
            });

            // Cấu hình MVC options
            services.AddMvc(options =>
            {
                // Thêm các filter global nếu cần
                // options.Filters.Add<GlobalExceptionFilter>();
            });

            // Cấu hình Antiforgery
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = "AutoAppManagement.Antiforgery";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            return services;
        }

        /// <summary>
        /// Đăng ký tất cả services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAllServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddApplicationServices(configuration);
            services.AddAuthenticationServices(configuration);
            services.AddCorsServices(configuration);
            services.AddSessionServices(configuration);
            services.AddLoggingServices(configuration);
            services.AddOtherServices(configuration);

            return services;
        }
    }
}
