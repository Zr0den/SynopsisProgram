using CatalogService.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DB
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public virtual DbSet<Product> Products { get; set; }

        // For mocking purposes
        protected CatalogDbContext() { }
    }
}
