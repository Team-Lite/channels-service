using ChannelsService.Core.Entities;
using ChannelsService.Core.Errors;
using ChannelsService.External.Database;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.UseCases.RemoveChannel;

internal sealed class RemoveChannelHandler : IHandler<RemoveChannelCommand>
{
    private readonly DatabaseContext _context;

    public RemoveChannelHandler(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<Result> HandleAsync(RemoveChannelCommand command)
    {
        Channel? channel = await _context
            .Channels
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == command.ChannelId);

        if (channel is null) return Result.Fail(new ChannelNotFoundError(command.ChannelId));
        
        if (!channel.CheckOwnership(command.InitiatorId)) return Result.Fail(new OwnershipError());
        
        _context.Channels.Remove(channel);
        
        await _context.SaveChangesAsync();
        
        return Result.Ok();
    }
}