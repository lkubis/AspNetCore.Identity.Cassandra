using System;
using Cassandra.Mapping.Attributes;

namespace AspNetCore.Identity.Cassandra.Models
{
    public class LockoutInfo
    {
        public DateTimeOffset? EndDate { get; internal set; }
        public bool Enabled { get; internal set; }
        public int AccessFailedCount { get; internal set; }

        [Ignore]
        public bool AllPropertiesAreSetToDefaults =>
            EndDate == null &&
            Enabled == false &&
            AccessFailedCount == 0;
    }
}