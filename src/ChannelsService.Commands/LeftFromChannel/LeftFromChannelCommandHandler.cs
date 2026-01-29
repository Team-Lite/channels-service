using ChannelsService.Core.Entities;
using ChannelsService.Core.Errors;
using ChannelsService.Core.Outbox;
using ChannelsService.Core.Outbox.Payloads;
using ChannelsService.External.Database;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.Commands.LeftFromChannel;

internal sealed class LeftFromChannelCommandHandler : ICommandHandler<LeftFromChannelCommand>
{
    private readonly DatabaseContext _databaseContext;
    
    public LeftFromChannelCommandHandler(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Result> HandleAsync(LeftFromChannelCommand command)
    {
        Channel? channel = await _databaseContext
            .Channels
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == command.ChannelId);

        if (channel is null) return Result.Fail(new ChannelNotFoundError(command.ChannelId));
        
        Result leftResult = channel.Left(command.UserId);

        if (leftResult.IsFailed) return leftResult;
        
        ChannelUserPayload payload = new ChannelUserPayload
        {
            UserId = command.UserId,
            ChannelId = command.ChannelId
        };

        _databaseContext.OutboxMessages.Add(OutboxMessage.CreateUserLeftMessage(payload));
        
        await _databaseContext.SaveChangesAsync();
        
        return Result.Ok();
    }
}