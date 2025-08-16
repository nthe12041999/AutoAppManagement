using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AdminAccount;
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
    public interface IAdminAccountService
    {
        Task<List<AdminAccountDTO>> GetAllAdminAccounts();
        Task<AdminAccountDTO> GetAdminAccountById(long id);
        Task<AdminAccountDTO> GetAdminAccountByUsername(string username);
        Task<RestOutput> CreateAdminAccount(CreateAdminAccountRequest request);
        Task<RestOutput> UpdateAdminAccount(UpdateAdminAccountRequest request);
        Task<RestOutput> DeleteAdminAccount(long id);
        Task<RestOutput> ChangePassword(long id, string newPassword);
        Task<RestOutput> LockAccount(long id, int minutes = 30);
        Task<RestOutput> UnlockAccount(long id);
        Task<RestOutput> VerifyEmail(long id);
        Task<RestOutput> VerifyPhone(long id);
        Task<RestOutput> EnableTwoFactor(long id);
        Task<RestOutput> DisableTwoFactor(long id);
        Task<AdminLoginResponse> Login(string username, string password, string ipAddress = null, string userAgent = null);
        Task<List<AdminAccountDTO>> GetAdminAccountsByRole(string role);
        Task<RestOutput> UpdatePermissions(long id, string permissions);
    }

    public class AdminAccountService : BaseService, IAdminAccountService
    {
        public AdminAccountService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache, 
            IUnitOfWork unitOfWork, IMapper mapper, INotificationSocketHub notificationSocketHub) 
            : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
        }

        /// <summary>
        /// Lấy tất cả admin accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<AdminAccountDTO>> GetAllAdminAccounts()
        {
            var adminAccounts = await UnitOfWork.AdminAccountRepository.GetAll();
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.Where(a => !a.IsDeleted).ToList());
        }

        /// <summary>
        /// Lấy admin account theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AdminAccountDTO> GetAdminAccountById(long id)
        {
            var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        /// <summary>
        /// Lấy admin account theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AdminAccountDTO> GetAdminAccountByUsername(string username)
        {
            var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.UserName == username && !a.IsDeleted);
            return Mapper.Map<AdminAccountDTO>(adminAccount);
        }

        /// <summary>
        /// Tạo admin account mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> CreateAdminAccount(CreateAdminAccountRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUsername = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.UserName == request.UserName);
                if (existingUsername != null)
                {
                    result.ErrorEventHandler("Username đã tồn tại");
                    return result;
                }

                // Kiểm tra email đã tồn tại chưa
                var existingEmail = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Email == request.Email);
                if (existingEmail != null)
                {
                    result.ErrorEventHandler("Email đã tồn tại");
                    return result;
                }

                var adminAccount = new AdminAccount
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    UserName = request.UserName,
                    PasswordHash = HashCodeUlti.EncodePassword(request.Password),
                    Role = request.Role,
                    Permissions = request.Permissions,
                    Department = request.Department,
                    Position = request.Position,
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    IsTwoFactorEnabled = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetUserAuthen()?.Id,
                    Status = "Active"
                };

                await UnitOfWork.AdminAccountRepository.CreateAsync(adminAccount);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AdminAccountDTO>(adminAccount));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật admin account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateAdminAccount(UpdateAdminAccountRequest request)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == request.Id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                // Kiểm tra username đã tồn tại chưa (trừ account hiện tại)
                var existingUsername = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => 
                    a.UserName == request.UserName && a.Id != request.Id);
                if (existingUsername != null)
                {
                    result.ErrorEventHandler("Username đã tồn tại");
                    return result;
                }

                // Kiểm tra email đã tồn tại chưa (trừ account hiện tại)
                var existingEmail = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => 
                    a.Email == request.Email && a.Id != request.Id);
                if (existingEmail != null)
                {
                    result.ErrorEventHandler("Email đã tồn tại");
                    return result;
                }

                adminAccount.FullName = request.FullName;
                adminAccount.Email = request.Email;
                adminAccount.PhoneNumber = request.PhoneNumber;
                adminAccount.UserName = request.UserName;
                adminAccount.Role = request.Role;
                adminAccount.Permissions = request.Permissions;
                adminAccount.Department = request.Department;
                adminAccount.Position = request.Position;
                adminAccount.Avatar = request.Avatar;
                adminAccount.SetUpdated(GetUserAuthen()?.Id);

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<AdminAccountDTO>(adminAccount));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Xóa admin account (soft delete)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeleteAdminAccount(long id)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.SetDeleted(GetUserAuthen()?.Id);
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
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.ChangePassword(HashCodeUlti.EncodePassword(newPassword), GetUserAuthen()?.Id);
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
        /// <param name="minutes"></param>
        /// <returns></returns>
        public async Task<RestOutput> LockAccount(long id, int minutes = 30)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.LockAccount(minutes, GetUserAuthen()?.Id);
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
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.UnlockAccount(GetUserAuthen()?.Id);
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
        /// Xác thực email
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> VerifyEmail(long id)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.VerifyEmail(GetUserAuthen()?.Id);
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
        /// Xác thực phone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> VerifyPhone(long id)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.VerifyPhone(GetUserAuthen()?.Id);
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
        /// Bật 2FA
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> EnableTwoFactor(long id)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.IsTwoFactorEnabled = true;
                adminAccount.SetUpdated(GetUserAuthen()?.Id);
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
        /// Tắt 2FA
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DisableTwoFactor(long id)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.IsTwoFactorEnabled = false;
                adminAccount.TwoFactorSecret = null;
                adminAccount.SetUpdated(GetUserAuthen()?.Id);
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
        /// Đăng nhập admin
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public async Task<AdminLoginResponse> Login(string username, string password, string ipAddress = null, string userAgent = null)
        {
            var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => 
                a.UserName == username && !a.IsDeleted);

            if (adminAccount == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            if (adminAccount.IsLocked)
            {
                throw new Exception("Tài khoản đã bị khóa");
            }

            var passwordHash = HashCodeUlti.EncodePassword(password);
            if (adminAccount.PasswordHash != passwordHash)
            {
                adminAccount.RecordFailedLogin();
                await UnitOfWork.CommitAsync();
                throw new Exception("Mật khẩu không chính xác");
            }

            if (!adminAccount.IsActive)
            {
                throw new Exception("Tài khoản không hoạt động");
            }

            // Ghi nhận đăng nhập thành công
            adminAccount.RecordLogin(ipAddress, userAgent);
            await UnitOfWork.CommitAsync();

            return new AdminLoginResponse
            {
                Id = adminAccount.Id,
                FullName = adminAccount.FullName,
                Email = adminAccount.Email,
                UserName = adminAccount.UserName,
                Role = adminAccount.Role,
                Permissions = adminAccount.Permissions,
                Avatar = adminAccount.Avatar,
                Department = adminAccount.Department,
                Position = adminAccount.Position
            };
        }

        /// <summary>
        /// Lấy admin accounts theo role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<List<AdminAccountDTO>> GetAdminAccountsByRole(string role)
        {
            var adminAccounts = await UnitOfWork.AdminAccountRepository.GetByCondition(a => 
                a.Role == role && !a.IsDeleted);
            return Mapper.Map<List<AdminAccountDTO>>(adminAccounts.ToList());
        }

        /// <summary>
        /// Cập nhật quyền
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdatePermissions(long id, string permissions)
        {
            var result = new RestOutput();

            try
            {
                var adminAccount = await UnitOfWork.AdminAccountRepository.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (adminAccount == null)
                {
                    result.ErrorEventHandler("Admin account không tồn tại");
                    return result;
                }

                adminAccount.Permissions = permissions;
                adminAccount.SetUpdated(GetUserAuthen()?.Id);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }
    }
}
