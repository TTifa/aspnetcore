using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Utils;
using Redis;
using Newtonsoft.Json;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : BaseController
    {
        public OrderController(RedisClient redisCli, TtifaContext ttifaContext) : base(redisCli, ttifaContext)
        {
        }

        [HttpGet]
        public ApiResult Get(string id)
        {
            var order = _db.orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
                return new ApiResult(ApiStatus.Fail, "订单信息不存在");

            return new ApiResult(data: order);
        }

        [HttpPost]
        //public ApiResult Post(PlaceOrder model)
        public ApiResult Post(int userId, string items, string address)
        {
            var goods = JsonConvert.DeserializeObject<Dictionary<int, int>>(items);
            if (goods.Count <= 0)
                return new ApiResult(ApiStatus.Fail, "参数错误");

            var ids = goods.Keys.ToList();
            var goodsList = _db.goods.Where(o => ids.Contains(o.Id)).AsNoTracking().ToList();

            var order = new Order
            {
                OrderId = CodeHelper.OrderNo(CodeHelper.CodeType.D),
                UserId = userId,
                Address = address,
                CreateTime = DateTime.Now,
                State = OrderStatus.created
            };
            _db.orders.Add(order);
            var totalAmount = 0m;
            foreach (var item in goodsList)
            {
                var orderGoods = new OrderGoods
                {
                    OrderId = order.OrderId,
                    GoodsId = item.Id,
                    GoodsName = item.Name,
                    Quantity = goods[item.Id],
                    Price = item.Price,
                    CreateTime = DateTime.Now
                };
                totalAmount += orderGoods.Price * orderGoods.Quantity;
                _db.ordergoods.Add(orderGoods);
            }
            order.Amount = totalAmount;
            _db.orders.Add(order);
            _db.SaveChanges();

            return new ApiResult(data: order);
        }
    }
}