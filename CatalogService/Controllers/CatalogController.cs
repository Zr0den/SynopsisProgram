using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "Pong!";
        }
    }
}
