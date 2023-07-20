using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Data;

namespace SkyrimLibrary.WebAPI.Queries.GetBookDetails
{
    public record GetBookDetailsQuery : IQuery<BookDetialsDTO>
    {
        public string BookId { get; set; }

        public GetBookDetailsQuery(string id) => BookId = id;
    }

    public class GetBookDetailsQueryHandler : IQueryHandler<GetBookDetailsQuery, BookDetialsDTO>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public GetBookDetailsQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task<BookDetialsDTO> Handle(GetBookDetailsQuery query)
        {
            var baseURL = _httpContext.HttpContext?.Request.Host;
            var scheme = _httpContext.HttpContext?.Request.Scheme;

            var book = _context.Books
                .AsNoTracking()
                .Include(b => b.Series)
                .SingleOrDefault(b => b.Id == query.BookId);

            SeriesDTO series = null;

            if (book.Series is not null)
            {
                var books = _context.Books
                .AsNoTracking()
                .Where(b => b.SeriesId == book.SeriesId)
                .Select(b => new BookSeriesDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Current = b.Id == book.Id
                }).ToList();

                series = new SeriesDTO
                {
                    Id = book.Series.Id,
                    Name = book.Series.Name,
                    Books = books
                };
            }

            if (book is null) return null;

            return new BookDetialsDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                CoverImage = $"{scheme}://{baseURL}/img/covers/{book.CoverImage}",
                Series = series
            };
        }
    }
}
