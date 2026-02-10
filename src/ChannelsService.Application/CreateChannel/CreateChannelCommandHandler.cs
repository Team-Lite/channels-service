using ChannelsService.Core.Entities;
using ChannelsService.External.Database;
using FluentResults;

namespace ChannelsService.Application.CreateChannel;

internal sealed class CreateChannelCommandHandler : ICommandHandler<CreateChannelCommand>
{
    private readonly DatabaseContext _context;
    
    public CreateChannelCommandHandler(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<Result> HandleAsync(CreateChannelCommand command)
    {
        Channel newChannel = new Channel(command.Name, command.Description, command.CreatorId);

        _context.Channels.Add(newChannel);
        
        await _context.SaveChangesAsync();
        
        return Result.Ok();
    }
}