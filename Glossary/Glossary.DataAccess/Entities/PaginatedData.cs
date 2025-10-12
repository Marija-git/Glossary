namespace Glossary.DataAccess.Entities
{
    public class PaginatedData<T>
    {
        public IEnumerable<T> Items { get; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public PaginatedData(IEnumerable<T> items, int pageIndex, int totalPages, int totalCount)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalPages = totalPages;
            TotalCount = totalCount;
        }
    }
}
