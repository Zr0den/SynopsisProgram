using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CatalogService.DB;
using CatalogService.Model;

public class CatalogServiceIntegrationTests
{
    private CatalogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: "CatalogTestDb")
            .Options;

        return new CatalogDbContext(options);
    }

    [Fact]
    public async Task AddProduct_SavesProductToDatabase()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var product = new Product { Name = "Book A", Price = 10.99m, Stock = 50 };

        // Act
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Assert
        var savedProduct = await dbContext.Products.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal("Book A", savedProduct.Name);
    }
}