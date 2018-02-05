using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [ResponseCache(CacheProfileName = "default")]
        public ApiResult Time()
        {
            return new ApiResult(data: DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}