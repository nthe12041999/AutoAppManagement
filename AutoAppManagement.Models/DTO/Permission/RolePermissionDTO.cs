using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.BaseEntity;

namespace AutoAppManagement.Models.DTO.Permission
{
    public class RolePermissionDTO : RolePermission, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    public class AssignPermissionToRoleRequest
    {
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
        public string ScopeDefault { get; set; } = "own";
        public int Priority { get; set; } = 0;
        public string? Constraints { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Notes { get; set; }
    }
    public class BulkAssignPermissionsRequest
    {
        public long RoleId { get; set; }
        public List<long> PermissionIds { get; set; } = new();
        public string DefaultScope { get; set; } = "own";
        public int DefaultPriority { get; set; } = 0;
        public string? Notes { get; set; }
    }

    public class PermissionCheckRequest
    {
        public long AccountId { get; set; }
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string RequiredScope { get; set; } = "own";
        public long? TargetAccountId { get; set; }
        public long? TargetOrganizationId { get; set; }
    }
}
