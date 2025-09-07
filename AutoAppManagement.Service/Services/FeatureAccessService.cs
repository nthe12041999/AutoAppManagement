using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.ToolFeature;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AutoAppManagement.Service.Services
{
    public class FeatureAccessService : IFeatureAccessService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IToolFeatureRepository _toolFeatureRepository;
        private readonly ILicenseFeatureRepository _licenseFeatureRepository;
        private readonly IFeatureUsageRepository _featureUsageRepository;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IAccountsRepository _accountRepository;

        public FeatureAccessService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _toolFeatureRepository = serviceProvider.GetService<IToolFeatureRepository>()!;
            _licenseFeatureRepository = serviceProvider.GetService<ILicenseFeatureRepository>()!;
            _featureUsageRepository = serviceProvider.GetService<IFeatureUsageRepository>()!;
            _licenseRepository = serviceProvider.GetService<ILicenseRepository>()!;
            _accountRepository = serviceProvider.GetService<IAccountsRepository>()!;
        }

        public async Task<FeatureAccessCheckResult> CheckFeatureAccessAsync(CheckFeatureAccessRequest request)
        {
            var result = new FeatureAccessCheckResult();

            try
            {
                // 1. Kiểm tra tính năng có tồn tại và đang hoạt động không
                var feature = await _toolFeatureRepository.GetByFeatureCodeAsync(request.FeatureCode);
                if (feature == null || !feature.IsActive)
                {
                    result.HasAccess = false;
                    result.Reason = "Tính năng không tồn tại hoặc đã bị vô hiệu hóa";
                    result.IsFeatureEnabled = false;
                    return result;
                }

                result.IsFeatureEnabled = true;

                // 2. Nếu tính năng không yêu cầu license
                if (!feature.RequiresLicense)
                {
                    result.HasAccess = true;
                    result.IsLicenseValid = true;
                    result.IsWithinLimits = true;
                    return result;
                }

                // 3. Tìm license phù hợp
                License? license = null;
                if (!string.IsNullOrEmpty(request.LicenseKey))
                {
                    license = await _licenseRepository.GetLicenseByKey(request.LicenseKey);
                }
                else
                {
                    // Lấy license đang hoạt động của account
                    license = await _licenseRepository.GetActiveLicense(request.AccountId);
                }

                if (license == null)
                {
                    result.HasAccess = false;
                    result.Reason = "Không tìm thấy license hợp lệ";
                    result.IsLicenseValid = false;
                    return result;
                }

                // 4. Kiểm tra license có hợp lệ không
                if (license.Status != "Active" || license.ExpiryDate <= DateTime.UtcNow)
                {
                    result.HasAccess = false;
                    result.Reason = "License đã hết hạn hoặc không hoạt động";
                    result.IsLicenseValid = false;
                    return result;
                }

                result.IsLicenseValid = true;

                // 5. Kiểm tra tính năng có được bật cho license này không
                var licenseFeature = await _licenseFeatureRepository.GetLicenseFeatureAsync(license.Id, feature.Id);
                if (licenseFeature == null || !licenseFeature.IsEnabled)
                {
                    result.HasAccess = false;
                    result.Reason = "Tính năng chưa được kích hoạt cho license này";
                    return result;
                }

                result.LicenseFeature = MapToDTO(licenseFeature);

                // 6. Kiểm tra giới hạn sử dụng
                var limitCheck = await CheckUsageLimitsAsync(request.AccountId, license.Id, feature.Id, request.UsageType, request.ResourceAmount, licenseFeature);
                result.IsWithinLimits = limitCheck.IsWithinLimits;
                result.LimitInfo = limitCheck.LimitInfo;

                if (!result.IsWithinLimits)
                {
                    result.HasAccess = false;
                    result.Reason = limitCheck.Reason;
                    return result;
                }

                // 7. Tất cả điều kiện đều thỏa mãn
                result.HasAccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.HasAccess = false;
                result.Reason = $"Lỗi khi kiểm tra quyền truy cập: {ex.Message}";
                return result;
            }
        }

        public async Task<BaseResponse> RecordFeatureUsageAsync(long accountId, string licenseKey, string featureCode, string usageType = "Access", decimal resourceAmount = 1, string? usageData = null)
        {
            try
            {
                // Kiểm tra quyền truy cập trước khi ghi nhận usage
                var accessCheck = await CheckFeatureAccessAsync(new CheckFeatureAccessRequest
                {
                    AccountId = accountId,
                    FeatureCode = featureCode,
                    LicenseKey = licenseKey,
                    UsageType = usageType,
                    ResourceAmount = resourceAmount
                });

                if (!accessCheck.HasAccess)
                {
                    return BaseResponse.Error($"Không có quyền sử dụng tính năng: {accessCheck.Reason}");
                }

                // Lấy thông tin cần thiết
                var feature = await _toolFeatureRepository.GetByFeatureCodeAsync(featureCode);
                var license = await _licenseRepository.GetLicenseByKey(licenseKey);

                if (feature == null || license == null)
                {
                    return BaseResponse.Error("Không tìm thấy thông tin tính năng hoặc license");
                }

                // Tạo record usage
                var usage = new FeatureUsage
                {
                    AccountId = accountId,
                    LicenseId = license.Id,
                    ToolFeatureId = feature.Id,
                    UsageType = usageType,
                    UsageCount = 1,
                    ResourceConsumed = resourceAmount,
                    UsageData = usageData,
                    UsageDate = DateTime.UtcNow
                };

                // TODO: Có thể lấy thêm thông tin từ HttpContext nếu cần
                // usage.IpAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                // usage.UserAgent = httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"];

                await _featureUsageRepository.CreateAsync(usage);
                // TODO: Cần implement UnitOfWork.SaveAsync cho FeatureUsageRepository
                // await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Ghi nhận sử dụng tính năng thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi ghi nhận sử dụng: {ex.Message}");
            }
        }

        public async Task<List<FeatureUsageReport>> GetUsageReportAsync(FeatureUsageReportRequest request)
        {
            var reports = new List<FeatureUsageReport>();

            try
            {
                var fromDate = request.FromDate ?? DateTime.UtcNow.AddDays(-30);
                var toDate = request.ToDate ?? DateTime.UtcNow;

                IEnumerable<FeatureUsage> usages;

                if (request.AccountId.HasValue)
                {
                    usages = await _featureUsageRepository.GetUsageByAccountAsync(request.AccountId.Value, fromDate, toDate);
                }
                else if (request.LicenseId.HasValue)
                {
                    usages = await _featureUsageRepository.GetUsageByLicenseAsync(request.LicenseId.Value, fromDate, toDate);
                }
                else if (!string.IsNullOrEmpty(request.FeatureCode))
                {
                    usages = await _featureUsageRepository.GetUsageByFeatureAsync(request.FeatureCode, fromDate, toDate);
                }
                else
                {
                    // Lấy tất cả usage trong khoảng thời gian
                    usages = await _featureUsageRepository.GetByCondition(u => u.UsageDate >= fromDate && u.UsageDate <= toDate && !u.IsDeleted);
                }

                // Group theo feature và tính toán thống kê
                var featureUsageDetails = from u in usages
                                        join tf in await _toolFeatureRepository.GetAll() on u.ToolFeatureId equals tf.Id
                                        select new { Usage = u, FeatureCode = tf.FeatureCode, FeatureName = tf.FeatureName };

                var groupedUsages = featureUsageDetails.GroupBy(u => new { u.Usage.ToolFeatureId, u.FeatureCode, u.FeatureName });

                foreach (var group in groupedUsages)
                {
                    var dailyUsage = group
                        .GroupBy(u => u.Usage.UsageDate.Date)
                        .Select(d => new DailyUsageInfo
                        {
                            Date = d.Key,
                            UsageCount = d.Sum(u => u.Usage.UsageCount),
                            ResourceConsumed = d.Sum(u => u.Usage.ResourceConsumed)
                        })
                        .OrderBy(d => d.Date)
                        .ToList();

                    reports.Add(new FeatureUsageReport
                    {
                        FeatureCode = group.Key.FeatureCode ?? "",
                        FeatureName = group.Key.FeatureName ?? "",
                        TotalUsageCount = group.Sum(u => u.Usage.UsageCount),
                        TotalResourceConsumed = group.Sum(u => u.Usage.ResourceConsumed),
                        FirstUsage = group.Min(u => u.Usage.UsageDate),
                        LastUsage = group.Max(u => u.Usage.UsageDate),
                        DailyUsage = dailyUsage
                    });
                }

                return reports;
            }
            catch (Exception ex)
            {
                // Log error và trả về empty list
                return new List<FeatureUsageReport>();
            }
        }

        public async Task<bool> IsWithinUsageLimitsAsync(long accountId, long licenseId, long toolFeatureId, string usageType, decimal requestedAmount)
        {
            var checkResult = await CheckUsageLimitsAsync(accountId, licenseId, toolFeatureId, usageType, requestedAmount, null);
            return checkResult.IsWithinLimits;
        }

        private async Task<(bool IsWithinLimits, FeatureLimitInfo? LimitInfo, string? Reason)> CheckUsageLimitsAsync(
            long accountId, long licenseId, long toolFeatureId, string usageType, decimal requestedAmount, LicenseFeature? licenseFeature)
        {
            try
            {
                if (licenseFeature == null)
                {
                    licenseFeature = await _licenseFeatureRepository.GetLicenseFeatureAsync(licenseId, toolFeatureId);
                }

                if (licenseFeature == null || string.IsNullOrEmpty(licenseFeature.UsageQuota))
                {
                    // Không có giới hạn được định nghĩa
                    return (true, null, null);
                }

                // Parse usage quota (JSON format)
                var quotaConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(licenseFeature.UsageQuota);
                if (quotaConfig == null)
                {
                    return (true, null, null);
                }

                // Kiểm tra các loại giới hạn
                var limitChecks = new List<(string Type, bool IsWithinLimit, FeatureLimitInfo? LimitInfo)>();

                // Daily limit
                if (quotaConfig.ContainsKey("daily"))
                {
                    var dailyLimit = Convert.ToDecimal(quotaConfig["daily"]);
                    var today = DateTime.UtcNow.Date;
                    var todayUsage = await _featureUsageRepository.GetTotalResourceUsageAsync(accountId, toolFeatureId, usageType, today, today.AddDays(1));
                    
                    var limitInfo = new FeatureLimitInfo
                    {
                        LimitType = "Daily",
                        MaxAllowed = dailyLimit,
                        CurrentUsage = todayUsage,
                        ResetDate = today.AddDays(1)
                    };

                    limitChecks.Add(("Daily", todayUsage + requestedAmount <= dailyLimit, limitInfo));
                }

                // Monthly limit
                if (quotaConfig.ContainsKey("monthly"))
                {
                    var monthlyLimit = Convert.ToDecimal(quotaConfig["monthly"]);
                    var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1);
                    var monthlyUsage = await _featureUsageRepository.GetTotalResourceUsageAsync(accountId, toolFeatureId, usageType, startOfMonth, endOfMonth);
                    
                    var limitInfo = new FeatureLimitInfo
                    {
                        LimitType = "Monthly",
                        MaxAllowed = monthlyLimit,
                        CurrentUsage = monthlyUsage,
                        ResetDate = endOfMonth
                    };

                    limitChecks.Add(("Monthly", monthlyUsage + requestedAmount <= monthlyLimit, limitInfo));
                }

                // Total limit (lifetime)
                if (quotaConfig.ContainsKey("total"))
                {
                    var totalLimit = Convert.ToDecimal(quotaConfig["total"]);
                    var totalUsage = await _featureUsageRepository.GetTotalResourceUsageAsync(accountId, toolFeatureId, usageType, DateTime.MinValue, DateTime.MaxValue);
                    
                    var limitInfo = new FeatureLimitInfo
                    {
                        LimitType = "Total",
                        MaxAllowed = totalLimit,
                        CurrentUsage = totalUsage
                    };

                    limitChecks.Add(("Total", totalUsage + requestedAmount <= totalLimit, limitInfo));
                }

                // Kiểm tra xem có giới hạn nào bị vượt quá không
                var violatedLimit = limitChecks.FirstOrDefault(c => !c.IsWithinLimit);
                if (violatedLimit.Type != null)
                {
                    return (false, violatedLimit.LimitInfo, $"Đã vượt quá giới hạn {violatedLimit.Type.ToLower()}");
                }

                // Trả về thông tin limit có usage cao nhất (để hiển thị cảnh báo)
                var highestUsageLimit = limitChecks
                    .Where(c => c.LimitInfo != null)
                    .OrderByDescending(c => c.LimitInfo!.CurrentUsage / c.LimitInfo.MaxAllowed)
                    .FirstOrDefault();

                return (true, highestUsageLimit.LimitInfo, null);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi khi parse hoặc kiểm tra, cho phép sử dụng (fail-open)
                return (true, null, null);
            }
        }

        private LicenseFeatureDTO MapToDTO(LicenseFeature entity)
        {
            // Simple mapping - trong thực tế nên dùng AutoMapper
            return new LicenseFeatureDTO
            {
                Id = entity.Id,
                LicenseId = entity.LicenseId,
                ToolFeatureId = entity.ToolFeatureId,
                IsEnabled = entity.IsEnabled,
                ResourceLimits = entity.ResourceLimits,
                UsageQuota = entity.UsageQuota,
                EffectiveFrom = entity.EffectiveFrom,
                EffectiveTo = entity.EffectiveTo,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                IsDeleted = entity.IsDeleted,
                DeletedDate = entity.DeletedDate,
                DeletedBy = entity.DeletedBy,
                Notes = entity.Notes,
                Status = entity.Status
            };
        }
    }
}
