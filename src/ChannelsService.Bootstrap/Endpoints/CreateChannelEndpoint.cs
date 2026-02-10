using System.Security.Claims;
using ChannelsService.Application;
using ChannelsService.Application.CreateChannel;
using ChannelsService.Bootstrap.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ChannelsService.Bootstrap.Endpoints;

public sealed record CreateChannelRequest(string ChannelName, string? ChannelDescription);

public sealed class CreateChannelEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/channels", async (HttpContext context, 
                [FromBody] CreateChannelRequest request, 
                [FromServices] ICommandHandler<CreateChannelCommand> commandHandler) => 
            {
                Guid id = context.User.GetUserId();
            
                Result result = await commandHandler.HandleAsync(
                    new CreateChannelCommand(id, request.ChannelName, request.ChannelDescription));
                
                if (result.IsFailed) return Results.BadRequest(ApiError.FromResultError(result.Errors.First()));
                
                return Results.Ok();
            })
            .RequireAuthorization();
    }
}