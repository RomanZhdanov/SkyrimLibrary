using Microsoft.EntityFrameworkCore;
using ReindexerClient;
using SkyrimLibrary.WebAPI.Data;
using SkyrimLibrary.WebAPI.DTO;
using SkyrimLibrary.WebAPI.Models;
using SkyrimLibrary.WebAPI.Services;
using SkyrimLibrary.WebAPI.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"),
        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.AddScoped<ApplicationDbContextInitialiser>();
builder.Services.AddReindexer();
builder.Services.AddTransient<SearchService>();
builder.Services.AddTransient<Worker>();
builder.Services.AddTransient<BooksParser>();
builder.Services.AddHttpClient<BooksParser>(client =>
{
    client.BaseAddress = new Uri("https://en.uesp.net");
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend-origins", corsBuilder => corsBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(builder.Configuration.GetSection("CorsOrigins").Get<string[]>())
        .AllowCredentials());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var worker = services.GetRequiredService<Worker>();

    try
    {
        await worker.Run();
    }
    catch (Exception ex)
    {
        throw;
    }
}

app.MapGet("/books/", (int page, int pageSize, ApplicationDbContext dbContext, HttpContext context) =>
{
    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;

    var count = dbContext.Books.AsNoTracking().Count();
    int startIndex = (page - 1) * pageSize;
    var books = dbContext.Books
            .AsNoTracking()
            .OrderBy(b => b.Title)
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

    foreach (var book in books)
    {
        book.CoverImage = $"{scheme}://{baseURL}/img/covers/thumb/{book.CoverImage}";
    }

    return Results.Ok(new PaginatedList<BookDTO>(books, count, page, pageSize));
});

app.MapGet("/books/{id}", (string id, ApplicationDbContext dbContext, HttpContext context) =>
{
    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;
    var book = dbContext.Books
        .AsNoTracking()
        .SingleOrDefault(b => b.Id == id);
    
    if (book is null) return Results.NotFound();

    book.Text = book.Text.RemoveLinks($"{scheme}://{baseURL}/img/pictures/");

    return Results.Ok(book);
});

app.MapGet("/books/{id}/details", (string id, HttpContext context, ApplicationDbContext dbContext) =>
{
    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;
    var book = dbContext.Books
        .AsNoTracking()
        .Include(b => b.Series)
        .SingleOrDefault(b => b.Id == id);

    SeriesDTO series = null;

    if (book.Series is not null)
    {
        var books = dbContext.Books
            .AsNoTracking()
            .Where(b => b.SeriesId == book.SeriesId)
            .Select(b => new BookSeriesDTO
            {
                Id = b.Id,
                Title = b.Title,
                Current = b.Id == book.Id
            }).ToList();

        series = new SeriesDTO
        {
            Id = book.Series.Id,
            Name = book.Series.Name,
            Books = books
        };
    }

    if (book is null) return Results.NotFound();

    return Results.Ok(new BookDTO
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Description = book.Description,
        CoverImage = $"{scheme}://{baseURL}/img/covers/{book.CoverImage}",
        Series = series
    });
});

app.MapGet("/search/", async (string input, SearchService searchService, HttpContext context) =>
{
    var result = await searchService.FindBooksAsync(input);

    if (!result.Any()) return Results.NotFound($"Not found anything on '{input}'");

    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;

    var items = result.Select(b => new BookDTO
    {
        Id = b.Id,
        Title = b.Title,
        Snippets = b.Text.Contains("<mark>") ? b.Text : null,
        Author = b.Author,
        Description = b.Description,
        CoverImage = $"{scheme}://{baseURL}/img/covers/thumb/{b.CoverImage}"
    });

    return Results.Ok(new SearchResult<BookDTO>
    {
        Items = items,
        ItemsCount = result.Count
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend-origins");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.Run();
