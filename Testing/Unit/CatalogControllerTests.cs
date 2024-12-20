using CatalogService.Controllers;
using CatalogService.DB;
using CatalogService.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class CatalogControllerTests
{
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
}
