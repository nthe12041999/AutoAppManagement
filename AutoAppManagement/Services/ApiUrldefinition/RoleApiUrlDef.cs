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
        public string AssignRoleToAccount()
        {
            return @$"{_pathController}/AssignRoleToAccount";
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string RemoveRoleFromAccount(long accountId, long roleId)
        {
            return @$"{_pathController}/RemoveRoleFromAccount?accountId={accountId}&roleId={roleId}";
        }

        /// <summary>
        /// Kiểm tra role có tồn tại không
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public string CheckRoleExists(string roleName)
        {
            return @$"{_pathController}/CheckRoleExists?roleName={roleName}";
        }
    }
}
