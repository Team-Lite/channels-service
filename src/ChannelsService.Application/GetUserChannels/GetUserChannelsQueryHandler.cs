using ChannelsService.External.Database;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.Application.GetUserChannels;

internal sealed class GetUserChannelsQueryHandler : IQueryHandler<GetUserChannelsQuery, IEnumerable<UserChannelDto>>
{
    private readonly DatabaseContext _context;
    
    public GetUserChannelsQueryHandler(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<UserChannelDto>> HandleAsync(GetUserChannelsQuery query) =>
        await _context
            .Channels
            .AsNoTracking()
            .Include(navigation => navigation.Users)
            .Where(channel => channel.Users.Any(u => u.Id == query.UserId))
            .Select(channel => new UserChannelDto(channel.Id, channel.Name))
            .ToListAsync();
}