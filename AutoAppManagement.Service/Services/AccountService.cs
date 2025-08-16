using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AccountDevice;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace AutoAppManagement.Service.Services
{
    public interface IAccountService
    {
        Task<List<AccountDTO>> GetAllAccounts();
        Task<AccountDTO> GetAccountById(long id);
        Task<AccountDTO> GetAccountByUsername(string username);
        Task<RestOutput> CreateAccount(CreateAccountRequest request);
        Task<RestOutput> UpdateAccount(UpdateAccountRequest request);
        Task<RestOutput> DeleteAccount(long id);
        Task<RestOutput> ChangePassword(long id, string newPassword);
        Task<RestOutput> LockAccount(long id, string reason = "");
        Task<RestOutput> UnlockAccount(long id);
        Task<RestOutput> ActivateAccount(long id);
        Task<RestOutput> DeactivateAccount(long id);
        Task<List<AccountDTO>> GetAccountsByLevel(int level);
        Task<List<AccountDTO>> GetExpiredAccounts();
        Task<List<AccountDTO>> GetExpiringAccounts(int days);
        Task<RestOutput> ExtendAccount(long id, DateTime newExpiryDate);
        Task<bool> ValidateAccount(string username, string password);
        Task<RestOutput> UpdateAccountInfo(UpdateAccountInfoRequest request);
        Task<RestOutput> UploadAvatar(long id, string avatarPath);
        Task<RestOutput> Login(LoginRequest request);

        // AccountDevice methods
        Task<List<AccountDeviceDTO>> GetAllAccountDevices();
        Task<List<AccountDeviceDTO>> GetAccountDevicesByAccountId(long accountId);
        Task<AccountDeviceDTO> GetAccountDeviceById(long id);
        Task<RestOutput> RegisterDevice(RegisterDeviceRequest request);
        Task<RestOutput> UpdateDevice(UpdateDeviceRequest request);
        Task<RestOutput> DeleteDevice(long id);
        Task<RestOutput> ActivateDevice(long id);
        Task<RestOutput> DeactivateDevice(long id);
        Task<List<AccountDeviceDTO>> GetActiveDevices(long accountId);
        Task<List<AccountDeviceDTO>> GetDevicesByType(string deviceType);
        Task<bool> IsDeviceRegistered(string deviceId, long accountId);
    }

    public class AccountService : BaseService, IAccountService
    {
        private readonly IJwtService _jwtService;

        public AccountService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache,
            IUnitOfWork unitOfWork, IMapper mapper, INotificationSocketHub notificationSocketHub,
            IJwtService jwtService)
            : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
            _jwtService = jwtService;
        }

        /// <summary>
        /// Lấy tất cả accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountDTO>> GetAllAccounts()
        {
            var accounts = await UnitOfWork.AccountsRepository.GetAll();
            return Mapper.Map<List<AccountDTO>>(accounts.Where(a => !a.IsDeleted).ToList());
        }

        /// <summary>
        /// Lấy account theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountDTO> GetAccountById(long id)
        {
            var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            return Mapper.Map<AccountDTO>(account);
        }

        /// <summary>
        /// Lấy account theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AccountDTO> GetAccountByUsername(string username)
        {
            var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.UserName == username && !a.IsDeleted);
            return Mapper.Map<AccountDTO>(account);
        }

        /// <summary>
        /// Tạo account mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> CreateAccount(CreateAccountRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUsername = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.UserName == request.UserName);
                if (existingUsername != null)
                {
                    result.ErrorEventHandler("Username đã tồn tại");
                    return result;
                }

                // Kiểm tra email đã tồn tại chưa
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var existingEmail = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Email == request.Email);
                    if (existingEmail != null)
                    {
                        result.ErrorEventHandler("Email đã tồn tại");
                        return result;
                    }
                }

                var account = new Account
                {
                    UserName = request.UserName,
                    Password = HashCodeUlti.EncodePassword(request.Password),
                    Level = request.Level,
                    Phone = request.Phone,
                    Email = request.Email,
                    RegisterDate = DateTime.UtcNow,
                    ExpiredDate = request.ExpiredDate,
                    Language = request.Language ?? "vi",
                    IsLocked = false,
                    Name = request.Name,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    MaxAccountFb = request.MaxAccountFb,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetUserAuthen()?.Id,
                    Status = "Active",
                    IsActive = true
                };

                await UnitOfWork.AccountsRepository.CreateAsync(account);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AccountDTO>(account));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateAccount(UpdateAccountRequest request)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.Id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                // Kiểm tra username đã tồn tại chưa (trừ account hiện tại)
                var existingUsername = await UnitOfWork.AccountsRepository.FirstOrDefault(a => 
                    a.UserName == request.UserName && a.Id != request.Id);
                if (existingUsername != null)
                {
                    result.ErrorEventHandler("Username đã tồn tại");
                    return result;
                }

                // Kiểm tra email đã tồn tại chưa (trừ account hiện tại)
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var existingEmail = await UnitOfWork.AccountsRepository.FirstOrDefault(a => 
                        a.Email == request.Email && a.Id != request.Id);
                    if (existingEmail != null)
                    {
                        result.ErrorEventHandler("Email đã tồn tại");
                        return result;
                    }
                }

                account.UserName = request.UserName;
                account.Level = request.Level;
                account.Phone = request.Phone;
                account.Email = request.Email;
                account.ExpiredDate = request.ExpiredDate;
                account.Language = request.Language ?? account.Language;
                account.Name = request.Name;
                account.Gender = request.Gender;
                account.DateOfBirth = request.DateOfBirth;
                account.MaxAccountFb = request.MaxAccountFb;
                account.UpdatedDate = DateTime.UtcNow;
                account.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AccountDTO>(account));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Xóa account (soft delete)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeleteAccount(long id)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.IsDeleted = true;
                account.DeletedDate = DateTime.UtcNow;
                account.DeletedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<RestOutput> ChangePassword(long id, string newPassword)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.Password = HashCodeUlti.EncodePassword(newPassword);
                account.UpdatedDate = DateTime.UtcNow;
                account.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<RestOutput> LockAccount(long id, string reason = "")
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.IsLocked = true;
                account.Notes = reason;
                account.UpdatedDate = DateTime.UtcNow;
                account.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> UnlockAccount(long id)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.IsLocked = false;
                account.UpdatedDate = DateTime.UtcNow;
                account.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Kích hoạt tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> ActivateAccount(long id)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.IsActive = true;
                account.Status = "Active";
                account.UpdatedDate = DateTime.UtcNow;
                account.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Vô hiệu hóa tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeactivateAccount(long id)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.IsActive = false;
                account.Status = "Inactive";
                account.UpdatedDate = DateTime.UtcNow;
                account.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Lấy accounts theo level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<List<AccountDTO>> GetAccountsByLevel(int level)
        {
            var accounts = await UnitOfWork.AccountsRepository.GetByCondition(a => 
                a.Level == level && !a.IsDeleted);
            return Mapper.Map<List<AccountDTO>>(accounts.ToList());
        }

        /// <summary>
        /// Lấy accounts đã hết hạn
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountDTO>> GetExpiredAccounts()
        {
            var accounts = await UnitOfWork.AccountsRepository.GetByCondition(a => 
                a.ExpiredDate < DateTime.UtcNow && !a.IsDeleted);
            return Mapper.Map<List<AccountDTO>>(accounts.ToList());
        }

        /// <summary>
        /// Lấy accounts sắp hết hạn
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<List<AccountDTO>> GetExpiringAccounts(int days)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var accounts = await UnitOfWork.AccountsRepository.GetByCondition(a => 
                a.ExpiredDate <= expiryDate && a.ExpiredDate > DateTime.UtcNow && !a.IsDeleted);
            return Mapper.Map<List<AccountDTO>>(accounts.ToList());
        }

        /// <summary>
        /// Gia hạn account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newExpiryDate"></param>
        /// <returns></returns>
        public async Task<RestOutput> ExtendAccount(long id, DateTime newExpiryDate)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                if (newExpiryDate <= account.ExpiredDate)
                {
                    result.ErrorEventHandler("Ngày hết hạn mới phải sau ngày hết hạn hiện tại");
                    return result;
                }

                account.ExpiredDate = newExpiryDate;
                account.UpdatedDate = DateTime.UtcNow;
                account.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AccountDTO>(account));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra tài khoản hợp lệ
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAccount(string username, string password)
        {
            var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.UserName == username && !a.IsDeleted);
            if (account == null || account.IsLocked || !account.IsActive)
                return false;

            var passwordHash = HashCodeUlti.EncodePassword(password);
            return account.Password == passwordHash && account.ExpiredDate > DateTime.UtcNow;
        }

        /// <summary>
        /// Đăng nhập bằng email/sdt và password, kiểm tra license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> Login(LoginRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Tìm account theo email hoặc phone
                Account? account = null;

                if (IsValidEmail(request.EmailOrPhone))
                {
                    // Đăng nhập bằng email
                    account = await UnitOfWork.AccountsRepository.FirstOrDefault(a =>
                        a.Email == request.EmailOrPhone && !a.IsDeleted);
                }
                else
                {
                    // Đăng nhập bằng số điện thoại
                    account = await UnitOfWork.AccountsRepository.FirstOrDefault(a =>
                        a.Phone == request.EmailOrPhone && !a.IsDeleted);
                }

                if (account == null)
                {
                    result.ErrorEventHandler("Tài khoản không tồn tại");
                    return result;
                }

                // Kiểm tra mật khẩu
                var passwordHash = HashCodeUlti.EncodePassword(request.Password);
                if (account.Password != passwordHash)
                {
                    result.ErrorEventHandler("Mật khẩu không chính xác");
                    return result;
                }

                // Kiểm tra trạng thái account
                if (account.IsLocked)
                {
                    result.ErrorEventHandler("Tài khoản đã bị khóa");
                    return result;
                }

                if (!account.IsActive)
                {
                    result.ErrorEventHandler("Tài khoản chưa được kích hoạt");
                    return result;
                }

                if (account.ExpiredDate <= DateTime.UtcNow)
                {
                    result.ErrorEventHandler("Tài khoản đã hết hạn");
                    return result;
                }

                // Kiểm tra license
                var licenseCheck = await CheckAccountLicense(account.Id);
                if (!licenseCheck.IsSuccess)
                {
                    result.ErrorEventHandler(licenseCheck.Message);
                    return result;
                }

                // Tạo JWT token
                var token = _jwtService.GenerateToken(account, licenseCheck.LicenseInfo);
                var tokenExpiry = DateTime.UtcNow.AddMinutes(1440); // 24 hours

                // Tạo response với thông tin đăng nhập
                var loginResponse = new LoginResponse
                {
                    Account = Mapper.Map<AccountDTO>(account),
                    LicenseInfo = licenseCheck.LicenseInfo,
                    LoginTime = DateTime.UtcNow,
                    Message = "Đăng nhập thành công",
                    Token = token,
                    TokenExpiry = tokenExpiry
                };

                // Cập nhật last access date
                account.UpdatedDate = DateTime.UtcNow;
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(loginResponse);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler($"Lỗi đăng nhập: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra license của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private async Task<LicenseCheckResult> CheckAccountLicense(long accountId)
        {
            try
            {
                // Lấy license active của account
                var activeLicenses = await UnitOfWork.LicenseRepository.GetByCondition(l =>
                    l.AccountId == accountId &&
                    l.Status == "Active" &&
                    l.StartDate <= DateTime.UtcNow &&
                    l.EndDate > DateTime.UtcNow &&
                    !l.IsDeleted);

                if (!activeLicenses.Any())
                {
                    return new LicenseCheckResult
                    {
                        IsSuccess = false,
                        Message = "Không có license hợp lệ"
                    };
                }

                var license = activeLicenses.OrderByDescending(l => l.EndDate).First();

                // Kiểm tra license sắp hết hạn (7 ngày)
                var daysToExpire = (license.EndDate - DateTime.UtcNow).Days;
                var warningMessage = "";
                if (daysToExpire <= 7)
                {
                    warningMessage = $"License sẽ hết hạn trong {daysToExpire} ngày";
                }

                return new LicenseCheckResult
                {
                    IsSuccess = true,
                    Message = "License hợp lệ",
                    LicenseInfo = new LicenseInfoDTO
                    {
                        LicenseId = license.Id,
                        LicenseKey = license.LicenseKey,
                        LicenseName = license.LicenseName,
                        LicenseType = license.LicenseType,
                        StartDate = license.StartDate,
                        EndDate = license.EndDate,
                        Status = license.Status,
                        DaysRemaining = daysToExpire,
                        WarningMessage = warningMessage
                    }
                };
            }
            catch (Exception ex)
            {
                return new LicenseCheckResult
                {
                    IsSuccess = false,
                    Message = $"Lỗi kiểm tra license: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateAccountInfo(UpdateAccountInfoRequest request)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.Id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.Name = request.Name;
                account.Phone = request.Phone;
                account.Email = request.Email;
                account.Gender = request.Gender;
                account.DateOfBirth = request.DateOfBirth;
                account.Language = request.Language ?? account.Language;
                account.UpdatedDate = DateTime.UtcNow;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AccountDTO>(account));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Upload avatar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatarPath"></param>
        /// <returns></returns>
        public async Task<RestOutput> UploadAvatar(long id, string avatarPath)
        {
            var result = new RestOutput();

            try
            {
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                account.ImgAvatar = avatarPath;
                account.UpdatedDate = DateTime.UtcNow;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        #region AccountDevice Methods

        /// <summary>
        /// Lấy tất cả account devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetAllAccountDevices()
        {
            var devices = await UnitOfWork.AccountDeviceRepository.GetAll();
            return Mapper.Map<List<AccountDeviceDTO>>(devices.Where(d => !d.IsDeleted).ToList());
        }

        /// <summary>
        /// Lấy devices theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetAccountDevicesByAccountId(long accountId)
        {
            var devices = await UnitOfWork.AccountDeviceRepository.GetByCondition(d =>
                d.AccountId == accountId && !d.IsDeleted);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        /// <summary>
        /// Lấy device theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountDeviceDTO> GetAccountDeviceById(long id)
        {
            var device = await UnitOfWork.AccountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            return Mapper.Map<AccountDeviceDTO>(device);
        }

        /// <summary>
        /// Đăng ký device mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> RegisterDevice(RegisterDeviceRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra account tồn tại
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.AccountId && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                // Kiểm tra device đã đăng ký chưa
                var existingDevice = await UnitOfWork.AccountDeviceRepository.FirstOrDefault(d =>
                    d.DeviceId == request.DeviceId && d.AccountId == request.AccountId && !d.IsDeleted);
                if (existingDevice != null)
                {
                    result.ErrorEventHandler("Device đã được đăng ký cho account này");
                    return result;
                }

                var device = new AccountDevice
                {
                    AccountId = request.AccountId,
                    DeviceId = request.DeviceId,
                    DeviceName = request.DeviceName,
                    DeviceType = request.DeviceType,
                    OperatingSystem = request.OperatingSystem,
                    OSVersion = request.OSVersion,
                    BrowserInfo = request.BrowserInfo,
                    IpAddress = request.IpAddress,
                    IsActive = true,
                    RegisteredDate = DateTime.UtcNow,
                    LastAccessDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetUserAuthen()?.Id,
                    Notes = request.Notes ?? ""
                };

                await UnitOfWork.AccountDeviceRepository.CreateAsync(device);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AccountDeviceDTO>(device));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật device
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateDevice(UpdateDeviceRequest request)
        {
            var result = new RestOutput();

            try
            {
                var device = await UnitOfWork.AccountDeviceRepository.FirstOrDefault(d => d.Id == request.Id && !d.IsDeleted);
                if (device == null)
                {
                    result.ErrorEventHandler("Device không tồn tại");
                    return result;
                }

                device.DeviceName = request.DeviceName;
                device.DeviceType = request.DeviceType;
                device.OperatingSystem = request.OperatingSystem;
                device.OSVersion = request.OSVersion;
                device.BrowserInfo = request.BrowserInfo;
                device.Notes = request.Notes ?? device.Notes;
                device.UpdatedDate = DateTime.UtcNow;
                device.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AccountDeviceDTO>(device));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Xóa device (soft delete)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeleteDevice(long id)
        {
            var result = new RestOutput();

            try
            {
                var device = await UnitOfWork.AccountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
                if (device == null)
                {
                    result.ErrorEventHandler("Device không tồn tại");
                    return result;
                }

                device.IsDeleted = true;
                device.DeletedDate = DateTime.UtcNow;
                device.DeletedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Kích hoạt device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> ActivateDevice(long id)
        {
            var result = new RestOutput();

            try
            {
                var device = await UnitOfWork.AccountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
                if (device == null)
                {
                    result.ErrorEventHandler("Device không tồn tại");
                    return result;
                }

                device.IsActive = true;
                device.UpdatedDate = DateTime.UtcNow;
                device.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Vô hiệu hóa device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeactivateDevice(long id)
        {
            var result = new RestOutput();

            try
            {
                var device = await UnitOfWork.AccountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
                if (device == null)
                {
                    result.ErrorEventHandler("Device không tồn tại");
                    return result;
                }

                device.IsActive = false;
                device.UpdatedDate = DateTime.UtcNow;
                device.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Lấy devices đang hoạt động của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetActiveDevices(long accountId)
        {
            var devices = await UnitOfWork.AccountDeviceRepository.GetByCondition(d =>
                d.AccountId == accountId && d.IsActive && !d.IsDeleted);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        /// <summary>
        /// Lấy devices theo loại
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public async Task<List<AccountDeviceDTO>> GetDevicesByType(string deviceType)
        {
            var devices = await UnitOfWork.AccountDeviceRepository.GetByCondition(d =>
                d.DeviceType == deviceType && !d.IsDeleted);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        /// <summary>
        /// Kiểm tra device đã đăng ký chưa
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<bool> IsDeviceRegistered(string deviceId, long accountId)
        {
            var device = await UnitOfWork.AccountDeviceRepository.FirstOrDefault(d =>
                d.DeviceId == deviceId && d.AccountId == accountId && !d.IsDeleted);
            return device != null;
        }

        #endregion
    }
}
