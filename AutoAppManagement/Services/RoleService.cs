using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IRoleService : IBaseBusinessService<RoleDTO>
    {
        Task<bool> AssignRoleToAccount(AssignRoleRequest request);
        Task<bool> RemoveRoleFromAccount(long accountId, long roleId);
        Task<bool> CheckRoleExists(string roleName);
    }

    public class RoleService : BaseBusinessService<RoleDTO>, IRoleService
    {
        public RoleService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> AssignRoleToAccount(AssignRoleRequest request)
        {
            return await RequestAuthenPostAsync<bool>(RoleApiUrlDef.AssignRoleToAccount(), request);
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveRoleFromAccount(long accountId, long roleId)
        {
            return await RequestAuthenDeleteAsync<bool>(RoleApiUrlDef.RemoveRoleFromAccount(accountId, roleId));
        }

        /// <summary>
        /// Kiểm tra role có tồn tại không
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<bool> CheckRoleExists(string roleName)
        {
            return await RequestAuthenGetAsync<bool>(RoleApiUrlDef.CheckRoleExists(roleName));
        }
    }
}
