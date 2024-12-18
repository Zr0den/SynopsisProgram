using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
