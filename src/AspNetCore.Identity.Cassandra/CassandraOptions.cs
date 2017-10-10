using System.Collections.Generic;

namespace AspNetCore.Identity.Cassandra
{
    public class CassandraOptions
    {
        public string KeyspaceName { get; set; }
        public Dictionary<string, string> Replication { get; set; } = null;
        public bool DurableWrites { get; set; } = true;
    }
}