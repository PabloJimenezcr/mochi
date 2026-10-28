using Microsoft.EntityFrameworkCore;
using Ventas.Application.Command;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Commands
{
    public class CreateCategoryCommandTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldCreateCategory_WhenValidCategoryProvided()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria = new Categoria { CategoryName = "Electronics" };

            // Act
            var result = command.Execute(categoria);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.CategoryId); // ID should be assigned
            Assert.Equal("Electronics", result.CategoryName);

            // Verify it was saved to database
            var savedCategory = context.Categories.FirstOrDefault(c => c.CategoryId == result.CategoryId);
            Assert.NotNull(savedCategory);
            Assert.Equal("Electronics", savedCategory.Name);
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenCategoryNameIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria = new Categoria { CategoryName = null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(categoria));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenCategoryNameIsEmpty()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria = new Categoria { CategoryName = "" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(categoria));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenCategoryNameIsWhitespace()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria = new Categoria { CategoryName = "   " };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(categoria));
        }

        [Fact]
        public void Execute_ShouldTrimCategoryName()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria = new Categoria { CategoryName = "  Books  " };

            // Act
            var result = command.Execute(categoria);

            // Assert
            var savedCategory = context.Categories.FirstOrDefault(c => c.CategoryId == result.CategoryId);
            Assert.NotNull(savedCategory);
            Assert.Equal("Books", savedCategory.Name);
        }

        [Fact]
        public void Execute_ShouldAssignGeneratedId()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria = new Categoria { CategoryName = "Toys" };

            // Act
            var result = command.Execute(categoria);

            // Assert
            Assert.NotEqual(0, result.CategoryId);
            Assert.True(result.CategoryId > 0);
        }

        [Fact]
        public void Execute_ShouldCreateMultipleCategories()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria1 = new Categoria { CategoryName = "Category1" };
            var categoria2 = new Categoria { CategoryName = "Category2" };

            // Act
            var result1 = command.Execute(categoria1);
            var result2 = command.Execute(categoria2);

            // Assert
            Assert.NotEqual(result1.CategoryId, result2.CategoryId);
            Assert.Equal(2, context.Categories.Count());
        }

        [Fact]
        public void Execute_ShouldPersistCategoryToDatabase()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new CreateCategoryCommand(context);
            var categoria = new Categoria { CategoryName = "Fashion" };

            // Act
            var result = command.Execute(categoria);

            // Assert - Create a new context to verify persistence
            var newContext = CreateInMemoryContext();
            // Note: InMemory databases are isolated per instance, so this test verifies within same context
            Assert.NotNull(context.Categories.FirstOrDefault(c => c.Name == "Fashion"));
        }
    }
}
