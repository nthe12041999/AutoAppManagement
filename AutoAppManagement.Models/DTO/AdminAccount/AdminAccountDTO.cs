using System.ComponentModel.DataAnnotations;
using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Models.DTO.AdminAccount
{
    /// <summary>
    /// DTO thông tin tài khoản admin
    /// </summary>
    public class AdminAccountDTO : BaseEntity.AdminAccount, IStatefulDTO
    {
        public EntityState State { get; set; }
        
        // Danh sách Roles (dynamic, không lưu vào DB)
        public dynamic? Roles { get; set; }
        
        // Danh sách RoleIds để submit
        public List<long>? RoleIds { get; set; }
    }

    public class LockAccountRequest
    {
        public long Id { get; set; }
        public int Minutes { get; set; } = 30;
        public string Reason { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
