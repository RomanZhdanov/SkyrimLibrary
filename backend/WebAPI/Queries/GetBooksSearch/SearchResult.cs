namespace SkyrimLibrary.WebAPI.Queries.GetBooksSearch
{
    public class SearchResult
    {
        public int ItemsCount { get; set; }

        public IEnumerable<BookDTO> Items { get; set; }
    }
}
