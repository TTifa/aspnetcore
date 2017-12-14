using aspnetcore.Filters;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    public class HomeController : Controller
    {
        [Log]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }
    }
}