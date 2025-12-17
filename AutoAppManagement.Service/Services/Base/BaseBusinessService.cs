using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Repository.Repositories.Base;
using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoAppManagement.Service.Services.Base
{
    public interface IBaseBusinessService<TDto>
        where TDto : class, IStatefulDTO
    {
        Task<IEnumerable<TDto>> GetAll();
        Task<TDto> GetById(long id);
        Task<object> GetPaging(PagingRequestDTO pagingRequestDTO);
        Task<BaseResponse> SubmitData(TDto dto);
        Task<BaseResponse> Delete(long id);
    }

    public abstract class BaseBusinessService<TEntity, TDto, TRepository> : BaseService, IBaseBusinessService<TDto>
        where TEntity :BaseCUEntity
        where TDto : class, IStatefulDTO
        where TRepository : class, IBaseRepository<TEntity>
    {
        private TRepository _repository;
        protected TRepository Repository => _repository ??= GetRepository();

        protected BaseBusinessService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected virtual TRepository GetRepository()
        {
            // Get repository from UnitOfWork's GetBaseRepository method
            return (TRepository)UnitOfWork.GetBaseRepository<TEntity>();
        }

        // 🔧 Helper methods to bridge method differences between IGenericRepository and IBaseRepository
        protected async Task Insert(TEntity entity)
        {
            await Repository.CreateAsync(entity);
        }

        protected async Task<TEntity> UpdateById(long id)
        {
            var dataModel = await Repository.FirstOrDefault(a => a.ID == id && a.Status == Models.Enum.StatusEnum.Active);
            dataModel.SetUpdated(GetCurrentUserId());
            return dataModel;
        }

        protected async Task Insert(IEnumerable<TEntity> entities)
        {
            await Repository.CreateRangeAsync(entities.ToList());
        }

        protected void SubmitData(TEntity entity)
        {
            // Phương thức submit data rõ ràng hơn
            // Đánh dấu entity đã được modified để submit vào database
            UnitOfWork.Context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        protected void Delete(TEntity entity)
        {
            Repository.Delete(entity);
        }

        protected async Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return await Repository.CheckExitsByCondition(predicate);
        }

        protected async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return await Repository.FirstOrDefault(predicate);
        }

        protected async Task<IEnumerable<TEntity>> GetByCondition(Expression<Func<TEntity, bool>> predicate)
        {
            return await Repository.GetByCondition(predicate);
        }

        // Same implementation as below but with TRepository
        public virtual async Task<IEnumerable<TDto>> GetAll()
        {
            var entities = await Repository.GetAll();
            return Mapper.Map<List<TDto>>(entities.Where(e => e.Status == Models.Enum.StatusEnum.Active).ToList());
        }

        public virtual async Task<TDto> GetById(long id)
        {
            var entity = await Repository.FirstOrDefault(e => e.ID == id && e.Status == Models.Enum.StatusEnum.Active);
            return entity == null ? default : Mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Lấy tên view từ View enum
        /// Override method này trong service cụ thể để map View enum sang tên view trong database
        /// </summary>
        protected virtual string GetViewName(Models.Enums.EnumView view)
        {
            // Mặc định return tên view từ enum
            return view.ToString();
        }

        /// <summary>
        /// Override để chỉ định các field được phép search cho từng view
        /// </summary>
        protected virtual List<string>? GetSearchFieldsForView(Models.Enums.EnumView view)
        {
            return null;
        }

        /// <summary>
        /// Override để customize base query cho GetPaging
        /// Mặc định chỉ lấy Status = Active, có thể override để lấy thêm các status khác
        /// </summary>
        protected virtual IQueryable<TEntity> GetBaseQuery(IEnumerable<TEntity> entities)
        {
            return entities.Where(e => e.Status == Models.Enum.StatusEnum.Active).AsQueryable();
        }

        /// <summary>
        /// Xây dựng danh sách field SELECT động dựa vào RequestedColumns từ FE
        /// </summary>
        protected virtual string GetSelectFieldsForView(PagingRequestDTO pagingRequestDTO, string defaultFields = "*")
        {
            if (pagingRequestDTO.RequestedColumns == null || !pagingRequestDTO.RequestedColumns.Any())
            {
                return defaultFields;
            }

            var normalizedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var column in pagingRequestDTO.RequestedColumns)
            {
                var trimmed = column?.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    continue;
                }

                var isValid = trimmed.All(c => char.IsLetterOrDigit(c) || c == '_');
                if (!isValid)
                {
                    continue;
                }

                normalizedColumns.Add(trimmed);
            }

            // Always include ID so FE can take actions (edit/delete)
            normalizedColumns.Add("ID");

            if (!normalizedColumns.Any())
            {
                return defaultFields;
            }

            return string.Join(", ", normalizedColumns.Select(c => $"[{c}]"));
        }

        /// <summary>
        /// Build WHERE clause và parameters từ filters cho Dapper query
        /// </summary>
        protected virtual (string whereClause, DynamicParameters parameters) BuildWhereClauseFromFilters(
            PagingRequestDTO pagingRequestDTO, 
            List<string> searchFields = null)
        {
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            // Parse filters nếu chưa parse
            pagingRequestDTO.ParseFilters();

            if (pagingRequestDTO.Filters != null && pagingRequestDTO.Filters.Any())
            {
                var filterConditions = new List<string>();
                int paramIndex = 0;
                
                foreach (var filter in pagingRequestDTO.Filters)
                {
                    if (string.IsNullOrEmpty(filter.field))
                        continue;

                    var paramName = $"@param{paramIndex++}";
                    var fieldName = $"[{filter.field}]";
                    
                    switch (filter.op)
                    {
                        case FilterOperator.Contains:
                            filterConditions.Add($"{fieldName} LIKE {paramName}");
                            parameters.Add(paramName, $"%{filter.value}%");
                            break;
                        case FilterOperator.Equals:
                            filterConditions.Add($"{fieldName} = {paramName}");
                            parameters.Add(paramName, filter.value);
                            break;
                        case FilterOperator.NotEquals:
                            filterConditions.Add($"{fieldName} <> {paramName}");
                            parameters.Add(paramName, filter.value);
                            break;
                        case FilterOperator.StartsWith:
                            filterConditions.Add($"{fieldName} LIKE {paramName}");
                            parameters.Add(paramName, $"{filter.value}%");
                            break;
                        case FilterOperator.EndsWith:
                            filterConditions.Add($"{fieldName} LIKE {paramName}");
                            parameters.Add(paramName, $"%{filter.value}");
                            break;
                        case FilterOperator.GreaterThan:
                        case FilterOperator.GreaterThan_Date:
                            filterConditions.Add($"{fieldName} > {paramName}");
                            parameters.Add(paramName, filter.value);
                            break;
                        case FilterOperator.GreaterThanOrEqual:
                        case FilterOperator.GreaterThanOrEqual_Date:
                            filterConditions.Add($"{fieldName} >= {paramName}");
                            parameters.Add(paramName, filter.value);
                            break;
                        case FilterOperator.LessThan:
                        case FilterOperator.LessThan_Date:
                            filterConditions.Add($"{fieldName} < {paramName}");
                            parameters.Add(paramName, filter.value);
                            break;
                        case FilterOperator.LessThanOrEqual:
                        case FilterOperator.LessThanOrEqual_Date:
                            filterConditions.Add($"{fieldName} <= {paramName}");
                            parameters.Add(paramName, filter.value);
                            break;
                    }
                }
                
                if (filterConditions.Any())
                {
                    whereClause += " AND " + string.Join(" AND ", filterConditions);
                }
            }
            else if (!string.IsNullOrEmpty(pagingRequestDTO.Filter))
            {
                // Simple search - tìm trong các field được chỉ định hoặc tất cả string fields
                if (searchFields != null && searchFields.Any())
                {
                    var searchConditions = searchFields.Select(f => $"[{f}] LIKE @search");
                    whereClause += " AND (" + string.Join(" OR ", searchConditions) + ")";
                }
                else
                {
                    // Default search fields nếu không chỉ định
                    whereClause += " AND ([ID] LIKE @search OR [Name] LIKE @search)";
                }
                parameters.Add("@search", $"%{pagingRequestDTO.Filter}%");
            }

            return (whereClause, parameters);
        }

        /// <summary>
        /// Query từ view bằng Dapper với paging
        /// </summary>
        protected virtual async Task<PagingResultDTO<object>> GetPagingFromView(
            string viewName,
            PagingRequestDTO pagingRequestDTO,
            string selectFields = "*",
            List<string> searchFields = null)
        {
            var connection = UnitOfWork.Context.Database.GetDbConnection();
            
            // Build WHERE clause
            var (whereClause, parameters) = BuildWhereClauseFromFilters(pagingRequestDTO, searchFields);

            // Query tổng số records
            var countSql = $"SELECT COUNT(*) FROM [{viewName}] {whereClause}";
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // Query data với paging
            var sortField = string.IsNullOrEmpty(pagingRequestDTO.Sort) ? "ID" : pagingRequestDTO.Sort;
            var offset = (pagingRequestDTO.PageIndex - 1) * pagingRequestDTO.PageSize;
            
            var selectClause = string.IsNullOrWhiteSpace(selectFields) ? "*" : selectFields;

            var dataSql = $@"
                SELECT {selectClause}
                FROM [{viewName}]
                {whereClause}
                ORDER BY [{sortField}]
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            parameters.Add("@offset", offset);
            parameters.Add("@pageSize", pagingRequestDTO.PageSize);

            var results = await connection.QueryAsync(dataSql, parameters);

            // Convert to list of objects
            var dataList = results.Select(r => (object)r).ToList();

            // Nếu FE có gửi RequestedColumns, filter columns
            if (pagingRequestDTO.RequestedColumns != null && pagingRequestDTO.RequestedColumns.Any())
            {
                dataList = FilterDynamicObjectsByColumns(dataList, pagingRequestDTO.RequestedColumns);
            }

            return new PagingResultDTO<object>
            {
                Data = dataList,
                TotalItems = totalCount,
                PageIndex = pagingRequestDTO.PageIndex,
                PageSize = pagingRequestDTO.PageSize
            };
        }

        /// <summary>
        /// Filter dynamic objects theo RequestedColumns
        /// </summary>
        protected virtual List<object> FilterDynamicObjectsByColumns(List<object> dataList, List<string> requestedColumns)
        {
            if (dataList == null || !dataList.Any() || requestedColumns == null || !requestedColumns.Any())
                return dataList;

            var columnsToInclude = new List<string>(requestedColumns);
            if (!columnsToInclude.Contains("ID", StringComparer.OrdinalIgnoreCase))
            {
                columnsToInclude.Insert(0, "ID");
            }

            return dataList.Select(item =>
            {
                var dynamicObject = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                var itemDict = item as IDictionary<string, object> ?? 
                              item.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(item));

                foreach (var column in columnsToInclude)
                {
                    var key = itemDict.Keys.FirstOrDefault(k => 
                        string.Equals(k, column, StringComparison.OrdinalIgnoreCase));
                    if (key != null)
                    {
                        dynamicObject[key] = itemDict[key];
                    }
                }

                return (object)dynamicObject;
            }).ToList();
        }

        /// <summary>
        /// Xử lý GetPaging theo View cụ thể
        /// Override method này trong service cụ thể để xử lý logic theo từng view
        /// </summary>
        protected virtual async Task<object?> GetPagingByView(PagingRequestDTO pagingRequestDTO)
        {
            if (!pagingRequestDTO.View.HasValue)
            {
                return null;
            }

            var viewName = GetViewName(pagingRequestDTO.View.Value);
            if (string.IsNullOrWhiteSpace(viewName))
            {
                return null;
            }

            var selectFields = GetSelectFieldsForView(pagingRequestDTO);
            var searchFields = GetSearchFieldsForView(pagingRequestDTO.View.Value);

            try
            {
                return await GetPagingFromView(viewName, pagingRequestDTO, selectFields, searchFields);
            }
            catch
            {
                // Nếu có lỗi, fallback về logic GetPaging thông thường
                return null;
            }
        }

        public virtual async Task<object> GetPaging(PagingRequestDTO pagingRequestDTO)
        {
            // Nếu có View, xử lý theo view
            if (pagingRequestDTO.View.HasValue)
            {
                var viewResult = await GetPagingByView(pagingRequestDTO);
                if (viewResult != null)
                {
                    return viewResult;
                }
            }

            // Parse Filter string to FilterCondition array
            pagingRequestDTO.ParseFilters();
            
            var query = GetBaseQuery(await Repository.GetAll());

            // Apply FilterCondition array if available
            if (pagingRequestDTO.Filters != null && pagingRequestDTO.Filters.Any())
            {
                query = ApplyFilterConditions(query, pagingRequestDTO.Filters);
            }
            // Fallback: simple string search for backward compatibility
            else if (!string.IsNullOrEmpty(pagingRequestDTO.Filter))
            {
                query = query.Where(e => e.GetType().GetProperties()
                    .Any(p => p.GetValue(e) != null && p.GetValue(e)!.ToString()!.Contains(pagingRequestDTO.Filter, StringComparison.OrdinalIgnoreCase)));
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pagingRequestDTO.PageSize);
            var entities = query.Skip((pagingRequestDTO.PageIndex - 1) * pagingRequestDTO.PageSize).Take(pagingRequestDTO.PageSize).ToList();

            // Kiểm tra có custom fields cần join không
            var dtos = await CustomDataAfterGetPaging(pagingRequestDTO, entities);
            if (dtos == null)
            {
                dtos = Mapper.Map<List<TDto>>(entities);
            }

            // Nếu FE có gửi RequestedColumns, chỉ trả về những fields được request
            if (pagingRequestDTO.RequestedColumns != null && pagingRequestDTO.RequestedColumns.Any())
            {
                var filteredData = FilterDtosByRequestedColumns(dtos, pagingRequestDTO.RequestedColumns);
                return new PagingResultDTO<object>
                {
                    Data = filteredData,
                    TotalItems = totalCount,
                    PageIndex = pagingRequestDTO.PageIndex,
                    PageSize = pagingRequestDTO.PageSize
                };
            }

            return new PagingResultDTO<object>
            {
                Data = dtos,
                TotalItems = totalCount,
                PageIndex = pagingRequestDTO.PageIndex,
                PageSize = pagingRequestDTO.PageSize
            };
        }

        /// <summary>
        /// Filter DTOs để chỉ trả về các fields được request từ FE
        /// </summary>
        protected virtual List<object> FilterDtosByRequestedColumns(List<TDto> dtos, List<string> requestedColumns)
        {
            if (dtos == null || !dtos.Any() || requestedColumns == null || !requestedColumns.Any())
                return dtos.Cast<object>().ToList();

            // Luôn include ID để FE có thể handle actions (edit, delete, etc)
            var columnsToInclude = new List<string>(requestedColumns);
            if (!columnsToInclude.Contains("ID", StringComparer.OrdinalIgnoreCase))
            {
                columnsToInclude.Insert(0, "ID");
            }

            // Tạo dynamic objects chỉ chứa các properties được request
            return dtos.Select(dto =>
            {
                var dynamicObject = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                var dtoType = dto.GetType();

                foreach (var column in columnsToInclude)
                {
                    var property = dtoType.GetProperty(column, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property != null)
                    {
                        dynamicObject[property.Name] = property.GetValue(dto);
                    }
                }

                return (object)dynamicObject;
            }).ToList();
        }

        /// <summary>
        /// Override method này để custom việc xử lý data sau khi GetPaging
        /// Có thể join với bảng khác, map thêm field, etc.
        /// </summary>
        /// <param name="pagingRequestDTO">Request chứa thông tin về fields cần include</param>
        /// <param name="entities">Entities từ database</param>
        /// <returns>List DTO đã được customize, return null để dùng Mapper mặc định</returns>
        public virtual async Task<List<TDto>> CustomDataAfterGetPaging(PagingRequestDTO pagingRequestDTO, List<TEntity> entities)
        {
            return null;
        }

        public virtual async Task CustomBeforeSubmitData(TDto dto)
        {

        }

        public virtual async Task<BaseResponse> SubmitData(TDto dto)
        {
            await CustomBeforeSubmitData(dto);
            try
            {
                switch (dto.State)
                {
                    case AutoAppManagement.Models.Common.EntityState.Add:
                        var entityToCreate = Mapper.Map<TEntity>(dto);
                        entityToCreate.SetCreated(GetCurrentUserId());
                        await Repository.CreateAsync(entityToCreate);
                        await UnitOfWork.SaveAsync();
                        return BaseResponse.Success("Lưu thành công");

                    case AutoAppManagement.Models.Common.EntityState.Edit:
                        var entityToUpdate = await Repository.FirstOrDefault(e => e.ID == dto.ID);
                        if (entityToUpdate == null)
                        {
                            return BaseResponse.Error("Record not found.");
                        }
                        // Chỉ map những field đã thay đổi so với dữ liệu hiện tại
                        MapOnlyChangedProperties(dto, entityToUpdate);
                        entityToUpdate.SetUpdated(GetCurrentUserId());
                        // IBaseRepository doesn't have Update, entity tracking handles this
                        await UnitOfWork.SaveAsync();
                        return BaseResponse.Success(Mapper.Map<TDto>(entityToUpdate), "Successfully updated.");

                    case AutoAppManagement.Models.Common.EntityState.Remove:
                        var entityToDelete = await Repository.FirstOrDefault(e => e.ID == dto.ID && e.Status == Models.Enum.StatusEnum.Active);
                        if (entityToDelete == null)
                        {
                            return BaseResponse.Error("Record not found.");
                        }
                        entityToDelete.SetDeleted(GetCurrentUserId());
                        // IBaseRepository doesn't have Update, entity tracking handles this
                        await UnitOfWork.SaveAsync();
                        return BaseResponse.Success("Successfully deleted.");

                    default:
                        return BaseResponse.Error("Invalid entity state.");
                }
            }
            catch (Exception ex)
            {
                // Tr? v? message chi ti?t t? exception thay v� message chung
                return BaseResponse.Error(ex.Message);
            }
        }

        public virtual async Task<BaseResponse> Delete(long id)
        {
            try
            {
                var entity = await Repository.FirstOrDefault(e => e.ID == id && e.Status == Models.Enum.StatusEnum.Active);
                if (entity == null)
                {
                    return BaseResponse.Error("Không tìm thấy đối tượng để xóa");
                }

                entity.SetDeleted(GetCurrentUserId());
                // IBaseRepository doesn't have Update, entity tracking handles this
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Xóa thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xóa: {ex.Message}");
            }
        }

        public AccountGenericDTO GetUserAuthen()
        {
            var userInfor = new AccountGenericDTO();
            var userContext = HttpContextAccessor?.HttpContext?.User;
            if (userContext?.Identity != null && userContext.Identity.IsAuthenticated)
            {
                var userInforUserName = userContext?.FindFirst(JwtRegisteredClaimsNamesConstant.Sub)?.Value;
                if (userInforUserName != null)
                {
                    userInfor.UserName = userInforUserName;
                    var valueAccId = userContext?.FindFirst(JwtRegisteredClaimsNamesConstant.AccId)?.Value;
                    if (valueAccId != null)
                    {
                        userInfor.AccountId = long.Parse(valueAccId);
                        userInfor.RoleList = userContext?.FindAll(JwtRegisteredClaimsNamesConstant.Role)
                            .Select(c => c.Value)
                            .ToList() ?? new List<string>();
                    }
                }
            }
            else
            {
                return null!;
            }

            return userInfor;
        }
        protected new long GetCurrentUserId()
        {
            var userContext = HttpContextAccessor?.HttpContext?.User;
            if (userContext?.Identity != null && userContext.Identity.IsAuthenticated)
            {
                var valueAccId = userContext?.FindFirst(JwtRegisteredClaimsNamesConstant.AccId)?.Value;
                if (valueAccId != null && long.TryParse(valueAccId, out long userId))
                {
                    return userId;
                }
                
                // Fallback: try to get from NameIdentifier
                var userIdClaim = userContext?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                                 ?? userContext?.FindFirst("UserId")?.Value;
                if (userIdClaim != null && long.TryParse(userIdClaim, out long userId2))
                {
                    return userId2;
                }
            }
            return 1; // Default for testing
        }

        /// <summary>
        /// Build expression cho một filter condition
        /// </summary>
        private Expression BuildFilterExpression(FilterCondition filter, ParameterExpression parameter)
            {
                if (string.IsNullOrEmpty(filter.field))
                return null;

                try
                {
                    var property = Expression.Property(parameter, filter.field);
                    var propertyInfo = (PropertyInfo)property.Member;
                    var propertyType = propertyInfo.PropertyType;
                    
                    // Convert value to correct type
                    
                var convertedValue = ConvertFilterValue(filter.value, propertyType);
                    var constant = Expression.Constant(convertedValue, propertyType);

                    Expression filterExpression = null;
                    
                    switch (filter.op)
                    {
                        case FilterOperator.Equals:
                            filterExpression = Expression.Equal(property, constant);
                            break;
                        case FilterOperator.NotEquals:
                            filterExpression = Expression.NotEqual(property, constant);
                            break;
                        case FilterOperator.Contains:
                            if (propertyType == typeof(string))
                            {
                                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                filterExpression = Expression.Call(property, containsMethod, Expression.Constant(filter.value, typeof(string)));
                            }
                            break;
                        case FilterOperator.StartsWith:
                            if (propertyType == typeof(string))
                            {
                                var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                                filterExpression = Expression.Call(property, startsWithMethod, Expression.Constant(filter.value, typeof(string)));
                            }
                            break;
                        case FilterOperator.EndsWith:
                            if (propertyType == typeof(string))
                            {
                                var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                                filterExpression = Expression.Call(property, endsWithMethod, Expression.Constant(filter.value, typeof(string)));
                            }
                            break;
                        case FilterOperator.GreaterThan:
                        case FilterOperator.GreaterThan_Date:
                            filterExpression = Expression.GreaterThan(property, constant);
                            break;
                        case FilterOperator.GreaterThanOrEqual:
                        case FilterOperator.GreaterThanOrEqual_Date:
                            filterExpression = Expression.GreaterThanOrEqual(property, constant);
                            break;
                        case FilterOperator.LessThan:
                        case FilterOperator.LessThan_Date:
                            filterExpression = Expression.LessThan(property, constant);
                            break;
                        case FilterOperator.LessThanOrEqual:
                        case FilterOperator.LessThanOrEqual_Date:
                            filterExpression = Expression.LessThanOrEqual(property, constant);
                            break;
                    }

                    if (filterExpression != null)
                    {
                    // Handle OR conditions trong filter.ors
                        if (filter.ors != null && filter.ors.Any())
                        {
                            var orExpressions = new List<Expression> { filterExpression };
                            foreach (var orFilter in filter.ors)
                            {
                            var orExpression = BuildFilterExpression(orFilter, parameter);
                                    if (orExpression != null)
                                    {
                                        orExpressions.Add(orExpression);
                                }
                            }
                            
                            if (orExpressions.Count > 1)
                            {
                                filterExpression = orExpressions.Aggregate((left, right) => Expression.OrElse(left, right));
                            }
                        }
                }

                return filterExpression;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Apply FilterCondition array to IQueryable với logic AND/OR đúng
        /// </summary>
        protected virtual IQueryable<TEntity> ApplyFilterConditions(IQueryable<TEntity> query, List<FilterCondition> filters)
        {
            if (filters == null || !filters.Any())
                return query;

            // Tạo một parameter duy nhất cho tất cả filters
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var filterExpressions = new List<(Expression Expression, LogicalOperator Operator)>();

            // Build tất cả filter expressions
            foreach (var filter in filters)
            {
                var filterExpression = BuildFilterExpression(filter, parameter);
                if (filterExpression != null)
                {
                    filterExpressions.Add((filterExpression, filter.aop));
                    }
                }

            if (!filterExpressions.Any())
                return query;

            // Xây dựng expression tree với logic AND/OR đúng
            Expression finalExpression = null;
            for (int i = 0; i < filterExpressions.Count; i++)
            {
                var current = filterExpressions[i];
                
                if (finalExpression == null)
                {
                    // Expression đầu tiên
                    finalExpression = current.Expression;
                }
                else
                {
                    // Kết hợp với expression trước đó dựa trên aop của filter hiện tại
                    if (current.Operator == LogicalOperator.AND)
                    {
                        finalExpression = Expression.AndAlso(finalExpression, current.Expression);
                    }
                    else // OR
                    {
                        finalExpression = Expression.OrElse(finalExpression, current.Expression);
                    }
                }
            }

            if (finalExpression != null)
            {
                var lambda = Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Chỉ map những property đã thay đổi so với giá trị hiện tại trong Entity
        /// So sánh giá trị trong DTO với Entity và chỉ update những field có sự thay đổi
        /// </summary>
        protected virtual void MapOnlyChangedProperties(TDto dto, TEntity entity)
        {
            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            
            // Lấy tất cả properties của DTO
            var dtoProperties = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var dtoProperty in dtoProperties)
            {
                // Bỏ qua ID và State vì chúng không cần map
                if (dtoProperty.Name == "ID" || dtoProperty.Name == "State")
                    continue;
                
                // Tìm property tương ứng trong Entity
                var entityProperty = entityType.GetProperty(dtoProperty.Name, BindingFlags.Public | BindingFlags.Instance);
                
                if (entityProperty == null || !entityProperty.CanWrite)
                    continue;
                
                // Lấy giá trị từ DTO và Entity
                var dtoValue = dtoProperty.GetValue(dto);
                var entityValue = entityProperty.GetValue(entity);
                
                // So sánh giá trị - chỉ map nếu có sự thay đổi
                if (!AreValuesEqual(dtoValue, entityValue, dtoProperty.PropertyType))
                {
                    try
                    {
                        var dtoPropertyType = Nullable.GetUnderlyingType(dtoProperty.PropertyType) ?? dtoProperty.PropertyType;
                        var entityPropertyType = Nullable.GetUnderlyingType(entityProperty.PropertyType) ?? entityProperty.PropertyType;
                        
                        // Kiểm tra kiểu dữ liệu có tương thích không
                        if (dtoPropertyType == entityPropertyType || entityPropertyType.IsAssignableFrom(dtoPropertyType))
                        {
                            // Convert giá trị nếu cần
                            object valueToSet = dtoValue;
                            if (dtoPropertyType != entityPropertyType)
                            {
                                valueToSet = Convert.ChangeType(dtoValue, entityPropertyType);
                            }
                            entityProperty.SetValue(entity, valueToSet);
                        }
                    }
                    catch
                    {
                        // Bỏ qua nếu không thể set (ví dụ: kiểu không tương thích)
                    }
                }
            }
        }

        /// <summary>
        /// So sánh hai giá trị có bằng nhau không
        /// Xử lý các trường hợp đặc biệt: null, string, DateTime, decimal, etc.
        /// </summary>
        protected virtual bool AreValuesEqual(object value1, object value2, Type propertyType)
        {
            // Cả hai đều null
            if (value1 == null && value2 == null)
                return true;
            
            // Một trong hai null
            if (value1 == null || value2 == null)
                return false;
            
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            
            // String: so sánh case-insensitive và trim
            if (underlyingType == typeof(string))
            {
                var str1 = (value1?.ToString() ?? "").Trim();
                var str2 = (value2?.ToString() ?? "").Trim();
                return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
            }
            
            // DateTime: so sánh với độ chính xác đến giây
            if (underlyingType == typeof(DateTime))
            {
                var dt1 = (DateTime)value1;
                var dt2 = (DateTime)value2;
                return dt1.Date == dt2.Date && dt1.Hour == dt2.Hour && dt1.Minute == dt2.Minute && dt1.Second == dt2.Second;
            }
            
            // DateTimeOffset: so sánh tương tự DateTime
            if (underlyingType == typeof(DateTimeOffset))
            {
                var dto1 = (DateTimeOffset)value1;
                var dto2 = (DateTimeOffset)value2;
                return dto1.DateTime.Date == dto2.DateTime.Date && 
                       dto1.DateTime.Hour == dto2.DateTime.Hour && 
                       dto1.DateTime.Minute == dto2.DateTime.Minute && 
                       dto1.DateTime.Second == dto2.DateTime.Second;
            }
            
            // Decimal: so sánh với độ chính xác
            if (underlyingType == typeof(decimal))
            {
                var dec1 = (decimal)value1;
                var dec2 = (decimal)value2;
                return Math.Abs(dec1 - dec2) < 0.0001m;
            }
            
            // Double/Float: so sánh với độ chính xác
            if (underlyingType == typeof(double) || underlyingType == typeof(float))
            {
                var d1 = Convert.ToDouble(value1);
                var d2 = Convert.ToDouble(value2);
                return Math.Abs(d1 - d2) < 0.0001;
            }
            
            // Mặc định: so sánh bằng Equals
            return value1.Equals(value2);
        }

        /// <summary>
        /// Convert filter value string to the correct property type
        /// </summary>
        private object ConvertFilterValue(string value, Type propertyType)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            var targetType = underlyingType ?? propertyType;

            // Handle DateTime
            if (targetType == typeof(DateTime))
            {
                return DateTime.Parse(value).ToUniversalTime();
            }

            // Handle Enum - support both enum name (e.g., "Active", "active") and numeric value (e.g., "1")
            if (targetType.IsEnum)
            {
                // Try to parse as enum name first (case-insensitive)
                if (Enum.TryParse(targetType, value, true, out var enumValue))
                {
                    return enumValue;
                }
                
                // If that fails, try to parse as numeric value
                // Get the underlying type (int, short, byte, etc.)
                var underlyingEnumType = Enum.GetUnderlyingType(targetType);
                
                // Try to parse directly as the underlying type
                try
                {
                    var parsedValue = Convert.ChangeType(value, underlyingEnumType);
                    return Enum.ToObject(targetType, parsedValue);
                }
                catch
                {
                    // If conversion fails, throw a more descriptive error
                    throw new ArgumentException($"Cannot convert '{value}' to enum type {targetType.Name}. Expected enum name or numeric value compatible with {underlyingEnumType.Name}.");
                }
            }

            // Handle Guid
            if (targetType == typeof(Guid))
            {
                return Guid.Parse(value);
            }

            // Handle other types (int, long, string, bool, etc.)
            return Convert.ChangeType(value, targetType);
        }
    }
}

