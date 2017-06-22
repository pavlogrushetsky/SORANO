using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Context;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SORANO.DAL.Repositories;

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

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.CookieName = ".SORANO.Session";
            });

            services.AddScoped(_ => new StockContext(Configuration.GetConnectionString("SORANO")));
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Add dependencies for services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IArticleService, ArticleService>();
            services.AddTransient<IArticleTypeService, ArticleTypeService>();
            services.AddTransient<ISupplierService, SupplierService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<ILocationTypeService, LocationTypeService>();
            services.AddTransient<IAttachmentTypeService, AttachmentTypeService>();
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

            app.UseSession();

            app.UseMvc(routes => 
            {
                routes.MapRoute(
                    "default", 
                    "{controller=Home}/{action=Index}/{id?}");
            });            
        }
    }
}
