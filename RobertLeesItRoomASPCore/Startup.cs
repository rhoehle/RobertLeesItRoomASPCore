using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFilesEx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RobertLeesItRoomASPCore
{
    public class Startup
    {

        private readonly IConfigurationRoot configuration;

        public Startup(IHostingEnvironment env)  
            { 
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile(env.ContentRootPath + "/config.json")
                .AddJsonFile(env.ContentRootPath + "/config.development.json", true)
                .Build();
    }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<FeatureToggles>(x => new FeatureToggles
            {
                EnableDeveloperExceptions =
                    configuration.GetValue<bool>("FeatureToggles:EnableDeveloperExceptions")
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, FeatureToggles features)
        {
           loggerFactory.AddConsole();

            // redirects error request to be displayed in the error.html pag
            app.UseExceptionHandler("/error.html");         
            
            if (features.EnableDeveloperExceptions)
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Contains("invalid"))
                    throw new Exception("Error!");
                await next(); 
            });

            app.UseMvc();

            app.UseFileServer();
        }
    }
}
