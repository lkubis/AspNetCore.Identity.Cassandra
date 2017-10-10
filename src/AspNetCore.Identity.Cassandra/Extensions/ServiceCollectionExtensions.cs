using System;
using Cassandra;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCassandraSession<TSession>(
            this IServiceCollection services,
            Func<TSession> getSession) where TSession : class, ISession
        {
            return services.AddSingleton(getSession());
        }
    }
}