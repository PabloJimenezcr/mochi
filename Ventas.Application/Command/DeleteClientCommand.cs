using Microsoft.EntityFrameworkCore;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;

namespace Ventas.Application.Command
{
    public class DeleteClientCommand : IDeleteClientCommand
    {
        private readonly VentasContext _context;

        public DeleteClientCommand(VentasContext context)
        {
            _context = context;
        }

        public bool Execute(int clientId)
        {
            try
            {
                var clienteBD = _context.Clients.FirstOrDefault(c => c.ClientId == clientId);

                if (clienteBD == null)
                {
                    throw new ArgumentException($"El cliente #. {clientId} no existe.");
                }

                _context.Remove(clienteBD);
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
