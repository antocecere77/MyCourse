﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MyCourse
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if(env.IsProduction()) 
            {
                app.UseHttpsRedirection();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
