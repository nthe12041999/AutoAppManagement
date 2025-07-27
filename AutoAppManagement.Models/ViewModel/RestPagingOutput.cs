namespace AutoAppManagement.Models.ViewModel
{
    public class RestPagingOutput<T>
    {
        public int StatusCode { get; set; }    // Mã trạng thái HTTP
        public string Message { get; set; }    // Thông điệp mô tả kết quả
        public List<T> Data { get; set; }      // Dữ liệu trả về
        public int PageIndex { get; set; }     // Trang hiện tại
        public int PageSize { get; set; }      // Kích thước trang
        public int TotalPages { get; set; }    // Tổng số trang
        public int TotalItems { get; set; }    // Tổng số phần tử

        // Constructor để tạo một đối tượng RestPagingOutput
        public RestPagingOutput(int statusCode, string message, List<T> data, int page, int pageSize, int totalPages, int totalItems)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
            PageIndex = page;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalItems = totalItems;
        }
    }
}