using Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderService.DB;
using OrderService.Model;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly CatalogServiceClient _catalogServiceClient;

        public OrderController(OrderDbContext context, CatalogServiceClient catalogServiceClient)
        {
            _context = context;
            _catalogServiceClient = catalogServiceClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto createOrderDto)
        {
            var productIds = createOrderDto.ProductIds.Split(',')
                                                      .Select(int.Parse)
                                                      .ToList();

            
            // Fetch product details from CatalogService
            var products = await _catalogServiceClient.GetProductsAsync(productIds);
            if (products == null || products.Count != productIds.Count)
            {
                return NotFound("One or more products were not found.");
            }

            // Calculate total price
            var totalPrice = products.Sum(p => p.Price);

            // Reduce stock in CatalogService
            var stockUpdateResult = await _catalogServiceClient.ReduceStockAsync(
                productIds.Select(id => (id, 1)).ToList() // We assume quantity = 1 for each product
            );

            if (!stockUpdateResult)
            {
                return BadRequest("Failed to update stock.");
            }

            // Create the order
            var order = new Order
            {
                CustomerId = createOrderDto.CustomerId,
                ProductIds = createOrderDto.ProductIds,
                TotalPrice = totalPrice,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet]
        public string Get()
        {
            return "Pong!";
        }
    }
}
