using FluentResults;

namespace ChannelsService.Core.Errors;

public sealed class ChannelNotFoundError : Error
{
    public ChannelNotFoundError(Guid channelId)
    {
        Metadata.Add("Code", "ChannelNotFound");
        Metadata.Add("ChannelId", channelId);
    }
}