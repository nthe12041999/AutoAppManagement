using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Common.Ulti;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AutoAppManagement.Service.Services
{
    public interface IAccountsService
    {
        Task<LoginResponse> Login(string userName, string password);
        Task<bool> Register(AccountRegister account);
        void Logout();
        AccountGenericDTO GetUserInfor();
        Task<RestOutput> UpdateUserInfor(AccountUpdate account);
        Task<RestOutput> ChangePassword(string newPassword, string oldPassword);
        Task<AccountUpdate> GetUserInforGeneric();
        Task<bool> UpdateLockedAccount(LockedAccountParam param);
        Task<List<AccountGenericVM>> GetAllAccount();
    }
    public class AccountsService : BaseService, IAccountsService
    {
        private IFileUlti _fileUlti;
        private readonly IConfiguration _configuration;
        public AccountsService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache, IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, INotificationSocketHub notificationSocketHub, IFileUlti fileUlti) : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
            _configuration = configuration;
            _fileUlti = fileUlti;
        }

        /// <summary>
        /// Hàm xử lý login
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<LoginResponse> Login(string userName, string password)
        {
            var account = await UnitOfWork.AccountsRepository.GetUserByUserNameAndPass(userName, password);
            if (account != null)
            {
                if (account.IsLocked)
                {
                    throw new Exception("Tài khoản đã bị khóa");
                }

                // lấy role
                var listRole = (from r in UnitOfWork.RoleRepository.Get()
                                join ra in UnitOfWork.RoleAccountRepository.Get() on r.Id equals ra.RoleId
                                join a in UnitOfWork.AccountsRepository.Get() on ra.AccountId equals a.Id
                                where a.Id == account.Id
                                select r.RoleName).ToList();
                var token = HandleSignInAndGenerateToken(account, listRole);

                if (token != null)
                {
                    // ghi token vào cookie
                    HttpContextAccessor?.HttpContext?.Response.Cookies.Append("access_token", token, new CookieOptions
                    {
                        HttpOnly = true, // Set HttpOnly to true for security
                        Secure = true,   // Set Secure to true if your site uses HTTPS
                        SameSite = SameSiteMode.Strict, // Set SameSite to Strict for added security
                        Expires = DateTime.UtcNow.AddDays(1) // Set the expiration time
                    });
                    var result = new LoginResponse
                    {
                        Token = token,
                        ImgAvatar = account.ImgAvatar,
                        Email = account.Email,
                        Id = account.Id,
                        RoleList = listRole
                    };
                    return result;
                }
            }
            else
            {
                throw new Exception("Sai tài khoản hoặc mật khẩu");
            }
            return null;
        }

        /// <summary>
        /// Hàm xử lý đăng kí
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<bool> Register(AccountRegister account)
        {
            var acc = await UnitOfWork.AccountsRepository.GetUserByUserNameAndPass(account.UserName, account.Password);
            if (acc == null)
            {
                // encode pass trước khi cất
                account.Password = HashCodeUlti.EncodePassword(account.Password);
                var accountInsert = Mapper.Map<Account>(account);
                // cất user
                await UnitOfWork.AccountsRepository.CreateAsync(accountInsert);

                // cất thêm cả role => mặc định role customer
                var roleCus = UnitOfWork.RoleRepository.FirstOrDefault(item => item.RoleName == "Customer");
                if (roleCus != null)
                {
                    var role = new RoleAccount
                    {
                        RoleId = roleCus.Id,
                        AccountId = accountInsert.Id
                    };
                    await UnitOfWork.RoleAccountRepository.CreateAsync(role);
                }

                await UnitOfWork.CommitAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hàm xử lý đăng xuất
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <returns></returns>
        public void Logout()
        {
            // SignOut
            HttpContextAccessor?.HttpContext?.Response?.Cookies.Delete("access_token");
        }

        /// <summary>
        /// Hàm lấy thông tin user theo authen
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <returns></returns>
        public AccountGenericDTO GetUserInfor()
        {
            return GetUserAuthen();
        }

        /// <summary>
        /// Hàm lấy thông tin user
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <returns></returns>
        public async Task<AccountUpdate> GetUserInforGeneric()
        {
            var userName = GetUserAuthen()?.UserName;
            if (!string.IsNullOrEmpty(userName))
            {
                var userFull = await UnitOfWork.AccountsRepository.FirstOrDefault(item => item.UserName == userName);
                if (userFull != null)
                {
                    return new AccountUpdate
                    {
                        Id = userFull.Id,
                        UserName = userFull.UserName,
                        Name = userFull.Name,
                        Email = userFull.Email,
                        Gender = userFull.Gender,
                        DateOfBirth = userFull.DateOfBirth,
                        ImgAvatar = userFull.ImgAvatar
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Hàm xử lý cập nhật thông tin user
        /// CreatedBy ntthe 03.03.2024
        /// </summary>
        /// <param name="account"></param>
        /// <param name="hasLogin"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateUserInfor(AccountUpdate account)
        {
            var res = new RestOutput();
            IFormFile fileAvatar = null;

            if (account != null)
            {
                //if (account.ImgContent != null)
                //{
                //    var listFile = ConvertBase64ToFormFile(new List<ImgInfor>() { account.ImgContent });
                //    fileAvatar = listFile.FirstOrDefault();
                //}

                var isExistUser = await UnitOfWork.AccountsRepository.CheckExitsByCondition(item => item.UserName == account.UserName && item.Id != account.Id);
                if (isExistUser)
                {
                    res.ErrorEventHandler(null, "Username đã tồn tại");
                }
                else
                {
                    var userNameCurrent = GetUserAuthen()?.UserName;
                    var accUserUpdate = await UnitOfWork.AccountsRepository.FirstOrDefault(item => item.UserName == userNameCurrent);
                    if (accUserUpdate != null)
                    {
                        // save ảnh
                        if (fileAvatar != null)
                        {
                            accUserUpdate.ImgAvatar = fileAvatar.FileName;
                            await _fileUlti.SaveFile(fileAvatar);
                        }

                        // update từng field của user
                        accUserUpdate.UserName = account.UserName;
                        accUserUpdate.Name = account.Name;
                        accUserUpdate.Email = account.Email;
                        accUserUpdate.Gender = account.Gender;
                        accUserUpdate.DateOfBirth = account.DateOfBirth;

                        // lưu acc
                        await UnitOfWork.CommitAsync();
                        var dataResponse = new UserInforGeneric
                        {
                            UserName = accUserUpdate.UserName,
                            ImgAvatar = accUserUpdate.ImgAvatar,
                        };
                        res.SuccessEventHandler(dataResponse);
                    }
                }
            }

            return res;

        }

        /// <summary>
        /// Hàm xử lý cập nhật password
        /// CreatedBy ntthe 03.03.2024
        /// </summary>
        /// <param name="account"></param>
        /// <param name="hasLogin"></param>
        /// <returns></returns>
        public async Task<RestOutput> ChangePassword(string newPassword, string oldPassword)
        {
            var res = new RestOutput();
            if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(oldPassword))
            {
                // nếu trùng là chửi
                if (newPassword.Trim() == oldPassword.Trim())
                {
                    res.ErrorEventHandler("Password mới trùng với password cũ");
                }
                else
                {
                    var userNameCurrent = GetUserAuthen()?.UserName;
                    var accUserUpdate = await UnitOfWork.AccountsRepository.FirstOrDefault(item => item.UserName == userNameCurrent);

                    var oldPasswordHash = HashCodeUlti.EncodePassword(oldPassword);

                    if (accUserUpdate != null && accUserUpdate.Password != oldPasswordHash)
                    {
                        res.ErrorEventHandler("Password cũ không chính xác");
                    }
                    else
                    {
                        var passWordHash = HashCodeUlti.EncodePassword(newPassword);
                        // update từng field của user
                        if (accUserUpdate != null)
                        {
                            accUserUpdate.Password = passWordHash;
                        }

                        // lưu acc
                        await UnitOfWork.CommitAsync();
                        res.SuccessEventHandler(true);
                    }
                }
            }
            else
            {
                res.ErrorEventHandler("Password không được để trống");
            }

            return res;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách user
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountGenericVM>> GetAllAccount()
        {
            var result = await UnitOfWork.AccountsRepository.GetAll();

            return Mapper.Map<List<AccountGenericVM>>(result.ToList());
        }

        /// <summary>
        /// Cập nhật trạng thái khóa tài khoản
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<bool> UpdateLockedAccount(LockedAccountParam param)
        {
            var user = await UnitOfWork.AccountsRepository.FirstOrDefault(f => f.Id == param.AccountId);

            if (user == null)
            {
                return false;
            }

            try
            {
                user.IsLocked = param.IsLocked;
                await UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        #region Private Method

        #region phần đăng nhập sử dụng BearToken

        /// <summary>
        /// Hàm ghi claim và gen AccessToken
        /// </summary>
        /// <param name="account"></param>
        /// <param name="listRole"></param>
        /// <returns></returns>
        private string HandleSignInAndGenerateToken(Account account, List<string> listRole)
        {
            if (_configuration != null)
            {
                var jwtToken = new JwtSecurityTokenHandler();
                var valueSecretKey = _configuration?.GetSection("Jwt")?.GetSection("SecretKey")?.Value;
                if (valueSecretKey != null)
                {
                    var secretKeyByte = Encoding.UTF8.GetBytes(valueSecretKey);
                    var tokenDescription = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(JwtRegisteredClaimsNamesConstant.Sid, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimsNamesConstant.Sub, account.UserName ?? ""),
                            new Claim(JwtRegisteredClaimsNamesConstant.AccId, account.Id.ToString()),
                            new Claim(JwtRegisteredClaimsNamesConstant.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimsNamesConstant.Avatar, account.ImgAvatar ?? ""),
                            new Claim(JwtRegisteredClaimsNamesConstant.Email, account.Email ?? ""),
                        }),
                        //Expires = DateTime.UtcNow.AddMinutes(10),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyByte), SecurityAlgorithms.HmacSha256)
                    };

                    // xử lý add Role

                    if (listRole != null && listRole.Any())
                    {
                        foreach (var role in listRole)
                        {
                            if (!string.IsNullOrEmpty(role))
                            {
                                tokenDescription?.Subject.AddClaim(new Claim(JwtRegisteredClaimsNamesConstant.Role, role));
                                tokenDescription?.Subject.AddClaim(new Claim(JwtRegisteredClaimsNamesConstant.RoleInfor, JsonSerializer.Serialize(role)));
                            }
                        }
                    }

                    var token = jwtToken.CreateToken(tokenDescription);
                    return jwtToken.WriteToken(token);
                }
            }
            return null;
        }
        #endregion

        #region Luồng đăng nhập với cookie
        /// <summary>
        /// Hàm xử lý sign cho đăng nhập với loại đăng nhập là cookie
        /// CreatedBy ntthe 28.02.2024
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [Obsolete]
        private async Task<AccountGenericDTO> HandleSignInWithCookieSign(Account account)
        {
            var authenObject = SignClaimWriteToken(account);
            if (authenObject != null)
            {
                await HttpContextAccessor?.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                            new ClaimsPrincipal(authenObject.ClaimsIdentity),
                                                            authenObject.AuthProperties);
                var data = new AccountGenericDTO
                {
                    UserName = account.UserName,
                };
                return data;
            }

            return null;
        }

        /// <summary>
        /// Hàm ghi thông tin vào claim
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [Obsolete]
        private AccountAuthenDTO SignClaimWriteToken(Account account)
        {
            var listClaim = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimsNamesConstant.Sid, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimsNamesConstant.Sub, account.UserName),
                    new Claim(JwtRegisteredClaimsNamesConstant.Jti, Guid.NewGuid().ToString())
                };

            //if (account.IsAdmin)
            //{
            //    listClaim.Add(new Claim(ClaimTypes.Role, RoleConstant.Admin));
            //}
            //else
            //{
            //    listClaim.Add(new Claim(ClaimTypes.Role, RoleConstant.Customer));
            //}

            var claimsIdentity = new ClaimsIdentity(listClaim, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                // Thông tin cấu hình cookie, ví dụ như thời gian hết hạn
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                IsPersistent = false, // Đặt true nếu bạn muốn cookie "nhớ đăng nhập" qua các session
                AllowRefresh = false
            };

            return new AccountAuthenDTO
            {
                ClaimsIdentity = claimsIdentity,
                AuthProperties = authProperties,
                UserName = account.UserName,
            };

        }

        /// <summary>
        /// Hàm xử lý đăng xuất
        /// CreatedBy ntthe 25.02.2024
        /// </summary>
        /// <returns></returns>
        public async Task LogoutCookie()
        {
            // SignOut
            await HttpContextAccessor?.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContextAccessor?.HttpContext?.Session.Clear();
        }


        #endregion
        #endregion
    }
}
