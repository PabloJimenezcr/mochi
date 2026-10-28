using Microsoft.EntityFrameworkCore;
using Ventas.Application.Queries;
using Ventas.Persistence.Model;
using Xunit;

namespace Ventas.Tests.Queries
{
    public class GetProductsQueryTests
    {
        private VentasContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VentasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new VentasContext(options);
        }

        [Fact]
        public void Execute_ShouldReturnAllProducts_WhenProductsExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "Electronics" };
            context.Categories.Add(category);
            context.Products.AddRange(new List<Product>
            {
                new Product { ProductId = 1, Name = "Laptop", CategoryId = 1, Price = 999.99m, Category = category },
                new Product { ProductId = 2, Name = "Mouse", CategoryId = 1, Price = 29.99m, Category = category },
                new Product { ProductId = 3, Name = "Keyboard", CategoryId = 1, Price = 79.99m, Category = category }
            });
            context.SaveChanges();

            var query = new GetProductsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.ProductId == 1 && p.ProductName == "Laptop");
            Assert.Contains(result, p => p.ProductId == 2 && p.ProductName == "Mouse");
        }

        [Fact]
        public void Execute_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var query = new GetProductsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Execute_ShouldMapProductNameCorrectly()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "Electronics" };
            context.Categories.Add(category);
            context.Products.Add(new Product 
            { 
                ProductId = 1, 
                Name = "TestProduct", 
                CategoryId = 1, 
                Price = 50.00m,
                Category = category 
            });
            context.SaveChanges();

            var query = new GetProductsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Single(result);
            Assert.Equal("TestProduct", result[0].ProductName);
            Assert.Equal(1, result[0].ProductId);
        }

        [Fact]
        public void Execute_ShouldIncludeCategoryName()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "Books" };
            context.Categories.Add(category);
            context.Products.Add(new Product 
            { 
                ProductId = 1, 
                Name = "C# Guide", 
                CategoryId = 1, 
                Price = 45.99m,
                Category = category 
            });
            context.SaveChanges();

            var query = new GetProductsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Single(result);
            Assert.Equal("Books", result[0].CategoryName);
        }

        [Fact]
        public void Execute_ShouldIncludePrice()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "Electronics" };
            context.Categories.Add(category);
            context.Products.Add(new Product 
            { 
                ProductId = 1, 
                Name = "Monitor", 
                CategoryId = 1, 
                Price = 299.99m,
                Category = category 
            });
            context.SaveChanges();

            var query = new GetProductsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Single(result);
            Assert.Equal(299.99m, result[0].Price);
        }

        [Fact]
        public void Execute_ShouldHandleNullImage()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var category = new Category { CategoryId = 1, Name = "Electronics" };
            context.Categories.Add(category);
            context.Products.Add(new Product 
            { 
                ProductId = 1, 
                Name = "Product", 
                CategoryId = 1, 
                Price = 100.00m,
                Image = null,
                Category = category 
            });
            context.SaveChanges();

            var query = new GetProductsQuery(context);

            // Act
            var result = query.Execute();

            // Assert
            Assert.Single(result);
            Assert.Null(result[0].Image);
        }
    }
}
