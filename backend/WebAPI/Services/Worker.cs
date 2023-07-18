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
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly string _jsonPath = "data/SkyrimBooks.json";

    public Worker(ILogger<Worker> logger, IWebHostEnvironment hostingEnvironment, BooksParser booksParser, SearchService searchService)
    {
        _logger = logger;
        _booksParser = booksParser;
        _searchService = searchService;
        _hostingEnvironment = hostingEnvironment;
        _jsonPath = Path.Combine(_hostingEnvironment.WebRootPath, _jsonPath);

        if (!Directory.Exists(Path.GetDirectoryName(_jsonPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath));
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
        var coverSavePath = Path.Combine(_hostingEnvironment.WebRootPath, "img/covers");
        var picturesPath = Path.Combine(_hostingEnvironment.WebRootPath, "img/pictures");

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
                var text = await _booksParser.GetBookTextAsync(bookUrl, picturesPath);
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
            var booksDb = new BooksDb(_hostingEnvironment);
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

            await _searchService.AddOrUpdateManyBooksAsync(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Во время инициализации поисковой службы возникло исключение!");

            throw;
        }
    }

    private void ResizeCovers()
    {
        var inPath = Path.Combine(_hostingEnvironment.WebRootPath, "img/covers");
        var outPath = Path.Combine(_hostingEnvironment.WebRootPath, "img/covers/thumb");

        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        var images = Directory.GetFiles(inPath);

        foreach (var filePath in images)
        {
            var savePath = Path.Combine(outPath, Path.GetFileName(filePath));

            if (!File.Exists(savePath))
            {
                using (Image image = Image.Load(filePath))
                {
                    image.Mutate(x => x.Resize(60, 60));

                    image.Save(savePath, new PngEncoder());
                }
            }
        }
    }
    






}
