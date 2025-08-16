using AutoAppManagement.Models.DTO.RoleAccount;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IPermissionService : IBaseService
    {
        Task<List<RoleAccountDTO>> GetAllRoleAccounts();
        Task<List<RoleAccountDTO>> GetRoleAccountsByAccountId(long accountId);
        Task<List<RoleAccountDTO>> GetRoleAccountsByRoleId(long roleId);
        Task<RoleAccountDTO> GetRoleAccountById(long id);
        Task<bool> AssignRoleToAccount(AssignRoleToAccountRequest request);
        Task<bool> RemoveRoleFromAccount(long accountId, long roleId);
        Task<bool> UpdateRoleAccount(UpdateRoleAccountRequest request);
        Task<bool> BulkAssignRoles(BulkAssignRolesRequest request);
        Task<bool> BulkRemoveRoles(BulkRemoveRolesRequest request);
        Task<List<AccountWithRolesDTO>> GetAccountsWithRoles();
        Task<List<RoleWithAccountsDTO>> GetRolesWithAccounts();
        Task<bool> CheckAccountHasRole(long accountId, long roleId);
        Task<bool> CheckAccountHasPermission(long accountId, string permission);
        Task<List<string>> GetAccountPermissions(long accountId);
        Task<bool> SyncAccountRoles(long accountId, List<long> roleIds);
    }

    public class PermissionService : BaseService, IPermissionService
    {
        public PermissionService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, config, httpContextAccessor)
        {

        }

        /// <summary>
        /// Lấy tất cả role accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleAccountDTO>> GetAllRoleAccounts()
        {
            return await RequestAuthenGetAsync<List<RoleAccountDTO>>(PermissionApiUrlDef.GetAllRoleAccounts());
        }

        /// <summary>
        /// Lấy role accounts theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<RoleAccountDTO>> GetRoleAccountsByAccountId(long accountId)
        {
            return await RequestAuthenGetAsync<List<RoleAccountDTO>>(PermissionApiUrlDef.GetRoleAccountsByAccountId(accountId));
        }

        /// <summary>
        /// Lấy role accounts theo role ID
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<RoleAccountDTO>> GetRoleAccountsByRoleId(long roleId)
        {
            return await RequestAuthenGetAsync<List<RoleAccountDTO>>(PermissionApiUrlDef.GetRoleAccountsByRoleId(roleId));
        }

        /// <summary>
        /// Lấy role account theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RoleAccountDTO> GetRoleAccountById(long id)
        {
            return await RequestAuthenGetAsync<RoleAccountDTO>(PermissionApiUrlDef.GetRoleAccountById(id));
        }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> AssignRoleToAccount(AssignRoleToAccountRequest request)
        {
            return await RequestAuthenPostAsync<bool>(PermissionApiUrlDef.AssignRoleToAccount(), request);
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveRoleFromAccount(long accountId, long roleId)
        {
            return await RequestAuthenDeleteAsync<bool>(PermissionApiUrlDef.RemoveRoleFromAccount(accountId, roleId));
        }

        /// <summary>
        /// Cập nhật role account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRoleAccount(UpdateRoleAccountRequest request)
        {
            return await RequestAuthenPutAsync<bool>(PermissionApiUrlDef.UpdateRoleAccount(), request);
        }

        /// <summary>
        /// Gán nhiều role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> BulkAssignRoles(BulkAssignRolesRequest request)
        {
            return await RequestAuthenPostAsync<bool>(PermissionApiUrlDef.BulkAssignRoles(), request);
        }

        /// <summary>
        /// Gỡ nhiều role khỏi account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> BulkRemoveRoles(BulkRemoveRolesRequest request)
        {
            return await RequestAuthenPostAsync<bool>(PermissionApiUrlDef.BulkRemoveRoles(), request);
        }

        /// <summary>
        /// Lấy accounts với roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountWithRolesDTO>> GetAccountsWithRoles()
        {
            return await RequestAuthenGetAsync<List<AccountWithRolesDTO>>(PermissionApiUrlDef.GetAccountsWithRoles());
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleWithAccountsDTO>> GetRolesWithAccounts()
        {
            return await RequestAuthenGetAsync<List<RoleWithAccountsDTO>>(PermissionApiUrlDef.GetRolesWithAccounts());
        }

        /// <summary>
        /// Kiểm tra account có role không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountHasRole(long accountId, long roleId)
        {
            return await RequestAuthenGetAsync<bool>(PermissionApiUrlDef.CheckAccountHasRole(accountId, roleId));
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountHasPermission(long accountId, string permission)
        {
            return await RequestAuthenGetAsync<bool>(PermissionApiUrlDef.CheckAccountHasPermission(accountId, permission));
        }

        /// <summary>
        /// Lấy tất cả permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAccountPermissions(long accountId)
        {
            return await RequestAuthenGetAsync<List<string>>(PermissionApiUrlDef.GetAccountPermissions(accountId));
        }

        /// <summary>
        /// Đồng bộ roles của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<bool> SyncAccountRoles(long accountId, List<long> roleIds)
        {
            var request = new SyncAccountRolesRequest { AccountId = accountId, RoleIds = roleIds };
            return await RequestAuthenPostAsync<bool>(PermissionApiUrlDef.SyncAccountRoles(), request);
        }
    }
}
