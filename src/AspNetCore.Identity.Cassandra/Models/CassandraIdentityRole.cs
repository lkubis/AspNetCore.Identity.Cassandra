using System;
using Cassandra.Mapping.Attributes;

namespace AspNetCore.Identity.Cassandra.Models
{
    public class CassandraIdentityRole
    {
        [PartitionKey]
        public Guid Id { get; internal set; }

        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public CassandraIdentityRole()
        {

        }

        public CassandraIdentityRole(Guid id)
            : this()
        {
            Id = id;
        }
    }
}