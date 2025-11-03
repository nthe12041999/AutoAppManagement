namespace AutoAppManagement.Models.ViewModel
{
    public interface IResponseOutput<T>
    {
        void SuccessEventHandler(T data = default!, string? message = null);
        void ErrorEventHandler(string? message = "Đã có lỗi xảy ra", T data = default!);
    }

    public class ResponseOutput<T> : IResponseOutput<T>
    {
        public bool IsSuccess { get; set; } // Trạng thái thành công
        public string Message { get; set; }
        public T Data { get; set; } = default!; // Dữ liệu trả về

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

        public void ErrorEventHandler(string? message = "Đã có lỗi xảy ra", T data = default!)
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
