using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Extensions;
using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Data;

namespace SkyrimLibrary.WebAPI.Queries.GetBook
{
    public class GetBookReadQuery : IQuery<BookDTO>
    {
        public string BookId { get; set; }

        public GetBookReadQuery(string id) => BookId = id;
    }

    public class GetBookQueryHandler : IQueryHandler<GetBookReadQuery, BookDTO>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public GetBookQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task<BookDTO> Handle(GetBookReadQuery query)
        {
            var baseURL = _httpContext.HttpContext.Request.Host;
            var scheme = _httpContext.HttpContext.Request.Scheme;
            var book = await _context.Books
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.Id == query.BookId);

            if (book is null) return null;

            return new BookDTO
            {
                Id = book.Id,
                Title = book.FullTitle,
                Author = book.Author,
                Text = book.Text?.UpdateImagesSrc($"{scheme}://{baseURL}/img/pictures/")
            };
        }
    }
}
