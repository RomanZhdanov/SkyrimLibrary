using HtmlAgilityPack;

namespace SkyrimLibrary.WebAPI.Services;

internal class BooksParser
{
    private readonly HttpClient _httpClient;
    private readonly IWebHostEnvironment _hostEnvironment;

    public BooksParser(HttpClient httpClient, IWebHostEnvironment hostEnvironment)
    {
        _httpClient = httpClient;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<string> GetBookTextAsync(string bookUrl)
    {
        var bookHtml = await GetHtmlPageAsync(bookUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(bookHtml);

        var book = doc.DocumentNode.SelectSingleNode("//div[@class='book']")?.InnerHtml;

        return book;
    }

    public async Task<string> GetBookCoverImageAsync(string coverUrl)
    {
        var coverHtml = await GetHtmlPageAsync(coverUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(coverHtml);

        var imageUrl = doc.DocumentNode.SelectSingleNode("//div[@class='fullImageLink']//a").GetAttributeValue("href", "");

        var fileName = imageUrl.Split('/').Last();

        await DownloadImageAsync("https:" + imageUrl, fileName);

        return fileName;
    }

    public async Task<string> GetHtmlPageAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadAsStringAsync();
    }

    private async Task DownloadImageAsync(string imageUrl, string fileName)
    {
        var folder = Path.Combine(_hostEnvironment.WebRootPath, "img/covers");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists($"{folder}/{fileName}"))
            return;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] imageData = await client.GetByteArrayAsync(imageUrl);

                // Save the image data to a file
                await System.IO.File.WriteAllBytesAsync(folder + fileName, imageData);

                Console.WriteLine("Image downloaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while downloading the image: " + ex.Message);
            }
        }
    }

}