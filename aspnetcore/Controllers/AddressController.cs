using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Redis;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/Address")]
    public class AddressController : BaseController
    {
        public AddressController(RedisClient redisCli, TtifaContext ttifaContext) : base(redisCli, ttifaContext)
        {
        }

        [HttpGet]
        public ApiResult Get(int userId)
        {
            var list = _db.deliveryaddresses.Where(o => o.UserId == userId).AsNoTracking().ToList();

            return new ApiResult(data: list);
        }

        [HttpGet("id")]
        public ApiResult GetById(int id, int userId)
        {
            var model = _db.deliveryaddresses.FirstOrDefault(o => o.Id == id && o.UserId == userId);

            return new ApiResult(data: model);
        }

        [HttpGet("area")]
        public ApiResult Area(int pid)
        {
            var provinces = _db.areas.Where(o => o.ParentId == pid && o.State == 1).OrderBy(o => o.OrderNo).AsNoTracking().ToList();

            return new ApiResult(data: provinces);
        }

        [HttpPost]
        public ApiResult Post(DeliveryAddress address)
        {
            if (string.IsNullOrEmpty(address.AreaCode))
                return new ApiResult(ApiStatus.Fail, "区域信息错误");

            if (address.Id > 0)
                _db.deliveryaddresses.Update(address);
            else
                _db.deliveryaddresses.Add(address);

            _db.SaveChanges();

            return new ApiResult(data: address);
        }

        [HttpPost("delete")]
        public ApiResult Delete(int userId, int id)
        {
            var model = _db.deliveryaddresses.AsNoTracking().FirstOrDefault(o => o.Id == id && o.UserId == userId);
            if (model == null)
                return new ApiResult(ApiStatus.Fail, "记录不存在");

            _db.deliveryaddresses.Remove(model);
            _db.SaveChanges();

            return new ApiResult();
        }
    }
}