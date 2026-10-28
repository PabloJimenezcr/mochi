using Ventas.Application.ValueObjects;

namespace Ventas.Application.Queries
{
    public interface IGetProductsQuery
    {
        List<Producto> Execute();
    }
}
