using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApiBase;

namespace apicore.Controllers
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
    }
}