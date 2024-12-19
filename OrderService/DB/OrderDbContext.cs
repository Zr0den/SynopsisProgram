using Microsoft.EntityFrameworkCore;
using OrderService.Model;

namespace OrderService.DB
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "Server=order.mysql;Database=order_db;User=order_user;Password=order_password;",
                new MySqlServerVersion(new Version(8, 0, 29)),
                options => options.EnableRetryOnFailure());
        }
    }
}
