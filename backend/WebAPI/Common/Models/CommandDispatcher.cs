using SkyrimLibrary.WebAPI.Common.Interfaces;

namespace SkyrimLibrary.WebAPI.Common.Models
{
    public class CommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
            if (handler != null)
            {
                handler.Handle(command);
            }
            else
            {
                throw new NotSupportedException($"No command handler registered for {typeof(TCommand).Name}.");
            }
        }
    }
}
