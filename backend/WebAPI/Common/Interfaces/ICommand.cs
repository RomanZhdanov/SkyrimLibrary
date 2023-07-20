namespace SkyrimLibrary.WebAPI.Common.Interfaces
{
    public interface ICommand { }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
