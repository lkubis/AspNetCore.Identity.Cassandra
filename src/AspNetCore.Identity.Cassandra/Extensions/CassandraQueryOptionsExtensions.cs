using Cassandra.Mapping;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class CassandraQueryOptionsExtensions
    {
        public static CqlQueryOptions AsCqlQueryOptions(this IOptionsSnapshot<CassandraOptions> snapshot)
        {
            var cqlQueryOptions = CqlQueryOptions.New();
            var options = snapshot.Value;
            if (options?.Query == null)
                return cqlQueryOptions;

            if (options.Query.ConsistencyLevel.HasValue)
                cqlQueryOptions.SetConsistencyLevel(options.Query.ConsistencyLevel.Value);

            if (options.Query.PageSize.HasValue)
                cqlQueryOptions.SetPageSize(options.Query.PageSize.Value);

            if (options.Query.TracingEnabled.HasValue)
            {
                if (options.Query.TracingEnabled.Value)
                    cqlQueryOptions.EnableTracing();
                else
                    cqlQueryOptions.DisableTracing();
            }

            return cqlQueryOptions;
        }
    }
}