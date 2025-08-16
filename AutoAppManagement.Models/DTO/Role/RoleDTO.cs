using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO.Role
{
    public class RoleDTO
    {
        public long Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
    }

    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string RoleDescription { get; set; } = string.Empty;
    }

    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string RoleDescription { get; set; } = string.Empty;
    }

    public class AssignRoleRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Role ID không được để trống")]
        public long RoleId { get; set; }
    }
}
