using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tedd.ShortUrl.Web.Db;
using Tedd.ShortUrl.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tedd.ShortUrl.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}
        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();

            AutoMapperBootstrapper.Initialize();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMvc();
            services.AddResponseCaching();

            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ShortUrlDbContext>(options => options.UseSqlServer(connection));

            //services.AddSingleton<INavigationDatabase, CacheNavigationDatabase>();
            services.AddScoped<INavigationDatabase, SqlNavigationDatabase>();

            //services.AddSingleton<ManagedConfig, ManagedConfig>();
            // Register the IConfiguration instance which MyOptions binds against.
            services.Configure<ManagedConfig>(Configuration);

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }

            //app.UseMvc();
            app.UseStaticFiles();
            app.UseMvc(
            routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}");

                routes.MapRoute(
                    "NavigateTo",
                    "{actionURL}",
                    new { controller = "Navigate", action = "NavigateTo" }
                );
                //routes.MapRoute(name: "createRoute",
                //    template: "{*url}",
                //    defaults: new { controller = "Navigate", action = "NavigateTo" });
                //routes.MapRoute(name: "navigateRoute",
                //    template: "{*url}",
                //    defaults: new { controller = "Navigate", action = "NavigateTo" });
            });
            app.UseResponseCaching();


            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        }
    }
}
