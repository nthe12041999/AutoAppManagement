using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AIConfig;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
                var userId = GetCurrentUserId();
                var aiconfigService = _serviceProvider.GetRequiredService<IAIConfigService>();
                var aiConfigs = await aiconfigService.GetMyAIConfig(userId);

                ResOutput.SuccessEventHandler(aiConfigs);

                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Error getting user features: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.FindFirst("AccountId") ?? HttpContext.User.FindFirst("UserId");
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in token");
        }
    }
}