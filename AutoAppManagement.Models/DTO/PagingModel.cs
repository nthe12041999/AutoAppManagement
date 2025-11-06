using System.ComponentModel.DataAnnotations;

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

        public string Filter { get; set; } = "";
        
        public string Sort { get; set; } = "Id";

        /// <summary>
        /// Danh sách các column fields cần hiển thị trên grid (từ FE gửi xuống)
        /// Ví dụ: ["Name", "Email", "Phone", "LicenseName", "Status", "CreatedDate"]
        /// Backend sẽ dựa vào list này để quyết định join bảng nào
        /// </summary>
        public List<string> RequestedColumns { get; set; } = new List<string>();
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
