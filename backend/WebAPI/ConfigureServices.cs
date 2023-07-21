using Microsoft.EntityFrameworkCore;
using SkyrimLibrary.WebAPI.Common.Extensions;
using SkyrimLibrary.WebAPI.Data;
using SkyrimLibrary.WebAPI.Services;
using ReindexerClient;

namespace SkyrimLibrary.WebAPI
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddHttpContextAccessor();
            services.AddScoped<ApplicationDbContextInitializer>();
            services.AddReindexer();
            services.AddCQRS();
            services.AddTransient<SearchService>();
            services.AddTransient<LibraryInitializer>();
            services.AddTransient<BooksParser>();
            services.AddHttpClient<BooksParser>(client =>
            {
                client.BaseAddress = new Uri("https://en.uesp.net");
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("frontend-origins", corsBuilder => corsBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(configuration.GetSection("CorsOrigins").Get<string[]>())
                    .AllowCredentials());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
