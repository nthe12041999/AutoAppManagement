namespace AutoAppManagement.Models.DTO
{
    public class SortedPaging
    {
        public string Field { get; set; }
        public bool IsAsc { get; set; } = true;
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
