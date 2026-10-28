using Microsoft.EntityFrameworkCore;
using Ventas.Application.Queries;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Queries
{
    public class GetClientsQueryTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldReturnAllClients_WhenClientsExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.AddRange(new List<Client>
            {
                new Client { ClientId = 1, Identifier = "C001", Name = "John Doe", Description = "Regular client" },
                new Client { ClientId = 2, Identifier = "C002", Name = "Jane Smith", Description = "Premium client" },
                new Client { ClientId = 3, Identifier = "C003", Name = "Bob Johnson", Description = null }
            });
            context.SaveChanges();

            var query = new GetClientsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, c => c.ClientId == 1 && c.Name == "John Doe");
            Assert.Contains(result, c => c.ClientId == 2 && c.Identifier == "C002");
        }

        [Fact]
        public void Execute_ShouldReturnEmptyList_WhenNoClientsExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var query = new GetClientsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetById_ShouldReturnClient_WhenClientExists()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Test Client", Description = "Test" });
            context.SaveChanges();

            var query = new GetClientsQuery(context);

            // Act
            var result = query.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ClientId);
            Assert.Equal("Test Client", result.Name);
            Assert.Equal("C001", result.Identifier);
            Assert.Equal("Test", result.Description);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var query = new GetClientsQuery(context);

            // Act
            var result = query.GetById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetByName_ShouldReturnClient_WhenClientExists()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Alice Brown", Description = "Description" });
            context.SaveChanges();

            var query = new GetClientsQuery(context);

            // Act
            var result = query.GetByName("Alice Brown");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice Brown", result.Name);
            Assert.Equal("C001", result.Identifier);
        }

        [Fact]
        public void GetByName_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var query = new GetClientsQuery(context);

            // Act
            var result = query.GetByName("NonexistentClient");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Execute_ShouldHandleNullDescription()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "C001", Name = "Test", Description = null });
            context.SaveChanges();

            var query = new GetClientsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Single(result);
            Assert.Null(result[0].Description);
        }

        [Fact]
        public void Execute_ShouldMapClientIdentifierCorrectly()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Clients.Add(new Client { ClientId = 1, Identifier = "UNIQUE-ID-123", Name = "Test", Description = "" });
            context.SaveChanges();

            var query = new GetClientsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Single(result);
            Assert.Equal("UNIQUE-ID-123", result[0].Identifier);
        }
    }
}
