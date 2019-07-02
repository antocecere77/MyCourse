﻿using System;
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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Models.Options;
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
            services.AddMvc(options =>
                {
                    var homeProfile = new CacheProfile();
                    //homeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
                    //homeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
                    //homeProfile.VaryByQueryKeys = new string[] { "page" };
                    Configuration.Bind("ResponseCache:Home", homeProfile);
                    options.CacheProfiles.Add("Home", homeProfile);
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
                
            services.AddTransient<ICourseService, AdoNetCouseService>();
            //services.AddTransient<ICourseService, EFCoreCoursesService>();
            services.AddTransient<IDatabaseAccessor, SqlliteDatabaseAccessor>();
            services.AddTransient<ICachedCourseService, MemoryCachedCoursesService>();

            services.AddAutoMapper();

            //services.AddScoped<MyCourseDbContext>(); //Equivalente alla riga sotto
            //services.AddDbContext<MyCourseDbContext>();
            services.AddDbContextPool<MyCourseDbContext>(optionsBuilder => {
                string connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("Default");
                optionsBuilder.UseSqlite(connectionString);
            });

            //Options
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<CachingOptions>(Configuration.GetSection("Caching"));
            services.Configure<CoursesOptions>(Configuration.GetSection("Courses"));
            services.Configure<MemoryCacheOptions>(Configuration.GetSection("MemoryCache"));

            #region Configurazione del servizio di cache distribuita

            //Se vogliamo usare Redis, ecco le istruzioni per installarlo: https://docs.microsoft.com/it-it/aspnet/core/performance/caching/distributed?view=aspnetcore-2.2#distributed-redis-cache
            //Bisogna anche installare il pacchetto NuGet: Microsoft.Extensions.Caching.StackExchangeRedis
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    Configuration.Bind("DistributedCache:Redis", options);
            //});
            
            //Se vogliamo usare Sql Server, ecco le istruzioni per preparare la tabella usata per la cache: https://docs.microsoft.com/it-it/aspnet/core/performance/caching/distributed?view=aspnetcore-2.2#distributed-sql-server-cache
            /*services.AddDistributedSqlServerCache(options => 
            {
                Configuration.Bind("DistributedCache:SqlServer", options);
            });*/

            //Se vogliamo usare la memoria, mentre siamo in sviluppo
            //services.AddDistributedMemoryCache();
            
            #endregion
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
            } else {
                app.UseExceptionHandler("/Error");
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
