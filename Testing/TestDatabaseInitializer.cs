using CatalogService.DB;
using CatalogService.Model;

public static class TestDatabaseInitializer
{
    public static void InitializeTestDatabase(CatalogDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Products.AddRange(new[]
        {
            new Product { Name = "Test Book A", Price = 10.99m, Stock = 50 },
            new Product { Name = "Test Book B", Price = 15.49m, Stock = 30 },
        });

        context.SaveChanges();
    }
}