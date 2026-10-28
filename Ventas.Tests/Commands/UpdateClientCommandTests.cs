using Microsoft.EntityFrameworkCore;
using Ventas.Application.Command;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Commands
{
    public class UpdateClientCommandTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldUpdateClient_WhenValidClientProvided()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var client = new Client { ClientId = 1, Identifier = "C001", Name = "Old Name", Description = "Old" };
            context.Clients.Add(client);
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "New Name", Identifier = "C001-NEW", Description = "New" };

            // Act
            var result = command.Execute(clientUpdate);

            // Assert
            Assert.Equal("New Name", result.Name);
            var updated = context.Clients.FirstOrDefault(c => c.ClientId == 1);
            Assert.Equal("New Name", updated.Name);
            Assert.Equal("C001-NEW", updated.Identifier);
            Assert.Equal("New", updated.Description);
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenNameIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Existing", Description = null });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = null, Identifier = "C001" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(clientUpdate));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenNameIsEmpty()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Existing", Description = null });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "", Identifier = "C001" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(clientUpdate));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenIdentifierIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Existing", Description = null });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "Test", Identifier = null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(clientUpdate));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenIdentifierIsEmpty()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Existing", Description = null });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "Test", Identifier = "" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(clientUpdate));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenClientDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 999, Name = "Test", Identifier = "C999" };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => command.Execute(clientUpdate));
            Assert.Contains("999", exception.Message);
        }

        [Fact]
        public void Execute_ShouldTrimPropertiesBeforeUpdate()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Old", Description = "Old" });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "  New Name  ", Identifier = "  C001-UPDATE  ", Description = "  Updated  " };

            // Act
            command.Execute(clientUpdate);

            // Assert
            var updated = context.Clients.FirstOrDefault(c => c.ClientId == 1);
            Assert.Equal("New Name", updated.Name);
            Assert.Equal("C001-UPDATE", updated.Identifier);
            Assert.Equal("Updated", updated.Description);
        }

        [Fact]
        public void Execute_ShouldSetDescriptionToNull_WhenDescriptionIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Test", Description = "OldDesc" });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "Test", Identifier = "C001", Description = null };

            // Act
            command.Execute(clientUpdate);

            // Assert
            var updated = context.Clients.FirstOrDefault(c => c.ClientId == 1);
            Assert.Null(updated.Description);
        }

        [Fact]
        public void Execute_ShouldSetDescriptionToNull_WhenDescriptionIsWhitespace()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Test", Description = "OldDesc" });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "Test", Identifier = "C001", Description = "   " };

            // Act
            command.Execute(clientUpdate);

            // Assert
            var updated = context.Clients.FirstOrDefault(c => c.ClientId == 1);
            Assert.Null(updated.Description);
        }

        [Fact]
        public void Execute_ShouldUpdateOnlyTargetClient()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.AddRange(new List<Client>
            {
                new Client { ClientId = 1, Identifier = "C001", Name = "Client1", Description = "Desc1" },
                new Client { ClientId = 2, Identifier = "C002", Name = "Client2", Description = "Desc2" }
            });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 1, Name = "UpdatedClient1", Identifier = "C001-UPD", Description = "Updated" };

            // Act
            command.Execute(clientUpdate);

            // Assert
            var updated1 = context.Clients.FirstOrDefault(c => c.ClientId == 1);
            var unchanged2 = context.Clients.FirstOrDefault(c => c.ClientId == 2);

            Assert.Equal("UpdatedClient1", updated1.Name);
            Assert.Equal("Client2", unchanged2.Name);
        }

        [Fact]
        public void Execute_ShouldPreserveClientIdAfterUpdate()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 5, Identifier = "C005", Name = "Test", Description = null });
            context.SaveChanges();

            var command = new UpdateClientCommand(context);
            var clientUpdate = new Cliente { ClientId = 5, Name = "Updated", Identifier = "C005-UPD" };

            // Act
            var result = command.Execute(clientUpdate);

            // Assert
            Assert.Equal(5, result.ClientId);
        }
    }
}
