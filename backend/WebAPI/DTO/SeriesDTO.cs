﻿namespace SkyrimLibrary.WebAPI.DTO
{
    public class SeriesDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<BookSeriesDTO> Books { get; set; }
    }
}
