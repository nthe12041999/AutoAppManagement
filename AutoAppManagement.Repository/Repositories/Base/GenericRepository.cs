using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoAppManagement.Repository.Repositories.Base
{
    public interface IGenericRepository<TEntity> where TEntity : BaseCUEntity
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task Insert(TEntity entity);
        Task Insert(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> Any(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetByCondition(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountByCondition(Expression<Func<TEntity, bool>> predicate);
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseCUEntity
    {
        protected readonly AutoAppManagementContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public GenericRepository(AutoAppManagementContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll() => await DbSet.ToListAsync();

        public virtual async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate) => await DbSet.FirstOrDefaultAsync(predicate);

        public virtual async Task Insert(TEntity entity) => await DbSet.AddAsync(entity);

        public virtual async Task Insert(IEnumerable<TEntity> entities) => await DbSet.AddRangeAsync(entities);

        public virtual void Update(TEntity entity) => DbSet.Update(entity);

        public virtual void Delete(TEntity entity) => DbSet.Remove(entity);

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> predicate) => await DbSet.AnyAsync(predicate);

        public virtual async Task<IEnumerable<TEntity>> GetByCondition(Expression<Func<TEntity, bool>> predicate) => await DbSet.Where(predicate).ToListAsync();

        public virtual async Task<int> CountByCondition(Expression<Func<TEntity, bool>> predicate) => await DbSet.CountAsync(predicate);
    }
}
