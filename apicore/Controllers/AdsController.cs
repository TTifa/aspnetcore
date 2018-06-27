using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis;
using System.Linq;
using WebApiBase;

namespace apicore.Controllers
{
    [Produces("application/json")]
    [Route("api/Ads")]
    public class AdsController : BaseController
    {
        public AdsController(RedisClient redisCli, TtifaContext ttifaContext) : base(redisCli, ttifaContext)
        {
        }

        [HttpGet("category")]
        public ApiResult Category(int id)
        {
            var banner = _db.ads.FirstOrDefault(o => o.Match == $"category:{id}" && o.State == AdState.enable);
            if (banner == null)
                return new ApiResult(ApiStatus.Fail, "get banner fail");

            return new ApiResult(data: banner);
        }

        [HttpGet("banner")]
        public ApiResult Banner(string match)
        {
            var banners = _db.ads.Where(o => o.Match == match && o.State == AdState.enable).AsNoTracking().ToList();
            if (banners == null)
                return new ApiResult(ApiStatus.Fail, "get banner fail");

            return new ApiResult(data: banners);
        }
    }
}