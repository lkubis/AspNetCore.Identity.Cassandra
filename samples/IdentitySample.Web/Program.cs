using AspNetCore.Identity.Cassandra.Extensions;
using IdentitySample.Web.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace IdentitySample.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .InitializeIdentityDb<ApplicationUser, ApplicationRole>();
    }
}
