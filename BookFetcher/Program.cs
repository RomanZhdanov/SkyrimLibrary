using BookFetcher;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<BooksParser>();
        services.AddHttpClient<BooksParser>(client =>
        {
            client.BaseAddress = new Uri("https://en.uesp.net");
        });
    })
    .Build();

host.Run();
