using Microsoft.AspNetCore.Mvc;
using System;

namespace apicore.Controllers
{
    [Produces("application/json")]
    [Route("api/Default")]
    public class DefaultController : Controller
    {
        public string Get()
        {
            return DateTime.Now.ToString();
        }
    }
}