﻿using System;
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

        [HttpGet("GetStat")]
        public ApiResult GetStat(string month)
        {
            var stat = _redisCli.hgetall($"Stat:{month}");

            return new ApiResult(data: stat);
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