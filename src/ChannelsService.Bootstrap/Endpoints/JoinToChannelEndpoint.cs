using System.Security.Claims;
using ChannelsService.Application;
using ChannelsService.Application.JoinToChannel;
using ChannelsService.Bootstrap.Extensions;
using ChannelsService.Core.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ChannelsService.Bootstrap.Endpoints;

public sealed record JoinToChannelRequest(Guid ChannelId);

public sealed class JoinToChannelEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/channels/join", async (HttpContext context, 
            [FromBody] JoinToChannelRequest request,
            [FromServices] ICommandHandler<JoinToChannelCommand> commandHandler) =>
            {
                var id = context.User.GetUserId();
                
                Result result = await commandHandler.HandleAsync(
                    new JoinToChannelCommand(request.ChannelId, id));

                if (result.IsFailed) return Results.BadRequest(ApiError.FromResultError(result.Errors.First()));
                
                return Results.Ok();
            })
            .RequireAuthorization();
    }
}