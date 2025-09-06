using AutoAppManagement.Models.DTO.RoleAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IPermissionService : IBaseBusinessService<RoleAccountDTO>
    {
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
        Task<ResponseOutput<bool>> AssignRolePermissionsAsync(long roleId, List<long> permissionIds);
    }

    public class PermissionService : BaseBusinessService<RoleAccountDTO, PermissionApiUrlDef>, IPermissionService
    {
        public PermissionService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> AssignRoleToAccount(AssignRoleToAccountRequest request)
        {
            return await RequestAuthenPostAsync<bool>(ApiUrlDef.AssignRoleToAccount(), request);
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveRoleFromAccount(long accountId, long roleId)
        {
            return await RequestAuthenDeleteAsync<bool>(ApiUrlDef.RemoveRoleFromAccount(accountId, roleId));
        }

        /// <summary>
        /// Cập nhật role account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRoleAccount(UpdateRoleAccountRequest request)
        {
            return await RequestAuthenPutAsync<bool>(ApiUrlDef.UpdateRoleAccount(), request);
        }

        /// <summary>
        /// Gán nhiều role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> BulkAssignRoles(BulkAssignRolesRequest request)
        {
            return await RequestAuthenPostAsync<bool>(ApiUrlDef.BulkAssignRoles(), request);
        }

        /// <summary>
        /// Gỡ nhiều role khỏi account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> BulkRemoveRoles(BulkRemoveRolesRequest request)
        {
            return await RequestAuthenPostAsync<bool>(ApiUrlDef.BulkRemoveRoles(), request);
        }

        /// <summary>
        /// Lấy accounts với roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountWithRolesDTO>> GetAccountsWithRoles()
        {
            return await RequestAuthenGetAsync<List<AccountWithRolesDTO>>(ApiUrlDef.GetAccountsWithRoles());
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleWithAccountsDTO>> GetRolesWithAccounts()
        {
            return await RequestAuthenGetAsync<List<RoleWithAccountsDTO>>(ApiUrlDef.GetRolesWithAccounts());
        }

        /// <summary>
        /// Kiểm tra account có role không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountHasRole(long accountId, long roleId)
        {
            return await RequestAuthenGetAsync<bool>(ApiUrlDef.CheckAccountHasRole(accountId, roleId));
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountHasPermission(long accountId, string permission)
        {
            return await RequestAuthenGetAsync<bool>(ApiUrlDef.CheckAccountHasPermission(accountId, permission));
        }

        /// <summary>
        /// Lấy tất cả permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAccountPermissions(long accountId)
        {
            return await RequestAuthenGetAsync<List<string>>(ApiUrlDef.GetAccountPermissions(accountId));
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
            return await RequestAuthenPostAsync<bool>(ApiUrlDef.SyncAccountRoles(), request);
        }

        public async Task<ResponseOutput<bool>> AssignRolePermissionsAsync(long roleId, List<long> permissionIds)
        {
            return await RequestFullAuthenPostAsync<bool>(
                ApiUrlDef.AssignRolePermissions(),
                new { RoleId = roleId, PermissionIds = permissionIds }
            );
        }
    }
}
