using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Data;

namespace SkyrimLibrary.WebAPI.Queries.GetSeriesDetails
{
    public class GetSeriesDetailsQuery : IQuery<SeriesDTO>
    {
        public int SeriesId { get; set; }

        public GetSeriesDetailsQuery(int id) => SeriesId = id;
    }

    public class GetSeriesDEtailsQueryHandler : IQueryHandler<GetSeriesDetailsQuery, SeriesDTO>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public GetSeriesDEtailsQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<SeriesDTO> Handle(GetSeriesDetailsQuery query)
        { 
            var baseURL = _contextAccessor.HttpContext?.Request.Host;
            var scheme = _contextAccessor.HttpContext?.Request.Scheme;

            var series = await _context.Series
                .AsNoTracking()
                .Include(s => s.Books)
                .Select(s => new SeriesDTO
                {
                    Id = s.Id,
                    Name = s.FullName,
                    Books = s.Books.OrderBy(b => b.SeriesOrder)
                    .Select(b => new BookSeriesDTO
                    {
                        Id = b.Id,
                        Title = b.Title,
                        CoverImage = $"{scheme}://{baseURL}/img/covers/thumb/{b.CoverImage}"
                    })
                })
                .SingleOrDefaultAsync(s => s.Id == query.SeriesId);

            return series;
        }
    }
}
