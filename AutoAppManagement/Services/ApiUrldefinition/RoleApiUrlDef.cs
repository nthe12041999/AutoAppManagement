namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class RoleApiUrlDef
    {
        private const string pathController = "/api/Role";

        /// <summary>
        /// Lấy tất cả roles
        /// </summary>
        /// <returns></returns>
        public static string GetAllRoles()
        {
            return @$"{pathController}/GetAllRoles";
        }

        /// <summary>
        /// Lấy role theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetRoleById(long id)
        {
            return @$"{pathController}/GetRoleById?id={id}";
        }

        /// <summary>
        /// Tạo role mới
        /// </summary>
        /// <returns></returns>
        public static string CreateRole()
        {
            return @$"{pathController}/CreateRole";
        }

        /// <summary>
        /// Cập nhật role
        /// </summary>
        /// <returns></returns>
        public static string UpdateRole()
        {
            return @$"{pathController}/UpdateRole";
        }

        /// <summary>
        /// Xóa role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeleteRole(long id)
        {
            return @$"{pathController}/DeleteRole?id={id}";
        }

        /// <summary>
        /// Lấy roles của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetRolesByAccountId(long accountId)
        {
            return @$"{pathController}/GetRolesByAccountId?accountId={accountId}";
        }

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
