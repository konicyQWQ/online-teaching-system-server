using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OTS_API.Services;
using OTS_API.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using OTS_API.Common;

namespace OTS_API
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
            services.AddDbContext<OTSDbContext>(options => options.UseMySQL(Config.connStr));
            services.AddScoped<UserService>();
            services.AddScoped<CourseService>();
            services.AddScoped<FileService>();
            services.AddSingleton<TokenService>();
            services.AddControllers();
            services.AddCors(options => options.AddPolicy("AllowCors", builder => builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowCors");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors("AllowCors");
            });
        }
    }
}
