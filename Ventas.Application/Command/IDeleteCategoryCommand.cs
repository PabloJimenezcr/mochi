namespace Ventas.Application.Command
{
    public interface IDeleteCategoryCommand
    {
        bool Execute(int categoryid);
    }
}
