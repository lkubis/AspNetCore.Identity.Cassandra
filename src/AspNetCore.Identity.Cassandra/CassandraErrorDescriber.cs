using AspNetCore.Identity.Cassandra.Properties;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Cassandra
{
    public class CassandraErrorDescriber
    {
        public virtual IdentityError NoHostAvailable()
        {
            return new IdentityError()
            {
                Code = "NoHostAvailable",
                Description =  Resources.NoHostAvailable
            };
        }

        public virtual IdentityError Unavailable()
        {
            return new IdentityError()
            {
                Code = "Unavailable",
                Description = Resources.Unavailable
            };
        }

        public virtual IdentityError ReadTimeout()
        {
            return new IdentityError()
            {
                Code = "ReadTimeout",
                Description = Resources.ReadTimeout
            };
        }

        public virtual IdentityError WriteTimeout()
        {
            return new IdentityError()
            {
                Code = "WriteTimeout",
                Description = Resources.WriteTimeout
            };
        }

        public virtual IdentityError QueryValidation()
        {
            return new IdentityError()
            {
                Code = "QueryValidation",
                Description = Resources.QueryValidation
            };
        }

        public virtual IdentityError DefaultError(string message)
        {
            return new IdentityError()
            {
                Code = "DefaultError",
                Description = message
            };
        }
    }
}