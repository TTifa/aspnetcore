using aspnetcore.Filters;
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
            services.Configure<UploadConfig>(Configuration.GetSection("Upload"));

            services.AddDbContext<TtifaContext>(options => options.UseNpgsql(Configuration.GetConnectionString("Postgres")));

            services.AddSingleton(new RedisClient(Configuration.GetConnectionString("Redis")));



            //注册swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Version = "v1",
                    Title = "aspnetcore api",
                    Description = "by ttifa"
                });
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new ApiExceptionAttribute());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc(routes => routes.MapRoute("default", "{controller}/{action}/{id?}"));

            app.UseSwagger();
            app.UseSwaggerUI(c =>

            {

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");

                c.DocExpansion("none");

            });
        }
    }
}
