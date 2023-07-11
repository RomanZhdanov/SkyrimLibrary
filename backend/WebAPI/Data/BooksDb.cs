using SkyrimLibrary.WebAPI.DTO;
using SkyrimLibrary.WebAPI.Models;
using System.Text.Json;

namespace SkyrimLibrary.WebAPI.Data;

public class BooksDb
{
    private readonly List<Book> books;

    public BooksDb()
    {
        string fileName = "Data/SkyrimBooks.json";
        string jsonString = File.ReadAllText(fileName);
        books = JsonSerializer.Deserialize<List<Book>>(jsonString)!;
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

        return books.Skip(startIndex)
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