using Microsoft.EntityFrameworkCore;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;

namespace Ventas.Application.Command
{
    public class DeleteCategoryCommand : IDeleteCategoryCommand
    {
        private readonly VentasContext _context;

        public DeleteCategoryCommand(VentasContext context)
        {
            _context = context;
        }

        public bool Execute(int categoryid)
        {

            try
            {

                // 
                var categoriaBD = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryid);

                if (categoriaBD == null)
                {
                    throw new ArgumentException($"La categoria #. {categoryid} no existe.");
                }

                var tieneProductos = _context.Products.Any(product => product.CategoryId == categoryid);

                if (tieneProductos)
                {
                    return false;
                }

                _context.Remove(categoriaBD);

                _context.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
