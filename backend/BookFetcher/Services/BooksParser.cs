using HtmlAgilityPack;

internal class BooksParser
{
    private readonly HttpClient _httpClient;

    public BooksParser(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

}