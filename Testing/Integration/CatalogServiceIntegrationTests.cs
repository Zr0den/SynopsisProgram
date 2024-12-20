using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CatalogService.DB;
using CatalogService.Model;
using Testing;

public class CatalogServiceIntegrationTests : IDisposable
{
    private readonly CatalogDbContext _dbContext;

    public CatalogServiceIntegrationTests()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: "CatalogTestDb")
            .Options;

        _dbContext = new CatalogDbContext(options);

        // Initialize the test database
        TestDatabaseInitializer.InitializeTestDatabase(_dbContext);
        DataSeeder.SeedData(_dbContext);
    }

    [Fact]
    public async Task AddProduct_SavesProductToDatabase()
    {
        // Arrange
        var product = new Product { Name = "Test Book C", Price = 20.00m, Stock = 10 };

        // Act
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // Assert
        var savedProduct = await _dbContext.Products.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal("Test Book C", savedProduct.Name);
    }

    [Fact]
    public async Task GetProductById_ReturnsCorrectProduct()
    {
        // Arrange
        var product = new Product { Name = "Book B", Price = 15.49m, Stock = 30 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // Act
        var savedProduct = await _dbContext.Products.FindAsync(product.Id);

        // Assert
        Assert.NotNull(savedProduct);
        Assert.Equal("Book B", savedProduct.Name);
        Assert.Equal(15.49m, savedProduct.Price);
    }

    [Fact]
    public async Task UpdateProduct_UpdatesProductInDatabase()
    {
        // Arrange
        var product = new Product { Name = "Book C", Price = 20.00m, Stock = 25 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // Act
        product.Stock = 15;
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedProduct = await _dbContext.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(15, updatedProduct.Stock);
    }

    [Fact]
    public async Task DeleteProduct_RemovesProductFromDatabase()
    {
        // Arrange
        var product = new Product { Name = "Book D", Price = 25.00m, Stock = 10 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // Act
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedProduct = await _dbContext.Products.FindAsync(product.Id);
        Assert.Null(deletedProduct);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted(); // Clean up database after each test
        _dbContext.Dispose();
    }
}