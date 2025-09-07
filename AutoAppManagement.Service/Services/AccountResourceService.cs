using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.ToolFeature;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AutoAppManagement.Service.Services
{
    public interface IAccountResourceService
    {
        Task<List<ToolResourceDTO>> GetAccountResourcesAsync(long accountId, long licenseId);
        Task<LoginWithResourcesResponse> GetLoginWithResourcesAsync(AccountDTO account, LicenseInfoDTO licenseInfo, string token, DateTime tokenExpiry);
    }

    public class AccountResourceService : IAccountResourceService
    {
        private readonly IFeatureAccessService _featureAccessService;
        private readonly IToolFeatureService _toolFeatureService;
        private readonly ILicenseFeatureService _licenseFeatureService;

        public AccountResourceService(
            IFeatureAccessService featureAccessService,
            IToolFeatureService toolFeatureService,
            ILicenseFeatureService licenseFeatureService)
        {
            _featureAccessService = featureAccessService;
            _toolFeatureService = toolFeatureService;
            _licenseFeatureService = licenseFeatureService;
        }

        public async Task<List<ToolResourceDTO>> GetAccountResourcesAsync(long accountId, long licenseId)
        {
            var resources = new List<ToolResourceDTO>();

            try
            {
                // Lấy tất cả features được assign cho license này
                var licenseFeatures = await _licenseFeatureService.GetByLicenseIdAsync(licenseId);

                foreach (var licenseFeature in licenseFeatures)
                {
                    // Lấy thông tin chi tiết của feature
                    var feature = await _toolFeatureService.GetByIdAsync(licenseFeature.ToolFeatureId);
                    if (feature == null) continue;

                    // Check feature access và usage
                    var checkRequest = new CheckFeatureAccessRequest 
                    { 
                        AccountId = accountId, 
                        FeatureCode = feature.FeatureCode 
                    };
                    var accessResult = await _featureAccessService.CheckFeatureAccessAsync(checkRequest);

                    var resource = new ToolResourceDTO
                    {
                        FeatureId = feature.Id,
                        FeatureName = feature.FeatureName,
                        FeatureCode = feature.FeatureCode,
                        ToolName = feature.Category ?? "Unknown",
                        Description = feature.Description ?? "",
                        IsEnabled = licenseFeature.IsEnabled && accessResult.HasAccess,
                        UsageLimit = !string.IsNullOrEmpty(licenseFeature.UsageQuota) ? 
                            JsonConvert.DeserializeObject<int?>(licenseFeature.UsageQuota) : null,
                        UsedCount = (int)(accessResult.LimitInfo?.CurrentUsage ?? 0),
                        PeriodStart = licenseFeature.EffectiveFrom,
                        PeriodEnd = licenseFeature.EffectiveTo,
                        LimitType = licenseFeature.Status // Sử dụng Status để represent limit type tạm thời
                    };

                    // Tính remaining count
                    if (resource.UsageLimit.HasValue)
                    {
                        resource.RemainingCount = Math.Max(0, resource.UsageLimit.Value - resource.UsedCount);
                    }
                    else
                    {
                        resource.RemainingCount = 999999; // Unlimited
                    }

                    // Xác định status
                    if (!resource.IsEnabled)
                    {
                        resource.Status = "disabled";
                        resource.WarningMessage = "Tính năng không được kích hoạt";
                    }
                    else if (resource.UsageLimit.HasValue && resource.RemainingCount <= 0)
                    {
                        resource.Status = "exhausted";
                        resource.WarningMessage = "Đã hết lượt sử dụng";
                    }
                    else if (resource.UsageLimit.HasValue && resource.RemainingCount <= 5)
                    {
                        resource.Status = "limited";
                        resource.WarningMessage = $"Còn lại {resource.RemainingCount} lượt sử dụng";
                    }
                    else
                    {
                        resource.Status = "available";
                        resource.WarningMessage = "";
                    }

                    // Check period validity
                    var now = DateTime.UtcNow;
                    if (resource.PeriodStart.HasValue && now < resource.PeriodStart.Value)
                    {
                        resource.Status = "not_started";
                        resource.WarningMessage = $"Tính năng sẽ khả dụng từ {resource.PeriodStart.Value:dd/MM/yyyy}";
                    }
                    else if (resource.PeriodEnd.HasValue && now > resource.PeriodEnd.Value)
                    {
                        resource.Status = "expired";
                        resource.WarningMessage = $"Tính năng đã hết hạn từ {resource.PeriodEnd.Value:dd/MM/yyyy}";
                    }

                    resources.Add(resource);
                }

                return resources.OrderBy(r => r.ToolName).ThenBy(r => r.FeatureName).ToList();
            }
            catch (Exception)
            {
                // Log error
                return new List<ToolResourceDTO>();
            }
        }

        public async Task<LoginWithResourcesResponse> GetLoginWithResourcesAsync(
            AccountDTO account, 
            LicenseInfoDTO licenseInfo, 
            string token, 
            DateTime tokenExpiry)
        {
            var resources = await GetAccountResourcesAsync(account.Id, licenseInfo.LicenseId);

            return new LoginWithResourcesResponse
            {
                Account = account,
                LicenseInfo = licenseInfo,
                AvailableResources = resources,
                LoginTime = DateTime.UtcNow,
                Message = "Đăng nhập thành công",
                Token = token,
                TokenExpiry = tokenExpiry
            };
        }
    }
}
