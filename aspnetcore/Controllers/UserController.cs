using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : BaseController
    {
        public ApiResult Get()
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();

            return new ApiResult(data: ip);
        }

        [HttpPost]
        public ApiResult Post()
        {
            return new ApiResult(data: CurrentUser);
        }
    }
}