using System;
using AspNetCore.Identity.Cassandra.Models;
using Cassandra.Mapping.Attributes;

namespace IdentitySample.Web.Data
{
    [Table("roles", Keyspace = "identity")]
    public class ApplicationRole : CassandraIdentityRole
    {
        public ApplicationRole()
            : base(Guid.NewGuid())
        {
            
        }
    }
}