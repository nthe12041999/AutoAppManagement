using System.ComponentModel.DataAnnotations;
using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Models.DTO.AdminAccount
{
    /// <summary>
    /// DTO thông tin tài khoản admin
    /// </summary>
    public class AdminAccountDTO : BaseEntity.AdminAccount, IStatefulDTO
    {
        public EntityState State { get; set; }
    }

    /// <summary>
    /// DTO hoạt động gần đây của admin
    /// </summary>
    public class RecentAdminActivityDTO
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string AdminName { get; set; }
        public DateTime ActivityTime { get; set; }
        public string IpAddress { get; set; }
        public string Details { get; set; }
        public string Severity { get; set; }
    }

    /// <summary>
    /// DTO điểm dữ liệu cho biểu đồ
    /// </summary>
    public class ChartDataPointDTO
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public string Color { get; set; }
        public DateTime? Date { get; set; }
        public string Category { get; set; }
    }
}
