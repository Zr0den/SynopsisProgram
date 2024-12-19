using CatalogService.DB;
using CatalogService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers
{
    [Route("api/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogDbContext _context;

        public CatalogController(CatalogDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] List<int> productIds)
        {
            var products = await _context.Products
                                         .Where(p => productIds.Contains(p.Id))
                                         .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }

        [HttpPost("reduce-stock")]
        public async Task<IActionResult> ReduceStock([FromBody] List<(int productId, int quantity)> items)
        {
            foreach (var item in items)
            {
                var product = await _context.Products.FindAsync(item.productId);
                if (product == null || product.Stock < item.quantity)
                {
                    return BadRequest($"Insufficient stock for Product ID {item.productId}.");
                }

                product.Stock -= item.quantity;
            }

            await _context.SaveChangesAsync();
            return Ok("Stock updated successfully.");
        }

        [HttpGet]
        public string Get()
        {
            return "Ping!";
        }
    }
}
