using aspnetcore.Filters;
using Entity;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

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
            var tokenKey = $"UserToken:{token}";
            redis.HashSet(tokenKey, "Uid", user.Uid);
            redis.HashSet(tokenKey, "Username", user.Username);
            redis.HashSet(tokenKey, "ExpiredTime", DateTime.Now.AddDays(3).ToString());

            return new ApiResult(data: new
            {
                user.Uid,
                user.Username,
                Token = token
            });
        }

        #region jwt token
        /*
        [HttpPost]
        public ApiResult SignIn(string username, string password)
        {
            var user = db.Users.FirstOrDefault(o => o.Username == username && o.Pwd == password);
            if (user == null)
                return new ApiResult(ApiStatus.Fail);

            //创建密钥
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("kimo7770123456789"));
            var claims = new Claim[] {
                new Claim(JwtClaimTypes.Id, user.Uid.ToString()),
                new Claim(JwtClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: "ttifa",
                audience: "ttifa webapp",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new ApiResult(data: new
            {
                user.Uid,
                user.Username,
                Token = tokenString
            });
        }
        */
        #endregion

        [Authorize, Log]
        public ApiResult Test()
        {
            var user = HttpContext.User;

            return new ApiResult(data: user.FindFirst(JwtClaimTypes.Name).Value);
        }
    }
}