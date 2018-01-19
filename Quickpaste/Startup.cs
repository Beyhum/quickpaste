using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Quickpaste.Data;
using Quickpaste.Services.AuthServices;
using Quickpaste.Services.PasteServices;
using Quickpaste.Services.UserServices;
using Quickpaste.StartupSetup;

namespace Quickpaste
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
            services.AddSingleton(Configuration);

            services.Configure<HostingSettings>(Configuration.GetSection("Hosting"));
            using (var serviceProvider = services.BuildServiceProvider())
            {
                CoreServicesSetup.CheckConfigSettings(serviceProvider.GetService<IOptions<HostingSettings>>());

            }

            services.AddAuth(Configuration);

            services.AddStorageService(Configuration);

            services.AddCoreAppServices(Configuration);

            MapInitializer.Initialize();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(Configuration["Database:ConnectionString"]);
            }
            );

            
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<HostingSettings> hostingSettings, AppDbContext dbContext)
        {

            if (hostingSettings.Value.RequireSSL)
            {
                app.UseCors(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                );
                var rewriteOptions = new RewriteOptions().AddRedirectToHttps();
                app.UseRewriter(rewriteOptions);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                WebpackDevMiddlewareOptions webpackOptions = new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ConfigFile = Path.Combine(Directory.GetCurrentDirectory(), "Angular/webpack.config.js"),
                    ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "Angular")

                };
                app.UseWebpackDevMiddleware(webpackOptions);
            }

            EfDbInitializer.Initialize(dbContext);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();

            app.UseStaticFiles();

            app.HandleBlobStoragePath(Configuration);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{id?}");
                // add route for index page which serves the angular application
                routes.MapSpaFallbackRoute(
                    name: "spa-route",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
