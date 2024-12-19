using CatalogService.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DB
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "Server=catalog.mysql;Database=catalog_db;User=catalog_user;Password=catalog_password;",
                new MySqlServerVersion(new Version(8, 0, 29)),
                options => options.EnableRetryOnFailure());
        }
    }
}
