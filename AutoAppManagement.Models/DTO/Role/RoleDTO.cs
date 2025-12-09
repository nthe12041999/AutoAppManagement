using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Role
{
using AutoAppManagement.Models.Common;

    public class RoleDTO : BaseEntity.Role, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    public class AssignRoleRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Role ID không được để trống")]
        public long RoleId { get; set; }
    }

    /// <summary>
    /// Request gán Permission cho Role
    /// </summary>
    public class AssignPermissionsRequest
    {
        [Required(ErrorMessage = "RoleId không được để trống")]
        public long RoleId { get; set; }

        [Required(ErrorMessage = "PermissionIds không được để trống")]
        public List<long> PermissionIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// Request xóa Permission khỏi Role
    /// </summary>
    public class RemovePermissionRequest
    {
        [Required(ErrorMessage = "RoleId không được để trống")]
        public long RoleId { get; set; }

        [Required(ErrorMessage = "PermissionId không được để trống")]
        public long PermissionId { get; set; }
    }
}
