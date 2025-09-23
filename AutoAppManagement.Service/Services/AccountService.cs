using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AccountDevice;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static AutoAppManagement.Models.Enum.DataModelType;
using AutoAppManagement.Models.Enum;
using Newtonsoft.Json;

namespace AutoAppManagement.Service.Services
{
    public interface IAccountService : IBaseBusinessService<AccountDTO>
    {
        Task<AccountDTO> GetAccountByUsername(string username);
        Task<BaseResponse> ChangePassword(long id, string newPassword);
        Task<BaseResponse> LockAccount(long id, string reason = "");
        Task<BaseResponse> UnlockAccount(long id);
        Task<BaseResponse> ActivateAccount(long id);
        Task<BaseResponse> DeactivateAccount(long id);
        Task<List<AccountDTO>> GetExpiredAccounts();
        Task<List<AccountDTO>> GetExpiringAccounts(int days);
        Task<BaseResponse> ExtendAccount(long id, DateTime newExpiryDate);
        Task<BaseResponse> UpdateAccountInfo(UpdateAccountInfoRequest request);
        Task<BaseResponse> UploadAvatar(long id, string avatarPath);
        Task<BaseResponse> Login(LoginRequest request);

        // AccountDevice methods
        Task<List<AccountDeviceDTO>> GetAllAccountDevices();
        Task<List<AccountDeviceDTO>> GetAccountDevicesByAccountId(long accountId);
        Task<AccountDeviceDTO> GetAccountDeviceById(long id);
        Task<BaseResponse> RegisterDevice(RegisterDeviceRequest request);
        Task<BaseResponse> UpdateDevice(UpdateDeviceRequest request);
        Task<BaseResponse> DeleteDevice(long id);
        Task<BaseResponse> ActivateDevice(long id);
        Task<BaseResponse> DeactivateDevice(long id);
        Task<List<AccountDeviceDTO>> GetActiveDevices(long accountId);
        Task<List<AccountDeviceDTO>> GetDevicesByType(string deviceType);
        Task<bool> IsDeviceRegistered(string deviceId, long accountId);
    }

    public class AccountService : BaseBusinessService<Account, AccountDTO, IAccountsRepository>, IAccountService
    {
        // Lazy load repositories thay vì direct injection
        private IGenericRepository<License>? _licenseRepository;
        protected IGenericRepository<License> LicenseRepository
            => _licenseRepository ??= UnitOfWork.GetRepository<License>();

        private IGenericRepository<AccountDevice>? _accountDeviceRepository;
        protected IGenericRepository<AccountDevice> AccountDeviceRepository
            => _accountDeviceRepository ??= UnitOfWork.GetRepository<AccountDevice>();

        private IJwtService? _jwtService;
        protected IJwtService JwtService
            => _jwtService ??= _serviceProvider.GetRequiredService<IJwtService>();

        public AccountService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<AccountDTO> GetAccountByUsername(string username)
        {
            var account = await Repository.FirstOrDefault(a => a.UserName == username && a.Status == StatusEnum.Active);
            return Mapper.Map<AccountDTO>(account);
        }

        public async Task<BaseResponse> ChangePassword(long id, string newPassword)
        {
            try
            {
                var account = await UpdateById(id);

                account.Password = HashCodeUlti.EncodePassword(newPassword);
                account.SetUpdated(GetCurrentUserId());
                // EF Core tracking will detect changes automatically
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đổi mật khẩu: {ex.Message}");
            }
        }

        public async Task<BaseResponse> LockAccount(long id, string reason = "")
        {
            try
            {
                var account = await UpdateById(id);
                account.IsLocked = true;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Khóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi khóa tài khoản: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UnlockAccount(long id)
        {
            try
            {
                var account = await UpdateById(id);
                account.IsLocked = false;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Mở khóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi mở khóa tài khoản: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ActivateAccount(long id)
        {
            try
            {
                var account = await UpdateById(id);
                account.Status = StatusEnum.Active;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Kích hoạt tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi kích hoạt tài khoản: {ex.Message}");
            }
        }

        public async Task<BaseResponse> DeactivateAccount(long id)
        {
            try
            {
                var account = await UpdateById(id);
                account.Status = StatusEnum.Inactive;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Vô hiệu hóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi vô hiệu hóa tài khoản: {ex.Message}");
            }
        }

        public async Task<List<AccountDTO>> GetExpiredAccounts()
        {
            var accounts = await Repository.GetByCondition(a => a.ExpiredDate < DateTime.UtcNow && a.Status == StatusEnum.Active);
            return Mapper.Map<List<AccountDTO>>(accounts.ToList());
        }

        public async Task<List<AccountDTO>> GetExpiringAccounts(int days)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var accounts = await Repository.GetByCondition(a => a.ExpiredDate <= expiryDate && a.ExpiredDate > DateTime.UtcNow && a.Status == StatusEnum.Active);
            return Mapper.Map<List<AccountDTO>>(accounts.ToList());
        }

        public async Task<BaseResponse> ExtendAccount(long id, DateTime newExpiryDate)
        {
            try
            {
                var account = await UpdateById(id);

                if (newExpiryDate <= account.ExpiredDate)
                {
                    return BaseResponse.Error("Ngày hết hạn mới phải sau ngày hết hạn hiện tại");
                }

                account.ExpiredDate = newExpiryDate;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<AccountDTO>(account), "Gia hạn tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gia hạn tài khoản: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UpdateAccountInfo(UpdateAccountInfoRequest request)
        {
            try
            {
                var account = await UpdateById(request.Id);

                var dto = Mapper.Map<AccountDTO>(account);
                Mapper.Map(request, dto);
                dto.State = EntityState.Edit;

                return await SubmitData(dto);
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật thông tin tài khoản: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UploadAvatar(long id, string avatarPath)
        {
            try
            {
                var account = await UpdateById(id);
                account.ImgAvatar = avatarPath;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Cập nhật avatar thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật avatar: {ex.Message}");
            }
        }

        // AccountDevice methods
        public async Task<List<AccountDeviceDTO>> GetAllAccountDevices()
        {
            var devices = await AccountDeviceRepository.GetAll();
            return Mapper.Map<List<AccountDeviceDTO>>(devices.Where(d => d.Status == StatusEnum.Active).ToList());
        }

        public async Task<List<AccountDeviceDTO>> GetAccountDevicesByAccountId(long accountId)
        {
            var devices = await AccountDeviceRepository.GetByCondition(d => d.AccountId == accountId && d.Status == StatusEnum.Active);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        public async Task<AccountDeviceDTO> GetAccountDeviceById(long id)
        {
            var device = await AccountDeviceRepository.FirstOrDefault(d => d.ID == id && d.Status == StatusEnum.Active);
            return Mapper.Map<AccountDeviceDTO>(device);
        }

        public async Task<BaseResponse> RegisterDevice(RegisterDeviceRequest request)
        {
            try
            {
                var existingDevice = await AccountDeviceRepository.FirstOrDefault(d => d.DeviceId == request.DeviceId && d.AccountId == request.AccountId && d.Status == StatusEnum.Active);
                if (existingDevice != null) return BaseResponse.Error("Device đã được đăng ký cho account này");

                var device = Mapper.Map<AccountDevice>(request);

                await AccountDeviceRepository.Insert(device);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<AccountDeviceDTO>(device), "Đăng ký device thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đăng ký device: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UpdateDevice(UpdateDeviceRequest request)
        {
            try
            {
                var device = await AccountDeviceRepository.FirstOrDefault(d => d.ID == request.Id && d.Status == StatusEnum.Active);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                Mapper.Map(request, device);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<AccountDeviceDTO>(device), "Cập nhật device thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật device: {ex.Message}");
            }
        }

        public async Task<BaseResponse> DeleteDevice(long id)
        {
            try
            {
                var device = await AccountDeviceRepository.FirstOrDefault(d => d.ID == id && d.Status == StatusEnum.Active);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Xóa device thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xóa device: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ActivateDevice(long id)
        {
            try
            {
                var device = await AccountDeviceRepository.FirstOrDefault(d => d.ID == id && d.Status == StatusEnum.Active);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                device.Status = StatusEnum.Active;
                device.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Kích hoạt device thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi kích hoạt device: {ex.Message}");
            }
        }

        public async Task<BaseResponse> DeactivateDevice(long id)
        {
            try
            {
                var device = await AccountDeviceRepository.FirstOrDefault(d => d.ID == id && d.Status == StatusEnum.Active);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                device.Status = StatusEnum.Inactive;
                device.SetUpdated(GetCurrentUserId());

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Vô hiệu hóa device thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi vô hiệu hóa device: {ex.Message}");
            }
        }

        public async Task<List<AccountDeviceDTO>> GetActiveDevices(long accountId)
        {
            var devices = await AccountDeviceRepository.GetByCondition(d => d.AccountId == accountId && d.Status == StatusEnum.Active);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        public async Task<List<AccountDeviceDTO>> GetDevicesByType(string deviceType)
        {
            var deviceTypeEnum = Enum.Parse<DeviceType>(deviceType, true);
            var devices = await AccountDeviceRepository.GetByCondition(d => d.DeviceType == deviceTypeEnum && d.Status == StatusEnum.Active);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        public async Task<bool> IsDeviceRegistered(string deviceId, long accountId)
        {
            return await AccountDeviceRepository.Any(d => d.DeviceId == deviceId && d.AccountId == accountId && d.Status == StatusEnum.Active);
        }

        // TEMPORARILY COMMENTED OUT - Need to implement proper repository methods
        // public async Task<BaseResponse> RefreshToken(RefreshTokenRequestDTO request)
        // {
        //     // Implementation needs proper repository methods and DTO definitions
        //     return BaseResponse.Error("RefreshToken method needs to be implemented with proper dependencies");
        // }

        public override Task CustomBeforeSubmitData(AccountDTO dto)
        {
            switch (dto.State)
            {
                case AutoAppManagement.Models.Common.EntityState.Add:
                    dto.Password = HashCodeUlti.EncodePassword(dto.Password);
                    break;

            }
            return Task.CompletedTask;
        }

        public async Task<BaseResponse> Login(LoginRequest request)
        {
            try
            {
                #region Xử lý login

                // Validate input
                if (string.IsNullOrEmpty(request.EmailOrPhone) || string.IsNullOrEmpty(request.Password))
                {
                    return BaseResponse.Error("Email/SĐT và mật khẩu không được để trống");
                }

                // Find account by email or phone
                var account = await Repository.FirstOrDefault(a => 
                    (a.Email == request.EmailOrPhone || a.Phone == request.EmailOrPhone) && a.Status == StatusEnum.Active);

                if (account == null)
                {
                    return BaseResponse.Error("Tài khoản không tồn tại");
                }

                // Check password
                var hashedPassword = HashCodeUlti.EncodePassword(request.Password);
                if (account.Password != hashedPassword)
                {
                    return BaseResponse.Error("Mật khẩu không chính xác");
                }

                // Check account status
                if (account.IsLocked)
                {
                    return BaseResponse.Error("Tài khoản đã bị khóa");
                }

                if (account.Status != StatusEnum.Active)
                {
                    return BaseResponse.Error("Tài khoản chưa được kích hoạt");
                }

                if (account.ExpiredDate < DateTime.UtcNow)
                {
                    return BaseResponse.Error("Tài khoản đã hết hạn");
                }

                // Check license
                var licenseCheckResult = await CheckAccountLicense(account);
                if (!licenseCheckResult.IsSuccess)
                {
                    return licenseCheckResult;
                }

                // Generate JWT token
                var licenseInfo = licenseCheckResult.Data as LicenseInfoDTO;
                var token = JwtService.GenerateToken(account, licenseInfo);

                // Update login info - sử dụng property đúng của Account entity
                // account.LastLoginAt = DateTime.UtcNow; // Property này không tồn tại
                account.SetUpdated(1); // Sử dụng SetUpdated thay thế
                await UnitOfWork.SaveAsync();

                #endregion

                #region Xử lý resource

                // Get additional resources and features
                var loginData = new LoginWithResourcesResponse
                {
                    Token = token.AccessToken,
                    LoginTime = DateTime.UtcNow,
                    LicenseInfo = licenseInfo
                };

                if (account?.LicenseId != null)
                {
                    // Get available resources and features for this account
                    var license = await LicenseRepository.FirstOrDefault(l => l.ID == account.LicenseId);
                    if (license != null)
                    {
                        loginData.AllowedFeatures = JsonConvert.DeserializeObject<List<string>>(license.Features);
                        loginData.AvailableResources = ParseResourceLimits(license.FeatureLimits);
                    }
                }

                return BaseResponse.Success(loginData, "Đăng nhập thành công");

                #endregion
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đăng nhập: {ex.Message}");
            }
        }

        private async Task<BaseResponse> CheckAccountLicense(Account account)
        {
            try
            {
                if (account.LicenseId == null)
                {
                    return BaseResponse.Error("Tài khoản chưa có license");
                }

                var license = await LicenseRepository.FirstOrDefault(l => l.ID == account.LicenseId && l.Status == StatusEnum.Active);
                if (license == null)
                {
                    return BaseResponse.Error("License không tồn tại");
                }

                //if (license.ExpiryDate < DateTime.UtcNow)
                //{
                //    return BaseResponse.Error("License đã hết hạn");
                //}

                //if (license.Status != "Active")
                //{
                //    return BaseResponse.Error("License không hoạt động");
                //}

                var licenseInfo = new LicenseInfoDTO
                {
                    LicenseId = license.ID,
                    LicenseKey = license.LicenseKey,
                    LicenseName = license.LicenseName,
                    LicenseType = license.LicenseType.ToString(),
                    StartDate = license.StartDate,
                    EndDate = license.EndDate ?? DateTime.MaxValue,
                    Status = license.Status,
                    DaysRemaining = license.EndDate != null ? (int)(license.EndDate.Value - DateTime.UtcNow).TotalDays : 0
                };

                return BaseResponse.Success(licenseInfo, "License hợp lệ");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi kiểm tra license: {ex.Message}");
            }
        }

        /// <summary>
        /// Map feature code to ID
        /// </summary>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        private int GetFeatureIdByCode(string featureCode)
        {
            // Simple mapping - có thể mở rộng thành lookup table hoặc database query
            return featureCode?.ToUpper() switch
            {
                "SEND_MESSAGE" => 1,
                "ADD_FRIEND" => 2,
                "BULK_SEND_MESSAGE" => 3,
                "AI_MESSAGE" => 4,
                "SEND_IMAGE" => 5,
                "SEND_VIDEO" => 6,
                "GROUP_CHAT" => 7,
                "FILE_SHARING" => 8,
                "VOICE_CALL" => 9,
                "VIDEO_CALL" => 10,
                _ => 0 // Unknown feature code
            };
        }

        /// <summary>
        /// Parse resource limits từ JSON string và trả về đầy đủ ToolResourceDTO
        /// </summary>
        /// <param name="usageLimits"></param>
        /// <returns></returns>
        private List<ToolResourceDTO> ParseResourceLimits(string? usageLimits)
        {
            var resources = new List<ToolResourceDTO>();

            if (string.IsNullOrEmpty(usageLimits))
                return resources;

            try
            {
                // Parse JSON format: {"FEATURE_CODE": {"daily": 100, "monthly": 3000}}
                var limitsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(usageLimits);
                
                if (limitsDict != null)
                {
                    foreach (var item in limitsDict)
                    {
                        var featureCode = item.Key;
                        var limits = item.Value;
                        
                        var dailyLimit = limits.GetValueOrDefault("daily", 0);
                        var monthlyLimit = limits.GetValueOrDefault("monthly", 0);
                        var totalLimit = limits.GetValueOrDefault("total", 0);
                        
                        // Calculate combined limit
                        var combinedLimit = totalLimit > 0 ? totalLimit : (dailyLimit + monthlyLimit);
                        
                        var resource = new ToolResourceDTO
                        {
                            FeatureId = GetFeatureIdByCode(featureCode),
                            FeatureCode = featureCode,
                            FeatureName = GetFeatureNameByCode(featureCode),
                            ToolName = GetToolNameByFeatureCode(featureCode),
                            Description = GetFeatureDescriptionByCode(featureCode),
                            IsEnabled = true,
                            UsageLimit = combinedLimit,
                            UsedCount = 0, // TODO: Lấy từ database thực tế
                            RemainingCount = combinedLimit,
                            PeriodStart = DateTime.UtcNow.Date,
                            PeriodEnd = DateTime.UtcNow.Date.AddDays(30), // Monthly period
                            LimitType = totalLimit > 0 ? "total" : (monthlyLimit > 0 ? "monthly" : "daily"),
                            Status = "available",
                            WarningMessage = ""
                        };
                        
                        // Set warning message for limited resources
                        if (resource.RemainingCount <= 5)
                        {
                            resource.Status = "limited";
                            resource.WarningMessage = $"Còn lại {resource.RemainingCount} lượt sử dụng";
                        }
                        else if (resource.RemainingCount == 0)
                        {
                            resource.Status = "exhausted";
                            resource.WarningMessage = "Đã hết lượt sử dụng";
                        }
                        
                        resources.Add(resource);
                    }
                }

                return resources;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing resource limits: {ex.Message}");
                return resources;
            }
        }

        /// <summary>
        /// Lấy tên feature từ code
        /// </summary>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        private string GetFeatureNameByCode(string featureCode)
        {
            return featureCode?.ToUpper() switch
            {
                "SEND_MESSAGE" => "Gửi tin nhắn",
                "ADD_FRIEND" => "Thêm bạn bè",
                "BULK_SEND_MESSAGE" => "Gửi tin nhắn hàng loạt",
                "AI_MESSAGE" => "Tin nhắn AI",
                "SEND_IMAGE" => "Gửi hình ảnh",
                "SEND_VIDEO" => "Gửi video",
                "GROUP_CHAT" => "Chat nhóm",
                "FILE_SHARING" => "Chia sẻ file",
                "VOICE_CALL" => "Gọi thoại",
                "VIDEO_CALL" => "Gọi video",
                _ => featureCode ?? "Tính năng không xác định"
            };
        }

        /// <summary>
        /// Lấy tên tool từ feature code
        /// </summary>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        private string GetToolNameByFeatureCode(string featureCode)
        {
            return featureCode?.ToUpper() switch
            {
                "SEND_MESSAGE" or "BULK_SEND_MESSAGE" => "Messaging Tools",
                "ADD_FRIEND" => "Social Tools",
                "AI_MESSAGE" => "AI Tools",
                "SEND_IMAGE" or "SEND_VIDEO" => "Media Tools",
                "GROUP_CHAT" => "Group Management",
                "FILE_SHARING" => "File Management",
                "VOICE_CALL" or "VIDEO_CALL" => "Communication Tools",
                _ => "General Tools"
            };
        }

        /// <summary>
        /// Lấy mô tả feature từ code
        /// </summary>
        /// <param name="featureCode"></param>
        /// <returns></returns>
        private string GetFeatureDescriptionByCode(string featureCode)
        {
            return featureCode?.ToUpper() switch
            {
                "SEND_MESSAGE" => "Gửi tin nhắn cá nhân",
                "ADD_FRIEND" => "Thêm bạn bè mới",
                "BULK_SEND_MESSAGE" => "Gửi tin nhắn đến nhiều người cùng lúc",
                "AI_MESSAGE" => "Sử dụng AI để tạo và gửi tin nhắn",
                "SEND_IMAGE" => "Gửi hình ảnh, ảnh GIF",
                "SEND_VIDEO" => "Gửi video, clip ngắn",
                "GROUP_CHAT" => "Tham gia và tạo nhóm chat",
                "FILE_SHARING" => "Chia sẻ file, tài liệu",
                "VOICE_CALL" => "Thực hiện cuộc gọi thoại",
                "VIDEO_CALL" => "Thực hiện cuộc gọi video",
                _ => "Tính năng của ứng dụng"
            };
        }
    }
}
