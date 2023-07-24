using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Extensions;
using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Data;

namespace SkyrimLibrary.WebAPI.Queries.GetSeriesRead
{
    public class GetSeriesReadQuery : IQuery<SeriesDTO>
    {
        public int SeriesId { get; set; }

        public GetSeriesReadQuery(int id) => SeriesId = id;
    }

    public class GetSeriesQueryHandler : IQueryHandler<GetSeriesReadQuery, SeriesDTO>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public GetSeriesQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<SeriesDTO> Handle(GetSeriesReadQuery query)
        {
            var baseURL = _contextAccessor.HttpContext?.Request.Host;
            var scheme = _contextAccessor.HttpContext?.Request.Scheme;

            var series = await _context.Series
                .AsNoTracking()
                .Select(s => new SeriesDTO
                {
                    Id = s.Id,
                    Name = s.FullName,
                    Author = s.Author,
                    Description = s.Description,
                    Books = s.Books
                    .OrderBy(b => b.SeriesOrder)
                    .Select(b => new BookDTO
                    {
                        Id = b.Id,
                        Title = b.SeriesTitle,
                        Text = b.Text.UpdateImagesSrc($"{scheme}://{baseURL}/img/pictures/")
                    })
                }).SingleOrDefaultAsync(s => s.Id == query.SeriesId);

            return series;
        }
    }
}
