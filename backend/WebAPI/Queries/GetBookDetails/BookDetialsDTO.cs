namespace SkyrimLibrary.WebAPI.Queries.GetBookDetails
{
    public class BookDetialsDTO
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string? Author { get; set; }

        public string? Description { get; set; }

        public string CoverImage { get; set; }

        public SeriesDTO? Series { get; set; }
    }
}
