namespace AutoAppManagement.WebApp.Services.ApiUrldefinition
{
    public class RolePermissionApiUrlDef
    {
        private const string pathController = "/api/RolePermission";

        #region Role APIs
        /// <summary>
        /// Lấy danh sách vai trò
        /// </summary>
        /// <returns></returns>
        public static string GetRoles()
        {
            return $"{pathController}/roles";
        }

        /// <summary>
        /// Lấy vai trò theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetRoleById(long id)
        {
            return $"{pathController}/roles/{id}";
        }

        /// <summary>
        /// Tạo vai trò mới
        /// </summary>
        /// <returns></returns>
        public static string CreateRole()
        {
            return $"{pathController}/roles";
        }

        /// <summary>
        /// Cập nhật vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string UpdateRole(long id)
        {
            return $"{pathController}/roles/{id}";
        }

        /// <summary>
        /// Xóa vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeleteRole(long id)
        {
            return $"{pathController}/roles/{id}";
        }

        /// <summary>
        /// Lấy quyền hạn của vai trò
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GetRolePermissions(long roleId)
        {
            return $"{pathController}/roles/{roleId}/permissions";
        }

        /// <summary>
        /// Gán quyền hạn cho vai trò
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string AssignRolePermissions(long roleId)
        {
            return $"{pathController}/roles/{roleId}/permissions";
        }
        #endregion

        #region Permission APIs
        /// <summary>
        /// Lấy danh sách quyền hạn
        /// </summary>
        /// <returns></returns>
        public static string GetPermissions()
        {
            return $"{pathController}/permissions";
        }

        /// <summary>
        /// Lấy quyền hạn theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetPermissionById(long id)
        {
            return $"{pathController}/permissions/{id}";
        }

        /// <summary>
        /// Tạo quyền hạn mới
        /// </summary>
        /// <returns></returns>
        public static string CreatePermission()
        {
            return $"{pathController}/permissions";
        }

        /// <summary>
        /// Cập nhật quyền hạn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string UpdatePermission(long id)
        {
            return $"{pathController}/permissions/{id}";
        }

        /// <summary>
        /// Xóa quyền hạn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeletePermission(long id)
        {
            return $"{pathController}/permissions/{id}";
        }

        /// <summary>
        /// Lấy quyền hạn theo nhóm
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string GetPermissionsByGroup(string group)
        {
            return $"{pathController}/permissions/group/{group}";
        }
        #endregion

        #region User Role Assignment APIs
        /// <summary>
        /// Lấy danh sách phân quyền người dùng
        /// </summary>
        /// <returns></returns>
        public static string GetUserRoleAssignments()
        {
            return $"{pathController}/user-roles";
        }

        /// <summary>
        /// Lấy vai trò của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetUserRoles(long userId)
        {
            return $"{pathController}/users/{userId}/roles";
        }

        /// <summary>
        /// Gán vai trò cho người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string AssignUserRole(long userId)
        {
            return $"{pathController}/users/{userId}/roles";
        }

        /// <summary>
        /// Xóa vai trò của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string RemoveUserRole(long userId, long roleId)
        {
            return $"{pathController}/users/{userId}/roles/{roleId}";
        }

        /// <summary>
        /// Lấy quyền hạn của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetUserPermissions(long userId)
        {
            return $"{pathController}/users/{userId}/permissions";
        }

        /// <summary>
        /// Kiểm tra quyền hạn của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static string CheckUserPermission(long userId, string permission)
        {
            return $"{pathController}/users/{userId}/check-permission?permission={permission}";
        }
        #endregion

        #region Statistics APIs
        /// <summary>
        /// Lấy thống kê vai trò và quyền hạn
        /// </summary>
        /// <returns></returns>
        public static string GetRolePermissionStatistics()
        {
            return $"{pathController}/statistics";
        }

        /// <summary>
        /// Lấy báo cáo phân quyền
        /// </summary>
        /// <returns></returns>
        public static string GetPermissionReport()
        {
            return $"{pathController}/reports/permissions";
        }

        /// <summary>
        /// Xuất báo cáo phân quyền ra Excel
        /// </summary>
        /// <returns></returns>
        public static string ExportPermissionReport()
        {
            return $"{pathController}/reports/permissions/export";
        }
        #endregion

        #region Search APIs
        /// <summary>
        /// Tìm kiếm vai trò
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string SearchRoles(string keyword = "", int pageIndex = 1, int pageSize = 10)
        {
            return $"{pathController}/roles/search?keyword={keyword}&pageIndex={pageIndex}&pageSize={pageSize}";
        }

        /// <summary>
        /// Tìm kiếm quyền hạn
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="group"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string SearchPermissions(string keyword = "", string group = "", int pageIndex = 1, int pageSize = 10)
        {
            return $"{pathController}/permissions/search?keyword={keyword}&group={group}&pageIndex={pageIndex}&pageSize={pageSize}";
        }

        /// <summary>
        /// Tìm kiếm phân quyền người dùng
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string SearchUserRoleAssignments(string keyword = "", string role = "", string status = "", int pageIndex = 1, int pageSize = 10)
        {
            return $"{pathController}/user-roles/search?keyword={keyword}&role={role}&status={status}&pageIndex={pageIndex}&pageSize={pageSize}";
        }
        #endregion
    }
}
