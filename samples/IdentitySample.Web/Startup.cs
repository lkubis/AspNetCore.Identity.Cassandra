using System.Linq;
using AspNetCore.Identity.Cassandra;
using AspNetCore.Identity.Cassandra.Extensions;
using IdentitySample.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentitySample.Web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<CassandraOptions>(Configuration.GetSection("CassandraOptions"));

            services.AddCassandraSession<Cassandra.ISession>(() =>
            {
                var cluster = Cassandra.Cluster.Builder()
                    .AddContactPoints(Configuration.GetSection("CassandraNodes").GetChildren().Select(x => x.Value))
                    .Build();
                var session = cluster.Connect();

                return session;
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .UseCassandraStores<Cassandra.ISession>()
                .AddDefaultTokenProviders();

            services.AddMvc()
                .WithRazorPagesRoot("/Pages")
                .AddRazorPagesOptions(x =>
                {
                    x.Conventions.AuthorizePage("/Index");
                });

            services.AddAuthentication("myCookie")
                .AddCookie("myCookie", options =>
                {
                    options.LoginPath = "/Account/Login";
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
