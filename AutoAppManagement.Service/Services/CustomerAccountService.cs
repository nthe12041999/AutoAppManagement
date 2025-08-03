using System.Security.Cryptography;
using System.Text;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AutoAppManagement.Service.Services
{
    public interface ICustomerAccountService
    {
        /// <summary>
        /// Đăng nhập với thông tin device
        /// </summary>
        /// <param name="userName">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="deviceInfo">Thông tin thiết bị</param>
        /// <returns>Kết quả đăng nhập</returns>
        Task<ResponseOutput<CustomerLoginResponse>> LoginWithDevice(
            string userName,
            string password,
            DeviceInfo deviceInfo
        );

        /// <summary>
        /// Đăng ký thiết bị mới cho tài khoản
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <param name="deviceInfo">Thông tin thiết bị</param>
        /// <returns>Kết quả đăng ký</returns>
        Task<ResponseOutput<string>> RegisterDevice(long accountId, DeviceInfo deviceInfo);

        /// <summary>
        /// Lấy danh sách thiết bị của tài khoản
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <returns>Danh sách thiết bị</returns>
        Task<ResponseOutput<List<CustomerDevice>>> GetAccountDevices(long accountId);

        /// <summary>
        /// Xóa thiết bị
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="accountId">Account ID</param>
        /// <returns>Kết quả xóa</returns>
        Task<ResponseOutput<string>> RemoveDevice(string deviceId, long accountId);

        /// <summary>
        /// Kiểm tra license của tài khoản
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <returns>Thông tin license</returns>
        Task<ResponseOutput<CustomerLicenseInfo>> CheckAccountLicense(long accountId);

        /// <summary>
        /// Tạo license mới cho tài khoản
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <param name="licenseRequest">Thông tin license</param>
        /// <param name="createdBy">Người tạo</param>
        /// <returns>Kết quả tạo license</returns>
        Task<ResponseOutput<string>> CreateLicense(
            long accountId,
            CreateLicenseRequest licenseRequest,
            long createdBy
        );

        /// <summary>
        /// Gia hạn license
        /// </summary>
        /// <param name="licenseKey">License key</param>
        /// <param name="newExpiryDate">Ngày hết hạn mới</param>
        /// <param name="updatedBy">Người cập nhật</param>
        /// <returns>Kết quả gia hạn</returns>
        Task<ResponseOutput<string>> RenewLicense(
            string licenseKey,
            DateTime newExpiryDate,
            long updatedBy
        );

        /// <summary>
        /// Kiểm tra quyền truy cập với device và license
        /// </summary>
        /// <param name="accountId">ID tài khoản</param>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Kết quả kiểm tra</returns>
        Task<ResponseOutput<AccessValidationResult>> ValidateAccess(
            long accountId,
            string deviceId
        );
    }

    public class CustomerAccountService : BaseService, ICustomerAccountService
    {
        private readonly IConfiguration _configuration;

        public CustomerAccountService(
            IHttpContextAccessor httpContextAccessor,
            IDistributedCacheCustom cache,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationSocketHub notificationSocketHub,
            IConfiguration configuration
        )
            : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
            _configuration = configuration;
        }

        public async Task<ResponseOutput<CustomerLoginResponse>> LoginWithDevice(
            string userName,
            string password,
            DeviceInfo deviceInfo
        )
        {
            try
            {
                // Kiểm tra thông tin đăng nhập
                var account = await UnitOfWork.AccountsRepository.GetUserByUserNameAndPass(
                    userName,
                    password
                );
                if (account == null)
                {
                    return new ResponseOutput<CustomerLoginResponse>
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng",
                    };
                }

                // Kiểm tra tài khoản có bị khóa không
                if (account.IsLocked)
                {
                    return new ResponseOutput<CustomerLoginResponse>
                    {
                        IsSuccess = false,
                        Message = "Tài khoản đã bị khóa",
                    };
                }

                // Kiểm tra license còn hiệu lực không
                var licenseCheck = await CheckAccountLicense(account.Id);
                if (
                    !licenseCheck.IsSuccess
                    || licenseCheck.Data == null
                    || !licenseCheck.Data.IsValid
                )
                {
                    return new ResponseOutput<CustomerLoginResponse>
                    {
                        IsSuccess = false,
                        Message = "License đã hết hạn hoặc không hợp lệ",
                    };
                }

                // Kiểm tra thiết bị
                var deviceId = GenerateDeviceId(deviceInfo);
                var existingDevice =
                    await UnitOfWork.CustomerDeviceRepository.GetDeviceByDeviceIdAndAccountId(
                        deviceId,
                        account.Id
                    );

                if (existingDevice == null)
                {
                    // Kiểm tra số lượng thiết bị tối đa
                    var deviceCount =
                        await UnitOfWork.CustomerDeviceRepository.CountDevicesByAccountId(
                            account.Id
                        );
                    if (deviceCount >= licenseCheck.Data.MaxDevices)
                    {
                        return new ResponseOutput<CustomerLoginResponse>
                        {
                            IsSuccess = false,
                            Message =
                                $"Đã đạt giới hạn số thiết bị tối đa ({licenseCheck.Data.MaxDevices})",
                        };
                    }

                    // Đăng ký thiết bị mới
                    var registerResult = await RegisterDevice(account.Id, deviceInfo);
                    if (!registerResult.IsSuccess)
                    {
                        return new ResponseOutput<CustomerLoginResponse>
                        {
                            IsSuccess = false,
                            Message = registerResult.Message,
                        };
                    }
                }
                else
                {
                    // Cập nhật thời gian đăng nhập cuối
                    await UnitOfWork.CustomerDeviceRepository.UpdateLastLoginDate(
                        deviceId,
                        account.Id
                    );
                    await UnitOfWork.CommitAsync();
                }

                return new ResponseOutput<CustomerLoginResponse>
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công",
                    Data = new CustomerLoginResponse
                    {
                        AccountId = account.Id,
                        UserName = account.UserName,
                        Name = account.Name,
                        Email = account.Email,
                        DeviceId = deviceId,
                        LicenseInfo = licenseCheck.Data,
                    },
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<CustomerLoginResponse>
                {
                    IsSuccess = false,
                    Message = $"Lỗi đăng nhập: {ex.Message}",
                };
            }
        }

        public async Task<ResponseOutput<string>> RegisterDevice(
            long accountId,
            DeviceInfo deviceInfo
        )
        {
            try
            {
                var deviceId = GenerateDeviceId(deviceInfo);

                // Kiểm tra thiết bị đã tồn tại chưa
                var existingDevice = await UnitOfWork.CustomerDeviceRepository.IsDeviceExists(
                    deviceId,
                    accountId
                );
                if (existingDevice)
                {
                    return new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = "Thiết bị đã được đăng ký",
                    };
                }

                var device = new CustomerDevice
                {
                    AccountId = accountId,
                    DeviceId = deviceId,
                    DeviceName = deviceInfo.DeviceName,
                    DeviceType = deviceInfo.DeviceType,
                    OperatingSystem = deviceInfo.OperatingSystem,
                    OSVersion = deviceInfo.OSVersion,
                    BrowserInfo = deviceInfo.BrowserInfo,
                    IpAddress = GetClientIpAddress(),
                    Status = "Active",
                    CreatedDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                };

                await UnitOfWork.CustomerDeviceRepository.CreateAsync(device);
                await UnitOfWork.CommitAsync();

                return new ResponseOutput<string>
                {
                    IsSuccess = true,
                    Message = "Đăng ký thiết bị thành công",
                    Data = deviceId,
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<string>
                {
                    IsSuccess = false,
                    Message = $"Lỗi đăng ký thiết bị: {ex.Message}",
                };
            }
        }

        private string GenerateDeviceId(DeviceInfo deviceInfo)
        {
            var deviceString =
                $"{deviceInfo.DeviceName}_{deviceInfo.OperatingSystem}_{deviceInfo.OSVersion}_{deviceInfo.BrowserInfo}";
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(deviceString));
                return Convert
                    .ToBase64String(hash)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
            }
        }

        private string GetClientIpAddress()
        {
            var ipAddress =
                HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return ipAddress ?? "Unknown";
        }

        public async Task<ResponseOutput<List<CustomerDevice>>> GetAccountDevices(long accountId)
        {
            try
            {
                var devices = await UnitOfWork.CustomerDeviceRepository.GetDevicesByAccountId(
                    accountId
                );
                return new ResponseOutput<List<CustomerDevice>>
                {
                    IsSuccess = true,
                    Data = devices.ToList(),
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<List<CustomerDevice>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi lấy danh sách thiết bị: {ex.Message}",
                };
            }
        }

        public async Task<ResponseOutput<string>> RemoveDevice(string deviceId, long accountId)
        {
            try
            {
                var device =
                    await UnitOfWork.CustomerDeviceRepository.GetDeviceByDeviceIdAndAccountId(
                        deviceId,
                        accountId
                    );
                if (device == null)
                {
                    return new ResponseOutput<string>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thiết bị",
                    };
                }

                await UnitOfWork.CustomerDeviceRepository.DeactivateDevice(deviceId, accountId);
                await UnitOfWork.CommitAsync();

                return new ResponseOutput<string>
                {
                    IsSuccess = true,
                    Message = "Xóa thiết bị thành công",
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<string>
                {
                    IsSuccess = false,
                    Message = $"Lỗi xóa thiết bị: {ex.Message}",
                };
            }
        }

        public async Task<ResponseOutput<CustomerLicenseInfo>> CheckAccountLicense(long accountId)
        {
            try
            {
                var license = await UnitOfWork.CustomerLicenseRepository.GetActiveLicense(
                    accountId
                );
                if (license == null)
                {
                    return new ResponseOutput<CustomerLicenseInfo>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy license hợp lệ",
                    };
                }

                var licenseInfo = new CustomerLicenseInfo
                {
                    LicenseKey = license.LicenseKey,
                    LicenseName = license.LicenseName,
                    LicenseType = license.LicenseType,
                    ExpiryDate = license.ExpiryDate,
                    IsValid = license.ExpiryDate > DateTime.Now && license.Status == "Active",
                    MaxDevices = license.MaxDevices,
                    MaxUsers = license.MaxUsers,
                    Status = license.Status,
                };

                return new ResponseOutput<CustomerLicenseInfo>
                {
                    IsSuccess = true,
                    Data = licenseInfo,
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<CustomerLicenseInfo>
                {
                    IsSuccess = false,
                    Message = $"Lỗi kiểm tra license: {ex.Message}",
                };
            }
        }

        public async Task<ResponseOutput<string>> CreateLicense(
            long accountId,
            CreateLicenseRequest licenseRequest,
            long createdBy
        )
        {
            try
            {
                var licenseKey = GenerateLicenseKey();

                var license = new CustomerLicense
                {
                    AccountId = accountId,
                    LicenseKey = licenseKey,
                    LicenseName = licenseRequest.LicenseName,
                    LicenseType = licenseRequest.LicenseType,
                    Description = licenseRequest.Description,
                    MaxDevices = licenseRequest.MaxDevices,
                    MaxUsers = licenseRequest.MaxUsers,
                    StartDate = licenseRequest.StartDate,
                    ExpiryDate = licenseRequest.ExpiryDate,
                    Status = "Active",
                    Price = licenseRequest.Price,
                    Currency = licenseRequest.Currency ?? "VND",
                    AllowedFeatures = licenseRequest.AllowedFeatures,
                    UsageLimits = licenseRequest.UsageLimits,
                    CreatedDate = DateTime.Now,
                    CreatedBy = createdBy,
                };

                await UnitOfWork.CustomerLicenseRepository.CreateAsync(license);
                await UnitOfWork.CommitAsync();

                return new ResponseOutput<string>
                {
                    IsSuccess = true,
                    Message = "Tạo license thành công",
                    Data = licenseKey,
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<string>
                {
                    IsSuccess = false,
                    Message = $"Lỗi tạo license: {ex.Message}",
                };
            }
        }

        public async Task<ResponseOutput<string>> RenewLicense(
            string licenseKey,
            DateTime newExpiryDate,
            long updatedBy
        )
        {
            try
            {
                await UnitOfWork.CustomerLicenseRepository.RenewLicense(
                    licenseKey,
                    newExpiryDate,
                    updatedBy
                );
                await UnitOfWork.CommitAsync();

                return new ResponseOutput<string>
                {
                    IsSuccess = true,
                    Message = "Gia hạn license thành công",
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<string>
                {
                    IsSuccess = false,
                    Message = $"Lỗi gia hạn license: {ex.Message}",
                };
            }
        }

        public async Task<ResponseOutput<AccessValidationResult>> ValidateAccess(
            long accountId,
            string deviceId
        )
        {
            try
            {
                // Kiểm tra license
                var licenseCheck = await CheckAccountLicense(accountId);
                if (!licenseCheck.IsSuccess)
                {
                    return new ResponseOutput<AccessValidationResult>
                    {
                        IsSuccess = false,
                        Message = licenseCheck.Message,
                    };
                }

                // Kiểm tra device
                var device =
                    await UnitOfWork.CustomerDeviceRepository.GetDeviceByDeviceIdAndAccountId(
                        deviceId,
                        accountId
                    );
                if (device == null || device.Status != "Active")
                {
                    return new ResponseOutput<AccessValidationResult>
                    {
                        IsSuccess = false,
                        Message = "Thiết bị không hợp lệ hoặc đã bị vô hiệu hóa",
                    };
                }

                var result = new AccessValidationResult
                {
                    IsValid = licenseCheck.Data.IsValid && device.Status == "Active",
                    Message =
                        licenseCheck.Data.IsValid && device.Status == "Active"
                            ? "Truy cập hợp lệ"
                            : "Truy cập không hợp lệ",
                    LicenseInfo = licenseCheck.Data,
                    DeviceInfo = device,
                };

                return new ResponseOutput<AccessValidationResult>
                {
                    IsSuccess = true,
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                return new ResponseOutput<AccessValidationResult>
                {
                    IsSuccess = false,
                    Message = $"Lỗi kiểm tra quyền truy cập: {ex.Message}",
                };
            }
        }

        private string GenerateLicenseKey()
        {
            var guid = Guid.NewGuid().ToString("N").ToUpper();
            return $"LIC-{guid.Substring(0, 8)}-{guid.Substring(8, 8)}-{guid.Substring(16, 8)}-{guid.Substring(24, 8)}";
        }
    }

    // DTO Classes
    public class DeviceInfo
    {
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string BrowserInfo { get; set; }
    }

    public class CustomerLoginResponse
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DeviceId { get; set; }
        public CustomerLicenseInfo LicenseInfo { get; set; }
    }

    public class CustomerLicenseInfo
    {
        public string LicenseKey { get; set; }
        public string LicenseName { get; set; }
        public string LicenseType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsValid { get; set; }
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public string Status { get; set; }
    }

    public class CreateLicenseRequest
    {
        public string LicenseName { get; set; }
        public string LicenseType { get; set; }
        public string Description { get; set; }
        public int MaxDevices { get; set; }
        public int MaxUsers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string AllowedFeatures { get; set; }
        public string UsageLimits { get; set; }
    }

    public class AccessValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public CustomerLicenseInfo LicenseInfo { get; set; }
        public CustomerDevice DeviceInfo { get; set; }
    }
}
