namespace SkyrimLibrary.WebAPI.Queries.GetBooksSearch
{
    public class BookDTO
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string? Author { get; set; }

        public string? Description { get; set; }

        public string? Snippets { get; set; }

        public string CoverImage { get; set; }
    }
}
