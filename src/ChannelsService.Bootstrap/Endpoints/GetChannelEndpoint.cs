using ChannelsService.Application;
using ChannelsService.Application.GetChannel;
using Microsoft.AspNetCore.Mvc;

namespace ChannelsService.Bootstrap.Endpoints;

public sealed class GetChannelEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/channels/{channelId}", async (Guid channelId,
            [FromServices] IQueryHandler<GetChannelQuery, ChannelDto?> queryHandler) =>
            {
                GetChannelQuery query = new GetChannelQuery(channelId);

                ChannelDto? channel = await queryHandler.HandleAsync(query);
                
                if (channel is null) return Results.NotFound();
                
                return Results.Ok(channel);
            })
            .RequireAuthorization();
    }
}