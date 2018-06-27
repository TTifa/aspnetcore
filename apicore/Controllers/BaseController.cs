using Entity;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Redis;
using System;
using System.Security.Claims;

namespace apicore.Controllers
{
    public class BaseController : Controller
    {
        protected LoginedUser CurrentUser { get; set; }
        protected RedisClient _redisCli;
        protected TtifaContext _db;
        public BaseController(RedisClient redisCli, TtifaContext ttifaContext)
        {
            this._redisCli = redisCli;
            this._db = ttifaContext;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var principal = context.HttpContext.User;
            if (principal.Identity != null && principal.Identity.IsAuthenticated)
            {
                CurrentUser = new LoginedUser();
                CurrentUser.Token = principal.FindFirstValue(JwtClaimTypes.JwtId);
                CurrentUser.Uid = Convert.ToInt32(principal.FindFirstValue(JwtClaimTypes.Id));
                CurrentUser.Username = principal.FindFirstValue(JwtClaimTypes.Name);
                CurrentUser.ExpiredTime = Convert.ToDateTime(principal.FindFirstValue(JwtClaimTypes.Expiration));
            }

            base.OnActionExecuting(context);
        }

        /*
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
        */
    }
}