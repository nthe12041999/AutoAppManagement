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
        public static string AssignRoleToAccount()
        {
            return $"{pathController}/AssignRoleToAccount";
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string RemoveRoleFromAccount(long accountId, long roleId)
        {
            return $"{pathController}/RemoveRoleFromAccount?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Cập nhật role account
        /// </summary>
        /// <returns></returns>
        public static string UpdateRoleAccount()
        {
            return $"{pathController}/UpdateRoleAccount";
        }

        /// <summary>
        /// Gán nhiều role cho account
        /// </summary>
        /// <returns></returns>
        public static string BulkAssignRoles()
        {
            return $"{pathController}/BulkAssignRoles";
        }

        /// <summary>
        /// Gỡ nhiều role khỏi account
        /// </summary>
        /// <returns></returns>
        public static string BulkRemoveRoles()
        {
            return $"{pathController}/BulkRemoveRoles";
        }

        /// <summary>
        /// Lấy accounts với roles
        /// </summary>
        /// <returns></returns>
        public static string GetAccountsWithRoles()
        {
            return $"{pathController}/GetAccountsWithRoles";
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        public static string GetRolesWithAccounts()
        {
            return $"{pathController}/GetRolesWithAccounts";
        }

        /// <summary>
        /// Kiểm tra account có role không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string CheckAccountHasRole(long accountId, long roleId)
        {
            return $"{pathController}/CheckAccountHasRole?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static string CheckAccountHasPermission(long accountId, string permission)
        {
            return $"{pathController}/CheckAccountHasPermission?accountId={accountId}&permission={permission}";
        }

        /// <summary>
        /// Lấy tất cả permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetAccountPermissions(long accountId)
        {
            return $"{pathController}/GetAccountPermissions?accountId={accountId}";
        }

        /// <summary>
        /// Đồng bộ roles của account
        /// </summary>
        /// <returns></returns>
        public static string SyncAccountRoles()
        {
            return $"{pathController}/SyncAccountRoles";
        }
    }
}
