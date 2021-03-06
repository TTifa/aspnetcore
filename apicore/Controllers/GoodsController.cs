﻿using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApiBase;

namespace apicore.Controllers
{
    [Produces("application/json")]
    [Route("api/Goods")]
    public class GoodsController : Controller
    {
        private TtifaContext _db;
        public GoodsController(TtifaContext ttifaContext)
        {
            _db = ttifaContext;
        }

        [HttpGet]
        public ApiResult Get(int id)
        {
            var goods = _db.goods.FirstOrDefault(o => o.Id == id);

            if (goods == null)
                return new ApiResult(ApiStatus.Fail, "产品不存在");

            return new ApiResult(data: new
            {
                id = goods.Id,
                title = goods.Name,
                price = goods.Price,
                avatar = goods.Avatar,
                images = goods.Images,
                detail = goods.Detail
            });
        }

        [HttpGet("evaluate")]
        public ApiResult Evaluate(int goodsId, int pageIndex = 1, int pageSize = 5)
        {
            if (goodsId <= 0)
                return new ApiResult(ApiStatus.Fail, "商品信息错误");

            var result = new ApiResult();
            result.Page = new ApiResultPage(pageIndex, pageSize);
            var source = _db.evaluates.Where(o => o.GoodsId == goodsId);

            var total = source.Count();
            result.Page.PageCount = total % pageSize == 0 ? (total / pageSize) : (total / pageSize + 1);
            result.Page.Total = total;
            var list = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
            result.Data = list;
            return result;
        }

        [HttpPost]
        public ApiResult List(int categoryId, int pageIndex = 1, int pageSize = 5, bool hot = false)
        {
            var result = new ApiResult();
            result.Page = new ApiResultPage(pageIndex, pageSize);

            var source = _db.goods.AsQueryable();
            if (categoryId > 0)
                source = source.Where(o => o.CategoryId == categoryId);
            if (hot)
                source = source.Where(o => o.Hot == true);

            var total = source.Count();
            result.Page.PageCount = total % pageSize == 0 ? (total / pageSize) : (total / pageSize + 1);
            result.Page.Total = total;
            var list = source.OrderBy(o => o.OrderNo).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
            result.Data = list;
            return result;
        }
    }
}