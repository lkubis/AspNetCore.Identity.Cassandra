using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCassandra<TUser, TRole>(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var services = scope.ServiceProvider;
                var options = services.GetService<CassandraOptions>();
                var initializer = services.GetService<DbInitializer>();

                if (initializer is null)
                    return app;

                initializer.Initialize<TUser, TRole>(options);
                return app;
            }
        }
    }
}
