using SkyrimLibrary.WebAPI.Models;

namespace SkyrimLibrary.WebAPI.DTO
{
    public class BookDTO
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Snippets { get; set; }

        public string CoverImage { get; set; }

        public SeriesDTO? Series { get; set; }
    }
}
