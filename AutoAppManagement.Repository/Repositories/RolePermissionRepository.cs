using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IRolePermissionRepository : IBaseRepository<RolePermission>
    {
    }

    public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(AutoAppManagementContext context) : base(context)
        {
        }
    }
}
