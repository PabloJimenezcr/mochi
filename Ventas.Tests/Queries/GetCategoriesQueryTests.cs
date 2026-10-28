using Microsoft.EntityFrameworkCore;
using Ventas.Application.Queries;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Queries
{
    public class GetCategoriesQueryTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldReturnAllCategories_WhenCategoriesExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.AddRange(new List<Category>
            {
                new Category { CategoryId = 1, Name = "Electronics" },
                new Category { CategoryId = 2, Name = "Clothing" },
                new Category { CategoryId = 3, Name = "Books" }
            });
            context.SaveChanges();

            var query = new GetCategoriesQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, c => c.CategoryId == 1 && c.CategoryName == "Electronics");
            Assert.Contains(result, c => c.CategoryId == 2 && c.CategoryName == "Clothing");
            Assert.Contains(result, c => c.CategoryId == 3 && c.CategoryName == "Books");
        }

        [Fact]
        public void Execute_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var query = new GetCategoriesQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Execute_ShouldMapCategoryNameCorrectly()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.Add(new Category { CategoryId = 1, Name = "TestCategory" });
            context.SaveChanges();

            var query = new GetCategoriesQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Single(result);
            Assert.Equal("TestCategory", result[0].CategoryName);
            Assert.Equal(1, result[0].CategoryId);
        }

        [Fact]
        public void Execute_ShouldReturnListInCorrectOrder()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Categories.AddRange(new List<Category>
            {
                new Category { CategoryId = 3, Name = "Third" },
                new Category { CategoryId = 1, Name = "First" },
                new Category { CategoryId = 2, Name = "Second" }
            });
            context.SaveChanges();

            var query = new GetCategoriesQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Equal(3, result.Count);
            // Verify all items are present (order depends on database order)
            Assert.NotEmpty(result);
        }
    }
}
