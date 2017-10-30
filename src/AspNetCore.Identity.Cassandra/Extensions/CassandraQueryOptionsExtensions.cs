using Cassandra.Mapping;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class CassandraQueryOptionsExtensions
    {
        public static CqlQueryOptions AsCqlQueryOptions(this IOptionsSnapshot<CassandraQueryOptions> snapshot)
        {
            var cqlQueryOptions = CqlQueryOptions.New();
            var options = snapshot.Value;
            if (options == null)
                return cqlQueryOptions;

            if (options.ConsistencyLevel.HasValue)
                cqlQueryOptions.SetConsistencyLevel(options.ConsistencyLevel.Value);

            if (options.PageSize.HasValue)
                cqlQueryOptions.SetPageSize(options.PageSize.Value);

            if (options.TracingEnabled.HasValue)
            {
                if (options.TracingEnabled.Value)
                    cqlQueryOptions.EnableTracing();
                else
                    cqlQueryOptions.DisableTracing();
            }

            return cqlQueryOptions;
        }
    }
}