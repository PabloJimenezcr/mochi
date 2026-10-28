using Microsoft.EntityFrameworkCore;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;

namespace Ventas.Application.Command
{
    public class CreateClientCommand : ICreateClientCommand
    {
        private readonly VentasContext _context;

        public CreateClientCommand(VentasContext context)
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

            var clientEntity = new Client
            {
                Identifier = cliente.Identifier.Trim(),
                Name = cliente.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(cliente.Description) ? null : cliente.Description.Trim()
            };

            _context.Clients.Add(clientEntity);
            _context.SaveChanges();

            cliente.ClientId = clientEntity.ClientId;
            return cliente;
        }
    }
}
