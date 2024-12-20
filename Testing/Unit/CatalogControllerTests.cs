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
        var mockDbContext = new Mock<CatalogDbContext>();
        var mockDbSet = DbSetMockHelper.CreateMockDbSet(new List<Product>().AsQueryable());
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
        var mockDbContext = new Mock<CatalogDbContext>();
        var mockDbSet = new Mock<DbSet<Product>>();
        mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

        var controller = new CatalogController(mockDbContext.Object);

        var newProduct = new Product { Name = "Book E", Price = 12.99m, Stock = 20 };

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

        var mockDbContext = new Mock<CatalogDbContext>();
        var mockDbSet = DbSetMockHelper.CreateMockDbSet(productList);
        mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

        var controller = new CatalogController(mockDbContext.Object);
        var stockUpdateRequest = new List<(int productId, int quantity)>
    {
        (1, 5)
    };

        // Act
        var result = await controller.ReduceStock(stockUpdateRequest) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Stock updated successfully.", result.Value);
        Assert.Equal(5, productList.First().Stock); // Mocking modifies the reference
    }
}
