using System.ComponentModel.DataAnnotations;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.Enums;
using Newtonsoft.Json;

namespace AutoAppManagement.Models.DTO
{
    public class SortedPaging
    {
        public string Field { get; set; }
        public bool IsAsc { get; set; } = true;
    }

    public class PagingRequestDTO
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// Filter string - có thể là JSON string của FilterCondition array hoặc simple search string
        /// </summary>
        public string Filter { get; set; } = "";
        
        /// <summary>
        /// Filter conditions array - parsed from Filter string if it's JSON
        /// </summary>
        public List<FilterCondition> Filters { get; set; } = new List<FilterCondition>();
        
        public string Sort { get; set; } = "Id";

        /// <summary>
        /// Danh sách các column fields cần hiển thị trên grid (từ FE gửi xuống)
        /// Ví dụ: ["Name", "Email", "Phone", "LicenseName", "Status", "CreatedDate"]
        /// Backend sẽ dựa vào list này để quyết định join bảng nào
        /// </summary>
        public List<string> RequestedColumns { get; set; } = new List<string>();

        /// <summary>
        /// View enum để xác định view nào đang được sử dụng
        /// </summary>
        public EnumView? View { get; set; }

        /// <summary>
        /// Lấy tên view dạng string
        /// </summary>
        public string GetViewName()
        {
            return View?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Parse Filter string to FilterCondition array if it's JSON
        /// Only parse if Filters is empty (priority: Filters > Filter string)
        /// </summary>
        public void ParseFilters()
        {
            // If Filters already has data, don't parse Filter string (Filters has priority)
            if (Filters != null && Filters.Any())
            {
                return;
            }

            if (string.IsNullOrEmpty(Filter))
            {
                Filters = new List<FilterCondition>();
                return;
            }

            // Try to parse as JSON array of FilterCondition
            try
            {
                var parsed = JsonConvert.DeserializeObject<List<FilterCondition>>(Filter);
                if (parsed != null)
                {
                    Filters = parsed;
                    return;
                }
            }
            catch
            {
                // Not JSON, treat as simple search string
            }

            // If not JSON, create a simple Contains filter for backward compatibility
            Filters = new List<FilterCondition>();
        }
    }

    public class PagingResultDTO<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
