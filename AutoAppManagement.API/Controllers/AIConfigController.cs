using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AIConfig;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class AIConfigController : BaseBusinessController<IAIConfigService, AIConfig, AIConfigDTO>
    {
        public AIConfigController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        /// <summary>
        /// L?y danh sách tính n?ng ???c phép cho user hi?n t?i
        /// </summary>
        [HttpGet("MyAI")]
        public async Task<IActionResult> GetMyAIConfig()
        {
            try
            {
                var aiconfigService = _serviceProvider.GetRequiredService<IAIConfigService>();
                var aiConfigs = await aiconfigService.GetMyAIConfig();

                ResOutput.SuccessEventHandler(aiConfigs);

                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting user features: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }
    }
}