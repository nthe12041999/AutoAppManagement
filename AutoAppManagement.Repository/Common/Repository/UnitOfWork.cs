using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAppManagement.Repository.Common.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        IBaseRepository<TEntity> GetBaseRepository<TEntity>() where TEntity : BaseEntity;
        
        // Dedicated repositories
        IRoleAccountRepository RoleAccountRepository { get; }
        IAccountsRepository AccountsRepository { get; }
        
        // Context access for entity state management
        AutoAppManagementContext Context { get; }
        
        Task<int> SaveAsync();
        DbSet<T> Set<T>() where T : class;
        IEnumerable<T> SqlQuery<T>(string query, SqlParameter[] array = null) where T : class, new();
        DataTable SqlQuery(string query, SqlParameter[] array = null);
        Task<int> SqlCommand(string query, SqlParameter[] array = null);
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AutoAppManagementContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private bool _isDisposed;

        // Lazy-loaded dedicated repositories
        private IRoleAccountRepository? _roleAccountRepository;
        private IAccountsRepository? _accountsRepository;

        public UnitOfWork(AutoAppManagementContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        // Dedicated repository properties
        public IRoleAccountRepository RoleAccountRepository => 
            _roleAccountRepository ??= new RoleAccountRepository(_context);
            
        public IAccountsRepository AccountsRepository => 
            _accountsRepository ??= new AccountsRepository(_context);

        // Context property for entity state management
        public AutoAppManagementContext Context => _context;

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity);
            if (_repositories.ContainsKey(type))
            {
                return (IGenericRepository<TEntity>)_repositories[type];
            }

            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

            _repositories.Add(type, repositoryInstance);
            return (IGenericRepository<TEntity>)repositoryInstance;
        }

        public IBaseRepository<TEntity> GetBaseRepository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity);
            var key = $"base_{type.Name}";
            
            if (_repositories.ContainsKey(type))
            {
                var existing = _repositories[type];
                if (existing is IBaseRepository<TEntity> baseRepo)
                    return baseRepo;
            }

            var repositoryType = typeof(BaseRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
            
            _repositories[type] = repositoryInstance!;
            return (IBaseRepository<TEntity>)repositoryInstance!;
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _context.Set<T>();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _context.Dispose();
        }

        // SqlQuery and SqlCommand methods remain unchanged...
        public IEnumerable<T> SqlQuery<T>(string query, SqlParameter[] array = null) where T : class, new()
        {
            return _context.Set<T>().FromSqlRaw(query, array).ToList();
        }

        public DataTable SqlQuery(string query, SqlParameter[] array = null)
        {
            var dt = new DataTable();
            var conn = _context.Database.GetDbConnection();
            var connectionState = conn.State;
            try
            {
                if (connectionState != ConnectionState.Open) conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    if (array != null) cmd.Parameters.AddRange(array);
                    using (var reader = cmd.ExecuteReader()) {
                        dt.Load(reader);
                    }
                }
            }
            finally
            {
                if (connectionState != ConnectionState.Closed) conn.Close();
            }
            return dt;
        }

        public async Task<int> SqlCommand(string query, SqlParameter[] array = null)
        {
            return await _context.Database.ExecuteSqlRawAsync(query, array);
        }
    }
}