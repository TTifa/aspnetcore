using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/Category")]
    public class CategoryController : Controller
    {
        private TtifaContext _db;
        public CategoryController(TtifaContext ttifaContext)
        {
            _db = ttifaContext;
        }

        [HttpGet]
        public ApiResult Get(int pid)
        {
            var list = _db.categorys.Where(o => o.ParentId == pid).AsNoTracking().ToList();

            return new ApiResult(data: list);
        }

        [HttpGet("banner")]
        public ApiResult Banner(int id)
        {
            var banner = _db.ads.FirstOrDefault(o => o.Match == $"category:{id}" && o.State == AdState.enable);
            if (banner == null)
                return new ApiResult(ApiStatus.Fail, "get banner fail");

            return new ApiResult(data: banner);
        }
    }
}