using Microsoft.AspNetCore.Mvc;
using Redis;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : BaseController
    {
        public UserController(RedisClient redisCli) : base(redisCli)
        {
        }

        public ApiResult Get()
        {
            return new ApiResult(data: CurrentUser);
        }
    }
}