using Bogus;
using CatalogService.DB;
using CatalogService.Model;
using OrderService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public static class DataSeeder
    {
        public static async Task SeedData(CatalogDbContext dbContext)
        {
            if (!dbContext.Products.Any())
            {
                var fakeProducts = GenerateFakeProducts(10);

                dbContext.Products.AddRange(fakeProducts);
                await dbContext.SaveChangesAsync();
            }
        }

        public static List<Product> GenerateFakeProducts(int count)
        {

            var faker = new Faker<Product>()
             .RuleFor(p => p.Name, f => f.Commerce.ProductName())
             .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
             .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(5, 100)))
             .RuleFor(p => p.Stock, f => f.Random.Int(1, 100));

            return faker.Generate(count);
        }
    }
}
