using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Redis;
using WebApiBase;

namespace apicore.Controllers
{
    [Produces("application/json")]
    [Route("api/Bill")]
    [Authorize]
    public class BillController : BaseController
    {
        public BillController(RedisClient redisCli, TtifaContext ttifaContext) : base(redisCli, ttifaContext)
        {
        }

        public ApiResult Get(int pageIndex, int pageSize, int year, int month, FlowType flowType = FlowType.None)
        {
            var date = new DateTime(year, month, 1);
            var end = date.AddMonths(1);
            var source = _db.bills.Where(o => o.Uid == CurrentUser.Uid);
            if (date > DateTime.MinValue)
                source = source.Where(o => o.PayDate >= date && o.PayDate < end);
            if (flowType != FlowType.None)
                source = source.Where(o => o.FlowType == flowType);

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

        [HttpGet("GenStat"), AllowAnonymous]
        public ApiResult GenStat(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                var today = DateTime.Now.Date;
                date = new DateTime(today.Year, today.Month, 1);
            }
            var userIds = _db.users.Where(o => o.Status == UserStatus.Normal).Select(o => o.Uid).ToList();
            foreach (var uid in userIds)
            {
                var end = new DateTime(date.Year, date.Month + 1, 1);
                var query = _db.bills.Where(o => o.Uid == uid && o.PayDate >= date && o.PayDate < end && o.Amount > 0);
                var list = query.GroupBy(o => o.FlowType).Select(o =>
                    new KeyValuePair<string, string>(o.Key.ToString(), o.Sum(b => b.Amount).ToString("0.00")))
                    .ToList();

                _redisCli.Hmset($"Stat:{uid}:{date.ToString("yyyyMM")}", list);
            }

            return new ApiResult();
        }

        [HttpGet("GenIncomeStat"), AllowAnonymous]
        public ApiResult GenIncomeStat(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                var today = DateTime.Now.Date;
                date = new DateTime(today.Year, today.Month, 1);
            }
            var userIds = _db.users.Where(o => o.Status == UserStatus.Normal).Select(o => o.Uid).ToList();
            foreach (var uid in userIds)
            {
                var end = new DateTime(date.Year, date.Month + 1, 1);
                var query = _db.bills.Where(o => o.Uid == uid && o.PayDate >= date && o.PayDate < end && o.Amount < 0);
                var list = query.GroupBy(o => o.FlowType).Select(o =>
                    new KeyValuePair<string, string>(o.Key.ToString(), o.Sum(b => -b.Amount).ToString("0.00")))
                    .ToList();

                _redisCli.Hmset($"IncomeStat:{uid}:{date.ToString("yyyyMM")}", list);
            }

            return new ApiResult();
        }

        [HttpGet("Stat")]
        public ApiResult GetStat(string month)
        {
            var stat = _redisCli.hgetall($"Stat:{CurrentUser.Uid}:{month}");

            return new ApiResult(data: stat);
        }

        [HttpGet("IncomeStat")]
        public ApiResult GetIncomeStat(string month)
        {
            var stat = _redisCli.hgetall($"IncomeStat:{CurrentUser.Uid}:{month}");

            return new ApiResult(data: stat);
        }

        [HttpGet("GenDailyStat"), AllowAnonymous]
        public ApiResult GenDailyStat(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                var today = DateTime.Now.Date;
                date = new DateTime(today.Year, today.Month, 1);
            }

            var userIds = _db.users.Where(o => o.Status == UserStatus.Normal).Select(o => o.Uid).ToList();
            foreach (var uid in userIds)
            {
                var statKey = $"DailyStat:{uid}:{date.ToString("yyyyMM")}";
                var end = new DateTime(date.Year, date.Month + 1, 1);
                var query = _db.bills.Where(o => o.Uid == uid && o.PayDate >= date && o.PayDate < end && o.FlowType < FlowType.Salary);
                var list = query.GroupBy(o => o.PayDate).Select(o =>
                    new KeyValuePair<string, string>(o.Key.ToString("yyyyMMdd"), o.Sum(b => b.Amount).ToString("0.00")))
                    .ToList();

                //补全没有记录日期
                var start = date;
                while (start < end)
                {
                    var dateStr = start.ToString("yyyyMMdd");
                    if (!list.Any(o => o.Key == dateStr))
                    {
                        list.Add(new KeyValuePair<string, string>(dateStr, "0"));
                    }
                    start = start.AddDays(1);
                }

                _redisCli.Hmset(statKey, list);
            }

            return new ApiResult();
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