using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    public class StatController : Controller
    {
        public IActionResult Bill()
        {
            return View();
        }
    }
}