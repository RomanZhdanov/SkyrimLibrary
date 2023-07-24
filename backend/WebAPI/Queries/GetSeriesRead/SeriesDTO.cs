namespace SkyrimLibrary.WebAPI.Queries.GetSeriesRead
{
    public class SeriesDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public IEnumerable<BookDTO> Books { get; set; }
    }
}
