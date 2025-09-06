using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class PermissionApiUrlDef : BaseApiUrlDef
    {
        public PermissionApiUrlDef() : base("/api/Permission") { }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <returns></returns>
        public string AssignRoleToAccount()
        {
            return $"{_pathController}/AssignRoleToAccount";
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string RemoveRoleFromAccount(long accountId, long roleId)
        {
            return $"{_pathController}/RemoveRoleFromAccount?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Cập nhật role account
        /// </summary>
        /// <returns></returns>
        public string UpdateRoleAccount()
        {
            return $"{_pathController}/UpdateRoleAccount";
        }

        /// <summary>
        /// Gán nhiều role cho account
        /// </summary>
        /// <returns></returns>
        public string BulkAssignRoles()
        {
            return $"{_pathController}/BulkAssignRoles";
        }

        /// <summary>
        /// Gỡ nhiều role khỏi account
        /// </summary>
        /// <returns></returns>
        public string BulkRemoveRoles()
        {
            return $"{_pathController}/BulkRemoveRoles";
        }

        /// <summary>
        /// Lấy accounts với roles
        /// </summary>
        /// <returns></returns>
        public string GetAccountsWithRoles()
        {
            return $"{_pathController}/GetAccountsWithRoles";
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        public string GetRolesWithAccounts()
        {
            return $"{_pathController}/GetRolesWithAccounts";
        }

        /// <summary>
        /// Kiểm tra account có role không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string CheckAccountHasRole(long accountId, long roleId)
        {
            return $"{_pathController}/CheckAccountHasRole?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public string CheckAccountHasPermission(long accountId, string permission)
        {
            return $"{_pathController}/CheckAccountHasPermission?accountId={accountId}&permission={permission}";
        }

        /// <summary>
        /// Lấy tất cả permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public string GetAccountPermissions(long accountId)
        {
            return $"{_pathController}/GetAccountPermissions?accountId={accountId}";
        }

        /// <summary>
        /// Đồng bộ roles của account
        /// </summary>
        /// <returns></returns>
        public string SyncAccountRoles()
        {
            return $"{_pathController}/SyncAccountRoles";
        }

        /// <summary>
        /// Gán quyền cho role
        /// </summary>
        /// <returns></returns>
        public string AssignRolePermissions()
        {
            return $"{_pathController}/AssignRolePermissions";
        }
    }
}
