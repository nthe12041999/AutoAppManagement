namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public static class PermissionApiUrlDef
    {
        private const string BaseUrl = "/api/Permission";

        /// <summary>
        /// Lấy tất cả role accounts
        /// </summary>
        /// <returns></returns>
        public static string GetAllRoleAccounts()
        {
            return $"{BaseUrl}/GetAllRoleAccounts";
        }

        /// <summary>
        /// Lấy role accounts theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetRoleAccountsByAccountId(long accountId)
        {
            return $"{BaseUrl}/GetRoleAccountsByAccountId?accountId={accountId}";
        }

        /// <summary>
        /// Lấy role accounts theo role ID
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GetRoleAccountsByRoleId(long roleId)
        {
            return $"{BaseUrl}/GetRoleAccountsByRoleId?roleId={roleId}";
        }

        /// <summary>
        /// Lấy role account theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetRoleAccountById(long id)
        {
            return $"{BaseUrl}/GetRoleAccountById?id={id}";
        }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <returns></returns>
        public static string AssignRoleToAccount()
        {
            return $"{BaseUrl}/AssignRoleToAccount";
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string RemoveRoleFromAccount(long accountId, long roleId)
        {
            return $"{BaseUrl}/RemoveRoleFromAccount?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Cập nhật role account
        /// </summary>
        /// <returns></returns>
        public static string UpdateRoleAccount()
        {
            return $"{BaseUrl}/UpdateRoleAccount";
        }

        /// <summary>
        /// Gán nhiều role cho account
        /// </summary>
        /// <returns></returns>
        public static string BulkAssignRoles()
        {
            return $"{BaseUrl}/BulkAssignRoles";
        }

        /// <summary>
        /// Gỡ nhiều role khỏi account
        /// </summary>
        /// <returns></returns>
        public static string BulkRemoveRoles()
        {
            return $"{BaseUrl}/BulkRemoveRoles";
        }

        /// <summary>
        /// Lấy accounts với roles
        /// </summary>
        /// <returns></returns>
        public static string GetAccountsWithRoles()
        {
            return $"{BaseUrl}/GetAccountsWithRoles";
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        public static string GetRolesWithAccounts()
        {
            return $"{BaseUrl}/GetRolesWithAccounts";
        }

        /// <summary>
        /// Kiểm tra account có role không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string CheckAccountHasRole(long accountId, long roleId)
        {
            return $"{BaseUrl}/CheckAccountHasRole?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static string CheckAccountHasPermission(long accountId, string permission)
        {
            return $"{BaseUrl}/CheckAccountHasPermission?accountId={accountId}&permission={permission}";
        }

        /// <summary>
        /// Lấy tất cả permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetAccountPermissions(long accountId)
        {
            return $"{BaseUrl}/GetAccountPermissions?accountId={accountId}";
        }

        /// <summary>
        /// Đồng bộ roles của account
        /// </summary>
        /// <returns></returns>
        public static string SyncAccountRoles()
        {
            return $"{BaseUrl}/SyncAccountRoles";
        }
    }
}
