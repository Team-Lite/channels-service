using ChannelsService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelsService.External.Database.Configurations;

internal sealed class ChannelsConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsMany(x => x.Users, navigationBuilder => 
            navigationBuilder.ToTable("ChannelUsers"));
    }
}