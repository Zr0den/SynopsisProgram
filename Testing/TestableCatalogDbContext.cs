using CatalogService.DB;
using CatalogService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public class TestableCatalogDbContext : CatalogDbContext
    {
        public TestableCatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> ProductsOverride { get; set; }

        public override DbSet<Product> Products
        {
            get => ProductsOverride ?? base.Products;
            set => ProductsOverride = value;
        }
    }
}
