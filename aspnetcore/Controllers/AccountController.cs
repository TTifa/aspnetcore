using aspnetcore.Middleware;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis;
using System;
using System.Linq;

namespace aspnetcore.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(RedisClient redisCli, TtifaContext ttifaContext) : base(redisCli, ttifaContext)
        {
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ApiResult SignIn(string username, string password)
        {
            var user = _db.users.FirstOrDefault(o => o.Username == username && o.Pwd == password);
            if (user == null)
                return new ApiResult(ApiStatus.Fail);

            user.LastLoginTime = DateTime.Now;
            user.LastLoginIP = HttpContext.Connection.RemoteIpAddress.ToString();
            _db.SaveChanges();

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
                user.Uid,
                user.Username,
                Token = tokenString
            });
        }

        [Authorize]
        public ApiResult SignOut()
        {
            //token 过期
            TokenHandler.Expire(CurrentUser.Token);
            //删除cookie
            HttpContext.Response.Cookies.Delete("access_token");

            return new ApiResult();
        }

        public ApiResult RegisterByWX()
        {


            return new ApiResult();
        }

        #region 用户管理
        public ApiResult Users(int pageindex, int pagesize)
        {
            var query = _db.users.AsNoTracking().OrderBy(o => o.Uid);
            var page = new ApiResultPage(pageindex, pagesize);
            var list = DataPage.GetPage(query, pagesize, pageindex, ref page.PageCount, ref page.Total);

            return new ApiResult(ApiStatus.OK, "success", list, page);
        }

        public ApiResult NewUser([FromBody]User model)
        {
            model.CreateTime = DateTime.Now;
            model.Status = UserStatus.Normal;
            _db.users.Add(model);
            _db.SaveChanges();

            return new ApiResult();
        }

        public ApiResult Delete(int uid)
        {
            var model = _db.users.First(o => o.Uid == uid);
            _db.Entry(model).State = EntityState.Deleted;
            _db.SaveChanges();

            return new ApiResult();
        }

        public ApiResult UpdatePwd(int uid, string newPwd)
        {
            var user = _db.users.First(o => o.Uid == uid);
            user.Pwd = newPwd;
            _db.SaveChanges();

            return new ApiResult();
        }
        #endregion

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
    }
}