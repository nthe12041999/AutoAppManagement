using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Services.ApiUrldefinition;
using AutoAppManagement.WebApp.Services.Base;

namespace AutoAppManagement.WebApp.Services
{
    public interface IRolePermissionService
    {
        // Role methods
        Task<List<RoleViewModel>> GetRolesAsync();
        Task<RoleViewModel> GetRoleByIdAsync(long id);
        Task<ResponseOutput<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model);
        Task<ResponseOutput<RoleViewModel>> UpdateRoleAsync(long id, UpdateRoleViewModel model);
        Task<ResponseOutput<bool>> DeleteRoleAsync(long id);
        Task<List<PermissionViewModel>> GetRolePermissionsAsync(long roleId);
        Task<ResponseOutput<bool>> AssignRolePermissionsAsync(
            long roleId,
            List<long> permissionIds
        );

        // Permission methods
        Task<List<PermissionViewModel>> GetPermissionsAsync();
        Task<PermissionViewModel> GetPermissionByIdAsync(long id);
        Task<ResponseOutput<PermissionViewModel>> CreatePermissionAsync(
            CreatePermissionViewModel model
        );
        Task<ResponseOutput<PermissionViewModel>> UpdatePermissionAsync(
            long id,
            UpdatePermissionViewModel model
        );
        Task<ResponseOutput<bool>> DeletePermissionAsync(long id);
        Task<List<PermissionViewModel>> GetPermissionsByGroupAsync(string group);

        // User role assignment methods
        Task<List<UserRoleAssignmentViewModel>> GetUserRoleAssignmentsAsync();
        Task<List<RoleViewModel>> GetUserRolesAsync(long userId);
        Task<ResponseOutput<bool>> AssignUserRoleAsync(long userId, long roleId);
        Task<ResponseOutput<bool>> RemoveUserRoleAsync(long userId, long roleId);
        Task<List<PermissionViewModel>> GetUserPermissionsAsync(long userId);
        Task<bool> CheckUserPermissionAsync(long userId, string permission);

        // Statistics and reports
        Task<RolePermissionStatisticsViewModel> GetRolePermissionStatisticsAsync();
        Task<List<PermissionReportViewModel>> GetPermissionReportAsync();
        Task<byte[]> ExportPermissionReportAsync();

        // Search methods
        Task<PagedResult<RoleViewModel>> SearchRolesAsync(
            string keyword = "",
            int pageIndex = 1,
            int pageSize = 10
        );
        Task<PagedResult<PermissionViewModel>> SearchPermissionsAsync(
            string keyword = "",
            string group = "",
            int pageIndex = 1,
            int pageSize = 10
        );
        Task<PagedResult<UserRoleAssignmentViewModel>> SearchUserRoleAssignmentsAsync(
            string keyword = "",
            string role = "",
            string status = "",
            int pageIndex = 1,
            int pageSize = 10
        );
    }

    public class RolePermissionService : BaseService, IRolePermissionService
    {
        public RolePermissionService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor
        )
            : base(httpClientFactory, config, httpContextAccessor) { }

        #region Role Methods
        public async Task<List<RoleViewModel>> GetRolesAsync()
        {
            var url = RolePermissionApiUrlDef.GetRoles();
            return await RequestAuthenGetAsync<List<RoleViewModel>>(url)
                ?? new List<RoleViewModel>();
        }

        public async Task<RoleViewModel> GetRoleByIdAsync(long id)
        {
            var url = RolePermissionApiUrlDef.GetRoleById(id);
            return await RequestAuthenGetAsync<RoleViewModel>(url);
        }

        public async Task<ResponseOutput<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model)
        {
            var url = RolePermissionApiUrlDef.CreateRole();
            return await RequestFullAuthenPostAsync<RoleViewModel>(url, model);
        }

        public async Task<ResponseOutput<RoleViewModel>> UpdateRoleAsync(
            long id,
            UpdateRoleViewModel model
        )
        {
            var url = RolePermissionApiUrlDef.UpdateRole(id);
            return await RequestFullAuthenPostAsync<RoleViewModel>(url, model);
        }

        public async Task<ResponseOutput<bool>> DeleteRoleAsync(long id)
        {
            var url = RolePermissionApiUrlDef.DeleteRole(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<List<PermissionViewModel>> GetRolePermissionsAsync(long roleId)
        {
            var url = RolePermissionApiUrlDef.GetRolePermissions(roleId);
            return await RequestAuthenGetAsync<List<PermissionViewModel>>(url)
                ?? new List<PermissionViewModel>();
        }

        public async Task<ResponseOutput<bool>> AssignRolePermissionsAsync(
            long roleId,
            List<long> permissionIds
        )
        {
            var url = RolePermissionApiUrlDef.AssignRolePermissions(roleId);
            return await RequestFullAuthenPostAsync<bool>(
                url,
                new { PermissionIds = permissionIds }
            );
        }
        #endregion

        #region Permission Methods
        public async Task<List<PermissionViewModel>> GetPermissionsAsync()
        {
            var url = RolePermissionApiUrlDef.GetPermissions();
            return await RequestAuthenGetAsync<List<PermissionViewModel>>(url)
                ?? new List<PermissionViewModel>();
        }

        public async Task<PermissionViewModel> GetPermissionByIdAsync(long id)
        {
            var url = RolePermissionApiUrlDef.GetPermissionById(id);
            return await RequestAuthenGetAsync<PermissionViewModel>(url);
        }

        public async Task<ResponseOutput<PermissionViewModel>> CreatePermissionAsync(
            CreatePermissionViewModel model
        )
        {
            var url = RolePermissionApiUrlDef.CreatePermission();
            return await RequestFullAuthenPostAsync<PermissionViewModel>(url, model);
        }

        public async Task<ResponseOutput<PermissionViewModel>> UpdatePermissionAsync(
            long id,
            UpdatePermissionViewModel model
        )
        {
            var url = RolePermissionApiUrlDef.UpdatePermission(id);
            return await RequestFullAuthenPostAsync<PermissionViewModel>(url, model);
        }

        public async Task<ResponseOutput<bool>> DeletePermissionAsync(long id)
        {
            var url = RolePermissionApiUrlDef.DeletePermission(id);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<List<PermissionViewModel>> GetPermissionsByGroupAsync(string group)
        {
            var url = RolePermissionApiUrlDef.GetPermissionsByGroup(group);
            return await RequestAuthenGetAsync<List<PermissionViewModel>>(url)
                ?? new List<PermissionViewModel>();
        }
        #endregion

        #region User Role Assignment Methods
        public async Task<List<UserRoleAssignmentViewModel>> GetUserRoleAssignmentsAsync()
        {
            var url = RolePermissionApiUrlDef.GetUserRoleAssignments();
            return await RequestAuthenGetAsync<List<UserRoleAssignmentViewModel>>(url)
                ?? new List<UserRoleAssignmentViewModel>();
        }

        public async Task<List<RoleViewModel>> GetUserRolesAsync(long userId)
        {
            var url = RolePermissionApiUrlDef.GetUserRoles(userId);
            return await RequestAuthenGetAsync<List<RoleViewModel>>(url)
                ?? new List<RoleViewModel>();
        }

        public async Task<ResponseOutput<bool>> AssignUserRoleAsync(long userId, long roleId)
        {
            var url = RolePermissionApiUrlDef.AssignUserRole(userId);
            return await RequestFullAuthenPostAsync<bool>(url, new { RoleId = roleId });
        }

        public async Task<ResponseOutput<bool>> RemoveUserRoleAsync(long userId, long roleId)
        {
            var url = RolePermissionApiUrlDef.RemoveUserRole(userId, roleId);
            return await RequestFullAuthenPostAsync<bool>(url);
        }

        public async Task<List<PermissionViewModel>> GetUserPermissionsAsync(long userId)
        {
            var url = RolePermissionApiUrlDef.GetUserPermissions(userId);
            return await RequestAuthenGetAsync<List<PermissionViewModel>>(url)
                ?? new List<PermissionViewModel>();
        }

        public async Task<bool> CheckUserPermissionAsync(long userId, string permission)
        {
            var url = RolePermissionApiUrlDef.CheckUserPermission(userId, permission);
            var result = await RequestAuthenGetAsync<ResponseOutput<bool>>(url);
            return result?.Data ?? false;
        }
        #endregion

        #region Statistics and Reports
        public async Task<RolePermissionStatisticsViewModel> GetRolePermissionStatisticsAsync()
        {
            var url = RolePermissionApiUrlDef.GetRolePermissionStatistics();
            return await RequestAuthenGetAsync<RolePermissionStatisticsViewModel>(url);
        }

        public async Task<List<PermissionReportViewModel>> GetPermissionReportAsync()
        {
            var url = RolePermissionApiUrlDef.GetPermissionReport();
            return await RequestAuthenGetAsync<List<PermissionReportViewModel>>(url)
                ?? new List<PermissionReportViewModel>();
        }

        public async Task<byte[]> ExportPermissionReportAsync()
        {
            var url = RolePermissionApiUrlDef.ExportPermissionReport();
            return await RequestAuthenGetFile(url);
        }
        #endregion

        #region Search Methods
        public async Task<PagedResult<RoleViewModel>> SearchRolesAsync(
            string keyword = "",
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            var url = RolePermissionApiUrlDef.SearchRoles(keyword, pageIndex, pageSize);
            return await RequestAuthenGetAsync<PagedResult<RoleViewModel>>(url)
                ?? new PagedResult<RoleViewModel>();
        }

        public async Task<PagedResult<PermissionViewModel>> SearchPermissionsAsync(
            string keyword = "",
            string group = "",
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            var url = RolePermissionApiUrlDef.SearchPermissions(
                keyword,
                group,
                pageIndex,
                pageSize
            );
            return await RequestAuthenGetAsync<PagedResult<PermissionViewModel>>(url)
                ?? new PagedResult<PermissionViewModel>();
        }

        public async Task<PagedResult<UserRoleAssignmentViewModel>> SearchUserRoleAssignmentsAsync(
            string keyword = "",
            string role = "",
            string status = "",
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            var url = RolePermissionApiUrlDef.SearchUserRoleAssignments(
                keyword,
                role,
                status,
                pageIndex,
                pageSize
            );
            return await RequestAuthenGetAsync<PagedResult<UserRoleAssignmentViewModel>>(url)
                ?? new PagedResult<UserRoleAssignmentViewModel>();
        }
        #endregion
    }

    #region ViewModels
    public class RoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int UserCount { get; set; }
        public List<PermissionViewModel> Permissions { get; set; } =
            new List<PermissionViewModel>();
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class CreateRoleViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<long> PermissionIds { get; set; } = new List<long>();
    }

    public class UpdateRoleViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<long> PermissionIds { get; set; } = new List<long>();
    }

    public class PermissionViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CreatePermissionViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
    }

    public class UpdatePermissionViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
    }

    public class UserRoleAssignmentViewModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public string Status { get; set; }
        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; }
    }

    public class RolePermissionStatisticsViewModel
    {
        public int TotalRoles { get; set; }
        public int ActiveRoles { get; set; }
        public int TotalPermissions { get; set; }
        public int ActivePermissions { get; set; }
        public int UsersWithRoles { get; set; }
        public Dictionary<string, int> PermissionsByGroup { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; }
        public List<RoleUsageViewModel> RoleUsage { get; set; }
    }

    public class RoleUsageViewModel
    {
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
    }

    public class PermissionReportViewModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Permissions { get; set; } = new List<string>();
        public string Status { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
    #endregion
}
