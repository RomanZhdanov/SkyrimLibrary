using SkyrimLibrary.WebAPI.DTO;
using SkyrimLibrary.WebAPI.Models;
using SkyrimLibrary.WebAPI.Utils;
using System.Text.Json;

namespace SkyrimLibrary.WebAPI.Services;

public class BooksDb
{
    private readonly List<Book> books;

    public BooksDb(IWebHostEnvironment hostEnvironment)
    {
        string fileName = Path.Combine(hostEnvironment.WebRootPath, "data/SkyrimBooks.json");
        string jsonString = File.ReadAllText(fileName);
        books = JsonSerializer.Deserialize<List<Book>>(jsonString)!;

        foreach (var book in books)
        {
            book.Text = book.Text.RemoveLinks();
        }
    }

    public IEnumerable<Book> GetAll()
    {
        return books;
    }

    public int Count()
    {
        return books.Count();
    }

    public Book? GetBook(string id)
    {
        return books.SingleOrDefault(b => b.Id == id);
    }

    public IList<BookDTO> Find(string text)
    {
        return books.Where(b => b.Title.ToLower().Contains(text.ToLower()))
            .Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Author = b.Author,
                Type = b.Type,
                CoverImage = b.CoverImage,
            }).ToList();
    }

    public IList<BookDTO> GetPage(int page, int pageSize)
    {
        int startIndex = (page - 1) * pageSize;

        return books.OrderBy(b => b.Title)
            .Skip(startIndex)
            .Take(pageSize)
            .Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Author = b.Author,
                Type = b.Type,
                CoverImage = b.CoverImage,
            }).ToList();
    }
}