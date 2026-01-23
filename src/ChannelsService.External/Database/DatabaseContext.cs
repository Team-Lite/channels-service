using ChannelsService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.External.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Channel> Channels { get; init; }
}