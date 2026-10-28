using Microsoft.EntityFrameworkCore;
using Ventas.Application.Command;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Commands
{
    public class DeleteCategoryCommandTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldDeleteCategory_WhenCategoryExists()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "ToDelete" };
            context.Categories.Add(category);
            context.SaveChanges();

            var command = new DeleteCategoryCommand(context);

            // Act
            var result = command.Execute(1);

            // Assert
            Assert.True(result);
            Assert.Null(context.Categories.FirstOrDefault(c => c.CategoryId == 1));
        }

        [Fact]
        public void Execute_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new DeleteCategoryCommand(context);

            // Act
            var result = command.Execute(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Execute_ShouldNotDeleteCategory_WhenCategoryHasProducts()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "With products" };
            context.Categories.Add(category);
            context.Products.Add(new Product
            {
                ProductId = 1,
                Name = "Product 1",
                CategoryId = category.CategoryId,
                Price = 10m,
                Category = category
            });
            context.SaveChanges();

            var command = new DeleteCategoryCommand(context);

            // Act
            var result = command.Execute(category.CategoryId);

            // Assert
            Assert.False(result);
            Assert.NotNull(context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId));
            Assert.NotNull(context.Products.FirstOrDefault(p => p.CategoryId == category.CategoryId));
        }

        [Fact]
        public void Execute_ShouldDeleteOnlyTargetCategory()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.AddRange(new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category1" },
                new Category { CategoryId = 2, Name = "Category2" },
                new Category { CategoryId = 3, Name = "Category3" }
            });
            context.SaveChanges();

            var command = new DeleteCategoryCommand(context);

            // Act
            var result = command.Execute(2);

            // Assert
            Assert.True(result);
            Assert.NotNull(context.Categories.FirstOrDefault(c => c.CategoryId == 1));
            Assert.Null(context.Categories.FirstOrDefault(c => c.CategoryId == 2));
            Assert.NotNull(context.Categories.FirstOrDefault(c => c.CategoryId == 3));
        }

        [Fact]
        public void Execute_ShouldReturnFalse_WhenDeleteMultipleTimes()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "ToDelete" };
            context.Categories.Add(category);
            context.SaveChanges();

            var command = new DeleteCategoryCommand(context);

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
            context.Categories.AddRange(new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category1" },
                new Category { CategoryId = 2, Name = "Category2" }
            });
            context.SaveChanges();

            var initialCount = context.Categories.Count();
            var command = new DeleteCategoryCommand(context);

            // Act
            var result = command.Execute(1);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCount - 1, context.Categories.Count());
        }

        [Fact]
        public void Execute_ShouldReturnFalse_WhenZeroIdProvided()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var command = new DeleteCategoryCommand(context);

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
            var command = new DeleteCategoryCommand(context);

            // Act
            var result = command.Execute(-1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Execute_ShouldPermanentlyDeleteCategory()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.Add(new Category { CategoryId = 1, Name = "ToDelete" });
            context.SaveChanges();

            var command = new DeleteCategoryCommand(context);
            command.Execute(1);

            // Act
            var remainingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == 1);

            // Assert
            Assert.Null(remainingCategory);
        }
    }
}
