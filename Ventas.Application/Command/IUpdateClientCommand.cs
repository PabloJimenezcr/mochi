using Ventas.Application.ValueObjects;

namespace Ventas.Application.Command
{
    public interface IUpdateClientCommand
    {
        Cliente Execute(Cliente cliente);
    }
}
