using System.Security.Claims;
using ChannelsService.Application;
using ChannelsService.Application.LeftFromChannel;
using ChannelsService.Bootstrap.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ChannelsService.Bootstrap.Endpoints;

public sealed record LeftFromChannelRequest(Guid ChannelId);

public sealed class LeftFromChannelEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/channels/left", async (HttpContext context,
            [FromBody] LeftFromChannelRequest request,
            [FromServices] ICommandHandler<LeftFromChannelCommand> commandHandler) =>
            {
                var id = context.User.GetUserId();
                
                Result result = await commandHandler.HandleAsync(new LeftFromChannelCommand(request.ChannelId, id));
                
                if (result.IsFailed) return Results.BadRequest(ApiError.FromResultError(result.Errors.First()));
                
                return Results.Ok();
            })
            .RequireAuthorization();
    }
}