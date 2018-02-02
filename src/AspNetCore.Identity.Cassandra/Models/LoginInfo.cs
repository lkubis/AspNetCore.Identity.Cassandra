using System;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Cassandra.Models
{
    public class LoginInfo : IEquatable<LoginInfo>, IEquatable<UserLoginInfo>
    {
        public string LoginProvider { get; internal set; }
        public string ProviderKey { get; internal set; }
        public string ProviderDisplayName { get; internal set; }

        public LoginInfo()
        {
            
        }

        public LoginInfo(string loginProvider, string providerKey, string displayName)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = displayName;
        }

        public static implicit operator LoginInfo(UserLoginInfo input)
            => new LoginInfo(input.LoginProvider, input.ProviderKey, input.ProviderDisplayName);

        public bool Equals(LoginInfo other)
            => LoginProvider == other.LoginProvider && ProviderKey == other.ProviderKey;

        public bool Equals(UserLoginInfo other)
            => LoginProvider == other.LoginProvider && ProviderKey == other.ProviderKey;
    }
}