using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Models.DTO.FeatureUsageTracking
{
    public class FeatureUsageTrackingDTO : BaseEntity.FeatureUsageTracking, IStatefulDTO
    {
        public EntityState State { get; set; }
    }
}
