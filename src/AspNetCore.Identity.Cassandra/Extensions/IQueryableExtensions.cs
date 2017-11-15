using System.Linq;
using Cassandra.Data.Linq;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class IQueryableExtensions
    {
        public static CqlQuery<TEntity> AsCqlQuery<TEntity>(this IQueryable<TEntity> source)
        {
            return (CqlQuery<TEntity>)source;
        }
    }
}