using ChannelsService.Core.Entities;
using ChannelsService.Core.Errors;
using ChannelsService.External.Database;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.UseCases.JoinToChannel;

internal sealed class JoinToChannelHandler : IHandler<JoinToChannelCommand>
{
    private readonly DatabaseContext _databaseContext;
    
    public JoinToChannelHandler(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Result> HandleAsync(JoinToChannelCommand command)
    {
        Channel? channel = await _databaseContext
            .Channels
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == command.ChannelId);

        if (channel is null) return Result.Fail(new ChannelNotFoundError(command.ChannelId));

        Result joiningResult = channel.Join(command.UserId);
        
        if (joiningResult.IsFailed) return joiningResult;
        
        await _databaseContext.SaveChangesAsync();
        
        return Result.Ok();
    }
}