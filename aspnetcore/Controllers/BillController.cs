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

        public ApiResult Get(int pageIndex, int pageSize, int year, int month)
        {
            var date = new DateTime(year, month, 1);
            var end = date.AddMonths(1);
            var source = _db.bills.Where(o => o.Uid == CurrentUser.Uid);
            if (date > DateTime.MinValue)
                source = source.Where(o => o.PayDate >= date && o.PayDate < end);

            var page = new ApiResultPage(pageIndex, pageSize);
            var list = DataPage.GetPage(source.OrderByDescending(o => o.PayDate), pageSize, pageIndex, ref page.PageCount, ref page.Total);

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

        #region 统计

        [HttpGet("Statistic")]
        public ApiResult Statistic(int year, int month)
        {
            var date = new DateTime(year, month, 1);
            var end = date.AddMonths(1);
            var query = _db.bills.Where(o => o.Uid == CurrentUser.Uid && o.PayDate >= date && o.PayDate < end);

            var total = query.Sum(o => o.Amount);
            var outlay = query.Where(o => o.Amount > 0).Sum(o => o.Amount);
            var income = 0 - query.Where(o => o.Amount < 0).Sum(o => o.Amount);

            return new ApiResult(data: new
            {
                total = total,
                outlay = outlay,
                income = income
            });
        }

        [HttpGet("GenStat")]
        public ApiResult GenStat(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                var today = DateTime.Now.Date;
                date = new DateTime(today.Year, today.Month, 1);
            }

            var end = new DateTime(date.Year, date.Month + 1, 1);
            var query = _db.bills.Where(o => o.Uid == CurrentUser.Uid && o.PayDate >= date && o.PayDate < end && o.Amount > 0);
            var list = query.GroupBy(o => o.FlowType).Select(o =>
                new KeyValuePair<string, string>(o.Key.ToString(), o.Sum(b => b.Amount).ToString("0.00")))
                .ToList();

            _redisCli.Hmset($"Stat:{CurrentUser.Uid}:{date.ToString("yyyyMM")}", list);

            return new ApiResult(data: list);
        }

        [HttpGet("Stat")]
        public ApiResult GetStat(string month)
        {
            var stat = _redisCli.hgetall($"Stat:{CurrentUser.Uid}:{month}");

            return new ApiResult(data: stat);
        }

        [HttpGet("GenDailyStat")]
        public ApiResult GenDailyStat(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                var today = DateTime.Now.Date;
                date = new DateTime(today.Year, today.Month, 1);
            }

            var statKey = $"DailyStat:{CurrentUser.Uid}:{date.ToString("yyyyMM")}";
            var end = new DateTime(date.Year, date.Month + 1, 1);
            var query = _db.bills.Where(o => o.Uid == CurrentUser.Uid && o.PayDate >= date && o.PayDate < end && o.Amount > 0);
            var list = query.GroupBy(o => o.PayDate).Select(o =>
                new KeyValuePair<string, string>(o.Key.ToString("yyyyMMdd"), o.Sum(b => b.Amount).ToString("0.00")))
                .ToList();

            //补全没有记录日期
            while (date < end)
            {
                var dateStr = date.ToString("yyyyMMdd");
                if (!list.Any(o => o.Key == dateStr))
                {
                    list.Add(new KeyValuePair<string, string>(dateStr, "0"));
                }
                date = date.AddDays(1);
            }

            _redisCli.Hmset(statKey, list);

            return new ApiResult(data: list);
        }

        [HttpGet("DailyStat")]
        public ApiResult GetDailyStat(string month)
        {
            var stat = _redisCli.hgetall($"DailyStat:{CurrentUser.Uid}:{month}");

            return new ApiResult(data: stat);
        }

        #endregion
    }
}