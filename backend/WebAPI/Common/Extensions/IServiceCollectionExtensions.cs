using SkyrimLibrary.WebAPI.Common.Interfaces;
using SkyrimLibrary.WebAPI.Common.Models;
using System.Reflection;

namespace SkyrimLibrary.WebAPI.Common.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var commandHandlers = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Any(IsCommandHandler))
            .ToList();

            var queryHandlers = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Any(IsQueryHandler))
            .ToList();

            foreach (var handler in commandHandlers)
            {
                var interfaceType = handler.GetInterfaces().Single(IsCommandHandler);
                services.AddTransient(interfaceType, handler);
            }

            foreach (var handler in queryHandlers)
            {
                var interfaceType = handler.GetInterfaces().Single(IsQueryHandler);
                services.AddTransient(interfaceType, handler);
            }

            services.AddTransient<CommandDispatcher>();
            services.AddTransient<QueryDispatcher>();

            return services;
        }

        private static bool IsCommandHandler(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICommandHandler<>);
        private static bool IsQueryHandler(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>);

    }
}
