using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Redis;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/Bill")]
    [Authorize]
    public class BillController : BaseController
    {
        public BillController(RedisClient redisCli, TtifaContext ttifaContext) : base(redisCli, ttifaContext)
        {
        }

        public ApiResult Get(int pageIndex, int pageSize)
        {
            var source = _db.bills.Where(o => o.Uid == CurrentUser.Uid).OrderByDescending(o => o.Id);
            var page = new ApiResultPage(pageIndex, pageSize);
            var list = DataPage.GetPage(source, pageSize, pageIndex, ref page.PageCount, ref page.Total);

            return new ApiResult(data: list, page: page);
        }

        [HttpPost]
        public ApiResult Post([FromBody]Bill bill)
        {
            bill.LogTime = DateTime.Now;
            bill.Uid = CurrentUser.Uid;
            _db.bills.Add(bill);
            _db.SaveChanges();

            return new ApiResult(data: bill);
        }
    }
}