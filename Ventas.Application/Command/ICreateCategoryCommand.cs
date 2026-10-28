using Ventas.Application.ValueObjects;

namespace Ventas.Application.Command
{
    public interface ICreateCategoryCommand
    {
        Categoria Execute(Categoria categoria);
    }
}
