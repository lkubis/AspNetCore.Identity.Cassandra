using Cassandra.Mapping;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class ICqlBatchExtensions
    {
        public static ICqlBatch WithOptions(this ICqlBatch batch, CassandraQueryOptions queryOptions)
        {
            return batch.WithOptions(o =>
            {
                if (queryOptions == null)
                    return;

                if (queryOptions.ConsistencyLevel.HasValue)
                    o.SetConsistencyLevel(queryOptions.ConsistencyLevel.Value);

                if (queryOptions.PageSize.HasValue)
                    o.SetPageSize(queryOptions.PageSize.Value);

                if (queryOptions.TracingEnabled.HasValue)
                {
                    if (queryOptions.TracingEnabled.Value)
                        o.EnableTracing();
                    else
                        o.DisableTracing();
                }
            });
        }
    }
}