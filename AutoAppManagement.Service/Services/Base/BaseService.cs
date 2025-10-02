using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AutoAppManagement.Service.Services.Base
{
    public class BaseService
    {
        protected readonly IServiceProvider _serviceProvider;

        // Lazy load properties
        private IHttpContextAccessor? _httpContextAccessor;
        protected IHttpContextAccessor HttpContextAccessor
            => _httpContextAccessor ??= _serviceProvider.GetRequiredService<IHttpContextAccessor>();

        private IDistributedCacheCustom? _cache;
        protected IDistributedCacheCustom Cache
            => _cache ??= _serviceProvider.GetRequiredService<IDistributedCacheCustom>();

        private IUnitOfWork? _unitOfWork;
        protected IUnitOfWork UnitOfWork
            => _unitOfWork ??= _serviceProvider.GetRequiredService<IUnitOfWork>();

        private IMapper? _mapper;
        protected IMapper Mapper
            => _mapper ??= _serviceProvider.GetRequiredService<IMapper>();

        private INotificationSocketHub? _notificationSocketHub;
        protected INotificationSocketHub NotificationSocketHub
            => _notificationSocketHub ??= _serviceProvider.GetRequiredService<INotificationSocketHub>();

        private IRestOutput? _res;
        protected IRestOutput ResOutput
            => _res ??= _serviceProvider.GetRequiredService<IRestOutput>();
        
        public BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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
        
        /// <summary>
        /// Get current user ID from claims
        /// </summary>
        /// <returns></returns>
        protected long GetCurrentUserId()
        {
            var userContext = HttpContextAccessor?.HttpContext?.User;
            if (userContext?.Identity != null && userContext.Identity.IsAuthenticated)
            {
                var valueAccId = userContext?.FindFirst(JwtRegisteredClaimsNamesConstant.AccId)?.Value;
                if (valueAccId != null && long.TryParse(valueAccId, out long userId))
                {
                    return userId;
                }
            }
            return 1; // Default for testing
        }
    }
}
