using System;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Identity.Cassandra.Extensions
{
    public static class IMapperExtensions
    {
        public static Task<IdentityResult> TryInsertAsync<TPoco>(this IMapper mapper,
            TPoco poco,
            CqlQueryOptions queryOptions,
            CassandraErrorDescriber errorDescriber,
            ILogger logger)
        {
            return TryExecuteAsync(() => mapper.InsertAsync(poco, queryOptions: queryOptions), errorDescriber, logger);
        }

        public static Task<IdentityResult> TryUpdateAsync<TPoco>(this IMapper mapper,
            TPoco poco,
            CqlQueryOptions queryOptions,
            CassandraErrorDescriber errorDescriber,
            ILogger logger)
        {
            return TryExecuteAsync(() => mapper.UpdateAsync(poco, queryOptions: queryOptions), errorDescriber, logger);
        }

        public static Task<IdentityResult> TryDeleteAsync<TPoco>(this IMapper mapper,
            TPoco poco,
            CqlQueryOptions queryOptions,
            CassandraErrorDescriber errorDescriber,
            ILogger logger)
        {
            
            return TryExecuteAsync(() => mapper.DeleteAsync(poco, queryOptions: queryOptions), errorDescriber, logger);
        }

        public static Task<IdentityResult> TryExecuteBatchAsync(this IMapper mapper,
            CassandraErrorDescriber errorDescriber,
            ILogger logger,
            CassandraQueryOptions options,
            params Action<ICqlBatch>[] actions)
        {
            var batch = mapper
                .CreateBatch()
                .WithOptions(options);

            foreach (var a in actions)
                a(batch);

            return TryExecuteAsync(() => mapper.ExecuteAsync(batch), errorDescriber, logger);
        }

        private static async Task<IdentityResult> TryExecuteAsync(
            Func<Task> action,
            CassandraErrorDescriber errorDescriber,
            ILogger logger)
        {
            IdentityResult result;

            try
            {
                await action();
                result = IdentityResult.Success;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while executing query.");

                var type = exception.GetType();
                switch (type)
                {
                    case Type _ when type == typeof(NoHostAvailableException):
                        result = IdentityResult.Failed(errorDescriber.NoHostAvailable());
                        break;

                    case Type _ when type == typeof(UnavailableException):
                        result = IdentityResult.Failed(errorDescriber.Unavailable());
                        break;

                    case Type _ when type == typeof(ReadTimeoutException):
                        result = IdentityResult.Failed(errorDescriber.ReadTimeout());
                        break;

                    case Type _ when type == typeof(WriteTimeoutException):
                        result = IdentityResult.Failed(errorDescriber.WriteTimeout());
                        break;

                    case Type _ when type == typeof(QueryValidationException):
                        result = IdentityResult.Failed(errorDescriber.QueryValidation());
                        break;

                    default:
                        result = IdentityResult.Failed(errorDescriber.DefaultError(exception.Message));
                        break;
                }
            }

            return result;
        }
    }
}