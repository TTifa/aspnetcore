using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult Error(string code)
        {
            ViewBag.StatusCode = code;
            return View();
        }
    }
}