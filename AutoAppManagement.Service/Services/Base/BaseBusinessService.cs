using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Repository.Repositories.Base;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public virtual async Task<object> GetPaging(PagingRequestDTO pagingRequestDTO)
        {
            // Parse Filter string to FilterCondition array
            pagingRequestDTO.ParseFilters();
            
            var query = (await Repository.GetAll()).Where(e => e.Status == Models.Enum.StatusEnum.Active).AsQueryable();

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
                        var entityToUpdate = await Repository.FirstOrDefault(e => e.ID == dto.ID && e.Status == Models.Enum.StatusEnum.Active);
                        if (entityToUpdate == null)
                        {
                            return BaseResponse.Error("Record not found.");
                        }
                        Mapper.Map(dto, entityToUpdate);
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
        /// Apply FilterCondition array to IQueryable
        /// </summary>
        protected virtual IQueryable<TEntity> ApplyFilterConditions(IQueryable<TEntity> query, List<FilterCondition> filters)
        {
            if (filters == null || !filters.Any())
                return query;

            foreach (var filter in filters)
            {
                if (string.IsNullOrEmpty(filter.field))
                    continue;

                // Build expression based on operator
                var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, filter.field);
                
                // Note: This is a simplified implementation
                // For complex filtering with LicenseName (which is not in Account entity),
                // we need to override in AccountService
                
                // For now, apply simple filters on entity properties
                if (filter.op == FilterOperator.Contains)
                {
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var valueExpression = System.Linq.Expressions.Expression.Constant(filter.value, typeof(string));
                    var containsCall = System.Linq.Expressions.Expression.Call(property, containsMethod, valueExpression);
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(containsCall, parameter);
                    query = query.Where(lambda);
                }
                else if (filter.op == FilterOperator.Equals)
                {
                    var equalsExpression = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(filter.value));
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(equalsExpression, parameter);
                    query = query.Where(lambda);
                }
                // Add more operators as needed
            }

            return query;
        }
    }
}

