using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Net.Sockets;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCassandra(this IServiceCollection services, IConfiguration configuration, Action<ISession> sessionCallback = null)
        {
            services.Configure<CassandraOptions>(configuration.GetSection("Cassandra"));
            services.AddSingleton(x => x.GetRequiredService<IOptions<CassandraOptions>>().Value);

            return services
                .AddTransient<IMapper>(serviceProvider =>
                {
                    var session = serviceProvider.GetRequiredService<ISession>();
                    return new Mapper(session);
                })
                .AddSingleton<ISession>(serviceProvider =>
                {
                    var options = serviceProvider.GetRequiredService<CassandraOptions>();
                    var logger = serviceProvider.GetRequiredService<ILogger<CassandraOptions>>();

                    var queryOptions = new QueryOptions();

                    if (options.Query != null && options.Query.ConsistencyLevel.HasValue)
                    {
                        if (Enum.TryParse(options.Query.ConsistencyLevel.ToString(), true, out ConsistencyLevel level))
                            queryOptions.SetConsistencyLevel(level);
                    }

                    var cluster = Cluster.Builder()
                        .AddContactPoints(options.ContactPoints)
                        .WithPort(options.Port)
                        .WithCredentials(
                            options.Credentials.UserName,
                            options.Credentials.Password)
                        .WithQueryOptions(queryOptions)
                        .Build();

                    ISession session = null;
                    var policy = Policy.Handle<SocketException>()
                        .Or<NoHostAvailableException>()
                        .WaitAndRetry(
                            options.RetryCount,
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            (exception, retryCount, context) => logger.LogWarning($"Retry {retryCount} due to: {exception}"))
                        .Execute(() => session = cluster.Connect());

                    if (session is null)
                        throw new ApplicationException("FATAL ERROR: Cassandra session could not be created");

                    sessionCallback?.Invoke(session);

                    logger.LogInformation("Cassandra session created");
                    return session;
                })
                .AddSingleton<DbInitializer>();
        }
    }
}