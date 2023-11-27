using Microsoft.AspNetCore.Mvc;

namespace HealthyMe.Controllers
{
    public class AboutUs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
