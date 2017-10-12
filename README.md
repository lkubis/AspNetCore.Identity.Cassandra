# AspNetCore.Identity.Cassandra
[Apache Cassandra](https://cassandra.apache.org/) data store adaptor for  [ASP.NET Core Identity](https://github.com/aspnet/Identity), which allows you to build ASP.NET Core web applications, including membership, login, and user data. With this library, you can store your user's membership related data on Apache Cassandra.  
Inspired by existing [AspNetCore.Identity.RaveDB](https://github.com/ElemarJR/AspNetCore.Identity.RavenDB) implementation.

## Instalation
Run the following command from the package manager console to install Cassandra identity provider.
```
Install-Package AspNetCore.Identity.Cassandra
```

### Create your own User and Role entities
```csharp
[Table("users", Keyspace = "identity")]
public class ApplicationUser : CassandraIdentityUser
{
    public ApplicationUser()
        : base(Guid.NewGuid())
    {
        
    }
}

[Table("roles", Keyspace = "identity")]
public class ApplicationRole : CassandraIdentityRole
{
    public ApplicationRole()
        : base(Guid.NewGuid())
    {
        
    }
}
```

### appsettings.json

`CassandraNodes` contains collection of IP addresses of all available nodes  
`CassandraOptions` contains information about **keyspace** and **replication** options  
```json
{   
    "CassandraNodes": [
        "127.0.0.1"
    ], 
    "CassandraOptions": {
        "KeyspaceName": "identity",
        "Replication": {
            "class": "NetworkTopologyStrategy",
            "datacenter1": "1"
        }
    } 
}
```

### Startup
```csharp
public void ConfigureServices(IServiceCollection services)
{    
    // CassandraOptions configuration
    services.Configure<CassandraOptions>(Configuration.GetSection("CassandraOptions"));

    // Cassandra ISession initialization
    services.AddCassandraSession<Cassandra.ISession>(() =>
    {
        var cluster = Cassandra.Cluster.Builder()
            .AddContactPoints(Configuration.GetSection("CassandraNodes").GetChildren().Select(x => x.Value))
            .Build();
        var session = cluster.Connect();
        return session;
    });
    
    // Added custom Cassandra stores
    services.AddIdentity<ApplicationUser, ApplicationRole>()
        .UseCassandraStores<Cassandra.ISession>()
        .AddDefaultTokenProviders();
    
    // Other code omitted
}
```

### Program
Initialize Cassandra DB by calling **InitializeIdentityDb<ApplicationUser, ApplicationRole>()** method on **IWebHost** interface.
```csharp
public static class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build()
            .InitializeIdentityDb<ApplicationUser, ApplicationRole>();
}
```
