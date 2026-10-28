namespace Ventas.Application.Command
{
    public interface IDeleteClientCommand
    {
        bool Execute(int clientId);
    }
}
