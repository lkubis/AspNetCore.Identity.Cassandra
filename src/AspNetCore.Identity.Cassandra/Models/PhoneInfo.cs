using System;
using Cassandra.Mapping.Attributes;

namespace AspNetCore.Identity.Cassandra.Models
{
    public class PhoneInfo
    {
        public string Number { get; internal set; }
        public DateTimeOffset? ConfirmationTime { get; internal set; }
        public bool IsConfirmed => ConfirmationTime != null;

        public static implicit operator PhoneInfo(string input)
            => new PhoneInfo { Number = input };

        [Ignore]
        public bool AllPropertiesAreSetToDefaults =>
            Number == null &&
            ConfirmationTime == null;
    }
}