using Microsoft.EntityFrameworkCore;
using Ventas.Application.Command;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Commands
{
    public class DeleteClientCommandTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldDeleteClient_WhenClientExists()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var client = new Client { ClientId = 1, Identifier = "C001", Name = "ToDelete", Description = null };
            context.Clients.Add(client);
            context.SaveChanges();

            var command = new DeleteClientCommand(context);

            // Act
            var result = command.Execute(1);

            // Assert
            Assert.True(result);
            Assert.Null(context.Clients.FirstOrDefault(c => c.ClientId == 1));
        }

        [Fact]
        public void Execute_ShouldReturnFalse_WhenClientDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new DeleteClientCommand(context);

            // Act
            var result = command.Execute(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Execute_ShouldDeleteOnlyTargetClient()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.AddRange(new List<Client>
            {
                new Client { ClientId = 1, Identifier = "C001", Name = "Client1", Description = null },
                new Client { ClientId = 2, Identifier = "C002", Name = "Client2", Description = null },
                new Client { ClientId = 3, Identifier = "C003", Name = "Client3", Description = null }
            });
            context.SaveChanges();

            var command = new DeleteClientCommand(context);

            // Act
            var result = command.Execute(2);

            // Assert
            Assert.True(result);
            Assert.NotNull(context.Clients.FirstOrDefault(c => c.ClientId == 1));
            Assert.Null(context.Clients.FirstOrDefault(c => c.ClientId == 2));
            Assert.NotNull(context.Clients.FirstOrDefault(c => c.ClientId == 3));
        }

        [Fact]
        public void Execute_ShouldReturnFalse_WhenDeleteMultipleTimes()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var client = new Client { ClientId = 1, Identifier = "C001", Name = "ToDelete", Description = null };
            context.Clients.Add(client);
            context.SaveChanges();

            var command = new DeleteClientCommand(context);

            // Act
            var firstDelete = command.Execute(1);
            var secondDelete = command.Execute(1);

            // Assert
            Assert.True(firstDelete);
            Assert.False(secondDelete);
        }

        [Fact]
        public void Execute_ShouldDecreaseCount()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.AddRange(new List<Client>
            {
                new Client { ClientId = 1, Identifier = "C001", Name = "Client1", Description = null },
                new Client { ClientId = 2, Identifier = "C002", Name = "Client2", Description = null }
            });
            context.SaveChanges();

            var initialCount = context.Clients.Count();
            var command = new DeleteClientCommand(context);

            // Act
            var result = command.Execute(1);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCount - 1, context.Clients.Count());
        }

        [Fact]
        public void Execute_ShouldReturnFalse_WhenZeroIdProvided()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new DeleteClientCommand(context);

            // Act
            var result = command.Execute(0);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Execute_ShouldReturnFalse_WhenNegativeIdProvided()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new DeleteClientCommand(context);

            // Act
            var result = command.Execute(-1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Execute_ShouldPermanentlyDeleteClient()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "ToDelete", Description = null });
            context.SaveChanges();

            var command = new DeleteClientCommand(context);
            command.Execute(1);

            // Act
            var remainingClient = context.Clients.FirstOrDefault(c => c.ClientId == 1);

            // Assert
            Assert.Null(remainingClient);
        }

        [Fact]
        public void Execute_ShouldHandleClientWithDescription()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "ToDelete", Description = "Some Description" });
            context.SaveChanges();

            var command = new DeleteClientCommand(context);

            // Act
            var result = command.Execute(1);

            // Assert
            Assert.True(result);
            Assert.Null(context.Clients.FirstOrDefault(c => c.ClientId == 1));
        }
    }
}
