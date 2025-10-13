using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Service.Common.Cache;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    /// <summary>
    /// Controller để quản lý cache (lấy theo key, xóa theo key)
    /// </summary>
    public class CacheController : BaseController
    {
        private readonly IDistributedCacheCustom _cache;
        private readonly ILogger<CacheController> _logger;

        public CacheController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _cache = serviceProvider.GetRequiredService<IDistributedCacheCustom>();
            _logger = serviceProvider.GetRequiredService<ILogger<CacheController>>();
        }

        /// <summary>
        /// Lấy giá trị cache theo key
        /// GET /Cache/Get?key=yourKey
        /// </summary>
        [HttpGet("get-cache")]
        public async Task<IActionResult> Get(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    ResOutput.ErrorEventHandler("Key không được để trống");
                    return BadRequest(ResOutput);
                }

                var value = await _cache.GetValueCacheAsync<object>(key);
                if (value == null)
                {
                    ResOutput.ErrorEventHandler("Không tìm thấy cache cho key này");
                    return NotFound(ResOutput);
                }

                ResOutput.SuccessEventHandler(value);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key {Key}", key);
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Xóa cache theo key
        /// DELETE /Cache/Delete?key=yourKey
        /// </summary>
        [HttpDelete("delete-cache")]
        public async Task<IActionResult> Delete(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    ResOutput.ErrorEventHandler("Key không được để trống");
                    return BadRequest(ResOutput);
                }

                await _cache.RemoveAsync(key);
                ResOutput.SuccessEventHandler(true, "Xóa cache thành công");
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key {Key}", key);
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }
    }
}
