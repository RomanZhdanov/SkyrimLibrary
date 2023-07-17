using HtmlAgilityPack;
using SixLabors.ImageSharp.Formats.Png;
using SkyrimLibrary.WebAPI.Models;
using System.Text.Json;

namespace SkyrimLibrary.WebAPI.Services;

internal class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly BooksParser _booksParser;
    private readonly SearchService _searchService;
    private readonly string _jsonPath = "wwwroot/data/SkyrimBooks.json";

    public Worker(ILogger<Worker> logger, BooksParser booksParser, SearchService searchService)
    {
        _logger = logger;
        _booksParser = booksParser;
        _searchService = searchService;
    }

    public async Task Run()
    {
        if (!File.Exists(_jsonPath))
            await ParseBooksData();

        await InitSearchEngine();
        ResizeCovers();
    }

    private async Task ParseBooksData()
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        var books = new List<Book>(536);

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

                books.Add(new Book
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

        var json = JsonSerializer.Serialize(books);
        File.WriteAllText(_jsonPath, json);
    }

    private async Task InitSearchEngine()
    {
        try
        {
            var booksDb = new BooksDb();
            var result = await _searchService.Initialize();

            if (!result.Succeeded)
            {
                _logger.LogWarning("Неудачная инициализация поисковой службы:", String.Join(" ", result.Errors));
                return;
            }

            var books = booksDb.GetAll().Select(b => new BookSearchItem
            {
                Id = b.Id,
                Title = b.Title,
                Text = b.Text,
                Author  = b.Author,
                Description = b.Description,
                CoverImage = b.CoverImage
            }).ToList();

            await _searchService.AddManyBooksAsync(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Во время инициализации поисковой службы возникло исключение!");

            throw;
        }
    }

    private void ResizeCovers()
    {
        var outPath = "wwwroot/img/covers/thumb";

        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        var images = Directory.GetFiles("wwwroot/img/covers");

        foreach (var filePath in images)
        {
            using (Image image = Image.Load(filePath))
            {
                image.Mutate(x => x.Resize(60, 60));
                var savePath = Path.Combine(outPath, Path.GetFileName(filePath));
                image.Save(savePath, new PngEncoder());
            }
        }
    }
    






}
