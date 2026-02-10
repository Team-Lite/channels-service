using ChannelsService.External.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace ChannelsService.IntegrationTests;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder("postgres:latest")
        .WithUsername("postgres")
        .WithPassword("test")
        .WithDatabase(Guid.CreateVersion7().ToString())
        .WithPortBinding(5432, assignRandomHostPort: true)
        .Build();
    
    public async Task InitializeAsync()
    {
        await _databaseContainer.StartAsync();

        await using var context = CreateDbContext();

        await context.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _databaseContainer.GetConnectionString());
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAuthenticationHandler>();
        });
    }

    private void ConfigureOptions(DbContextOptionsBuilder builder, string connectionString)
    {
        builder.UseNpgsql(connectionString);
    }

    public DatabaseContext CreateDbContext()
    {
        var builder = new DbContextOptionsBuilder<DatabaseContext>();
        
        ConfigureOptions(builder, _databaseContainer.GetConnectionString());
        
        return new DatabaseContext(builder.Options);
    }
    
    public async Task SeedDatabaseAsync(Action<DatabaseContext> seedingAction)
    {
        await using var context = CreateDbContext();

        seedingAction(context);
        
        await context.SaveChangesAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await _databaseContainer.DisposeAsync();
    }
}