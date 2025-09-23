using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAppManagement.Models.DTO.Feature
{
    /// <summary>
    /// Feature Usage Summary - View model cho thống kê
    /// </summary>
    public class FeatureUsageSummary
    {
        public long UserId { get; set; }
        public long FeatureId { get; set; }
        public string FeatureCode { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int TotalUsage { get; set; }
        public decimal TotalResourceConsumed { get; set; }
        public DateTime FirstUsed { get; set; }
        public DateTime LastUsed { get; set; }
        public int UsageDays { get; set; }
    }
}
