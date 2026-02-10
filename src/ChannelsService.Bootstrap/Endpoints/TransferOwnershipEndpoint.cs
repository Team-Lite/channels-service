using System.Security.Claims;
using ChannelsService.Application;
using ChannelsService.Application.TransferOwnership;
using ChannelsService.Bootstrap.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ChannelsService.Bootstrap.Endpoints;

public sealed record TransferOwnershipRequest(Guid ChannelId, Guid NewOwnerId);

public sealed class TransferOwnershipEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPatch("/api/channels", async (HttpContext context,
            [FromBody] TransferOwnershipRequest request,
            [FromServices] ICommandHandler<TransferOwnershipCommand> commandHandler) =>
            {
                var id = context.User.GetUserId();
 
                var result = await commandHandler.HandleAsync(new TransferOwnershipCommand(
                    request.ChannelId,
                    id,
                    request.NewOwnerId));

                if (result.IsFailed) return Results.BadRequest(ApiError.FromResultError(result.Errors.First()));

                return Results.Ok();
            })
            .RequireAuthorization();
    }
}