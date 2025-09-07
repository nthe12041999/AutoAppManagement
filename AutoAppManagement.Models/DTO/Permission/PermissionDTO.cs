using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.BaseEntity;

namespace AutoAppManagement.Models.DTO.Permission
{
    public class PermissionDTO : IStatefulDTO
    {
        public long Id { get; set; }
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
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
    }

    public class CreatePermissionRequest
    {
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
    }

    public class UpdatePermissionRequest
    {
        public long Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
    }

    public class PermissionSearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Resource { get; set; }
        public string? Status { get; set; }
    }
}
