using Microsoft.EntityFrameworkCore;
using Ventas.Application.Command;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Commands
{
    public class UpdateCategoryCommandTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldUpdateCategory_WhenValidCategoryProvided()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "Old Name" };
            context.Categories.Add(category);
            context.SaveChanges();

            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 1, CategoryName = "New Name" };

            // Act
            var result = command.Execute(categoriaUpdate);

            // Assert
            Assert.Equal("New Name", result.CategoryName);
            var updatedCategory = context.Categories.FirstOrDefault(c => c.CategoryId == 1);
            Assert.Equal("New Name", updatedCategory.Name);
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenCategoryNameIsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.Add(new Category { CategoryId = 1, Name = "Existing" });
            context.SaveChanges();

            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 1, CategoryName = null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(categoriaUpdate));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenCategoryNameIsEmpty()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.Add(new Category { CategoryId = 1, Name = "Existing" });
            context.SaveChanges();

            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 1, CategoryName = "" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(categoriaUpdate));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenCategoryNameIsWhitespace()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.Add(new Category { CategoryId = 1, Name = "Existing" });
            context.SaveChanges();

            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 1, CategoryName = "   " };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute(categoriaUpdate));
        }

        [Fact]
        public void Execute_ShouldThrowArgumentException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 999, CategoryName = "NonExistent" };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => command.Execute(categoriaUpdate));
            Assert.Contains("999", exception.Message);
        }

        [Fact]
        public void Execute_ShouldUpdateOnlyTargetCategory()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.AddRange(new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category1" },
                new Category { CategoryId = 2, Name = "Category2" }
            });
            context.SaveChanges();

            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 1, CategoryName = "UpdatedCategory1" };

            // Act
            command.Execute(categoriaUpdate);

            // Assert
            var updated = context.Categories.FirstOrDefault(c => c.CategoryId == 1);
            var unchanged = context.Categories.FirstOrDefault(c => c.CategoryId == 2);

            Assert.Equal("UpdatedCategory1", updated.Name);
            Assert.Equal("Category2", unchanged.Name);
        }

        [Fact]
        public void Execute_ShouldUpdateCategoryWithSpecialCharacters()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.Add(new Category { CategoryId = 1, Name = "Old" });
            context.SaveChanges();

            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 1, CategoryName = "Electronics & Gadgets" };

            // Act
            var result = command.Execute(categoriaUpdate);

            // Assert
            Assert.Equal("Electronics & Gadgets", result.CategoryName);
            var updated = context.Categories.FirstOrDefault(c => c.CategoryId == 1);
            Assert.Equal("Electronics & Gadgets", updated.Name);
        }

        [Fact]
        public void Execute_ShouldPreserveCategoryIdAfterUpdate()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.Add(new Category { CategoryId = 5, Name = "Original" });
            context.SaveChanges();

            var command = new UpdateCategoryCommand(context);
            var categoriaUpdate = new Categoria { CategoryId = 5, CategoryName = "Updated" };

            // Act
            var result = command.Execute(categoriaUpdate);

            // Assert
            Assert.Equal(5, result.CategoryId);
        }
    }
}
