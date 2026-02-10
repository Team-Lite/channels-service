using System.Security.Claims;
using ChannelsService.Application;
using ChannelsService.Application.RemoveChannel;
using ChannelsService.Bootstrap.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ChannelsService.Bootstrap.Endpoints;

public sealed record RemoveChannelRequest(Guid ChannelId);

public sealed class RemoveChannelEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("/api/channels", async (HttpContext context,
            [FromBody] RemoveChannelRequest request,
            [FromServices] ICommandHandler<RemoveChannelCommand> commandHandler) =>
            {
                var id = context.User.GetUserId();

                Result result = await commandHandler.HandleAsync(new RemoveChannelCommand(request.ChannelId, id));

                if (result.IsFailed) return Results.BadRequest(ApiError.FromResultError(result.Errors.First()));
                
                return Results.Ok();
            })
            .RequireAuthorization();
    }
}