using CatalogService.Controllers;
using CatalogService.DB;
using CatalogService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Testing;

public class CatalogControllerTests
{
    public CatalogControllerTests()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
    }

    [Fact]
    public async Task GetProducts_ReturnsProducts_WhenValidIdsProvided()
    {
        // Arrange
        var mockDbContext = new Mock<CatalogDbContext>();
        var productList = new List<Product>
        {
            new Product { Id = 1, Name = "Book A", Price = 10.99m, Stock = 50 },
            new Product { Id = 2, Name = "Book B", Price = 15.49m, Stock = 30 }
        }.AsQueryable();

        var mockDbSet = DbSetMockHelper.CreateMockDbSet(productList);
        mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

        var controller = new CatalogController(mockDbContext.Object);

        // Act
        var result = await controller.GetProducts(new List<int> { 1, 2 }) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var products = Assert.IsType<List<Product>>(result.Value);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task GetProducts_ReturnsNotFound_WhenInvalidIdsProvided()
    {
        // Arrange
        var productList = new List<Product>
    {
        new Product { Id = 1, Name = "Book A", Price = 10.99m, Stock = 50 }
    }.AsQueryable();

        var mockDbSet = DbSetMockHelper.CreateMockDbSet(productList);

        var mockDbContext = new Mock<CatalogDbContext>();
        mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

        var controller = new CatalogController(mockDbContext.Object);

        // Act
        var result = await controller.GetProducts(new List<int> { 999 }) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("No products found.", result.Value);
    }

    [Fact]
    public void CreateProduct_AddsProductToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var mockDbSet = DbSetMockHelper.CreateMockDbSet(new List<Product>().AsQueryable());
        var mockDbContext = new Mock<TestableCatalogDbContext>(options);

        mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

        var controller = new CatalogController(mockDbContext.Object);

        var newProduct = new Product { Name = "Book E", Price = 12.99m, Description = "Test Book E", Stock = 20 };

        // Act
        var result = controller.CreateProduct(newProduct) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
        mockDbSet.Verify(m => m.Add(It.IsAny<Product>()), Times.Once);
        mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task ReduceStock_DecreasesProductStock_WhenStockIsAvailable()
    {
        // Arrange
        var productList = new List<Product>
        {
            new Product { Id = 1, Name = "Book F", Stock = 10 }
        }.AsQueryable();

        var mockDbSet = DbSetMockHelper.CreateMockDbSet(productList); // Mocking the DbSet

        var mockDbContext = new Mock<CatalogDbContext>();
        mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);
        mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(1);

        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
        .ReturnsAsync((object[] keyValues) =>
        {
                var productId = (int)keyValues[0]; // Cast the first parameter as int
                return productList.FirstOrDefault(p => p.Id == productId); // Return the product with that Id
        });

        var controller = new CatalogController(mockDbContext.Object);
        var stockUpdateRequest = new List<(int productId, int quantity)>
        {
            (1, 5) // Request to reduce 5 units of product with ID 1
        };

        // Act
        var result = await controller.ReduceStock(stockUpdateRequest) as OkObjectResult;

        // Assert
        Assert.NotNull(result); // Ensure that the result is not null
        Assert.Equal(200, result.StatusCode); // Status code should be 200 OK
        Assert.Equal("Stock updated successfully.", result.Value); // Response message should match

        var updatedProduct = productList.First(); // Retrieve the updated product
        Assert.Equal(5, updatedProduct.Stock); // Ensure stock was updated correctly    }
    }
}
