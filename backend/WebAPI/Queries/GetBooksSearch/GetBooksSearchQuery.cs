using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Services;

namespace SkyrimLibrary.WebAPI.Queries.GetBooksSearch
{
    public record GetBooksSearchQuery : IQuery<SearchResult>
    {
        public string Input { get; set; }

        public GetBooksSearchQuery(string input) => Input = input;
    }

    public class GetBookSearchQueryHandler : IQueryHandler<GetBooksSearchQuery, SearchResult>
    {
        private readonly SearchService _searchService;
        private readonly IHttpContextAccessor _contextAccessor;

        public GetBookSearchQueryHandler(SearchService searchService, IHttpContextAccessor contextAccessor)
        {
            _searchService = searchService;
            _contextAccessor = contextAccessor;
        }

        public async Task<SearchResult> Handle(GetBooksSearchQuery query)
        {
            var result = await _searchService.FindBooksAsync(query.Input);

            if (!result.Any()) return new SearchResult
            {
                Items = null,
                ItemsCount = 0
            };

            var baseURL = _contextAccessor.HttpContext?.Request.Host;
            var scheme = _contextAccessor.HttpContext?.Request.Scheme;

            var items = result.Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                Snippets = b.Text.Contains("<mark>") ? b.Text : null,
                Author = b.Author,
                Description = b.Description,
                CoverImage = $"{scheme}://{baseURL}/img/covers/thumb/{b.CoverImage}"
            });

            return new SearchResult
            {
                Items = items,
                ItemsCount = result.Count
            };
        }
    }
}
