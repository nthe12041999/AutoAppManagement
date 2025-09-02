using AutoAppManagement.Models.Common;
using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;

namespace AutoAppManagement.WebApp.Services.Base
{
    public interface IBaseBusinessService<TDto>: IBaseService
        where TDto : class, IStatefulDTO
    {
        Task<List<TDto>> GetAll();

        Task<TDto?> GetById(long id);

        Task<object> GetPaging(int page, int pageSize, string? filter = null, string? sort = null);

        Task<BaseResponse> SubmitData(TDto dto);

        Task<BaseResponse> Delete(long id);

        Task<byte[]> ExportToExcelAsync();
    }

    public class BaseBusinessService<TDto> : BaseService, IBaseBusinessService<TDto>
        where TDto : class, IStatefulDTO
    {
        public BaseBusinessService(IServiceProvider serviceProvider):base(serviceProvider) { }

        public async Task<List<TDto>> GetAll()
        {
            return await RequestAuthenGetAsync<List<TDto>>(BaseApiUrlDef.GetAll());
        }

        public virtual async Task<TDto?> GetById(long id)
        {
            return await RequestAuthenGetAsync<TDto>(BaseApiUrlDef.GetById(id));
        }

        public virtual async Task<object> GetPaging(int page, int pageSize, string? filter = null, string? sort = null)
        {
            return await RequestAuthenPostAsync<TDto>(BaseApiUrlDef.GetPaging());
        }

        public virtual async Task<BaseResponse> SubmitData(TDto dto)
        {
            return await RequestAuthenPostAsync<BaseResponse>(BaseApiUrlDef.SubmitData());
        }

        public virtual async Task<BaseResponse> Delete(long id)
        {
            return await RequestAuthenPostAsync<BaseResponse>(BaseApiUrlDef.Delete(id));
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            // Mock implementation
            return new byte[0];
        }
    }
}
