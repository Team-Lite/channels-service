using ChannelsService.Core.Entities;
using ChannelsService.Core.Errors;
using ChannelsService.External.Database;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.UseCases.TransferOwnership;

internal sealed class TransferOwnershipHandler : IHandler<TransferOwnershipCommand>
{
    private readonly DatabaseContext _context;
    
    public TransferOwnershipHandler(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<Result> HandleAsync(TransferOwnershipCommand command)
    {
        Channel? channel = await _context
            .Channels
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == command.ChannelId);
        
        if (channel is null) return Result.Fail(new ChannelNotFoundError(command.ChannelId));

        Result transferringResult = channel.TransferOwnership(command.InitiatorId, command.NewOwnerId);
        
        if (transferringResult.IsFailed) return transferringResult;
        
        await _context.SaveChangesAsync();
        
        return Result.Ok();
    }
}