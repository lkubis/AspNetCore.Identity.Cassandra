namespace AspNetCore.Identity.Cassandra
{
    internal static class CassandraSessionHelper
    {
        public static string UsersTableName { get; set; }
        public static string RolesTableName { get; set; }
    }
}