using ReindexerClient;
using ReindexerClient.Helpers;
using SkyrimLibrary.WebAPI.Attributes;
using SkyrimLibrary.WebAPI.Models;
using SkyrimLibrary.WebAPI.Utils;

namespace SkyrimLibrary.WebAPI.Services
{
    public class SearchService
    {
        private readonly IReindexer _rx;

        public SearchService(IReindexer reindexer)
        {
            _rx = reindexer;
        }

        public async Task<Result> Initialize()
        {
            try
            {
                if (!await _rx.DatabaseExistAsync())
                    await _rx.CreateDatabaseAsync();

                if (!await _rx.NamespaceExistAsync("books"))
                    await _rx.CreateNamespaceFromTypeAsync(typeof(BookSearchItem));

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new List<string> { ex.Message });
            }
        }

        public async Task<ICollection<BookSearchItem>> FindBooksAsync(string query)
        {
            var dsl = RxQueryHelper.CreateDSLQuery(typeof(BookSearchItem), query);
            var response = await _rx.Query<BookSearchItem>(dsl);
            return response.Items;
        }        

        public async Task AddOrUpdateManyBooksAsync(ICollection<BookSearchItem> books)
        {
            foreach (var book in books)
            {
                ProcessBookItem(book);

                await _rx.UpsertDocumentsInNamespaceAsync("books", new List<BookSearchItem> { book });
            }
        }

        private void ProcessBookItem(BookSearchItem item)
        {
            var properties = item.GetType().GetProperties();

            foreach (var property in properties)
            {
                var attrs = Attribute.GetCustomAttributes(property);

                foreach (var attr in attrs)
                {
                    if (attr is StripHTML)
                    {
                        var value = (string)property.GetValue(item);
                        property.SetValue(item, value.ToStripHtml());
                    }
                }
            }
        }
    }
}
