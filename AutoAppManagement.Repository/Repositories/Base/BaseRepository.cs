using AutoAppManagement.Models.DTO;
using AutoAppManagement.Repository.Common.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;

namespace AutoAppManagement.Repository.Repositories.Base
{
    public interface IBaseRepository<T>
    {
        Task<PagingResultDTO<T>> GetPaging(int pageSize = 0, int pageIndex = 0, Expression<Func<T, bool>> predicateFilter = null, List<SortedPaging> sortList = null);
        Task<IEnumerable<T>> GetDataLimit(int to = 0, int from = 0, List<SortedPaging> sortList = null, Expression<Func<T, bool>> predicateFilter = null);
        Task<IEnumerable<T>> GetAll();
        IEnumerable<T> Get();
        Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<bool> CheckExitsByCondition(Expression<Func<T, bool>> predicate);
        void Create(T entity);
        Task CreateAsync(T entity);
        void CreateRange(List<T> entity);
        Task CreateRangeAsync(List<T> entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);
        void DeleteRangeByCondition(Expression<Func<T, bool>> predicate);
        Task BulkInsert(IEnumerable<T> listEntity);
        IEnumerable<N> ExecuteStoredProcedureObject<N>(string nameProcedure, SqlParameter[] array = null) where N : class;
        (List<T1>, List<T2>, List<T3>) ExecuteStoredProcedureMultiObject<T1, T2, T3>(string nameProcedure, DynamicParameters array);
        Task<IEnumerable<N>> GetDataBySorted<N>(IEnumerable<N> data, List<SortedPaging> sortList = null) where N : class;
    }
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbset;
        readonly AutoAppManagementContext _context;
        protected BaseRepository(AutoAppManagementContext context)
        {
            _dbset = context.Set<T>();
            _context = context;
        }

        #region Paging
        /// <summary>
        /// Hàm xử lý lấy dữ liệu paging - chỉ lấy trong model (lưu ý chỉ làm với dữ liệu nhỏ)
        /// TODO: cần xử lý lại cái này, không dùng chung type T => như thế sẽ bị lấy all field
        /// CreaetedBy ntthe 29.02.2024
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="predicateFilter"></param>
        /// <param name="sortList"></param>
        /// <returns></returns>
        public async Task<PagingResultDTO<T>> GetPaging(int pageSize = 0, int pageIndex = 0, Expression<Func<T, bool>> predicateFilter = null, List<SortedPaging> sortList = null)
        {
            // phân 2 task => , 1 task lấy data

            // task đếm tổng bản ghi
            var taskCalTotalRecord = Task.Run(async () =>
            {
                return await _dbset.CountAsync();
            });

            // 1 task lấy data
            var taskGetDataRecord = Task.Run(async () =>
            {
                var data = await GetDataByCondition(predicateFilter);
                data = await GetDataBySorted(data, sortList);

                // phân trang
                if (data != null && data.Count() > 0)
                {
                    if (pageIndex > 0 && pageSize > 0)
                    {
                        // tính số bản ghi bỏ qua
                        var numerRecoredSkip = pageIndex * pageSize;
                        // nếu có cả bộ 2 tham số thì thực hiện phân trang
                        data = data.Skip(numerRecoredSkip).Take(pageSize).ToList();
                    }
                    else if (pageSize > 0)
                    {
                        // nếu chỉ có pageSize thì thực hiện lấy top
                        data = data.Take(pageSize).ToList();
                    }
                }
                return data;
            });

            await Task.WhenAll(taskCalTotalRecord, taskGetDataRecord);

            var pagingResult = new PagingResultDTO<T>
            {
                Data = taskGetDataRecord?.Result,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = taskCalTotalRecord.Result
            };

            return pagingResult;
        }

        /// <summary>
        /// Hàm lấy dữ liệu bản theo số ghi cao nhất thấp nhất theo điều kiện lọc theo sort
        /// CreatedBy ntthe 29.02.2024
        /// </summary>
        /// <param name="limitValue"></param>
        /// <param name="predicateFilter"></param>
        /// <param name="sortList"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetDataLimit(int to = 0, int from = 0, List<SortedPaging> sortList = null, Expression<Func<T, bool>> predicateFilter = null)
        {
            if (to > 0)
            {
                var data = await GetDataByCondition(predicateFilter);
                data = await GetDataBySorted(data, sortList);
                return await Task.Run(() => data?.Skip(from).Take(to).AsEnumerable());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Hàm lấy dữ liệu lọc
        /// CreatedBy ntthe 29.02.2024
        /// </summary>
        /// <param name="predicateFilter"></param>
        /// <returns></returns>
        private async Task<IEnumerable<T>> GetDataByCondition(Expression<Func<T, bool>> predicateFilter = null)
        {
            IEnumerable<T> data = null;
            if (predicateFilter != null)
            {
                data = _dbset.Where(predicateFilter);
            }
            else
            {
                data = await GetAll();
            }
            return data;
        }


        /// <summary>
        /// Hàm lấy dữ liệu theo sort
        /// CreatedBy ntthe 29.02.2024
        /// </summary>
        /// <param name="predicateFilter"></param>
        /// <param name="sortList"></param>
        /// <returns></returns>
        public async Task<IEnumerable<N>> GetDataBySorted<N>(IEnumerable<N> data, List<SortedPaging> sortList = null) where N : class
        {
            // sort lại custom
            if (data != null && data.Count() > 0 && sortList != null && sortList.Count > 0)
            {
                IOrderedEnumerable<N> orderedData = null;
                foreach (var item in sortList)
                {
                    // lần đầu thì order thường, sau đó thì then
                    if (orderedData == null)
                    {
                        if (item.IsAsc)
                        {
                            orderedData = data.OrderBy(s => s?.GetType()?.GetProperty(item.Field)?.GetValue(s, null));
                        }
                        else
                        {
                            orderedData = data.OrderByDescending(s => s?.GetType()?.GetProperty(item.Field)?.GetValue(s, null));
                        }
                    }
                    else
                    {
                        if (item.IsAsc)
                        {
                            orderedData = orderedData.ThenBy(s => s?.GetType()?.GetProperty(item.Field)?.GetValue(s, null));
                        }
                        else
                        {
                            orderedData = orderedData.ThenByDescending(s => s?.GetType()?.GetProperty(item.Field)?.GetValue(s, null));
                        }
                    }
                };

                // thực hiện dùng orderedData thì nó sẽ giữ được dạng OrderBy().ThenBy().ThenBy()
                data = orderedData?.ToList() ?? data.ToList();
            }

            return await Task.Run(() => data);
        }

        #endregion

        public async Task<IEnumerable<T>> GetAll()
        {
            return await Task.Run(() => _dbset.AsEnumerable());
        }

        public IEnumerable<T> Get()
        {
            return _dbset.ToList();
        }

        public async Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _dbset.Where(predicate).AsEnumerable());
        }

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _dbset.Where(predicate).AsEnumerable().FirstOrDefault());
        }

        public async Task<bool> CheckExitsByCondition(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _dbset.Any(predicate));
        }

        public void Create(T entity)
        {
            _dbset.Add(entity);
        }
        public async Task CreateAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }
        public void CreateRange(List<T> entity)
        {
            _dbset.AddRange(entity);
        }
        public async Task CreateRangeAsync(List<T> entity)
        {
            await _dbset.AddRangeAsync(entity);
        }
        public void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _dbset.RemoveRange(entity);
        }

        public void DeleteRangeByCondition(Expression<Func<T, bool>> predicate)
        {
            _dbset.RemoveRange(_dbset.Where(predicate).AsEnumerable());
        }


        /// <summary>
        /// Insert 1 list object
        /// TODO:ntthe đánh giá lại có vẻ cái này phải nằm ở Unitofwork
        /// </summary>
        /// <param name="listEntity">List object</param>
        /// <returns>Trả về 1: Thành công - 0: Thất bại</returns>
        public async Task BulkInsert(IEnumerable<T> listEntity)
        {
            await _context.BulkInsertAsync(listEntity);
        }

        public IEnumerable<TN> ExecuteStoredProcedureObject<TN>(string nameProcedure, SqlParameter[] array = null) where TN : class
        {
            try
            {
                //Duyệt array sqlparameter để lấy tên tạo câu query
                var sb = new StringBuilder();
                sb.Append("exec ").Append(nameProcedure);
                if (array != null)
                {
                    for (var i = 0; i < array.Length; i++)
                    {
                        if (i != 0)
                        {
                            sb.Append(",").Append(array[i].ParameterName);
                        }
                        else
                        {
                            sb.Append(" ").Append(array[i].ParameterName);
                        }
                    }
                }

                var sqlRaw = sb.ToString();

                //execute StoredProcedure 
                //query là câu lệnh query, 
                //array là mảng tham số truyền vào theo kiểu dữ liệu SqlParameter
                _context.Database.SetCommandTimeout(1800);
                if (array == null)
                {
                    array = Array.Empty<SqlParameter>();
                }
                var obj = _context.Set<TN>().FromSqlRaw(sqlRaw, array).ToList();
                return obj;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //TODO: ntthe nghiên cứu tối ưu lại k chơi cứng thế này
        public (List<T1>, List<T2>, List<T3>) ExecuteStoredProcedureMultiObject<T1, T2, T3>(string nameProcedure, DynamicParameters array)
        {
            using var connection = _context.Database.GetDbConnection();
            connection.Open();
            using (var results = connection.QueryMultiple(nameProcedure, array, commandType: CommandType.StoredProcedure))
            {
                var list1 = results.Read<T1>().ToList();
                var list2 = results.Read<T2>().ToList();
                var list3 = results.Read<T3>().ToList();
                return (list1, list2, list3);
            };
        }
    }
}
