namespace SkyrimLibrary.WebAPI.Models;

public class Book
{
    public string Id { get; set; }

    public int? SeriesId { get; set; }

    public string Title { get; set; }

    public string? Text { get; set; }

    public string? Author { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public string CoverImage { get; set; }

    public Series? Series { get; set; }

    public int? SeriesOrder { get; set; }
}