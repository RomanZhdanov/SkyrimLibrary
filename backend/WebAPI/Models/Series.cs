namespace SkyrimLibrary.WebAPI.Models
{
    public class Series
    {
        public Series()
        {
            this.Books = new HashSet<Book>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string? Author { get; set; }

        public string? Description { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
