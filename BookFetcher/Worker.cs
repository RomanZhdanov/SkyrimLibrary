using HtmlAgilityPack;
using System.Text.Json;

namespace BookFetcher;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly BooksParser _booksParser;

    public Worker(ILogger<Worker> logger, BooksParser booksParser)
    {
        _logger = logger;
        _booksParser = booksParser;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var books = new List<BookItem>();

            var html = await _booksParser.GetHtmlPageAsync("/wiki/Skyrim:Books");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tables = doc.DocumentNode.SelectNodes("//table");
            var booksTable = tables[1];
            var bookRows = booksTable.SelectNodes(".//tr").ToArray();

            for (int i = 1; i < bookRows.Length; i++)
            {
                try
                {
                    var imageLink = bookRows[i].SelectSingleNode(".//td[1]//a[1]");
                    var imageUrl = imageLink.GetAttributeValue("href", "");
                    var coverImage = await _booksParser.GetBookCoverImageAsync(imageUrl);
                    var bookLink = bookRows[i].SelectSingleNode(".//td[2]//a[1]");
                    var bookUrl = bookLink.GetAttributeValue("href", "");
                    var title = bookLink?.InnerText;
                    var text = await _booksParser.GetBookTextAsync(bookUrl);
                    var id = bookRows[i].SelectSingleNode(".//span[@class='idall']").InnerText;
                    var author = bookRows[i].SelectSingleNode(".//td[4]")?.InnerText;
                    var description = bookRows[i].SelectSingleNode(".//td[5]")?.InnerText;
                    var type = bookRows[i].SelectSingleNode(".//td[6]")?.InnerText;

                    books.Add(new BookItem
                    {
                        Id = id,
                        Title = title,
                        Text = text,
                        Author = author,
                        Description = description,
                        Type = type,
                        CoverImage = coverImage
                    });

                    _logger.LogInformation($"Book #{i} {title} hase been added");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception was thrown on item {i}: {ex.Message}");
                }
            }

            var fileName = "Files/SkyrimBooks.json";
            var json = JsonSerializer.Serialize(books);
            File.WriteAllText(fileName, json);
        }
    }






}
