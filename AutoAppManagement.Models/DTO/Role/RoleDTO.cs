using AutoAppManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Role
{
using AutoAppManagement.Models.Common;

    public class RoleDTO : IStatefulDTO
    {
        public EntityState State { get; set; }
        public long Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
    }

    public class AssignRoleRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Role ID không được để trống")]
        public long RoleId { get; set; }
    }

    public class CreateRoleRequest
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }

    public class UpdateRoleRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }
}
