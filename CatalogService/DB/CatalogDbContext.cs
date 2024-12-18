using CatalogService.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DB
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
