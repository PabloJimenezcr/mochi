using Ventas.Application.ValueObjects;

namespace Ventas.Application.Command
{
    public interface IUpdateCategoryCommand
    {
        Categoria Execute(Categoria categoria);
    }
}
