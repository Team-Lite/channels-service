using ChannelsService.External.Database;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.Application.GetChannel;

internal sealed class GetChannelQueryHandler : IQueryHandler<GetChannelQuery, ChannelDto?>
{
    private readonly DatabaseContext _context;
    
    public GetChannelQueryHandler(DatabaseContext context)
    {
        _context = context;
    }
    
    public Task<ChannelDto?> HandleAsync(GetChannelQuery query) =>
        _context
            .Channels
            .Include(navigation => navigation.Users)
            .Select(channel => new ChannelDto(
                ChannelId: channel.Id,
                Name: channel.Name,
                Description: channel.Description ?? string.Empty,
                Users: channel.Users.Select(user => new ChannelUserDto(user.Id, user.IsOwner))))
            .FirstOrDefaultAsync(x => x.ChannelId == query.ChannelId);
}