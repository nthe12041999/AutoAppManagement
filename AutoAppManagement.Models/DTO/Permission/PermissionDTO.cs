using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Models.DTO.Permission
{
    public class PermissionDTO : BaseEntity.Permission, IStatefulDTO
    {
        public EntityState State { get; set; }
    }
}
