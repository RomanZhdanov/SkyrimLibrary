using SkyrimLibrary.WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<BooksDb>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/books/", (int page, int pageSize, BooksDb booksDb) =>
    booksDb.GetPage(page, pageSize));

app.MapGet("/books/{id}", (string id, BooksDb booksDb) =>
    booksDb.GetBook(id));

app.MapGet("/search/", (string input, BooksDb booksDb) =>
{
    var result = booksDb.Find(input);

    if (!result.Any()) return Results.NotFound($"Not found anything on '{input}'");

    return Results.Ok(result);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
