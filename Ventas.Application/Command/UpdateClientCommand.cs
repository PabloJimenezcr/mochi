using Microsoft.EntityFrameworkCore;
using Ventas.Application.ValueObjects;
using Ventas.Application.Queries;
using Ventas.Persistence.Model;

namespace Ventas.Application.Command
{
    public class UpdateClientCommand : IUpdateClientCommand
    {
        private readonly VentasContext _context;

        public UpdateClientCommand(VentasContext context)
        {
            _context = context;
        }

        public Cliente Execute(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Name))
            {
                throw new ArgumentException("El nombre del cliente es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(cliente.Identifier))
            {
                throw new ArgumentException("El identificador del cliente es obligatorio.");
            }

            var clienteBD = _context.Clients.FirstOrDefault(c => c.ClientId == cliente.ClientId);

            if (clienteBD == null)
            {
                throw new ArgumentException($"El cliente #. {cliente.ClientId} no existe.");
            }

            clienteBD.Name = cliente.Name.Trim();
            clienteBD.Identifier = cliente.Identifier.Trim();
            clienteBD.Description = string.IsNullOrWhiteSpace(cliente.Description) ? null : cliente.Description.Trim();

            _context.SaveChanges();

            return cliente;
        }
    }
}
