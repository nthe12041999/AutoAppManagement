using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Data.Models;
using AutoAppManagement.Repository.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAppManagement.Repository.Repositories
{
    public interface IAIConfigRepository : IBaseRepository<AIConfig>
    {
        Task<IEnumerable<AIConfig>> GetByUserId(long accountId);
    }
    public class AIConfigRepository : BaseRepository<AIConfig>, IAIConfigRepository
    {
        public AIConfigRepository(AutoAppManagementContext context) : base(context)
        {

        }
        public async Task<IEnumerable<AIConfig>> GetByUserId(long accountId)
        {
            return await FindBy(d => d.AccountId == accountId);
        }
    }
}
