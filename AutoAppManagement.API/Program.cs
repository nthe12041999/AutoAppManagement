#region Config service
using AspNetCoreRateLimit;
using AutoAppManagement.API.Common.Mappings;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
#region IConfiguration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

services.AddSingleton(configuration);
#endregion

#region config http
// Add services to the container.
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddHttpContextAccessor();

#endregion

#region Swagger
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Bearer Auth"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        new string[]{}
                    }
                });
});
#endregion

#region config Author, Authen
// sử dụng jwt bear token
var secretKey = configuration.GetSection("Jwt").GetSection("SecretKey").Value;
if (secretKey != null)
{
    var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
    services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(opt =>
    {
        opt.SaveToken = true;
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            // các mã xác thực thông báo
            //grant token
            ValidateIssuer = false,
            ValidateAudience = false,

            //sign token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "Role"
        };
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request for hub
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
}


#endregion

#region config CORS
// TODO: ntthe => xác định lại cần truy cập từ đâu nữa để config thêm nhé -> hiện tại defined allow all rồi
services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", item =>
    {
        item.WithOrigins("https://localhost:44388") // Địa chỉ client
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Cho phép cookie và thông tin xác thực
    });
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

#region Config cache
//TODO: hiện tại đang làm cache distributed => sau có cần cache khác thì custom thêm
services.AddDistributedMemoryCache();
services.AddSingleton<IDistributedCacheCustom, DistributedCacheCustom>();
#endregion

#region Context DB
services.AddDbContext<AutoAppManagementContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("WebFBCommit.API"));
});
#endregion

#region Đăng kí lifetime cho các Service, Respository

//Common
services.AddScoped<IRestOutput, RestOutput>();

services.AddScoped<AutoAppManagementContext>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

// mỗi lần gọi 1 phát tạo insert nên dùng luôn Transient

//Service
services.AddTransient<IAccountsService, AccountsService>();

//Ulti
services.AddTransient<IFileUlti, FileUlti>();

#endregion

#region Config AutoMapper
services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});
#endregion

#region Config Socket

services.AddSignalR();
services.AddTransient<INotificationSocketHub, NotificationSocketHub>();
services.AddSingleton<IUserIdProvider, UserIdCustomProvider>();

#endregion

#endregion

#region Run service pipleline
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region CORS
//app.UseCors("AllowAnyOrigin");
app.UseCors("CorsPolicy"); // Áp dụng chính sách CORS
#endregion

#region DDOS
app.UseIpRateLimiting();
#endregion
app.UseRouting();


#region Authen, Author
app.UseAuthentication();
app.UseAuthorization();
#endregion

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("notificationHub");
});

app.MapControllers();

app.Run();
#endregion