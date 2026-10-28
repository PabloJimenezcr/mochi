using Microsoft.EntityFrameworkCore;
using Ventas.Application.Command;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Commands
{
    public class CreateClientCommandTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldCreateClient_WhenValidClientProvided()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "John Doe", Identifier = "C001", Description = "Test client" };

            // Act
            var result = command.Execute(cliente);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.ClientId);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("C001", result.Identifier);

            // Verify it was saved
            var savedClient = context.Clients.FirstOrDefault(c => c.ClientId == result.ClientId);
            Assert.NotNull(savedClient);
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenNameIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = null, Identifier = "C001" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(cliente));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenNameIsEmpty()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "", Identifier = "C001" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(cliente));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenIdentifierIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "John", Identifier = null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(cliente));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenIdentifierIsEmpty()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "John", Identifier = "" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(cliente));
        }

        [Fact]
        public void Execute_ShouldTrimNameAndIdentifier()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "  John Doe  ", Identifier = "  C001  ", Description = "  Test  " };

            // Act
            var result = command.Execute(cliente);

            // Assert
            var savedClient = context.Clients.FirstOrDefault(c => c.ClientId == result.ClientId);
            Assert.Equal("John Doe", savedClient.Name);
            Assert.Equal("C001", savedClient.Identifier);
            Assert.Equal("Test", savedClient.Description);
        }

        [Fact]
        public void Execute_ShouldSetDescriptionToNull_WhenDescriptionIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "John", Identifier = "C001", Description = null };

            // Act
            var result = command.Execute(cliente);

            // Assert
            var savedClient = context.Clients.FirstOrDefault(c => c.ClientId == result.ClientId);
            Assert.Null(savedClient.Description);
        }

        [Fact]
        public void Execute_ShouldSetDescriptionToNull_WhenDescriptionIsWhitespace()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "John", Identifier = "C001", Description = "   " };

            // Act
            var result = command.Execute(cliente);

            // Assert
            var savedClient = context.Clients.FirstOrDefault(c => c.ClientId == result.ClientId);
            Assert.Null(savedClient.Description);
        }

        [Fact]
        public void Execute_ShouldAssignGeneratedId()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "Jane", Identifier = "C002" };

            // Act
            var result = command.Execute(cliente);

            // Assert
            Assert.NotEqual(0, result.ClientId);
            Assert.True(result.ClientId > 0);
        }

        [Fact]
        public void Execute_ShouldCreateMultipleClients()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente1 = new Cliente { Name = "Client1", Identifier = "ID1" };
            var cliente2 = new Cliente { Name = "Client2", Identifier = "ID2" };

            // Act
            var result1 = command.Execute(cliente1);
            var result2 = command.Execute(cliente2);

            // Assert
            Assert.NotEqual(result1.ClientId, result2.ClientId);
            Assert.Equal(2, context.Clients.Count());
        }

        [Fact]
        public void Execute_ShouldHandleSpecialCharactersInName()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateClientCommand(context);
            var cliente = new Cliente { Name = "José María García", Identifier = "C001" };

            // Act
            var result = command.Execute(cliente);

            // Assert
            var savedClient = context.Clients.FirstOrDefault(c => c.ClientId == result.ClientId);
            Assert.Equal("José María García", savedClient.Name);
        }
    }
}
