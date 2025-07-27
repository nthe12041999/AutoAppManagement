using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
    }

    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(AutoAppManagementContext context) : base(context)
        {
        }
    }
}
