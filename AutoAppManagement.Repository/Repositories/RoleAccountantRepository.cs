using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IRoleAccountRepository : IBaseRepository<RoleAccount>
    {
    }

    public class RoleAccountRepository : BaseRepository<RoleAccount>, IRoleAccountRepository
    {
        public RoleAccountRepository(AutoAppManagementContext context) : base(context)
        {
        }
    }
}
