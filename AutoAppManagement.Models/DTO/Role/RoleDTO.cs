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
}
