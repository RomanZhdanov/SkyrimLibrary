using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Extensions;
using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Data;

namespace SkyrimLibrary.WebAPI.Queries.GetBook
{
    public class GetBookQuery : IQuery<BookDTO>
    {
        public string BookId { get; set; }

        public GetBookQuery(string id) => BookId = id;
    }

    public class GetBookQueryHandler : IQueryHandler<GetBookQuery, BookDTO>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public GetBookQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task<BookDTO> Handle(GetBookQuery query)
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
                Title = book.Title,
                Author = book.Title,
                Text = book.Text?.RemoveLinks($"{scheme}://{baseURL}/img/pictures/")
            };
        }
    }
}
