using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Repository.Repositories.Base;
using System.Linq.Expressions;

namespace AutoAppManagement.Service.Services.Base
{
    public interface IBaseBusinessService<TDto>
        where TDto : class, IStatefulDTO
    {
        Task<IEnumerable<TDto>> GetAll();
        Task<TDto?> GetById(long id);
        Task<object> GetPaging(PagingRequestDTO pagingRequestDTO);
        Task<BaseResponse> SubmitData(TDto dto);
        Task<BaseResponse> Delete(long id);
    }

    public abstract class BaseBusinessService<TEntity, TDto, TRepository> : BaseService, IBaseBusinessService<TDto>
        where TEntity :BaseCUEntity
        where TDto : class, IStatefulDTO
        where TRepository : class, IBaseRepository<TEntity>
    {
        private TRepository? _repository;
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

        protected async Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
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

        public virtual async Task<TDto?> GetById(long id)
        {
            var entity = await Repository.FirstOrDefault(e => e.ID == id && e.Status == Models.Enum.StatusEnum.Active);
            return entity == null ? default : Mapper.Map<TDto>(entity);
        }

        public virtual async Task<object> GetPaging(PagingRequestDTO pagingRequestDTO)
        {
            var query = (await Repository.GetAll()).Where(e => e.Status == Models.Enum.StatusEnum.Active).AsQueryable();

            if (!string.IsNullOrEmpty(pagingRequestDTO.Filter))
            {
                query = query.Where(e => e.GetType().GetProperties()
                    .Any(p => p.GetValue(e) != null && p.GetValue(e)!.ToString()!.Contains(pagingRequestDTO.Filter, StringComparison.OrdinalIgnoreCase)));
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pagingRequestDTO.PageSize);
            var entities = query.Skip((pagingRequestDTO.PageIndex - 1) * pagingRequestDTO.PageSize).Take(pagingRequestDTO.PageSize).ToList();
            var dtos = Mapper.Map<List<TDto>>(entities);

            return new { Data = dtos, TotalCount = totalCount, TotalPages = totalPages, CurrentPage = pagingRequestDTO.PageIndex, PageSize = pagingRequestDTO.PageSize };
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
                        return BaseResponse.Success(Mapper.Map<TDto>(entityToCreate), "Successfully created.");

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
                return BaseResponse.Error($"An error occurred: {ex.Message}");
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
        protected long GetCurrentUserId()
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
    }
}

