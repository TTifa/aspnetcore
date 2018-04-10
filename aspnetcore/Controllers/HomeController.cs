﻿using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Encode()
        {
            return View();
        }

        public IActionResult Bill()
        {
            return View();
        }

        public IActionResult Error(string code)
        {
            if (code == "401")
                return Redirect("/Account/SignIn");

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