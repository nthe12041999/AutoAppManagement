using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
    }

    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AutoAppManagementContext context) : base(context)
        {
        }
    }
}
