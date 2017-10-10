using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Cassandra.Models
{
    public class LoginInfo
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }

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
    }
}