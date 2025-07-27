namespace AutoAppManagement.Models.ViewModel
{
    public interface IResponseOutput<T>
    {
        void SuccessEventHandler(T data = default!, string? message = null);
        void ErrorEventHandler(T data = default!, string? message = "Đã có lỗi xảy ra");
    }
    public class ResponseOutput<T> : IResponseOutput<T>
    {
        public bool IsSuccess { get; set; }  // Trạng thái thành công
        public string Message { get; set; } = "Đã có lỗi xảy ra"; // Thông điệp mô tả kết quả
        public T Data { get; set; } = default!;        // Dữ liệu trả về

        public void SuccessEventHandler(T data = default!, string? message = null)
        {
            IsSuccess = true;
            if (data != null)
            {
                Data = data;
            }
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }

        public void ErrorEventHandler(T data = default!, string? message = "Đã có lỗi xảy ra")
        {
            IsSuccess = false;
            if (data != null)
            {
                Data = data;
            }
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }
    }
}
