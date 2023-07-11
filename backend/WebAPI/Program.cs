using SkyrimLibrary.WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<BooksDb>();

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

app.MapGet("/books/", (int page, int pageSize, BooksDb booksDb) =>
    booksDb.GetPage(page, pageSize));

app.MapGet("/books/{id}", (string id, BooksDb booksDb) =>
    booksDb.GetBook(id));

app.MapGet("/search/", (string input, BooksDb booksDb, HttpContext context) =>
{
    var result = booksDb.Find(input);

    if (!result.Any()) return Results.NotFound($"Not found anything on '{input}'");

    var baseURL = context.Request.Host;
    var scheme = context.Request.Scheme;

    foreach (var book in result)
    {
        book.CoverImage = $"{scheme}://{baseURL}/covers/{book.CoverImage}";
    }

    return Results.Ok(result);
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
