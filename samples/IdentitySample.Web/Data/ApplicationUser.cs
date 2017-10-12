using System;
using AspNetCore.Identity.Cassandra.Models;
using Cassandra.Mapping.Attributes;

namespace IdentitySample.Web.Data
{
    [Table("users", Keyspace = "identity")]
    public class ApplicationUser : CassandraIdentityUser
    {
        public ApplicationUser()
            : base(Guid.NewGuid())
        {
            
        }
    }
}