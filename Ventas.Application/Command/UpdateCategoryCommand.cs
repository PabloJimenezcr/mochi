using Microsoft.EntityFrameworkCore;
using Ventas.Application.ValueObjects;
using Ventas.Application.Queries;
using Ventas.Persistence.Model;

namespace Ventas.Application.Command
{
    public class UpdateCategoryCommand : IUpdateCategoryCommand
    {
        private readonly VentasContext _context;

        public UpdateCategoryCommand(VentasContext context)
        {
            _context = context;
        }

        public Categoria Execute(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.CategoryName))
            {
                throw new ArgumentException("El nombre de la categoría es obligatorio.");
            }

            //obtener la categoria por medio del id recibido

            var categoriaBD = _context.Categories.FirstOrDefault(c => c.CategoryId == categoria.CategoryId);

            if (categoriaBD == null)
            {
                throw new ArgumentException($"La categoria #. {categoria.CategoryId} no existe." );
            }

            categoriaBD.Name = categoria.CategoryName;

            _context.SaveChanges();

            return categoria;
        }
    }
}
