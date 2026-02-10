using ChannelsService.Core.Entities;
using ChannelsService.Core.Outbox;
using ChannelsService.External.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.External.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Channel> Channels { get; init; }
    public DbSet<OutboxMessage> OutboxMessages { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ChannelsConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxConfiguration());
    }
}