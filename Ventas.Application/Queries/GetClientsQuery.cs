using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;

namespace Ventas.Application.Queries
{
    public class GetClientsQuery : IGetClientsQuery
    {
        private readonly VentasContext _clientsContext;

        public GetClientsQuery(VentasContext clientsContext)
        {
            _clientsContext = clientsContext;
        }

        public List<Cliente> Execute()
        {
            var clients = _clientsContext.Clients.Select(dbClient => new Cliente
            {
                ClientId = dbClient.ClientId,
                Identifier = dbClient.Identifier,
                Name = dbClient.Name,
                Description = dbClient.Description
            }).ToList();

            return clients;
        }

        public Cliente? GetById(int id)
        {
            var dbClient = _clientsContext.Clients.FirstOrDefault(c => c.ClientId == id);
            if (dbClient == null) return null;

            return new Cliente
            {
                ClientId = dbClient.ClientId,
                Identifier = dbClient.Identifier,
                Name = dbClient.Name,
                Description = dbClient.Description
            };
        }

        public Cliente? GetByName(string name)
        {
            var dbClient = _clientsContext.Clients.FirstOrDefault(c => c.Name == name);
            if (dbClient == null) return null;

            return new Cliente
            {
                ClientId = dbClient.ClientId,
                Identifier = dbClient.Identifier,
                Name = dbClient.Name,
                Description = dbClient.Description
            };
        }
    }
}
