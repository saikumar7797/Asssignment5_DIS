using Microsoft.AspNetCore.Mvc;

namespace HealthyMe.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
