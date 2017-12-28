using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore.Middleware
{
    public class UserMiddlerware
    {
        private readonly RequestDelegate _next;

        public UserMiddlerware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Token"].FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;

                return;
            }
            else
            {
                //TODO： 验证token有效

                await _next.Invoke(context);
            }
        }
    }

    public static class UserMiddlewareExtension
    {
        public static IApplicationBuilder UseUserMiddlerware(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserMiddlerware>();
        }
    }
}