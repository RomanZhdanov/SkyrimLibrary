namespace SkyrimLibrary.WebAPI.Queries.GetSeriesDetails
{
    public class SeriesDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<BookSeriesDTO> Books { get; set; }
    }
}
