using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Redis;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebApiBase.Middleware
{
    public class TokenHandler : AuthenticationHandler<TokenOptions>
    {
        public TokenHandler(IOptionsMonitor<TokenOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected new TokenEvents Events
        {
            get { return (TokenEvents)base.Events; }
            set { base.Events = value; }
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new TokenEvents());

        /// <summary>
        /// 认证token:header中的Token
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = string.Empty;
            try
            {
                var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);
                // event can set the token
                await Events.MessageReceived(messageReceivedContext);
                if (messageReceivedContext.Result != null)
                {
                    return messageReceivedContext.Result;
                }

                // If application retrieved token from somewhere else, use that.
                token = messageReceivedContext.Token;
                //default get token
                if (string.IsNullOrEmpty(token))
                {
                    token = Request.Headers["access_token"].FirstOrDefault();

                    //get from cookie
                    if (string.IsNullOrEmpty(token))
                    {
                        Request.Cookies.TryGetValue("access_token", out token);
                    }

                    // If no token found, no further work possible
                    if (string.IsNullOrEmpty(token))
                    {
                        return AuthenticateResult.NoResult();
                    }
                }

                //get logined userinfo
                var err = string.Empty;
                var principal = ValidateToken(token, ref err);
                if (!string.IsNullOrEmpty(err))
                {
                    var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                    {
                        Exception = new Exception(err)
                    };

                    await Events.AuthenticationFailed(authenticationFailedContext);
                    if (authenticationFailedContext.Result != null)
                    {
                        return authenticationFailedContext.Result;
                    }

                    return AuthenticateResult.Fail(authenticationFailedContext.Exception);
                }

                //validate success
                var tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                {
                    Principal = principal,
                    Token = token
                };
                await Events.TokenValidated(tokenValidatedContext);
                if (tokenValidatedContext.Result != null)
                {
                    return tokenValidatedContext.Result;
                }

                tokenValidatedContext.Success();
                return tokenValidatedContext.Result;
            }
            catch (Exception ex)
            {
                var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                await Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                throw;
            }
        }

        public static string WriteToken(ApiToken token)
        {
            var redis = new RedisClient().GetDatabase();
            var tokenKey = $"UserToken:{token.Guid}";
            redis.HashSet(tokenKey, "Uid", token.Uid);
            redis.HashSet(tokenKey, "Username", token.Username);
            redis.HashSet(tokenKey, "ExpiredTime", token.ExpiredTime.ToString());
            redis.KeyExpire(tokenKey, token.ExpiredTime);

            return token.Guid;
        }

        public static bool KeepToken(string tokenKey, DateTime expires)
        {
            var redis = new RedisClient().GetDatabase();
            return redis.KeyExpire(tokenKey, expires);
        }

        public static bool Expire(string guid)
        {
            var redis = new RedisClient().GetDatabase();
            var tokenKey = $"UserToken:{guid}";

            return redis.KeyDelete(tokenKey);
        }

        /// <summary>
        /// get userinfo by token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        private ClaimsPrincipal ValidateToken(string token, ref string err)
        {
            var tokenKey = $"UserToken:{token}";
            var dict = new RedisClient().hgetall(tokenKey);
            if (dict == null || dict.Count <= 0)
            {
                err = "登录用户信息不存在";
                return null;
            }

            var expiredTime = Convert.ToDateTime(dict["ExpiredTime"]);
            if (expiredTime < DateTime.Now)
            {
                err = "登录用户信息已过期";
                return null;
            }

            var claimIdentity = new ClaimsIdentity("token");
            claimIdentity.AddClaim(new Claim(JwtClaimTypes.JwtId, token));
            claimIdentity.AddClaim(new Claim(JwtClaimTypes.Id, dict["Uid"]));
            claimIdentity.AddClaim(new Claim(JwtClaimTypes.Expiration, dict["ExpiredTime"]));
            claimIdentity.AddClaim(new Claim(JwtClaimTypes.Name, dict["Username"]));
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, dict["Username"]));//HttpContext.User.Identity.Name
            var principal = new ClaimsPrincipal(claimIdentity);

            return principal;
        }
    }
}