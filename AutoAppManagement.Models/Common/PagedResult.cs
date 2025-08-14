namespace AutoAppManagement.Models.Common
{
    /// <summary>
    /// Generic paged result class
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PagedResult()
        {
        }

        public PagedResult(List<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasPreviousPage = pageIndex > 1;
            HasNextPage = pageIndex < TotalPages;
        }
    }
}
