using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Models.DTO.AIConfig
{
    public class AIConfigDTO: BaseEntity.AIConfig, IStatefulDTO
    {
        public EntityState State { get; set; }
    }
}
