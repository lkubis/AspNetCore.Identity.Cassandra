﻿using System;
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
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(options.KeyspaceName))
                throw new InvalidOperationException("Keyspace is null or empty.");

            // Keyspace
            try 
            {
                // Attempt to switch to keyspace
                _session.ChangeKeyspace(options.KeyspaceName);
            }
            catch (InvalidQueryException)
            {
                // If failed with InvalidQueryException then keyspace does not exist
                // -> create new one
                _session.CreateKeyspaceIfNotExists(options.KeyspaceName, replication: options.Replication, durableWrites: options.DurableWrites);
                _session.ChangeKeyspace(options.KeyspaceName);
            }

            // User Defined Types
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.LockoutInfo (EndDate timestamp, Enabled boolean, AccessFailedCount int);");
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.PhoneInfo (Number text, ConfirmationTime timestamp);");
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.LoginInfo (LoginProvider text, ProviderKey text, ProviderDisplayName text);");
            _session.Execute($"CREATE TYPE IF NOT EXISTS {options.KeyspaceName}.TokenInfo (LoginProvider text, Name text, Value text);");

            _session.UserDefinedTypes.Define(
                UdtMap.For<LockoutInfo>(),
                UdtMap.For<PhoneInfo>(),
                UdtMap.For<LoginInfo>(),
                UdtMap.For<TokenInfo>());

            // Tables
            var usersTable = new Table<TUser>(_session);
            usersTable.CreateIfNotExists();

            var rolesTable = new Table<TRole>(_session);
            rolesTable.CreateIfNotExists();

            _session.Execute($"CREATE TABLE IF NOT EXISTS {options.KeyspaceName}.userclaims (" +
                " UserId uuid, " +
                " Type text, " +
                " Value text, " +
                " PRIMARY KEY (UserId, Type, Value));");

            _session.Execute($"CREATE TABLE IF NOT EXISTS {options.KeyspaceName}.roleclaims (" +
                " RoleId uuid, " +
                " Type text, " +
                " Value text, " +
                " PRIMARY KEY (RoleId, Type, Value));");

            // Materialized views
            CassandraSessionHelper.UsersTableName = usersTable.GetTable().Name;
            CassandraSessionHelper.RolesTableName = rolesTable.GetTable().Name;

            _session.Execute("CREATE MATERIALIZED VIEW IF NOT EXISTS users_by_email AS" +
                             $" SELECT * FROM {options.KeyspaceName}.{CassandraSessionHelper.UsersTableName}" +
                             " WHERE NormalizedEmail IS NOT NULL" +
                             " PRIMARY KEY (NormalizedEmail, Id)");

            _session.Execute("CREATE MATERIALIZED VIEW IF NOT EXISTS users_by_username AS" +
                             $" SELECT * FROM {options.KeyspaceName}.{CassandraSessionHelper.UsersTableName}" +
                             " WHERE NormalizedUserName IS NOT NULL" +
                             " PRIMARY KEY (NormalizedUserName, Id)");

            _session.Execute("CREATE MATERIALIZED VIEW IF NOT EXISTS roles_by_name AS" +
                             $" SELECT * FROM {options.KeyspaceName}.{CassandraSessionHelper.RolesTableName}" +
                             " WHERE NormalizedName IS NOT NULL" +
                             " PRIMARY KEY (NormalizedName, Id)");

            _session.Execute("CREATE MATERIALIZED VIEW IF NOT EXISTS userclaims_by_type_and_value AS" +
                             $" SELECT * FROM {options.KeyspaceName}.userclaims" +
                             " WHERE Type IS NOT NULL AND Value IS NOT NULL" +
                             " PRIMARY KEY ((Type, Value), UserId)");
        }
    }
}