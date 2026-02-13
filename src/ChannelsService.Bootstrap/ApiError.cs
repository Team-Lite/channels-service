using FluentResults;

namespace ChannelsService.Bootstrap;

public sealed record ApiError(string Code)
{
    public static ApiError FromResultError(IError error)
    {
        object code = error.Metadata.FirstOrDefault(x => x.Key == "Code").Value;

        if (code is not string stringCode) return new ApiError("Unknown");
        
        return new ApiError(stringCode);
    }
}