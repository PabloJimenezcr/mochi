using Ventas.Application.ValueObjects;

namespace Ventas.Application.Queries
{
    public interface IGetClientsQuery
    {
        List<Cliente> Execute();
        Cliente? GetById(int id);
        Cliente? GetByName(string name);
    }
}
