using AspNetCore.Identity.Cassandra.Models;
using Cassandra;
using Cassandra.Data.Linq;

namespace AspNetCore.Identity.Cassandra
{
    public class DbInitializer
    {
        private readonly ISession _session;

        public DbInitializer(ISession session)
        {
            _session = session;
        }

        public void Initialize<TUser, TRole>(CassandraOptions options)
        {
            // Keyspace
            _session.CreateKeyspaceIfNotExists(options.KeyspaceName, replication: options.Replication, durableWrites: options.DurableWrites);
            _session.ChangeKeyspace(options.KeyspaceName);

            // User Defined Types
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.LockoutInfo (EndDate timestamp, Enabled boolean, AccessFailedCount int);");
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.PhoneInfo (Number text, ConfirmationTime timestamp);");
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.LoginInfo (LoginProvider text, ProviderKey text, ProviderDisplayName text);");
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.SimplifiedClaim (Type text, Value text);");

            _session.UserDefinedTypes.Define(
                UdtMap.For<LockoutInfo>(),
                UdtMap.For<PhoneInfo>(),
                UdtMap.For<LoginInfo>(),
                UdtMap.For<SimplifiedClaim>());

            // Tables
            
            var usersTable = new Table<TUser>(_session);
            usersTable.CreateIfNotExists();

            var rolesTable = new Table<TRole>(_session);
            rolesTable.CreateIfNotExists();
        }
    }
}