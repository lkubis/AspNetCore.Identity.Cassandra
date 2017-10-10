using Cassandra;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder UseCassandraStores<TSession>(this IdentityBuilder builder)
            where TSession : class, ISession
            => builder
                .AddCassandraUserStore<TSession>()
                .AddCassandraRoleStore<TSession>();

        private static IdentityBuilder AddCassandraUserStore<TSession>(this IdentityBuilder builder)
        {
            var userStoreType = typeof(CassandraUserStore<,>).MakeGenericType(builder.UserType, typeof(TSession));

            builder.Services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(builder.UserType),
                userStoreType
            );

            return builder;
        }

        private static IdentityBuilder AddCassandraRoleStore<TSession>(this IdentityBuilder builder)
        {
            var roleStoreType = typeof(CassandraRoleStore<,>).MakeGenericType(builder.RoleType, typeof(TSession));

            builder.Services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                roleStoreType
            );

            return builder;
        }
    }
}