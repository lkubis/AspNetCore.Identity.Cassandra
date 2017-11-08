using System.Collections.Generic;
using Cassandra;

namespace AspNetCore.Identity.Cassandra
{
    public class CassandraOptions
    {
        public string KeyspaceName { get; set; }
        public Dictionary<string, string> Replication { get; set; } = null;
        public bool DurableWrites { get; set; } = true;
        public CassandraQueryOptions Query { get; set; }
    }

    public class CassandraQueryOptions
    {
        public ConsistencyLevel? ConsistencyLevel { get; set; }
        public bool? TracingEnabled { get; set; }
        public int? PageSize { get; set; }
    }
}