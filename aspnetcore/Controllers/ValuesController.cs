using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore.Filters;

namespace aspnetcore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet, Log]
        public IEnumerable<string> Get()
        {
            throw new Exception("test exception");
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}"), Log]
        public ApiResult Get(int id)
        {
            return new ApiResult();
        }

        // POST api/values
        [HttpPost]
        public void Post()
        {
            throw new Exception("test exception");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
