using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.BaseEntity;

namespace AutoAppManagement.Models.DTO.Permission
{
    public class RolePermissionDTO : IStatefulDTO
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
        public string ScopeDefault { get; set; } = "own";
        public string? Constraints { get; set; }
        public int Priority { get; set; }
        public bool IsInherited { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Active";
        public EntityState State { get; set; }

        // Navigation properties
        public PermissionDTO? Permission { get; set; }
        public string? RoleName { get; set; }
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

    public class UpdateRolePermissionRequest
    {
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
        public string? ScopeDefault { get; set; }
        public int? Priority { get; set; }
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

    public class SyncRolePermissionsRequest
    {
        public long RoleId { get; set; }
        public List<RolePermissionSyncItem> Permissions { get; set; } = new();
    }

    public class RolePermissionSyncItem
    {
        public long PermissionId { get; set; }
        public string ScopeDefault { get; set; } = "own";
        public int Priority { get; set; } = 0;
        public string? Constraints { get; set; }
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
