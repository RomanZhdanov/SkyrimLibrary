using SkyrimLibrary.WebAPI.Common.Interfaces;

namespace SkyrimLibrary.WebAPI.Common.Models
{
    public class QueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>();
            if (handler != null)
            {
                return await handler.Handle(query);
            }
            else
            {
                throw new NotSupportedException($"No query handler registered for {typeof(TQuery).Name}.");
            }
        }
    }
}
