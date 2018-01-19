using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore.Filters;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace aspnetcore.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("local")]
    public class ArticleController : Controller
    {
        private TtifaContext _db;
        public ArticleController(TtifaContext db)
        {
            _db = db;
        }

        // GET api/values
        [HttpGet, Log]
        public ApiResult Get()
        {
            var articles = _db.articles.OrderBy(o => o.Id).AsNoTracking().ToList();

            return new ApiResult(data: articles);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ApiResult Get(int id)
        {
            throw new Exception("unkown error");
        }

        // POST api/values
        [HttpPost]
        public void Post()
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [DisableCors]
        public void Delete(int id)
        {
        }
    }
}
