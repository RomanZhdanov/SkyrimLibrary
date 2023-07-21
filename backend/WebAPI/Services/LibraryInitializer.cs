using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Png;
using SkyrimLibrary.WebAPI.Data;
using SkyrimLibrary.WebAPI.Models;

namespace SkyrimLibrary.WebAPI.Services;

internal class LibraryInitializer
{
    private readonly ILogger<LibraryInitializer> _logger;
    private readonly SearchService _searchService;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ApplicationDbContext _context;
    private readonly ApplicationDbContextInitializer _applicationDbContextInitializer;

    public LibraryInitializer(ILogger<LibraryInitializer> logger, SearchService searchService, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, ApplicationDbContextInitializer applicationDbContextInitializer)
    {
        _logger = logger;
        _searchService = searchService;
        _hostingEnvironment = hostingEnvironment;
        _context = context;
        _applicationDbContextInitializer = applicationDbContextInitializer;
    }

    public async Task Run()
    {
        await _applicationDbContextInitializer.InitialiseAsync();
        await _applicationDbContextInitializer.SeedAsync(_hostingEnvironment.WebRootPath);
        await InitSearchEngine();
        ResizeCovers();
    }    

    private async Task InitSearchEngine()
    {
        try
        {
            var result = await _searchService.Initialize();

            if (!result.Succeeded)
            {
                _logger.LogWarning("Неудачная инициализация поисковой службы:", String.Join(" ", result.Errors));
                return;
            }

            var books = _context.Books
                .AsNoTracking()
                .Select(b => new BookSearchItem
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
