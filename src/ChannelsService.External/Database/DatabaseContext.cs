using ChannelsService.Core.Entities;
using ChannelsService.Core.Outbox;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.External.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Channel> Channels { get; init; }
    public DbSet<OutboxMessage> OutboxMessages { get; init; }
}