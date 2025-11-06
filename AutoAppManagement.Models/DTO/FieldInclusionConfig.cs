using System.ComponentModel.DataAnnotations;

namespace AutoAppManagement.Models.DTO
{
    /// <summary>
    /// Configuration cho việc include fields trong GetPaging
    /// </summary>
    public class FieldInclusionConfig
    {
        /// <summary>
        /// Tên field cần include
        /// </summary>
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// Loại join (Inner, Left, Right)
        /// </summary>
        public string JoinType { get; set; } = "Left";

        /// <summary>
        /// Tên bảng cần join
        /// </summary>
        public string JoinTable { get; set; } = string.Empty;

        /// <summary>
        /// Điều kiện join
        /// </summary>
        public string JoinCondition { get; set; } = string.Empty;

        /// <summary>
        /// Field từ bảng join để lấy value
        /// </summary>
        public string SourceField { get; set; } = string.Empty;

        /// <summary>
        /// Field đích trong DTO để set value
        /// </summary>
        public string TargetField { get; set; } = string.Empty;

        /// <summary>
        /// Có cache dữ liệu join không (để tránh query nhiều lần)
        /// </summary>
        public bool EnableCache { get; set; } = true;

        /// <summary>
        /// Thời gian cache (minutes)
        /// </summary>
        public int CacheMinutes { get; set; } = 5;
    }

    /// <summary>
    /// Extension methods cho PagingRequestDTO - Simplified
    /// </summary>
    public static class PagingRequestExtensions
    {
        /// <summary>
        /// Set danh sách columns từ grid configuration
        /// </summary>
        public static PagingRequestDTO WithColumns(this PagingRequestDTO request, List<string> columns)
        {
            request.RequestedColumns = columns ?? new List<string>();
            return request;
        }

        /// <summary>
        /// Set columns từ array
        /// </summary>
        public static PagingRequestDTO WithColumns(this PagingRequestDTO request, params string[] columns)
        {
            return request.WithColumns(columns.ToList());
        }

        /// <summary>
        /// Kiểm tra có column này trong requested columns không
        /// </summary>
        public static bool HasColumn(this PagingRequestDTO request, string columnName)
        {
            return request.RequestedColumns.Contains(columnName) || request.RequestedColumns.Count == 0;
        }

        /// <summary>
        /// Kiểm tra có cần field này không (alias cho HasColumn)
        /// </summary>
        public static bool NeedsField(this PagingRequestDTO request, string fieldName)
        {
            return request.HasColumn(fieldName);
        }
    }
}