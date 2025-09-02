using AutoAppManagement.Models.Common;

using AutoAppManagement.Service.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseBusinessController<TService, TEntity, TDto> : BaseController
        where TEntity : class
        where TDto : class, IStatefulDTO
        where TService : IBaseBusinessService<TDto>
    {
        protected TService _service;
        protected TService Service
            => _service ??= _serviceProvider.GetRequiredService<TService>();

        protected ILogger<TService> _logger;
        protected ILogger<TService> Logger
            => _logger ??= _serviceProvider.GetRequiredService<ILogger<TService>>();

        public BaseBusinessController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [HttpGet("GetPaging")]
        public virtual async Task<IActionResult> GetPaging(int page = 1, int pageSize = 10, string? filter = null)
        {
            try
            {
                var result = await Service.GetPaging(page, pageSize, filter);
                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("GetAll")]
        public virtual async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await Service.GetAll();
                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpGet("GetById")]
        public virtual async Task<IActionResult> GetById(long id)
        {
            try
            {
                if (id <= 0)
                {
                    ResOutput.ErrorEventHandler("ID không hợp lệ");
                    return BadRequest(ResOutput);
                }

                var result = await Service.GetById(id);
                if (result == null)
                {
                    ResOutput.ErrorEventHandler("Không tìm thấy dữ liệu");
                    return NotFound(ResOutput);
                }

                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpPost("SubmitData")]
        public virtual async Task<IActionResult> SubmitData([FromBody] TDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResOutput.ErrorEventHandler(ModelState);
                    return BadRequest(ResOutput);
                }

                var result = await Service.SubmitData(request);

                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(result.Data, result.Message);
                    return Ok(ResOutput);
                }
                else
                {
                    ResOutput.ErrorEventHandler(result.Message);
                    return BadRequest(ResOutput);
                }
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await Service.Delete(id);
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(result.Data, result.Message);
                    return Ok(ResOutput);
                }
                ResOutput.ErrorEventHandler(result.Message);
                return BadRequest(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }
    }
}
