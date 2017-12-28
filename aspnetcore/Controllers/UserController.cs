using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : BaseController
    {
        public ApiResult Get()
        {
            return new ApiResult(data: CurrentUser);
        }

        [HttpPost]
        public ApiResult Post()
        {
            return new ApiResult(data: CurrentUser);
        }
    }
}