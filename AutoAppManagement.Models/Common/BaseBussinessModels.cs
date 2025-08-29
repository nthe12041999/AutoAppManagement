namespace AutoAppManagement.Models.Common
{
    /// <summary>
    /// Enum định nghĩa các state cho Submit action
    /// </summary>
    public enum SubmitState
    {
        /// <summary>
        /// Tạo mới
        /// </summary>
        Create = 1,
        
        /// <summary>
        /// Cập nhật
        /// </summary>
        Update = 2,
        
        /// <summary>
        /// Xóa
        /// </summary>
        Delete = 3
    }

    /// <summary>
    /// Request model cho Submit action
    /// </summary>
    /// <typeparam name="TCreateRequest">Type của create request</typeparam>
    /// <typeparam name="TUpdateRequest">Type của update request</typeparam>
    public class SubmitRequest
    {
        /// <summary>
        /// State của action (Create/Update/Delete)
        /// </summary>
        public SubmitState State { get; set; }
    }

    /// <summary>
    /// Response cơ bản cho các action
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// Trạng thái thành công
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        /// <param name="data">Dữ liệu</param>
        /// <param name="message">Thông báo</param>
        /// <returns></returns>
        public static BaseResponse Success(object? data = null, string message = "Thành công")
        {
            return new BaseResponse
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Tạo response lỗi
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="data">Dữ liệu (nếu có)</param>
        /// <returns></returns>
        public static BaseResponse Error(string message, object? data = null)
        {
            return new BaseResponse
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
        }
    }

    /// <summary>
    /// Response cho GetPaging
    /// </summary>
    /// <typeparam name="T">Type của dữ liệu</typeparam>
    public class PagingResponse<T>
    {
        /// <summary>
        /// Danh sách dữ liệu
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Tổng số lượng record
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Số lượng item mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Có trang trước không
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Có trang sau không
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
