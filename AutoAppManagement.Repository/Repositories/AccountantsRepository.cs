using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Common.Ulti;
using AutoAppManagement.Repository.Repositories.Base;
using System.Data.SqlClient;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IAccountsRepository : IBaseRepository<Account>
    {
        Task<Account> GetUserByUserNameAndPass(string userName, string password);

        IEnumerable<Role> GetListRoleByAccId(long accId);

    }
    public class AccountsRepository : BaseRepository<Account>, IAccountsRepository
    {
        public AccountsRepository(AutoAppManagementContext context) : base(context)
        {

        }

        public async Task<Account> GetUserByUserNameAndPass(string userName, string password)
        {
            var passwordEncode = HashCodeUlti.EncodePassword(password);
            var user = await FindBy(a => a.UserName == userName && a.Password == passwordEncode);
            return user.FirstOrDefault();
        }

        public IEnumerable<Role> GetListRoleByAccId(long accId)
        {
            var param = new SqlParameter[]
            {
                new SqlParameter("@AccID", accId)
            };
            var roleList = ExecuteStoredProcedureObject<Role>("GetRoleByAccId", param);
            return roleList;
        }
    }
}
