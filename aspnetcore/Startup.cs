using aspnetcore.Middleware;
using Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis;

namespace aspnetcore
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

            services.AddDbContext<TtifaContext>(options => options.UseNpgsql(Configuration.GetConnectionString("Postgres")));

            services.AddSingleton(new RedisClient(Configuration.GetConnectionString("Redis")));

            //services.Configure<ForwardedHeadersOptions>(option => { option.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForward‌​edFor; });

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
                    //OnAuthenticationFailed = c =>
                    //{
                    //    c.NoResult();
                    //    return new ApiResult(ApiStatus.NoLogin, c.Exception.Message).ExecuteApiResultAsync(c.HttpContext);
                    //}
                };
            });


            services.AddMvc(options =>
            {
                //options.Filters.Add(new ApiExceptionAttribute());
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            //400、500错误页面
            //app.UseStatusCodePagesWithRedirects("~/error/{0}");//客户端跳转
            //app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");//服务端跳转

            app.UseStaticFiles();

            /*
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                c.DocExpansion("none");
            });
            */
            app.UseAuthentication();
            app.UseMvc(routes => routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
