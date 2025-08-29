using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class RoleApiUrlDef : BaseApiUrlDef
    {
        public RoleApiUrlDef() : base("/api/Role") { }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <returns></returns>
        public static string AssignRoleToAccount()
        {
            return @$"{pathController}/AssignRoleToAccount";
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string RemoveRoleFromAccount(long accountId, long roleId)
        {
            return @$"{pathController}/RemoveRoleFromAccount?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Kiểm tra role có tồn tại không
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static string CheckRoleExists(string roleName)
        {
            return @$"{pathController}/CheckRoleExists?roleName={roleName}";
        }
    }
}
