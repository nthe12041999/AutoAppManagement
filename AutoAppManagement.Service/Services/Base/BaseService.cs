using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AutoAppManagement.Service.Services.Base
{
    public class BaseService
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly IDistributedCacheCustom Cache;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IMapper Mapper;
        protected readonly INotificationSocketHub NotificationSocketHub;

        public BaseService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache, IUnitOfWork unitOfWork, IMapper mapper, INotificationSocketHub notificationSocketHub)
        {
            HttpContextAccessor = httpContextAccessor;
            Cache = cache;
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            NotificationSocketHub = notificationSocketHub;
        }

        public AccountGenericDTO GetUserAuthen()
        {
            var userInfor = new AccountGenericDTO();
            var userContext = HttpContextAccessor?.HttpContext?.User;
            if (userContext?.Identity != null && userContext.Identity.IsAuthenticated)
            {
                var userInforUserName = userContext?.FindFirst(JwtRegisteredClaimsNamesConstant.Sub)?.Value;
                if (userInforUserName != null)
                {
                    userInfor.UserName = userInforUserName;
                    var valueAccId = userContext?.FindFirst(JwtRegisteredClaimsNamesConstant.AccId)?.Value;
                    if (valueAccId != null)
                    {
                        userInfor.AccountId =
                            long.Parse(valueAccId);
                        userInfor.RoleList = userContext.FindAll(JwtRegisteredClaimsNamesConstant.Role)
                            .Select(c => c.Value)
                            .ToList();
                    }
                }
            }
            else
            {
                return null;
            }

            return userInfor;
        }

        public Dictionary<string, Role> GetRoleAuthen()
        {
            var roleInfor = new Dictionary<string, Role>();
            var userContext = HttpContextAccessor?.HttpContext?.User;
            if (userContext?.Identity != null && userContext.Identity.IsAuthenticated)
            {
                var lstRoleJson = userContext.FindAll(JwtRegisteredClaimsNamesConstant.RoleInfor).Select(c => c.Value).ToList();
                foreach (var item in lstRoleJson)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var roleObject = JsonSerializer.Deserialize<Role>(item);
                        roleInfor.Add(roleObject.RoleName, roleObject);
                    }
                }
            }
            else
            {
                return null;
            }

            return roleInfor;
        }

        /// <summary>
        /// TODO: Hàm này đẻ ra chỉ để lấy role có grant type lớn nhất, sau này thống nhất lại thì phải làm động không làm thế này
        /// </summary>
        /// <returns></returns>
        public Role GetMaxRoleAuthen()
        {
            var roleList = GetRoleAuthen();
            if (roleList.ContainsKey(RoleConstant.Admin))
            {
                // là admin lấy luôn role admin luôn
                return roleList[RoleConstant.Admin];
            }
            else if (roleList.ContainsKey(RoleConstant.Employee))
            {
                return roleList[RoleConstant.Employee];
            }
            else if (roleList.ContainsKey(RoleConstant.Customer))
            {
                return roleList[RoleConstant.Customer];
            }
            return null;
        }
        protected List<IFormFile> ConvertBase64ToFormFile(List<ImgInfor> imgContent)
        {
            var formFiles = new List<IFormFile>();

            foreach (var item in imgContent)
            {
                if (!string.IsNullOrEmpty(item.Base64))
                {
                    byte[] bytes = Convert.FromBase64String(item.Base64);
                    MemoryStream stream = new(bytes);

                    //item.File = new FormFile(stream, 0, bytes.Length, item.SeoFilename, item.SeoFilename);
                    formFiles.Add(new FormFile(stream, 0, stream.Length, null, item.SeoFilename)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = item.ContentType
                    });
                }
            }
            return formFiles;
        }
    }
}
