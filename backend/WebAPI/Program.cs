using ReindexerClient;
using SkyrimLibrary.WebAPI.DTO;
using SkyrimLibrary.WebAPI.Models;
using SkyrimLibrary.WebAPI.Services;
using SkyrimLibrary.WebAPI.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<BooksDb>();
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

app.MapGet("/books/", (int page, int pageSize, BooksDb booksDb, HttpContext context) =>
{
    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;

    var count = booksDb.Count();
    var books = booksDb.GetPage(page, pageSize);

    foreach (var book in books)
    {
        book.CoverImage = $"{scheme}://{baseURL}/img/covers/thumb/{book.CoverImage}";
    }

    return Results.Ok(new PaginatedList<BookDTO>(books, count, page, pageSize));
});

app.MapGet("/books/{id}", (string id, BooksDb booksDb, HttpContext context) =>
{
    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;
    var book = booksDb.GetBook(id);
    
    if (book is null) return Results.NotFound();

    book.Text = book.Text.RemoveLinks($"{scheme}://{baseURL}/img/pictures/");

    return Results.Ok(book);
});

app.MapGet("/books/{id}/details", (string id, HttpContext context, BooksDb booksDb) =>
{
    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;
    var book = booksDb.GetBook(id);

    if (book is null) return Results.NotFound();

    return Results.Ok(new BookDTO
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Description = book.Description,
        CoverImage = $"{scheme}://{baseURL}/img/covers/{book.CoverImage}"
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
