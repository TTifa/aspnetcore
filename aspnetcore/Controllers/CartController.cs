using Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis;
using System.Linq;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/Cart")]
    [EnableCors("local")]

    public class CartController : Controller
    {
        private RedisClient _redisCli;
        private TtifaContext _db;
        public CartController(RedisClient redisCli, TtifaContext ttifaContext)
        {
            this._redisCli = redisCli;
            this._db = ttifaContext;
        }

        [HttpGet]
        public ApiResult Get(int userId)
        {
            if (userId <= 0)
                return new ApiResult(ApiStatus.Fail, "userid error");

            var redis = new RedisClient().GetDatabase();
            var data = redis.StringGet($"Cart:{userId}");

            return new ApiResult(data: data.ToString());
        }

        [HttpPost]
        public ApiResult Post(int userId, string carts)
        {
            if (userId <= 0)
                return new ApiResult(ApiStatus.Fail, "userid error");
            if (string.IsNullOrEmpty(carts))
                return new ApiResult(ApiStatus.Fail, "carts data error");

            var redis = new RedisClient().GetDatabase();
            var result = redis.StringSet($"Cart:{userId}", carts);

            return new ApiResult();
        }

        #region 购物车操作
        public ApiResult Add(int userId, int goodsId)
        {
            if (userId <= 0)
                return new ApiResult(ApiStatus.Fail, "userid error");

            var goods = _db.goods.FirstOrDefault(o => o.Id == goodsId);
            var key = $"Carts:{userId}";
            var field = $"Good:{goodsId}";
            ShoppingCart item;
            if (_redisCli.Hexists(key, field))
            {
                item = JsonConvert.DeserializeObject<ShoppingCart>(_redisCli.Hget(key, field));
                item.quantity++;
            }
            else
            {
                item = new ShoppingCart
                {
                    goodsId = goods.Id,
                    avatar = goods.Avatar,
                    title = goods.Name,
                    price = goods.Price,
                    selected = true,
                    quantity = 1
                };
            }

            _redisCli.Hset(key, field, JsonConvert.SerializeObject(item));

            return new ApiResult();
        }

        public ApiResult Remove(int userId, int goodsId)
        {
            if (userId <= 0)
                return new ApiResult(ApiStatus.Fail, "userid error");

            var key = $"Carts:{userId}";
            var field = $"Good:{goodsId}";
            _redisCli.Hdel(key, field);

            return new ApiResult();
        }
        #endregion
    }
}