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
using Infrastructure.Utils;

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

            if (fans.UserId > 0)
                return new ApiResult(ApiStatus.Fail, "it's already existed");

            var user = new User
            {
                Username = username,
                Nickname = fans.NickName,
                Pwd = password,
                Status = 1,
                CreateTime = DateTime.Now,
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
        /// 获取用户openid
        /// </summary>
        /// <param name="id">公众号配置id</param>
        /// <param name="jsCode"></param>
        /// <returns></returns>
        [HttpPost("openid")]
        public ApiResult OpenId(int id, string jsCode)
        {
            var wxAccount = _db.wechataccounts.FirstOrDefault(o => o.Id == id);
            var codeResult = SnsApi.JsCode2Json(wxAccount.AppId, wxAccount.AppSecret, jsCode);

            //保存sessionkey
            var redis = _redisCli.GetDatabase();
            redis.StringSet($"WXSessionKey:{codeResult.openid}", codeResult.session_key, new TimeSpan(24, 0, 0));

            return new ApiResult(data: codeResult.openid);
        }

        /// <summary>
        /// 小程序登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("signin")]
        public ApiResult SignIn(WechatUserInfo model)
        {
            var fans = _db.wechatfans.FirstOrDefault(o => o.OpenId == model.openid);
            if (fans == null)
            {
                //记录访客信息
                fans = new WechatFans
                {
                    OpenId = model.openid,
                    NickName = model.nickName,
                    Avatar = model.avatarUrl,
                    Sex = model.gender,
                    Area = $"{model.country}-{model.province}-{model.city}",
                    SubscribeTime = DateTime.Now,
                    State = FansStatus.Subscribe,
                    WAId = model.WAId
                };

                _db.wechatfans.Add(fans);
                _db.SaveChangesAsync();

                return new ApiResult(ApiStatus.NoRegister, "尚未注册账号");
            }

            if (fans.UserId == null || fans.UserId < 0)
                return new ApiResult(ApiStatus.NoRegister, "尚未注册账号");

            var user = _db.users.FirstOrDefault(o => o.Uid == fans.UserId);
            if (user == null)
                return new ApiResult(ApiStatus.Fail, "用户信息不存在");

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
        /// 支付签名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("paysign")]
        public ApiResult PaySign(int id)
        {
            var wxAccount = _db.wechataccounts.FirstOrDefault(o => o.Id == id);
            if (wxAccount == null)
                return new ApiResult(ApiStatus.Fail, "获取公众号信息失败");

            var timestamp = DateTime.UtcNow.Ticks.ToString();
            var nonceStr = Guid.NewGuid().ToString("N");
            var package = "prepay_id=" + CodeHelper.OrderNo(CodeHelper.CodeType.Null);
            var paySign = CryptoHelper.MD5_Encrypt($"appId={wxAccount.AppId}&nonceStr={nonceStr}&package={package}&signType=MD5&timeStamp={timestamp}");

            return new ApiResult(data: new
            {
                timeStamp = timestamp,
                nonceStr = nonceStr,
                package = package,
                paySign = paySign
            });
        }
    }
}