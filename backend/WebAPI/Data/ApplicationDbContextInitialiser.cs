using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Models;
using SkyrimLibrary.WebAPI.Services;

namespace SkyrimLibrary.WebAPI.Data
{
    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly BooksParser _booksParser;

        public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, BooksParser booksParser)
        {
            _logger = logger;
            _context = context;
            _booksParser = booksParser;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlite())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync(string rootPath)
        {
            try
            {
                await SeedBookDataAsync(rootPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedBookDataAsync(string rootPath)
        {
            if (!_context.Books.Any())
            {
                var books = await GetBookData(rootPath);

                _context.Books.AddRange(books);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<IList<Book>> GetBookData(string rootPath)
        {
            var coverSavePath = Path.Combine(rootPath, "img/covers");
            var picturesPath = Path.Combine(rootPath, "img/pictures");

            var html = await _booksParser.GetHtmlPageAsync("/wiki/Skyrim:Books");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tables = doc.DocumentNode.SelectNodes("//table");
            var booksTable = tables[1];
            var bookRows = booksTable.SelectNodes(".//tr").ToArray();
            
            var books = new List<Book>(bookRows.Length);

            for (int i = 1; i < bookRows.Length; i++)
            {
                try
                {
                    var imageLink = bookRows[i].SelectSingleNode(".//td[1]//a[1]");
                    var imageUrl = imageLink.GetAttributeValue("href", "");
                    var coverImage = await _booksParser.GetBookCoverImageAsync(imageUrl, coverSavePath);
                    var bookLink = bookRows[i].SelectSingleNode(".//td[2]//a[1]");
                    var bookUrl = bookLink.GetAttributeValue("href", "");
                    var title = bookLink?.InnerText;

                    var bookPageHtml = await _booksParser.GetHtmlPageAsync(bookUrl);
                    var bookDocoument = new HtmlDocument();
                    bookDocoument.LoadHtml(bookPageHtml);

                    var text = await _booksParser.GetBookTextAsync(bookDocoument, picturesPath);
                    var bookSeries = _booksParser.GetBookSeries(bookDocoument);
                    int? seriesId = null;
                    
                    if (bookSeries is not null)
                    {
                        seriesId = await GetSeriesIdAsync(bookSeries);
                    }

                    var id = bookRows[i].SelectSingleNode(".//span[@class='idall']").InnerText;
                    var author = bookRows[i].SelectSingleNode(".//td[4]")?.InnerText;
                    var description = bookRows[i].SelectSingleNode(".//td[5]")?.InnerText;
                    var type = bookRows[i].SelectSingleNode(".//td[6]")?.InnerText;

                    books.Add(new Book
                    {
                        Id = id,
                        Title = title,
                        Text = text,
                        Author = author,
                        Description = description,
                        Type = type,
                        SeriesId = seriesId,
                        CoverImage = coverImage
                    });

                    _logger.LogInformation($"Book #{i} {title} hase been added");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception was thrown on item {i}: {ex.Message}");
                }
            }

            return books;
        }

        private async Task<int> GetSeriesIdAsync(string seriesName)
        {
            var series = await _context.Series.SingleOrDefaultAsync(s => s.Name == seriesName);

            if (series is null)
            {
                series = new Series { Name = seriesName };
                _context.Series.Add(series);
                await _context.SaveChangesAsync();
            }

            return series.Id;
        }
    }
}
