using Cassandra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class IWebHostExtensions
    {
        public static IWebHost InitializeIdentityDb<TUser, TRole>(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var session = services.GetRequiredService<ISession>();
                var options = services.GetRequiredService<IOptions<CassandraOptions>>();
                
                var initializer = new DbInitializer(session);
                initializer.Initialize<TUser, TRole>(options.Value);

                return host;
            }
        }
    }
}