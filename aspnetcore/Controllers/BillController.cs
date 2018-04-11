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

        public ApiResult Get(int pageIndex, int pageSize, DateTime date)
        {
            var end = date.AddDays(1);
            var source = _db.bills.Where(o => o.Uid == CurrentUser.Uid);
            if (date > DateTime.MinValue)
                source = source.Where(o => o.PayDate >= date && o.PayDate < end);

            var page = new ApiResultPage(pageIndex, pageSize);
            var list = DataPage.GetPage(source.OrderByDescending(o => o.PayDate), pageSize, pageIndex, ref page.PageCount, ref page.Total);

            return new ApiResult(data: list, page: page);
        }

        [HttpGet("Statistic")]
        public ApiResult Statistic()
        {
            var total = _db.bills.Where(o => o.Uid == CurrentUser.Uid).Sum(o => o.Amount);

            return new ApiResult(data: total);
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