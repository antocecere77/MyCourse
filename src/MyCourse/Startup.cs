using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;

namespace MyCourse
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddTransient<ICourseService, AdoNetCouseService>();
            services.AddTransient<ICourseService, EFCoreCoursesService>();
            services.AddTransient<IDatabaseAccessor, SqlliteDatabaseAccessor>();
            services.AddAutoMapper();

            //services.AddScoped<MyCourseDbContext>(); //Equivalente alla riga sotto
            //services.AddDbContext<MyCourseDbContext>();
            services.AddDbContextPool<MyCourseDbContext>(optionsBuilder => {
                string connectionString = Configuration.GetSection("ConnectionsStrings").GetValue<string>("Default");
                optionsBuilder.UseSqlite(connectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {

            if(env.IsProduction()) 
            {
                app.UseHttpsRedirection();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                //Aggiorno un file per notificare al BrowserSync che deve aggiornare la pagina
                lifetime.ApplicationStarted.Register(() => 
                    {
                        string filePath = Path.Combine(env.ContentRootPath, "bin/reload.txt");
                        File.WriteAllText(filePath, DateTime.Now.ToString());
                    });
            }

            app.UseStaticFiles();

            // app.Run(async (context) =>
            // {
            //     string nome = context.Request.Query["nome"];
            //     string host = context.Request.Host.Value;
            //     await context.Response.WriteAsync($"Hello {nome.ToUpper()}!!!");
            // });

            //Equivalente a quanto scritto sotto
            //app.UseMvcWithDefaultRoute();

            app.UseMvc(routeBuilder => 
                {
                    // /courses/detail/5
                    routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }
}
