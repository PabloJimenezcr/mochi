using Ventas.Application.ValueObjects;

namespace Ventas.Application.Command
{
    public interface ICreateClientCommand
    {
        Cliente Execute(Cliente cliente);
    }
}
