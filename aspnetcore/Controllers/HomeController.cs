using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}