using AutoAppManagement.Models.DTO.ToolFeature;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AutoAppManagement.API.Common.Attribute
{
    /// <summary>
    /// Attribute để kiểm tra quyền truy cập tính năng dựa trên license
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class FeatureAccessAttribute : ActionFilterAttribute
    {
        private readonly string _featureCode;
        private readonly string _usageType;
        private readonly decimal _resourceAmount;

        public FeatureAccessAttribute(string featureCode, string usageType = "Access", decimal resourceAmount = 1)
        {
            _featureCode = featureCode;
            _usageType = usageType;
            _resourceAmount = resourceAmount;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                // Lấy service
                var featureAccessService = context.HttpContext.RequestServices.GetService<IFeatureAccessService>();
                if (featureAccessService == null)
                {
                    context.Result = new ObjectResult(new { message = "Feature access service not found" })
                    {
                        StatusCode = 500
                    };
                    return;
                }

                // Lấy thông tin user từ Claims
                var accountIdClaim = context.HttpContext.User.FindFirst("AccountId") ?? context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !long.TryParse(accountIdClaim.Value, out long accountId))
                {
                    context.Result = new UnauthorizedObjectResult(new { message = "User not authenticated" });
                    return;
                }

                // Lấy license key từ header hoặc query parameter
                var licenseKey = context.HttpContext.Request.Headers["License-Key"].FirstOrDefault() 
                    ?? context.HttpContext.Request.Query["licenseKey"].FirstOrDefault();

                // Kiểm tra quyền truy cập tính năng
                var checkRequest = new CheckFeatureAccessRequest
                {
                    AccountId = accountId,
                    FeatureCode = _featureCode,
                    LicenseKey = licenseKey,
                    UsageType = _usageType,
                    ResourceAmount = _resourceAmount
                };

                var accessResult = await featureAccessService.CheckFeatureAccessAsync(checkRequest);

                if (!accessResult.HasAccess)
                {
                    context.Result = new ObjectResult(new 
                    { 
                        message = "Access denied to feature", 
                        reason = accessResult.Reason,
                        featureCode = _featureCode,
                        hasAccess = false
                    })
                    {
                        StatusCode = 403
                    };
                    return;
                }

                // Lưu thông tin access result vào HttpContext để sử dụng trong action
                context.HttpContext.Items["FeatureAccessResult"] = accessResult;

                // Tiếp tục thực hiện action
                var executedContext = await next();

                // Sau khi action thực hiện xong, ghi nhận usage (nếu thành công)
                if (executedContext.Exception == null && context.HttpContext.Response.StatusCode < 400)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await featureAccessService.RecordFeatureUsageAsync(
                                accountId, 
                                licenseKey ?? "", 
                                _featureCode, 
                                _usageType, 
                                _resourceAmount,
                                GetUsageData(context)
                            );
                        }
                        catch
                        {
                            // Ignore errors in background recording
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error nhưng vẫn cho phép tiếp tục (fail-open policy)
                // Trong môi trường production nên log lại
                await next();
            }
        }

        private string? GetUsageData(ActionExecutingContext context)
        {
            try
            {
                var usageData = new
                {
                    Controller = context.ActionDescriptor.RouteValues["controller"],
                    Action = context.ActionDescriptor.RouteValues["action"],
                    Method = context.HttpContext.Request.Method,
                    Path = context.HttpContext.Request.Path.Value,
                    UserAgent = context.HttpContext.Request.Headers["User-Agent"].FirstOrDefault(),
                    Timestamp = DateTime.UtcNow
                };

                return System.Text.Json.JsonSerializer.Serialize(usageData);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Attribute để kiểm tra quota trước khi thực hiện action tốn tài nguyên
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ResourceQuotaCheckAttribute : ActionFilterAttribute
    {
        private readonly string _featureCode;
        private readonly string _resourceType;
        private readonly string _amountParameterName;

        public ResourceQuotaCheckAttribute(string featureCode, string resourceType, string amountParameterName = "amount")
        {
            _featureCode = featureCode;
            _resourceType = resourceType;
            _amountParameterName = amountParameterName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var featureAccessService = context.HttpContext.RequestServices.GetService<IFeatureAccessService>();
                if (featureAccessService == null)
                {
                    await next();
                    return;
                }

                // Lấy thông tin user
                var accountIdClaim = context.HttpContext.User.FindFirst("AccountId") ?? context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !long.TryParse(accountIdClaim.Value, out long accountId))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // Lấy amount từ parameters
                decimal amount = 1;
                if (context.ActionArguments.ContainsKey(_amountParameterName))
                {
                    if (context.ActionArguments[_amountParameterName] is decimal decimalAmount)
                        amount = decimalAmount;
                    else if (decimal.TryParse(context.ActionArguments[_amountParameterName]?.ToString(), out decimal parsedAmount))
                        amount = parsedAmount;
                }

                // Lấy license key
                var licenseKey = context.HttpContext.Request.Headers["License-Key"].FirstOrDefault() 
                    ?? context.HttpContext.Request.Query["licenseKey"].FirstOrDefault();

                // Kiểm tra quota
                var checkRequest = new CheckFeatureAccessRequest
                {
                    AccountId = accountId,
                    FeatureCode = _featureCode,
                    LicenseKey = licenseKey,
                    UsageType = _resourceType,
                    ResourceAmount = amount
                };

                var accessResult = await featureAccessService.CheckFeatureAccessAsync(checkRequest);

                if (!accessResult.HasAccess)
                {
                    context.Result = new ObjectResult(new 
                    { 
                        message = "Resource quota exceeded or access denied",
                        reason = accessResult.Reason,
                        featureCode = _featureCode,
                        resourceType = _resourceType,
                        requestedAmount = amount,
                        limitInfo = accessResult.LimitInfo
                    })
                    {
                        StatusCode = 429 // Too Many Requests
                    };
                    return;
                }

                // Lưu thông tin để ghi nhận usage sau khi thành công
                context.HttpContext.Items["ResourceUsage"] = new
                {
                    AccountId = accountId,
                    LicenseKey = licenseKey,
                    FeatureCode = _featureCode,
                    ResourceType = _resourceType,
                    Amount = amount
                };

                await next();
            }
            catch
            {
                // Fail-open: nếu có lỗi trong quá trình kiểm tra, vẫn cho phép tiếp tục
                await next();
            }
        }
    }
}
