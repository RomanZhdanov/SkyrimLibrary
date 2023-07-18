using HtmlAgilityPack;

namespace SkyrimLibrary.WebAPI.Services;

internal class BooksParser
{
    private readonly HttpClient _httpClient;

    public BooksParser(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetBookTextAsync(string bookUrl, string picturesPath)
    {
        var bookHtml = await GetHtmlPageAsync(bookUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(bookHtml);

        string[] bookClasses = { "book", "poem" , "floatnone" };

        HtmlNode? book = null;

        foreach (var bookClass in bookClasses )
        {
            book = doc.DocumentNode.SelectSingleNode($"//div[@class='{bookClass}']");

            if (book is not null) break;
        }

        if (book is null )
            return null;

        var imageNodes = book.SelectNodes(".//img");

        if (imageNodes != null)
        {
            foreach (HtmlNode iamgeNode in imageNodes)
            {
                string imgUrl = iamgeNode.GetAttributeValue("src", "");

                if (!string.IsNullOrEmpty(imgUrl))
                {
                    var picture = await GetImageFromUrlAsync("https:" + imgUrl, picturesPath);
                    iamgeNode.SetAttributeValue("src", picture);
                }
            }
        }

        return book.InnerHtml;
    }

    public async Task<string> GetBookCoverImageAsync(string coverUrl, string savePath)
    {
        var coverHtml = await GetHtmlPageAsync(coverUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(coverHtml);

        var imageUrl = doc.DocumentNode.SelectSingleNode("//div[@class='fullImageLink']//a").GetAttributeValue("href", "");

        return await GetImageFromUrlAsync("https:" + imageUrl, savePath);        
    }

    public async Task<string> GetImageFromUrlAsync(string imageUrl, string savePath)
    {
        var fileName = imageUrl.Split('/').Last();

        await DownloadImageAsync(imageUrl, Path.Combine(savePath, fileName));

        return fileName;
    }

    public async Task<string> GetHtmlPageAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);

        return await response.Content.ReadAsStringAsync();
    }

    private async Task DownloadImageAsync(string imageUrl, string savePath)
    {
        if (File.Exists(savePath))
            return;
        
        var dir = Path.GetDirectoryName(savePath);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] imageData = await client.GetByteArrayAsync(imageUrl);

                // Save the image data to a file
                await System.IO.File.WriteAllBytesAsync(savePath, imageData);

                Console.WriteLine("Image downloaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while downloading the image: " + ex.Message);
            }
        }
    }

}