using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoAppManagement.Service.Services
{
    /// <summary>
    /// Background service để tự động dọn dẹp các mã OTP đã hết hạn
    /// </summary>
    public class VerificationCodeCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VerificationCodeCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Chạy mỗi 1 giờ
        private readonly TimeSpan _retentionPeriod = TimeSpan.FromHours(24); // Giữ lại 24 giờ

        public VerificationCodeCleanupService(
            IServiceProvider serviceProvider,
            ILogger<VerificationCodeCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VerificationCodeCleanupService is starting.");

            // Đợi 10 giây sau khi app khởi động mới bắt đầu cleanup
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredCodesAsync();
                    
                    _logger.LogInformation(
                        "Next cleanup scheduled at {NextRun}", 
                        DateTime.UtcNow.Add(_cleanupInterval));
                    
                    await Task.Delay(_cleanupInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("VerificationCodeCleanupService is stopping.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up verification codes");
                    
                    // Nếu lỗi, đợi 5 phút rồi thử lại
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task CleanupExpiredCodesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            
            try
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var repository = unitOfWork.GetRepository<VerificationCode>();

                var cutoffDate = DateTime.UtcNow.Subtract(_retentionPeriod);

                // Lấy các OTP cần xóa:
                // 1. OTP đã được sử dụng và cũ hơn 1 giờ
                // 2. OTP chưa dùng nhưng cũ hơn 24 giờ
                // 3. OTP đã hết hạn và cũ hơn 24 giờ
                var expiredCodes = await repository.GetByCondition(v =>
                    (v.IsUsed && v.UsedDate < DateTime.UtcNow.AddHours(-1)) ||  // Đã dùng > 1h
                    (v.CreatedDate < cutoffDate) ||                               // Cũ > 24h
                    (v.ExpiryDate < cutoffDate));                                 // Hết hạn > 24h

                var expiredList = expiredCodes.ToList();

                if (!expiredList.Any())
                {
                    _logger.LogInformation("No expired verification codes to clean up.");
                    return;
                }

                _logger.LogInformation(
                    "Found {Count} expired verification codes to clean up", 
                    expiredList.Count);

                foreach (var code in expiredList)
                {
                    repository.Delete(code);
                }

                await unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "Successfully cleaned up {Count} verification codes at {Time}",
                    expiredList.Count,
                    DateTime.UtcNow);

                // Log chi tiết theo loại
                var breakdown = expiredList
                    .GroupBy(v => v.Type)
                    .Select(g => new { Type = g.Key, Count = g.Count() });

                foreach (var item in breakdown)
                {
                    _logger.LogInformation(
                        "  - {Type}: {Count} codes", 
                        item.Type, 
                        item.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup verification codes");
                throw;
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VerificationCodeCleanupService is stopping.");
            return base.StopAsync(stoppingToken);
        }
    }
}
