using System;
using AspNetCore.Identity.Cassandra.Models;
using Cassandra.Mapping.Attributes;

namespace IdentitySample.Web.Models
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