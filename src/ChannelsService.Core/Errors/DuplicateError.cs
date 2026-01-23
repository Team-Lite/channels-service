using FluentResults;

namespace ChannelsService.Core.Errors;

public sealed class DuplicateError : Error
{
    public DuplicateError()
    {
        Metadata.Add("Code", "DuplicateError");
    }
}