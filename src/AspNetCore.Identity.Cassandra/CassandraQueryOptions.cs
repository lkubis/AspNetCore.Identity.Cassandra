using Cassandra;

namespace AspNetCore.Identity.Cassandra
{
    public class CassandraQueryOptions
    {
        public ConsistencyLevel? ConsistencyLevel { get; set; }
        public bool? TracingEnabled { get; set; }
        public int? PageSize { get; set; }
    }
}