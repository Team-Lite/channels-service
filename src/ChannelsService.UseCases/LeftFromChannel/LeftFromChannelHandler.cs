using ChannelsService.Core.Entities;
using ChannelsService.Core.Errors;
using ChannelsService.External.Database;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.UseCases.LeftFromChannel;

internal sealed class LeftFromChannelHandler : IHandler<LeftFromChannelCommand>
{
    private readonly DatabaseContext _context;
    
    public LeftFromChannelHandler(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<Result> HandleAsync(LeftFromChannelCommand command)
    {
        Channel? channel = await _context
            .Channels
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == command.ChannelId);

        if (channel is null) return Result.Fail(new ChannelNotFoundError(command.ChannelId));
        
        Result leftResult = channel.Left(command.UserId);

        if (leftResult.IsFailed) return leftResult;
        
        await _context.SaveChangesAsync();
        
        return Result.Ok();
    }
}