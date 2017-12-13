using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis;
using System;
using System.Linq;

namespace aspnetcore.Controllers
{
    public class AccountController : Controller
    {
        private TtifaContext db;
        private RedisClient _redisCli;

        public AccountController(TtifaContext ttifaContext, RedisClient redisClient)
        {
            db = ttifaContext;
            _redisCli = redisClient;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ApiResult SignIn(string username, string password)
        {
            var user = db.Users.FirstOrDefault(o => o.Username == username && o.Pwd == password);
            if (user == null)
                return new ApiResult(ApiStatus.Fail);

            user.LastLoginTime = DateTime.Now;
            user.LastLoginIP = HttpContext.Connection.RemoteIpAddress.ToString();
            var token = Guid.NewGuid().ToString("N");
            db.SaveChanges();

            var redis = _redisCli.GetDatabase();
            redis.StringSet($"UserToken:{token}", JsonConvert.SerializeObject(new
            {
                user.Uid,
                user.Username,
                ExpiredTime = DateTime.Now.AddDays(1)
            }));

            return new ApiResult(data: new
            {
                user.Uid,
                user.Username,
                Token = token
            });
        }
    }
}