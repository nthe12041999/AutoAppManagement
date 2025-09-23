using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Service.Services;
using AutoAppManagement.WebApp.Services.ApiUrldefinition.Base;
using Azure;
using System.Net.WebSockets;

namespace AutoAppManagement.WebApp.Services.Base
{
    public interface IBaseBusinessService<TDto> : IBaseService
        where TDto : class, IStatefulDTO
    {
        Task<List<TDto>> GetAll();

        Task<TDto?> GetById(long id);

        Task<object> GetPaging(int page, int pageSize, string? filter = null, string? sort = null);

        Task<BaseResponse> SubmitData(TDto dto);

        Task<BaseResponse> Delete(long id);

        Task<byte[]> ExportToExcelAsync();
    }

    public class BaseBusinessService<TDto, TApiUrlDef> : BaseService, IBaseBusinessService<TDto>
        where TDto : class, IStatefulDTO
        where TApiUrlDef: BaseApiUrlDef
        
    {
        protected TApiUrlDef _apiUrlDef;
        protected TApiUrlDef ApiUrlDef
            => _apiUrlDef ??= _serviceProvider.GetRequiredService<TApiUrlDef>();
        public BaseBusinessService(IServiceProvider serviceProvider):base(serviceProvider) { }

        public async Task<List<TDto>> GetAll()
        {
            return await RequestAuthenGetAsync<List<TDto>>(ApiUrlDef.GetAll());
        }

        public virtual async Task<TDto?> GetById(long id)
        {
            return await RequestAuthenGetAsync<TDto>(ApiUrlDef.GetById(id));
        }

        public virtual async Task<object> GetPaging(int page, int pageSize, string? filter = null, string? sort = null)
        {
            var param = new PagingRequestDTO()
            {
                PageIndex = page,
                PageSize = pageSize,
                Filter = filter ?? "", // Đảm bảo không null
                Sort = sort ?? "Id"    // Đảm bảo không null, mặc định sort theo Id
            };
            return await RequestAuthenPostAsync<PagingResultDTO<TDto>>(ApiUrlDef.GetPaging(), param);
        }

        public virtual async Task<BaseResponse> SubmitData(TDto dto)
        {
            return await RequestAuthenPostAsync<BaseResponse>(ApiUrlDef.SubmitData());
        }

        public virtual async Task<BaseResponse> Delete(long id)
        {
            return await RequestAuthenPostAsync<BaseResponse>(ApiUrlDef.Delete(id));
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            // Mock implementation
            return new byte[0];
        }
    }
}
