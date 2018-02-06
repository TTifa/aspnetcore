using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Redis;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using aspnetcore.Middleware;

namespace aspnetcore.Controllers
{
    /// <summary>
    /// 微信小程序
    /// </summary>
    [Produces("application/json")]
    [Route("api/WxOpen")]
    public class WxOpenController : BaseController
    {
        public WxOpenController(RedisClient redisCli, TtifaContext ttifaContext) : base(redisCli, ttifaContext)
        {
        }

        [HttpPost("register")]
        public ApiResult Register(string openid, string username, string password)
        {
            var fans = _db.wechatfans.FirstOrDefault(o => o.OpenId == openid);
            if (fans == null)
                return new ApiResult(ApiStatus.Fail, "用户信息不存在");

            var user = new User
            {
                Username = username,
                Nickname = fans.NickName,
                Pwd = password,
                Status = 1,
                LastLoginTime = DateTime.Now,
                Avatar = fans.Avatar,
                Admin = false
            };
            _db.users.Add(user);
            _db.SaveChanges();

            user = _db.users.First(o => o.Username == username);
            //关联微信账号
            fans.UserId = user.Uid;
            _db.wechatfans.Update(fans);
            _db.SaveChanges();

            //生成token
            var token = new ApiToken
            {
                Guid = Guid.NewGuid().ToString("N"),
                Uid = user.Uid,
                Username = user.Username,
                ExpiredTime = DateTime.Now.AddDays(3)
            };
            var tokenString = TokenHandler.WriteToken(token);

            HttpContext.Response.Cookies.Append("access_token", tokenString);

            return new ApiResult(data: new
            {
                UserId = user.Uid,
                Token = tokenString
            });
        }

        /// <summary>
        /// 小程序登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("signin")]
        public ApiResult SignIn(WechatUserInfo model)
        {
            var wxAccount = _db.wechataccounts.FirstOrDefault(o => o.Id == model.WAId);
            var codeResult = SnsApi.JsCode2Json(wxAccount.AppId, wxAccount.AppSecret, model.jsCode);
            var fans = _db.wechatfans.FirstOrDefault(o => o.OpenId == codeResult.openid);
            if (fans == null)
            {
                //记录访客信息
                fans = new WechatFans
                {
                    OpenId = codeResult.openid,
                    NickName = model.nickName,
                    Avatar = model.avatarUrl,
                    Sex = model.gender,
                    Area = $"{model.country}-{model.province}-{model.city}",
                    SubscribeTime = DateTime.Now,
                    State = FansStatus.Subscribe,
                    WAId = model.WAId,
                    SessionKey = codeResult.session_key,
                    SessionExpire = DateTime.Now.AddDays(1) //sessionkey 保存一天
                };

                _db.wechatfans.Add(fans);
                _db.SaveChangesAsync();

                return new ApiResult(ApiStatus.NoRegister, "尚未注册账号");
            }

            var user = _db.users.FirstOrDefault(o => o.Uid == fans.UserId);
            if (user == null)
                return new ApiResult(ApiStatus.NoRegister, "尚未注册账号");

            var token = new ApiToken
            {
                Guid = Guid.NewGuid().ToString("N"),
                Uid = user.Uid,
                Username = user.Username,
                ExpiredTime = DateTime.Now.AddDays(3)
            };
            var tokenString = TokenHandler.WriteToken(token);

            HttpContext.Response.Cookies.Append("access_token", tokenString);

            return new ApiResult(data: new
            {
                UserId = user.Uid,
                Token = tokenString
            });
        }
    }
}