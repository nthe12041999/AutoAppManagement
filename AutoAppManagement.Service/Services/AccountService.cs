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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        Task<List<AccountDTO>> GetAccountsByLevel(int level);
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
        private readonly IGenericRepository<License> _licenseRepository;
        private readonly IGenericRepository<AccountDevice> _accountDeviceRepository;
        private readonly IJwtService _jwtService;

        public AccountService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _licenseRepository = UnitOfWork.GetRepository<License>();
            _accountDeviceRepository = UnitOfWork.GetRepository<AccountDevice>();
            _jwtService = serviceProvider.GetRequiredService<IJwtService>();
        }

        public async Task<AccountDTO> GetAccountByUsername(string username)
        {
            var account = await Repository.FirstOrDefault(a => a.UserName == username && !a.IsDeleted);
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
                account.Notes = reason;
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
                account.IsActive = true;
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
                account.IsActive = false;
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Vô hiệu hóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi vô hiệu hóa tài khoản: {ex.Message}");
            }
        }

        public async Task<List<AccountDTO>> GetAccountsByLevel(int level)
        {
            var accounts = await Repository.GetByCondition(a => a.Level == level && !a.IsDeleted);
            return Mapper.Map<List<AccountDTO>>(accounts.ToList());
        }

        public async Task<List<AccountDTO>> GetExpiredAccounts()
        {
            var accounts = await Repository.GetByCondition(a => a.ExpiredDate < DateTime.UtcNow && !a.IsDeleted);
            return Mapper.Map<List<AccountDTO>>(accounts.ToList());
        }

        public async Task<List<AccountDTO>> GetExpiringAccounts(int days)
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            var accounts = await Repository.GetByCondition(a => a.ExpiredDate <= expiryDate && a.ExpiredDate > DateTime.UtcNow && !a.IsDeleted);
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

        public async Task<BaseResponse> Login(LoginRequest request)
        {
            try
            {
                Account? account = request.EmailOrPhone.Contains("@")
                    ? await Repository.FirstOrDefault(a => a.Email == request.EmailOrPhone && !a.IsDeleted)
                    : await Repository.FirstOrDefault(a => a.Phone == request.EmailOrPhone && !a.IsDeleted);

                if (account == null) return BaseResponse.Error("Tài khoản không tồn tại");

                if (account.Password != HashCodeUlti.EncodePassword(request.Password)) return BaseResponse.Error("Mật khẩu không chính xác");
                if (account.IsLocked) return BaseResponse.Error("Tài khoản đã bị khóa");
                if (!account.IsActive) return BaseResponse.Error("Tài khoản chưa được kích hoạt");
                if (account.ExpiredDate <= DateTime.UtcNow) return BaseResponse.Error("Tài khoản đã hết hạn");

                var license = (await _licenseRepository.GetByCondition(l => l.AccountId == account.Id && l.Status == "Active" && l.StartDate <= DateTime.UtcNow && l.ExpiryDate > DateTime.UtcNow && !l.IsDeleted)).FirstOrDefault();
                if (license == null) return BaseResponse.Error("Không có license hợp lệ");

                var licenseInfo = new LicenseInfoDTO
                {
                    LicenseId = license.Id,
                    LicenseKey = license.LicenseKey,
                    LicenseName = license.LicenseName,
                    LicenseType = license.LicenseType,
                    Status = license.Status,
                    StartDate = license.StartDate,
                    EndDate = license.ExpiryDate.GetValueOrDefault(),
                    DaysRemaining = license.ExpiryDate.HasValue ? (int)Math.Max(0, (license.ExpiryDate.Value - DateTime.UtcNow).TotalDays) : 9999
                };

                var token = _jwtService.GenerateToken(account, licenseInfo);

                account.SetUpdated(account.Id);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(new { Token = token }, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đăng nhập: {ex.Message}");
            }
        }

        // AccountDevice methods
        public async Task<List<AccountDeviceDTO>> GetAllAccountDevices()
        {
            var devices = await _accountDeviceRepository.GetAll();
            return Mapper.Map<List<AccountDeviceDTO>>(devices.Where(d => !d.IsDeleted).ToList());
        }

        public async Task<List<AccountDeviceDTO>> GetAccountDevicesByAccountId(long accountId)
        {
            var devices = await _accountDeviceRepository.GetByCondition(d => d.AccountId == accountId && !d.IsDeleted);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        public async Task<AccountDeviceDTO> GetAccountDeviceById(long id)
        {
            var device = await _accountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            return Mapper.Map<AccountDeviceDTO>(device);
        }

        public async Task<BaseResponse> RegisterDevice(RegisterDeviceRequest request)
        {
            try
            {
                var existingDevice = await _accountDeviceRepository.FirstOrDefault(d => d.DeviceId == request.DeviceId && d.AccountId == request.AccountId && !d.IsDeleted);
                if (existingDevice != null) return BaseResponse.Error("Device đã được đăng ký cho account này");

                var device = Mapper.Map<AccountDevice>(request);
                device.SetCreated(GetCurrentUserId());

                await _accountDeviceRepository.Insert(device);
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
                var device = await _accountDeviceRepository.FirstOrDefault(d => d.Id == request.Id && !d.IsDeleted);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                Mapper.Map(request, device);
                device.SetUpdated(GetCurrentUserId());
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
                var device = await _accountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                device.SetDeleted(GetCurrentUserId());
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
                var device = await _accountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                device.IsActive = true;
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
                var device = await _accountDeviceRepository.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
                if (device == null) return BaseResponse.Error("Device không tồn tại");

                device.IsActive = false;
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
            var devices = await _accountDeviceRepository.GetByCondition(d => d.AccountId == accountId && d.IsActive && !d.IsDeleted);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        public async Task<List<AccountDeviceDTO>> GetDevicesByType(string deviceType)
        {
            var devices = await _accountDeviceRepository.GetByCondition(d => d.DeviceType == deviceType && !d.IsDeleted);
            return Mapper.Map<List<AccountDeviceDTO>>(devices.ToList());
        }

        public async Task<bool> IsDeviceRegistered(string deviceId, long accountId)
        {
            return await _accountDeviceRepository.Any(d => d.DeviceId == deviceId && d.AccountId == accountId && !d.IsDeleted);
        }
    }
}
