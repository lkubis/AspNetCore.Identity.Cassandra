using AspNetCore.Identity.Cassandra;
using AspNetCore.Identity.Cassandra.Extensions;
using IdentitySample.Web.Data;
using IdentitySample.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentitySample.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        public IWebHostEnvironment Environment { get; private set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddCassandra(Configuration);

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddCassandraErrorDescriber<CassandraErrorDescriber>()
                .UseCassandraStores<Cassandra.ISession>()
                .AddDefaultTokenProviders();

            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Account/Manage");
                options.Conventions.AuthorizePage("/Account/Logout");
            });

            services.AddAuthentication("myCookie")
                .AddCookie("myCookie", options =>
                {
                    options.LoginPath = "/Account/Login";
                });

            services.AddSingleton<IEmailSender, EmailSender>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.UseCassandra<ApplicationUser, ApplicationRole>();
        }
    }
}
