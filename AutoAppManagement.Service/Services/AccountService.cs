using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AccountDevice;
using AutoAppManagement.Models.DTO.Verification;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static AutoAppManagement.Models.Enum.DataModelType;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AutoAppManagement.Service.Services
{
    public interface IAccountService : IBaseBusinessService<AccountDTO>
    {
        Task<BaseResponse> ChangePassword(long id, string newPassword);
        Task<BaseResponse> SendOtpForChangePassword();
        Task<BaseResponse> ResendOtpForChangePassword();
        Task<BaseResponse> ChangePasswordWithOtp(ChangePasswordWithOtpRequest request);
        Task<BaseResponse> ForgotPassword(string email);
        Task<BaseResponse> ConfirmOtpResetPassword(string email, string otp);
        Task<BaseResponse> ResendOtp(string email);
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
        Task<BaseResponse> RefreshTokenAsync(string refreshToken, string ip = null, string userAgent = null);
        Task<BaseResponse> RevokeAllTokensForAccount(long accountId, string revokedByIp = null);
        Task<BaseResponse> RevokeToken();
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
        Task<BaseResponse> GetCustomerAccountStatisticsAsync();
    }

    public class AccountService : BaseBusinessService<Account, AccountDTO, IAccountsRepository>, IAccountService
    {
        // Lưu mật khẩu gốc trước khi hash để gửi email
        private string _originalPasswordForEmail = null;
        
        // Lazy load repositories thay vì direct injection
        private IGenericRepository<License> _licenseRepository;
        protected IGenericRepository<License> LicenseRepository
            => _licenseRepository ??= UnitOfWork.GetRepository<License>();

        private IGenericRepository<AccountDevice> _accountDeviceRepository;
        protected IGenericRepository<AccountDevice> AccountDeviceRepository
            => _accountDeviceRepository ??= UnitOfWork.GetRepository<AccountDevice>();

        private IJwtService _jwtService;
        protected IJwtService JwtService
            => _jwtService ??= _serviceProvider.GetRequiredService<IJwtService>();

        public AccountService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Map View enum sang tên view trong database
        /// </summary>
        protected override string GetViewName(Models.Enums.EnumView view)
        {
            return view switch
            {
                Models.Enums.EnumView.ViewAccountCustomer => "ViewAccountCustomer",
                _ => base.GetViewName(view)
            };
        }

        /// <summary>
        /// Chỉ định các field được phép search cho ViewAccountCustomer
        /// </summary>
        protected override List<string>? GetSearchFieldsForView(Models.Enums.EnumView view)
        {
            if (view == Models.Enums.EnumView.ViewAccountCustomer)
            {
                return new List<string> { "Email", "Phone", "UserName", "FirstName", "LastName" };
            }
            return base.GetSearchFieldsForView(view);
        }

        /// <summary>
        /// Class để map kết quả từ ViewAccountCustomer view
        /// </summary>
        private class ViewAccountCustomerResult
        {
            public long ID { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public DateTime? RegisterDate { get; set; }
            public DateTime? ExpiredDate { get; set; }
            public long LicenseId { get; set; }
            public int? Status { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public long? CreatedBy { get; set; }
            public long? UpdatedBy { get; set; }
        }

        private static DateTime GetRefreshTokenNoExpiryUtc()
        {
            // Dùng cận trên hợp lệ của SQL Server để coi như vô hạn
            return new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
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

        public async Task<BaseResponse> SendOtpForChangePassword()
        {
            try
            {
                // Lấy accountId từ token
                var accountId = GetCurrentUserId();
                if (accountId <= 0)
                {
                    return BaseResponse.Error("Không thể xác thực tài khoản");
                }

                // Tìm account
                var account = await Repository.FirstOrDefault(a => a.ID == accountId && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Tài khoản không tồn tại");
                }

                // Gửi OTP qua email
                var verificationService = _serviceProvider.GetRequiredService<IVerificationService>();
                var sendOtpResult = await verificationService.SendOtpAsync(new Models.DTO.Verification.SendOtpRequest
                {
                    Email = account.Email,
                    Type = Models.BaseEntity.VerificationType.ChangePassword
                });

                if (!sendOtpResult.IsSuccess)
                {
                    return BaseResponse.Error("Không thể gửi mã OTP. Vui lòng thử lại sau.");
                }

                return BaseResponse.Success("Mã OTP đã được gửi đến email của bạn");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gửi OTP: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ResendOtpForChangePassword()
        {
            try
            {
                // Lấy accountId từ token
                var accountId = GetCurrentUserId();
                if (accountId <= 0)
                {
                    return BaseResponse.Error("Không thể xác thực tài khoản");
                }

                // Tìm account
                var account = await Repository.FirstOrDefault(a => a.ID == accountId && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Tài khoản không tồn tại");
                }

                // Gửi lại OTP (service sẽ check xem có OTP cũ chưa hết hạn không)
                var verificationService = _serviceProvider.GetRequiredService<IVerificationService>();
                var resendResult = await verificationService.ResendOtpAsync(account.Email, Models.BaseEntity.VerificationType.ChangePassword);

                return resendResult;
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gửi lại mã OTP: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ChangePasswordWithOtp(ChangePasswordWithOtpRequest request)
        {
            try
            {
                var accountId = GetCurrentUserId();
                if (accountId <= 0)
                {
                    return BaseResponse.Error("Không thể xác thực tài khoản");
                }
                // Tìm account
                var account = await Repository.FirstOrDefault(a => a.ID == accountId && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Tài khoản không tồn tại");
                }

                // Kiểm tra mật khẩu cũ
                var hashedOldPassword = HashCodeUlti.EncodePassword(request.OldPassword);
                if (account.Password != hashedOldPassword)
                {
                    return BaseResponse.Error("Mật khẩu cũ không chính xác");
                }

                // Verify OTP trực tiếp
                var verificationService = _serviceProvider.GetRequiredService<IVerificationService>();
                var verifyResult = await verificationService.VerifyOtpAsync(new Models.DTO.Verification.VerifyOtpRequest
                {
                    Email = account.Email,
                    Code = request.Otp,
                    Type = Models.BaseEntity.VerificationType.ChangePassword
                });

                if (!verifyResult.IsSuccess)
                {
                    return verifyResult; // Trả về lỗi từ verify (OTP sai, hết hạn, etc)
                }

                // Đổi mật khẩu
                account.Password = HashCodeUlti.EncodePassword(request.NewPassword);
                account.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đổi mật khẩu: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ForgotPassword(string email)
        {
            try
            {
                // Kiểm tra email có tồn tại không
                var account = await Repository.FirstOrDefault(a => a.Email == email && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Email không tồn tại trong hệ thống");
                }

                // Gọi OTP qua email
                var verificationService = _serviceProvider.GetRequiredService<IVerificationService>();
                var sendOtpResult = await verificationService.SendOtpAsync(new Models.DTO.Verification.SendOtpRequest
                {
                    Email = email,
                    Type = Models.BaseEntity.VerificationType.ForgotPassword
                });

                if (!sendOtpResult.IsSuccess)
                {
                    return BaseResponse.Error("Không thể gửi mã OTP. Vui lòng thử lại sau.");
                }

                return BaseResponse.Success("Mã OTP đã được gửi đến email của bạn");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xử lý quên mật khẩu: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ConfirmOtpResetPassword(string email, string otp)
        {
            try
            {
                // 1. Verify OTP
                var verificationService = _serviceProvider.GetRequiredService<IVerificationService>();
                var verifyResult = await verificationService.VerifyOtpAsync(new Models.DTO.Verification.VerifyOtpRequest
                {
                    Email = email,
                    Code = otp,
                    Type = Models.BaseEntity.VerificationType.ForgotPassword
                });

                if (!verifyResult.IsSuccess)
                {
                    return verifyResult; // Trả về lỗi từ verify (OTP sai, hết hạn, etc)
                }

                // 2. Tìm account
                var account = await Repository.FirstOrDefault(a => a.Email == email && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Tài khoản không tồn tại");
                }

                // 3. Tạo mật khẩu ngẫu nhiên
                var newPassword = GenerateRandomPassword(8);
                account.Password = HashCodeUlti.EncodePassword(newPassword);
                account.SetUpdated();
                await UnitOfWork.SaveAsync();

                // 4. Gửi email mật khẩu mới
                var emailService = _serviceProvider.GetRequiredService<IEmailService>();
                await emailService.SendPasswordResetEmailAsync(email, newPassword, account.FirstName + account.LastName);

                return BaseResponse.Success("Đặt lại mật khẩu thành công. Mật khẩu mới đã được gửi đến email của bạn.");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đặt lại mật khẩu: {ex.Message}");
            }
        }

        public async Task<BaseResponse> ResendOtp(string email)
        {
            try
            {
                // Kiểm tra email có tồn tại không
                var account = await Repository.FirstOrDefault(a => a.Email == email && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Email không tồn tại trong hệ thống");
                }

                // Gửi lại OTP (service sẽ check xem có OTP cũ chưa hết hạn không)
                var verificationService = _serviceProvider.GetRequiredService<IVerificationService>();
                var resendResult = await verificationService.ResendOtpAsync(email, Models.BaseEntity.VerificationType.ForgotPassword);

                return resendResult;
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gửi lại mã OTP: {ex.Message}");
            }
        }

        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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

        public override async Task CustomBeforeSubmitData(AccountDTO dto)
        {
            switch (dto.State)
            {
                case AutoAppManagement.Models.Common.EntityState.Add:
                    // Đảm bảo các field có unique constraint không NULL để tránh lỗi duplicate NULL
                    if (string.IsNullOrWhiteSpace(dto.Email))
                    {
                        throw new ArgumentException("Email không được để trống");
                    }
                    if (string.IsNullOrWhiteSpace(dto.Phone))
                    {
                        throw new ArgumentException("Số điện thoại không được để trống");
                    }
                    
                    // Generate random password nếu không có password
                    if (string.IsNullOrEmpty(dto.Password))
                    {
                        dto.Password = GenerateRandomPassword(12); // 12 ký tự
                    }
                    
                    // Lưu password gốc để gửi email sau
                    _originalPasswordForEmail = dto.Password;
                    
                    // Kiểm tra email trùng
                    if (!string.IsNullOrEmpty(dto.Email))
                    {
                        var existingEmail = await Repository.FirstOrDefault(a => 
                            a.Email == dto.Email && a.Status == StatusEnum.Active);
                        if (existingEmail != null)
                        {
                            throw new Exception($"Email '{dto.Email}' đã tồn tại trong hệ thống");
                        }
                    }

                    // Kiểm tra số điện thoại trùng
                    if (!string.IsNullOrEmpty(dto.Phone))
                    {
                        var existingPhone = await Repository.FirstOrDefault(a => 
                            a.Phone == dto.Phone && a.Status == StatusEnum.Active);
                        if (existingPhone != null)
                        {
                            throw new Exception($"Số điện thoại '{dto.Phone}' đã tồn tại trong hệ thống");
                        }
                    }

                    // Hash password
                    dto.Password = HashCodeUlti.EncodePassword(dto.Password);
                    break;

            }
        }

        public override async Task<BaseResponse> SubmitData(AccountDTO dto)
        {
            // Gọi base SubmitData để tạo/cập nhật account
            var result = await base.SubmitData(dto);

            // Nếu tạo mới thành công và có flag SendWelcomeEmail = true
            if (result.IsSuccess && 
                dto.State == AutoAppManagement.Models.Common.EntityState.Add && 
                dto.SendWelcomeEmail && 
                !string.IsNullOrEmpty(dto.Email) &&
                !string.IsNullOrEmpty(_originalPasswordForEmail))
            {
                try
                {
                    var emailService = _serviceProvider.GetRequiredService<IEmailService>();
                    
                    // Gửi email chào mừng (không chặn quá trình tạo account)
                    _ = emailService.SendWelcomeEmailAsync(dto.Email, dto.Name, _originalPasswordForEmail);
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không fail request
                    // Email gửi thất bại không ảnh hưởng đến việc tạo account
                    Console.WriteLine($"Không thể gửi email chào mừng: {ex.Message}");
                }
                finally
                {
                    // Clear password sau khi xử lý xong
                    _originalPasswordForEmail = null;
                }
            }

            return result;
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

                // Generate JWT token với deviceId
                var licenseInfo = licenseCheckResult.Data as LicenseInfoDTO;
                var token = JwtService.GenerateToken(account, licenseInfo, request.DeviceId);

                // Update login info - sử dụng property đúng của Account entity
                // account.LastLoginAt = DateTime.UtcNow; // Property này không tồn tại
                account.SetUpdated(1); // Sử dụng SetUpdated thay thế

                // Issue refresh token and persist
                var refreshTokenStr = JwtService.GenerateRefreshToken();
                string Hash(string s)
                {
                    using var sha = SHA256.Create();
                    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
                    return Convert.ToBase64String(bytes);
                }
                var tokenHash = Hash(refreshTokenStr);
                var familyId = Guid.NewGuid();
                var fingerprintHash = string.IsNullOrEmpty(request.Fingerprint) ? null : Hash(request.Fingerprint);
                var refresh = new RefreshToken
                {
                    AccountId = account.ID,
                    Token = refreshTokenStr,
                    ExpiryDate = GetRefreshTokenNoExpiryUtc(),
                    IsUsed = false,
                    IsRevoked = false,
                    Status = StatusEnum.Active,
                    CreatedDate = DateTime.UtcNow,
                    TokenHash = tokenHash,
                    FamilyId = familyId,
                    FingerprintHash = fingerprintHash,
                    UserAgent = null,
                    DeviceInfo = request.DeviceId
                };
                account.RefreshTokens.Add(refresh);
                await UnitOfWork.SaveAsync();

                #endregion

                #region Xử lý resource

                // Get additional resources and features
                var loginData = new LoginWithResourcesResponse
                {
                    Token = token.AccessToken,
                    LoginTime = DateTime.UtcNow,
                    TokenExpiry = token.AccessTokenExpired,
                    LicenseInfo = licenseInfo,
                    RefreshToken = refresh.Token,
                    RefreshTokenExpired = refresh.ExpiryDate
                };

                // attach refresh token output also to TokenDTO if needed by consumers
                token.RefreshToken = refresh.Token;
                token.RefreshTokenExpired = refresh.ExpiryDate;

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

        public async Task<BaseResponse> RefreshTokenAsync(string refreshToken, string ip = null, string userAgent = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return BaseResponse.Error("RefreshToken is required");
                }

                string Hash(string s)
                {
                    using var sha = System.Security.Cryptography.SHA256.Create();
                    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
                    return Convert.ToBase64String(bytes);
                }
                var refreshRepo = UnitOfWork.GetRepository<RefreshToken>();
                var tokenHash = Hash(refreshToken);
                var tokenEntity = await refreshRepo.FirstOrDefault(x => x.TokenHash == tokenHash || x.Token == refreshToken);
                if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.IsUsed || tokenEntity.ExpiryDate <= DateTime.UtcNow)
                {
                    return BaseResponse.Error("RefreshToken không hợp lệ hoặc đã hết hạn");
                }

                var account = await Repository.FirstOrDefault(a => a.ID == tokenEntity.AccountId && a.Status == StatusEnum.Active);
                if (account == null)
                {
                    return BaseResponse.Error("Tài khoản không hợp lệ");
                }

                var licenseCheckResult = await CheckAccountLicense(account);
                if (!licenseCheckResult.IsSuccess)
                {
                    return licenseCheckResult;
                }
                var licenseInfo = licenseCheckResult.Data as LicenseInfoDTO;

                tokenEntity.RevokedDate = DateTime.Now;
                refreshRepo.Update(tokenEntity);
                await UnitOfWork.SaveAsync();

                var token = JwtService.GenerateToken(account, licenseInfo, tokenEntity.DeviceInfo);

                token.RefreshToken = tokenEntity.Token;
                token.RefreshTokenExpired = tokenEntity.ExpiryDate;

                return BaseResponse.Success(token, "Làm mới token thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi làm mới token: {ex.Message}");
            }
        }

        public async Task<BaseResponse> RevokeAllTokensForAccount(long accountId, string revokedByIp = null)
        {
            try
            {
                var refreshRepo = UnitOfWork.GetRepository<RefreshToken>();
                var tokens = await refreshRepo.GetByCondition(rt =>
                    rt.AccountId == accountId && !rt.IsRevoked && !rt.IsUsed && rt.ExpiryDate > DateTime.UtcNow);

                foreach (var t in tokens)
                {
                    t.IsRevoked = true;
                    t.RevokedDate = DateTime.UtcNow;
                    t.RevokedByIp = revokedByIp;
                    refreshRepo.Update(t);
                }
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success(true, "Đã thu hồi tất cả refresh token của tài khoản");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi thu hồi refresh token: {ex.Message}");
            }
        }

        public async Task<BaseResponse> RevokeToken()
        {
            try
            {
                // Lấy accountId từ JWT token hiện tại
                var accountId = GetCurrentUserId();
                if (accountId == 0)
                {
                    return BaseResponse.Error("Không tìm thấy thông tin tài khoản từ token");
                }

                // Lấy thông tin IP và UserAgent từ HttpContext
                var httpContext = HttpContextAccessor?.HttpContext;
                var userContext = httpContext.User;
                var deviceId = userContext?.FindFirst("deviceId")?.Value;


                // Sử dụng RefreshTokenService để xóa token
                var refreshRepo = UnitOfWork.GetRepository<RefreshToken>();
                var tokenEntity = await refreshRepo.FirstOrDefault(x => x.DeviceInfo == deviceId && x.AccountId == accountId);
                // Xóa token dựa trên account, IP và UserAgent
                refreshRepo.Delete(tokenEntity);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(true, "Đã xóa token của thiết bị hiện tại");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xóa token device hiện tại: {ex.Message}");
            }
        }

        private async Task<BaseResponse> CheckAccountLicense(Account account)
        {
            try
            {
                if (account.LicenseId == 0)
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
        private List<ToolResourceDTO> ParseResourceLimits(string usageLimits)
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

        /// <summary>
        /// Override GetPaging để xử lý filter theo LicenseName (không có trong Account entity)
        /// Chỉ override khi có filter LicenseName, còn lại dùng base implementation
        /// </summary>
        public override async Task<object> GetPaging(PagingRequestDTO pagingRequestDTO)
        {
            // Parse Filter string to FilterCondition array
            pagingRequestDTO.ParseFilters();
            
            // Check if there's a filter on LicenseName field
            var licenseFilter = pagingRequestDTO.Filters?.FirstOrDefault(f => 
                f.field != null && f.field.Equals("LicenseName", StringComparison.OrdinalIgnoreCase));
            
            // If filtering by LicenseName, need to join License table first
            if (licenseFilter != null)
            {
                return await GetPagingWithLicenseFilter(pagingRequestDTO, licenseFilter);
            }
            
            // If no LicenseName filter, use base implementation (simpler and faster)
            return await base.GetPaging(pagingRequestDTO);
        }

        /// <summary>
        /// GetPaging với filter LicenseName - tách logic riêng để dễ maintain
        /// </summary>
        private async Task<object> GetPagingWithLicenseFilter(PagingRequestDTO pagingRequestDTO, FilterCondition licenseFilter)
        {
            var allAccounts = await Repository.GetAll();
            var allLicenses = await LicenseRepository.GetAll();
            
            // Join Account with License
            var query = from account in allAccounts
                       join license in allLicenses on account.LicenseId equals license.ID into licenseGroup
                       from license in licenseGroup.DefaultIfEmpty()
                       where account.Status == Models.Enum.StatusEnum.Active
                       select new { Account = account, License = license };
            
            // Apply LicenseName filter
            if (licenseFilter.op == FilterOperator.Contains)
            {
                query = query.Where(x => x.License != null && 
                    x.License.LicenseName.Contains(licenseFilter.value, StringComparison.OrdinalIgnoreCase));
            }
            else if (licenseFilter.op == FilterOperator.Equals)
            {
                query = query.Where(x => x.License != null && 
                    x.License.LicenseName.Equals(licenseFilter.value, StringComparison.OrdinalIgnoreCase));
            }
            
            // Apply other filters (non-LicenseName filters) on Account properties
            var otherFilters = pagingRequestDTO.Filters?.Where(f => 
                f.field == null || !f.field.Equals("LicenseName", StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (otherFilters != null && otherFilters.Any())
            {
                // Convert to IQueryable<Account> to apply filters
                var accountQuery = query.Select(x => x.Account).AsQueryable();
                accountQuery = ApplyFilterConditions(accountQuery, otherFilters);
                query = from account in accountQuery
                       join license in allLicenses on account.LicenseId equals license.ID into licenseGroup
                       from license in licenseGroup.DefaultIfEmpty()
                       select new { Account = account, License = license };
            }
            
            var totalCount = query.Count();
            var entities = query
                .Skip((pagingRequestDTO.PageIndex - 1) * pagingRequestDTO.PageSize)
                .Take(pagingRequestDTO.PageSize)
                .Select(x => x.Account)
                .ToList();
            
            // Custom data after get paging
            var dtos = await CustomDataAfterGetPaging(pagingRequestDTO, entities);
            if (dtos == null)
            {
                dtos = Mapper.Map<List<AccountDTO>>(entities);
            }
            
            // Filter DTOs by requested columns
            if (pagingRequestDTO.RequestedColumns != null && pagingRequestDTO.RequestedColumns.Any())
            {
                var filteredData = FilterDtosByRequestedColumns(dtos, pagingRequestDTO.RequestedColumns);
                return new PagingResultDTO<object>
                {
                    Data = filteredData,
                    TotalItems = totalCount,
                    PageIndex = pagingRequestDTO.PageIndex,
                    PageSize = pagingRequestDTO.PageSize
                };
            }
            
            return new PagingResultDTO<object>
            {
                Data = dtos,
                TotalItems = totalCount,
                PageIndex = pagingRequestDTO.PageIndex,
                PageSize = pagingRequestDTO.PageSize
            };
        }

        /// <summary>
        /// Override GetPaging để join dựa trên RequestedColumns từ FE
        /// </summary>
        public override async Task<List<AccountDTO>> CustomDataAfterGetPaging(PagingRequestDTO pagingRequestDTO, List<Account> entities)
        {
            try
            {
                var accountDtos = Mapper.Map<List<AccountDTO>>(entities);
                
                // Debug: Log requested columns từ FE
                Console.WriteLine($"FE requested columns: {string.Join(", ", pagingRequestDTO.RequestedColumns)}");
                
                // Kiểm tra FE có cần LicenseName không
                var hasLicenseName = pagingRequestDTO.RequestedColumns?.Any(c => 
                    string.Equals(c, "LicenseName", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c, "licenseName", StringComparison.OrdinalIgnoreCase)) == true;
                
                if (hasLicenseName)
                {
                    Console.WriteLine("Joining License table...");
                    // Chỉ query những license cần thiết thay vì GetAll()
                    var licenseIds = accountDtos.Where(a => a.LicenseId > 0).Select(a => a.LicenseId).Distinct().ToList();
                    var licenses = new Dictionary<long, License>();
                    
                    if (licenseIds.Any())
                    {
                        var licenseList = await LicenseRepository.GetByCondition(l => licenseIds.Contains(l.ID));
                        licenses = licenseList.ToDictionary(l => l.ID, l => l);
                    }

                    foreach (var item in accountDtos)
                    {
                        if (item.LicenseId > 0 && licenses.TryGetValue(item.LicenseId, out var license))
                        {
                            item.LicenseName = license.LicenseName ?? "Chưa có gói cước";
                        }
                        else
                        {
                            item.LicenseName = "Chưa có gói cước";
                        }
                    }
                }

                // Kiểm tra FE có cần StatusName không
                var hasStatusName = pagingRequestDTO.RequestedColumns?.Any(c => 
                    string.Equals(c, "StatusName", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c, "statusName", StringComparison.OrdinalIgnoreCase)) == true;
                
                if (hasStatusName)
                {
                    Console.WriteLine("Converting Status enum to text...");
                    foreach (var item in accountDtos)
                    {
                        item.StatusName = item.Status switch
                        {
                            Models.Enum.StatusEnum.Active => "Hoạt động",
                            Models.Enum.StatusEnum.Inactive => "Không hoạt động", 
                            Models.Enum.StatusEnum.Locked => "Đã khóa",
                            _ => "Không xác định"
                        };
                    }
                }

                // Có thể thêm các field khác tương tự nếu cần
                // if (pagingRequestDTO.HasColumn("RoleName")) { ... }

                return accountDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách tài khoản: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thống kê tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> GetCustomerAccountStatisticsAsync()
        {
            try
            {
                // Lấy tất cả accounts (không filter theo status để có thể tính tổng chính xác)
                var allAccounts = await Repository.GetAll();
                var accountsList = allAccounts.ToList();

                // Lấy tất cả licenses để join
                var allLicenses = await LicenseRepository.GetAll();
                var licensesList = allLicenses.ToList();

                // Tổng số khách hàng (tất cả accounts có status Active)
                var totalCustomers = accountsList.Count(a => a.Status == StatusEnum.Active);

                // Số khách hàng đang hoạt động (Status = Active và không bị khóa)
                var activeCustomers = accountsList.Count(a => a.Status == StatusEnum.Active && !a.IsLocked);

                // Số khách hàng Premium/VIP (dựa vào LicenseName từ bảng License)
                // Chỉ tính những account có status Active
                var premiumCustomers = accountsList.Count(a =>
                {
                    // Chỉ tính accounts có status Active
                    if (a.Status != StatusEnum.Active) return false;
                    
                    if (a.LicenseId == 0) return false;
                    var license = licensesList.FirstOrDefault(l => l.ID == a.LicenseId);
                    if (license == null) return false;
                    
                    // Kiểm tra LicenseName - so sánh không phân biệt hoa thường
                    var licenseName = license.LicenseName?.Trim() ?? "";
                    if (string.IsNullOrEmpty(licenseName)) return false;
                    
                    var licenseNameUpper = licenseName.ToUpper();
                    
                    // Kiểm tra nếu LicenseName chứa các từ khóa Premium/VIP
                    // Có thể là: "Premium", "VIP", "Pro", "Professional", "Enterprise", etc.
                    if (licenseNameUpper.Contains("PREMIUM") || 
                        licenseNameUpper.Contains("VIP") || 
                        licenseNameUpper.Contains("PRO") || 
                        licenseNameUpper.Contains("ENTERPRISE"))
                    {
                        return true;
                    }
                    
                    return false;
                });

                // Số khách hàng bị khóa (chỉ tính accounts có status Active)
                var lockedCustomers = accountsList.Count(a => a.Status == StatusEnum.Active && a.IsLocked);

                // Số khách hàng hết hạn (chỉ tính accounts có status Active)
                var expiredCustomers = accountsList.Count(a => 
                    a.Status == StatusEnum.Active && 
                    a.ExpiredDate.HasValue && 
                    a.ExpiredDate.Value < DateTime.UtcNow);

                // Số khách hàng mới trong tháng này (chỉ tính accounts có status Active)
                var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var newCustomersThisMonth = accountsList.Count(a => 
                    a.Status == StatusEnum.Active &&
                    a.RegisterDate.HasValue && 
                    a.RegisterDate.Value >= startOfMonth);

                var statistics = new CustomerAccountStatisticsDTO
                {
                    TotalCustomers = totalCustomers,
                    ActiveCustomers = activeCustomers,
                    PremiumCustomers = premiumCustomers,
                    LockedCustomers = lockedCustomers,
                    ExpiredCustomers = expiredCustomers,
                    NewCustomersThisMonth = newCustomersThisMonth
                };

                return BaseResponse.Success(statistics);
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi lấy thống kê tài khoản khách hàng: {ex.Message}");
            }
        }
    }
}
