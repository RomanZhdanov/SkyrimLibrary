using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Common.Models;
using SkyrimLibrary.WebAPI.Data;

namespace SkyrimLibrary.WebAPI.Queries.GetBooksPage
{
    public class GetBooksPageQuery : IQuery<PaginatedList<BookDTO>>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }

    public class GetBookPageQueryHandler : IQueryHandler<GetBooksPageQuery, PaginatedList<BookDTO>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public GetBookPageQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task<PaginatedList<BookDTO>> Handle(GetBooksPageQuery query)
        {
            var baseURL = _httpContext.HttpContext?.Request.Host;
            var scheme = _httpContext.HttpContext?.Request.Scheme;

            var count = _context.Books.AsNoTracking().Count();
            int startIndex = (query.Page - 1) * query.PageSize;
            
            var books = await _context.Books
                .AsNoTracking()
                .Include(b => b.Series)
                .OrderBy(b => b.Title)
                .Skip(startIndex)
                .Take(query.PageSize)
                .Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = string.IsNullOrEmpty(b.Description) ? null : b.Description,
                    Author = string.IsNullOrEmpty(b.Author) ? null : b.Author,
                    CoverImage = b.CoverImage,
                    Series = b.Series == null ? null : new SeriesDTO
                    {
                        Id = b.Series.Id,
                        Name = b.Series.Name
                    }
                }).ToListAsync();

            foreach (var book in books)
            {
                book.CoverImage = $"{scheme}://{baseURL}/img/covers/thumb/{book.CoverImage}";
            }

            return new PaginatedList<BookDTO>(books, count, query.Page, query.PageSize);
        }
    }
}
