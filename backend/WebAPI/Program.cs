using SkyrimLibrary.WebAPI;
using SkyrimLibrary.WebAPI.Common.Models;
using SkyrimLibrary.WebAPI.Queries.GetBook;
using SkyrimLibrary.WebAPI.Queries.GetBookDetails;
using SkyrimLibrary.WebAPI.Queries.GetBooksPage;
using SkyrimLibrary.WebAPI.Queries.GetBooksSearch;
using SkyrimLibrary.WebAPI.Queries.GetSeriesDetails;
using SkyrimLibrary.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddWebAPIServices(builder.Configuration);

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

app.MapGet("/books/", async (int page, int pageSize, QueryDispatcher queryDispatcher) =>
{
    var query = new GetBooksPageQuery { Page = page, PageSize = pageSize };
    var booksPage = await queryDispatcher
        .DispatchAsync<GetBooksPageQuery, PaginatedList<SkyrimLibrary.WebAPI.Queries.GetBooksPage.BookDTO>> (query);

    return Results.Ok(booksPage);
});

app.MapGet("/books/{id}", async (string id, QueryDispatcher queryDispatcher) =>
{
    var query = new GetBookQuery(id.Trim());
    var book = await queryDispatcher.DispatchAsync<GetBookQuery, SkyrimLibrary.WebAPI.Queries.GetBook.BookDTO>(query);

    if (book is null) return Results.NotFound($"Book with id '{query.BookId}' was not found.");

    return Results.Ok(book);
});

app.MapGet("/series/{id}", async (int id, QueryDispatcher queryDispatcher) =>
{
    var series = await queryDispatcher
        .DispatchAsync<GetSeriesDetailsQuery, SkyrimLibrary.WebAPI.Queries.GetSeriesDetails.SeriesDTO>(
    new GetSeriesDetailsQuery(id)
    );

    if (series is null) return Results.NotFound($"Series with id '{id}' was not found.");

    return Results.Ok(series);
});

app.MapGet("/books/{id}/details", async (string id, QueryDispatcher queryDispatcher) =>
{
    var query = new GetBookDetailsQuery(id.Trim());
    var book = await queryDispatcher.DispatchAsync<GetBookDetailsQuery, BookDetialsDTO>(query);

    if (book is null) return Results.NotFound($"Book with id '{query.BookId}' was not found.");

    return Results.Ok(book);
});

app.MapGet("/search/", async (string input, QueryDispatcher queryDispatcher) =>
{
    var query = new GetBooksSearchQuery(input);
    var result = await queryDispatcher.DispatchAsync<GetBooksSearchQuery, SearchResult>(query);

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
