using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChannelsService.External.Database;

public sealed class DesignTimeContext : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        DbContextOptions<DatabaseContext> options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=channels_service;" +
                       "Username=postgres;Password=postgres")
            .Options;
        
        return new DatabaseContext(options);
    }
}