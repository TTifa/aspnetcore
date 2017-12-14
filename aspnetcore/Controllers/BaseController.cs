using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Redis;
using System;
using System.Linq;

namespace aspnetcore.Controllers
{
    public class BaseController : Controller
    {
        protected RedisClient _redisCli;
        protected LoginedUser CurrentUser { get; set; }
        protected string token;

        public BaseController(RedisClient redisCli)
        {
            _redisCli = redisCli;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            token = context.HttpContext.Request.Headers["Token"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new ApiResult(ApiStatus.NoLogin, "请登录"));
                return;
            }

            var redis = _redisCli.GetDatabase();
            var tokenKey = $"UserToken:{token}";
            if (!redis.KeyExists(tokenKey))
            {
                context.Result = new JsonResult(new ApiResult(ApiStatus.NoLogin, "登录信息错误"));
                return;
            }
            var expiredTime = Convert.ToDateTime(redis.HashGet(tokenKey, "ExpiredTime").ToString());
            if (expiredTime < DateTime.Now)
            {
                context.Result = new JsonResult(new ApiResult(ApiStatus.NoLogin, "登录信息已过期，请重新登录"));
                return;
            }

            CurrentUser = new LoginedUser();
            CurrentUser.Uid = Convert.ToInt32(redis.HashGet(tokenKey, "Uid"));
            CurrentUser.Username = redis.HashGet(tokenKey, "Username");
            CurrentUser.ExpiredTime = expiredTime;

            base.OnActionExecuting(context);
        }
    }
}