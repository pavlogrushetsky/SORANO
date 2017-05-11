﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories;
using SORANO.DAL.Repositories.Abstract;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace SORANO.WEB
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddScoped(_ => new StockContext(Configuration.GetConnectionString("SORANO")));

            // Add dependencies for repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IDeliveryItemRepository, DeliveryItemRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();

            // Add dependencies for services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IArticleService, ArticleService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                LoginPath = new PathString("/Account/Login"),
                AccessDeniedPath = new PathString("/Account/Login"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            // Global exceptions handling
            app.UseExceptionHandler(handler => handler.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                var error = context.Features.Get<IExceptionHandlerFeature>();
                if (error != null)
                {
                    await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                }
            }));

            app.UseMvc(routes => 
            {
                routes.MapRoute(
                    "default", 
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
