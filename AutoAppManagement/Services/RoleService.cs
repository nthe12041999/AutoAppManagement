using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IRoleService : IBaseService
    {
        Task<List<RoleDTO>> GetAllRoles();
        Task<RoleDTO> GetRoleById(long id);
        Task<bool> CreateRole(CreateRoleRequest request);
        Task<bool> UpdateRole(UpdateRoleRequest request);
        Task<bool> DeleteRole(long id);
        Task<List<RoleDTO>> GetRolesByAccountId(long accountId);
        Task<bool> AssignRoleToAccount(AssignRoleRequest request);
        Task<bool> RemoveRoleFromAccount(long accountId, long roleId);
        Task<bool> CheckRoleExists(string roleName);
    }

    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor) 
            : base(httpClientFactory, config, httpContextAccessor)
        {
        }

        /// <summary>
        /// Lấy tất cả roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleDTO>> GetAllRoles()
        {
            return await RequestAuthenGetAsync<List<RoleDTO>>(RoleApiUrlDef.GetAllRoles());
        }

        /// <summary>
        /// Lấy role theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RoleDTO> GetRoleById(long id)
        {
            return await RequestAuthenGetAsync<RoleDTO>(RoleApiUrlDef.GetRoleById(id));
        }

        /// <summary>
        /// Tạo role mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> CreateRole(CreateRoleRequest request)
        {
            return await RequestAuthenPostAsync<bool>(RoleApiUrlDef.CreateRole(), request);
        }

        /// <summary>
        /// Cập nhật role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRole(UpdateRoleRequest request)
        {
            return await RequestAuthenPutAsync<bool>(RoleApiUrlDef.UpdateRole(), request);
        }

        /// <summary>
        /// Xóa role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRole(long id)
        {
            return await RequestAuthenDeleteAsync<bool>(RoleApiUrlDef.DeleteRole(id));
        }

        /// <summary>
        /// Lấy roles của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<RoleDTO>> GetRolesByAccountId(long accountId)
        {
            return await RequestAuthenGetAsync<List<RoleDTO>>(RoleApiUrlDef.GetRolesByAccountId(accountId));
        }

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
