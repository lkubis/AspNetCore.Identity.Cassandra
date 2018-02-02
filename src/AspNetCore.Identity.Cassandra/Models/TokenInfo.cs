namespace AspNetCore.Identity.Cassandra.Models
{
    public class TokenInfo
    {
        public string LoginProvider { get; internal set; }
        public string Name { get; internal set; }
        public string Value { get; internal set; }

        public TokenInfo()
        {
            
        }

        public TokenInfo(string loginProvider, string name, string value)
        {
            LoginProvider = loginProvider;
            Name = name;
            Value = value;
        }
    }
}