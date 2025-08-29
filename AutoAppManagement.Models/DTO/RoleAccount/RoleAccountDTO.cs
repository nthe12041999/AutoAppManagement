using System.ComponentModel.DataAnnotations;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.Enums;

namespace AutoAppManagement.Models.DTO.RoleAccount
{
using AutoAppManagement.Models.Common;

    public class RoleAccountDTO : IStatefulDTO
    {
        public EntityState State { get; set; }
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long AccountId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        public RoleDTO? Role { get; set; }
        public AccountDTO? Account { get; set; }
    }

    public class AssignRoleToAccountRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Role ID không được để trống")]
        public long RoleId { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateRoleAccountRequest
    {
        [Required(ErrorMessage = "ID không được để trống")]
        public long Id { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }

    public class BulkAssignRolesRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Danh sách Role ID không được để trống")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 Role ID")]
        public List<long> RoleIds { get; set; } = new List<long>();

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }

    public class BulkRemoveRolesRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Danh sách Role ID không được để trống")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 Role ID")]
        public List<long> RoleIds { get; set; } = new List<long>();
    }

    public class SyncAccountRolesRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Danh sách Role ID không được để trống")]
        public List<long> RoleIds { get; set; } = new List<long>();
    }

    public class AccountWithRolesDTO
    {
        public AccountDTO Account { get; set; } = new AccountDTO();
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
    }

    public class RoleWithAccountsDTO
    {
        public RoleDTO Role { get; set; } = new RoleDTO();
        public List<AccountDTO> Accounts { get; set; } = new List<AccountDTO>();
    }

    public class PermissionCheckRequest
    {
        [Required(ErrorMessage = "Account ID không được để trống")]
        public long AccountId { get; set; }

        [Required(ErrorMessage = "Permission không được để trống")]
        [StringLength(100, ErrorMessage = "Permission không được vượt quá 100 ký tự")]
        public string Permission { get; set; } = string.Empty;
    }

    public class AccountPermissionsDTO
    {
        public long AccountId { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
    }
}
