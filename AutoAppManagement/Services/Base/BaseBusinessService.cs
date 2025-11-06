using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.ViewModel;
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

        Task<TDto> GetById(long id);

        Task<object> GetPaging(PagingRequestDTO request);

        Task<ResponseOutput<object>> SubmitData(TDto dto);

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

        public virtual async Task<TDto> GetById(long id)
        {
            return await RequestAuthenGetAsync<TDto>(ApiUrlDef.GetById(id));
        }

        public virtual async Task<object> GetPaging(PagingRequestDTO request)
        {
            return await RequestAuthenPostAsync<PagingResultDTO<TDto>>(ApiUrlDef.GetPaging(), request);
        }

        public virtual async Task<ResponseOutput<object>> SubmitData(TDto dto)
        {
            return await RequestFullAuthenPostAsync<object>(ApiUrlDef.SubmitData(), dto);
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
