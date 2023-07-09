using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookFetcher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private string _baseURL = "https://en.uesp.net";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var books = new List<BookItem>();

            var html = await GetHtmlPageAsync(_baseURL + "/wiki/Skyrim:Books");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tables = doc.DocumentNode.SelectNodes("//table");
            var booksTable = tables[1];
            var bookRows = booksTable.SelectNodes(".//tr").ToArray();

            for (int i = 1; i < bookRows.Length; i++)
            {
                try
                {
                    // var links = bookRows[i].SelectNodes(".//a");
                    var imageLink = bookRows[i].SelectSingleNode(".//td[1]//a[1]");
                    var imageUrl = imageLink.GetAttributeValue("href", "");
                    var coverImage = await GetBookCoverImageAsync(imageUrl);
                    var bookLink = bookRows[i].SelectSingleNode(".//td[2]//a[1]");
                    var bookUrl = bookLink.GetAttributeValue("href", "");
                    var title = bookLink?.InnerText;
                    var text = await GetBookTextAsync(bookUrl);
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

    private async Task<string> GetHtmlPageAsync(string url)
    {
        using var client = new HttpClient();

        var response = await client.GetAsync(url);

        return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> GetBookCoverImageAsync(string coverUrl)
    {
        var coverHtml = await GetHtmlPageAsync(_baseURL + coverUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(coverHtml);

        var imageUrl = doc.DocumentNode.SelectSingleNode("//div[@class='fullImageLink']//a").GetAttributeValue("href", "");

        var fileName = imageUrl.Split('/').Last();

        await DownloadImageAsync("https:" + imageUrl, fileName);

        return fileName;
    }

    private async Task DownloadImageAsync(string imageUrl, string fileName)
    {
        var folder = "Files/Covers";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists($"{folder}/{fileName}"))
            return;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] imageData = await client.GetByteArrayAsync(imageUrl);

                if (!Directory.Exists("Covers"))
                    Directory.CreateDirectory("Covers");

                // Save the image data to a file
                await System.IO.File.WriteAllBytesAsync("Covers/" + fileName, imageData);

                Console.WriteLine("Image downloaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while downloading the image: " + ex.Message);
            }
        }
    }

    private async Task<string> GetBookTextAsync(string bookUrl)
    {
        var bookHtml = await GetHtmlPageAsync(_baseURL + bookUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(bookHtml);

        var book = doc.DocumentNode.SelectSingleNode("//div[@class='book']")?.InnerHtml;

        return book;
    }
}
