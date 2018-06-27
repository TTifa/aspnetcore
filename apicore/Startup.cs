using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Redis;
using Senparc.Weixin.Threads;
using WebApiBase;
using WebApiBase.Middleware;

namespace apicore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //读取配置
            services.Configure<UploadOptions>(Configuration.GetSection("Upload"));
            services.Configure<WechatOptions>(Configuration.GetSection("Wechat"));
            services.Configure<SiteOptions>(Configuration.GetSection("Site"));

            services.AddDbContext<TtifaContext>(options => options.UseNpgsql(Configuration.GetConnectionString("Postgres")));
            services.AddSingleton(new RedisClient(Configuration.GetConnectionString("Redis")));
            services.AddTimedJob();
            //注册swagger
            /*
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Version = "v1",
                    Title = "aspnetcore api",
                    Description = "by ttifa"
                });
            });
            */

            #region 使用jwt验证用户
            /*
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                //自定义获取token方法
                jwtBearerOptions.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Headers["access_token"][0];
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };

                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("kimo7770123456789")),

                    ValidateIssuer = true,
                    ValidIssuer = "ttifa",

                    ValidateAudience = true,
                    ValidAudience = "ttifa webapp",

                    ValidateLifetime = true, //validate the expiration and not before values in the token

                    ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                };
            });
            */
            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy("local", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials();//允许读取凭据（cookie等）
                });
            });

            //自定义登录验证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "access_token";
                options.DefaultChallengeScheme = "access_token";
            })
            .AddToken("access_token", options =>
            {
                options.Events = new TokenEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        return Task.CompletedTask;
                        //return new ApiResult(ApiStatus.NoLogin, context.Exception.Message).ExecuteApiResultAsync(context.HttpContext);
                    }
                };
            });

            services.AddResponseCaching();
            services.AddMvc(options =>
            {
                //options.Filters.Add(new ApiExceptionAttribute());
                options.CacheProfiles.Add("default", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 60,
                    Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any
                });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<WechatOptions> wechatOptions)
        {
            app.UseTimedJob();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
            });

            /*
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                c.DocExpansion("none");
            });
            */
            app.UseCors("local");
            app.UseAuthentication();
            app.UseMvc(routes => routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
            app.UseResponseCaching();
            RegisterWechatThreads();//激活微信缓存及队列线程（必须）
        }

        /// <summary>
        /// 激活微信缓存
        /// </summary>
        private void RegisterWechatThreads()
        {
            ThreadUtility.Register();//如果不注册此线程，则AccessToken、JsTicket等都无法使用SDK自动储存和管理。
        }
    }
}
