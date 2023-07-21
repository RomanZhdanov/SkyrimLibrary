using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Models;
using SkyrimLibrary.WebAPI.Services;

namespace SkyrimLibrary.WebAPI.Data
{
    public class ApplicationDbContextInitializer
    {
        private readonly ILogger<ApplicationDbContextInitializer> _logger;
        private readonly ApplicationDbContext _context;
        private readonly BooksParser _booksParser;
        private readonly Dictionary<int, string[]> _seriesBooksUrlDirctionary = new ();

        public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context, BooksParser booksParser)
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
                    var (bookSeries, seriesUrl) = _booksParser.GetBookSeries(bookDocoument);
                    int? seriesId = null;
                    int? seriesOrder = null;
                    
                    if (bookSeries is not null && seriesUrl is not null)
                    {
                        seriesId = await GetSeriesIdAsync(bookSeries, seriesUrl);

                        seriesOrder = _seriesBooksUrlDirctionary[seriesId.Value]
                            .Select((url, i) => new { i, url })
                            .Where((b => b.url == bookUrl))
                            .Select(b => b.i + 1)
                            .FirstOrDefault();
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
                        SeriesOrder = seriesOrder,
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

        private async Task<int> GetSeriesIdAsync(string seriesName, string seriesUrl)
        {
            var series = await _context.Series.SingleOrDefaultAsync(s => s.FullName == seriesName);

            if (series is null)
            {
                (series, var booksUrls) = await GetSeriesData(seriesUrl);
                series.FullName = seriesName;
                _context.Series.Add(series);
                await _context.SaveChangesAsync();

                _seriesBooksUrlDirctionary.Add(series.Id, booksUrls);
            }

            return series.Id;
        }

        private async Task<(Series, string[])> GetSeriesData(string seriesUrl)
        {
            var seriesHtml = await _booksParser.GetHtmlPageAsync(seriesUrl);

            var seriesDoc = new HtmlDocument();
            seriesDoc.LoadHtml(seriesHtml);

            try
            {
                var books = seriesDoc.DocumentNode.SelectNodes("//span[@class='mw-headline']");
                string[] booksUrl = new string[books.Count];

                for (int i = 0; i < books.Count; i++)
                {
                    booksUrl[i] = books[i].SelectSingleNode(".//a")?.GetAttributeValue("href", "");
                }

                var titleBlocks = seriesDoc.DocumentNode
                .SelectSingleNode("//div[@class='mw-parser-output']")
                .SelectNodes(".//div")[0]
                .SelectNodes("./div");

                var title = titleBlocks[0].InnerText;
                string? author = null;
                string? description = null;

                if (titleBlocks.Count == 3)
                {
                    author = titleBlocks[1].InnerText;
                    description = titleBlocks[2].InnerText;
                }
                else
                {
                    description = titleBlocks[1].InnerText;
                }

                var series = new Series
                {
                    Name = title,
                    Author = author,
                    Description = description,
                };

                return (series, booksUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parsing series page: {message}", ex.Message);
                throw;
            }            
        }
    }
}
