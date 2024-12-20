using PactNet;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using CatalogService.Model;
using PactNet.Matchers;
using System.Text;

public class OrderServiceContractTests
{
    private readonly string _mockProviderServiceBaseUri = "http://localhost:9222"; //default port PactNet's mock server runs on

    [Fact]
    public async Task ItHandlesValidProductIdsCorrectly()
    {
        var config = new PactConfig
        {
            PactDir = "/pacts/",
            LogLevel = PactLogLevel.Debug
        };
        var pact = Pact.V3("OrderService", "CatalogService", config);
        var httpPact = pact.WithHttpInteractions();

        List<Product> exampleResponse = new List<Product>
        {
            new Product { Id = 1, Name = "Book A", Price = 10.99m, Stock = 50 },
            new Product { Id = 2, Name = "Book B", Price = 15.49m, Stock = 30 }
        };

        httpPact.UponReceiving("A request for product details")
            .WithRequest(HttpMethod.Get, "/api/catalog/products?productIds=1,2")
            .WillRespond()
            .WithStatus(System.Net.HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(Match.Type(exampleResponse));

        await httpPact.VerifyAsync(async ctx =>
        {
            using var client = new HttpClient { BaseAddress = new Uri(_mockProviderServiceBaseUri) };

            var response = await client.GetAsync("/api/catalog/products?productIds=1,2");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<dynamic>>(content);

            Assert.Equal(2, products.Count);
        });
    }

    [Fact]
    public async Task ItHandlesMissingProductsGracefully()
    {
        var config = new PactConfig
        {
            PactDir = "/pacts/",
            LogLevel = PactLogLevel.Debug
        };
        var pact = Pact.V3("OrderService", "CatalogService", config);
        var httpPact = pact.WithHttpInteractions();

        httpPact.UponReceiving("A request for non-existent product details")
            .WithRequest(HttpMethod.Get, "/api/catalog/products?productIds=99,100")
            .WillRespond()
            .WithStatus(System.Net.HttpStatusCode.NotFound)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(Match.Type(new { message = "Products not found" }));

        await httpPact.VerifyAsync(async ctx =>
        {
            using var client = new HttpClient { BaseAddress = new Uri(_mockProviderServiceBaseUri) };

            var response = await client.GetAsync("/api/catalog/products?productIds=99,100");

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<dynamic>(content);

            Assert.Equal("Products not found", (string)errorResponse.message);
        });
    }

    [Fact]
    public async Task ItHandlesStockReductionCorrectly()
    {
        var config = new PactConfig
        {
            PactDir = "/pacts/",
            LogLevel = PactLogLevel.Debug
        };
        var pact = Pact.V3("OrderService", "CatalogService", config);
        var httpPact = pact.WithHttpInteractions();

        httpPact.UponReceiving("A request to reduce product stock")
            .WithRequest(HttpMethod.Post, "/api/catalog/reduce-stock")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(Match.Type(new List<object>
            {
            new { productId = 1, quantity = 2 },
            new { productId = 2, quantity = 1 }
            }))
            .WillRespond()
            .WithStatus(System.Net.HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(Match.Type(new { message = "Stock updated successfully" }));

        await httpPact.VerifyAsync(async ctx =>
        {
            using var client = new HttpClient { BaseAddress = new Uri(_mockProviderServiceBaseUri) };

            var payload = JsonConvert.SerializeObject(new List<object>
        {
            new { productId = 1, quantity = 2 },
            new { productId = 2, quantity = 1 }
        });

            var response = await client.PostAsync("/api/catalog/reduce-stock",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var successResponse = JsonConvert.DeserializeObject<dynamic>(content);

            Assert.Equal("Stock updated successfully", (string)successResponse.message);
        });
    }
}