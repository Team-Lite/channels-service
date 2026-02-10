using System.Security.Claims;
using ChannelsService.Application;
using ChannelsService.Application.GetUserChannels;
using ChannelsService.Bootstrap.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ChannelsService.Bootstrap.Endpoints;

public sealed class GetUserChannelsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/channels", async (HttpContext context, 
                [FromServices] IQueryHandler<GetUserChannelsQuery, IEnumerable<UserChannelDto>> queryHandler) => 
            {
                var id = context.User.GetUserId();

                GetUserChannelsQuery query = new GetUserChannelsQuery(id);

                List<UserChannelDto> response = (await queryHandler.HandleAsync(query)).ToList();

                if (response.Count == 0) return Results.NotFound();
                
                return Results.Ok(response);
            })
            .RequireAuthorization();
    }
}