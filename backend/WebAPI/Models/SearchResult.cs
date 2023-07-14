namespace SkyrimLibrary.WebAPI.Models
{
    public class SearchResult<T>
    {
        public int ItemsCount { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
