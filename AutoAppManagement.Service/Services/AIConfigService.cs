using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.AIConfig;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Service.Services.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AutoAppManagement.Service.Services
{
    public interface IAIConfigService : IBaseBusinessService<AIConfigDTO>
    {
        /// <summary>
        /// Lấy danh sách tất cả features được phép sử dụng của user
        /// </summary>
        Task<List<AIConfigDTO>> GetMyAIConfig();
    }
    public class AIConfigService : BaseBusinessService<AIConfig, AIConfigDTO, IAIConfigRepository>, IAIConfigService
    {
        public AIConfigService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<List<AIConfigDTO>> GetMyAIConfig()
        {
            var userId = GetCurrentUserId();
            var repo = _serviceProvider.GetRequiredService<IAIConfigRepository>();
            var configEntities = await repo.GetByUserId(userId);
            if (configEntities != null)
            {
                return Mapper.Map<List<AIConfigDTO>>(configEntities);
            }
            return null;
        }
    }
}
