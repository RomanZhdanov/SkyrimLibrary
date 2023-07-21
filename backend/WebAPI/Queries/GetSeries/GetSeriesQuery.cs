using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Data;

namespace SkyrimLibrary.WebAPI.Queries.GetSeries
{
    public class GetSeriesQuery : IQuery<IList<SeriesDTO>>
    {
    }

    public class GetSeriesQueryHandler : IQueryHandler<GetSeriesQuery, IList<SeriesDTO>>
    {
        private readonly ApplicationDbContext _context;

        public GetSeriesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<SeriesDTO>> Handle(GetSeriesQuery query)
        {
            return await _context.Series
                .AsNoTracking()
                .Include(s => s.Books)
                .OrderBy(s => s.Name)
                .Select(s => new SeriesDTO
                {
                    Id = s.Id,
                    Name = s.FullName,
                    BooksCount = s.Books.Count
                }).ToListAsync();
        }
    }
}
