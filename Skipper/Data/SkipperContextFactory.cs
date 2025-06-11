using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NetBlocks.Models.Environment;
using NetBlocks.Utilities.Environment;

namespace Skipper.Data;

/// <summary>
/// Design-time DbContext factory for EF Core migrations.
/// This enables running migration commands like 'dotnet ef migrations add' and 'dotnet ef database update'
/// </summary>
public class SkipperContextFactory : IDesignTimeDbContextFactory<SkipperContext>
{
    public SkipperContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SkipperContext>();

        var connection = LoadConnection();
        
        optionsBuilder.UseNpgsql(connection.ConnectionString);

        return new SkipperContext(optionsBuilder.Options);
    }

    private Connection LoadConnection()
    {
        Connections? connections = ConnectionStringTools.LoadFromFile("environment/connections.json");
        
        if (connections == null) throw new Exception("No connections configuration found");

        Connection? connection = connections.ConnectionStrings
                .FirstOrDefault(c => c.ID == connections.ActiveConnectionID);
        
        if (connection == null) throw new Exception("Active connection not found");
        
        return connection;
    }
} 