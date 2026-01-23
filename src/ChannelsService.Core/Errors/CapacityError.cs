using FluentResults;

namespace ChannelsService.Core.Errors;

public sealed class CapacityError : Error
{
    public CapacityError()
    {
        Metadata.Add("Code", "CapacityError");
    }
}