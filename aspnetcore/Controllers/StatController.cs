using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    [Authorize]
    public class StatController : Controller
    {
        public IActionResult Bill()
        {
            return View();
        }
    }
}