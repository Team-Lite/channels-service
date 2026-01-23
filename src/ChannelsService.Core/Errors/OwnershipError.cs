using FluentResults;

namespace ChannelsService.Core.Errors;

public sealed class OwnershipError : Error
{
    public OwnershipError()
    {
        Metadata.Add("Code", "OwnerShipError");
    }
}