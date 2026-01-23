using FluentResults;

namespace ChannelsService.Core.Errors;

public sealed class UserNotFoundError : Error
{
    public UserNotFoundError(Guid userId)
    {
        Metadata.Add("Code", "UserNotFound");
        Metadata.Add("UserId", userId);
    }
}