using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Models.DTO.RolePermission
{
    public class RolePermissionDTO : IStatefulDTO
    {
        public long ID { get; set; }
        public EntityState State { get; set; }
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
        public int Status { get; set; }
    }
}
