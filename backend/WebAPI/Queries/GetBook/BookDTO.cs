namespace SkyrimLibrary.WebAPI.Queries.GetBook
{
    public class BookDTO
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string? Text { get; set; }
    }
}
