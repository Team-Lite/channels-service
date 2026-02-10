using ChannelsService.Application.CreateChannel;
using ChannelsService.Application.GetChannel;
using ChannelsService.Application.GetUserChannels;
using ChannelsService.Application.JoinToChannel;
using ChannelsService.Application.LeftFromChannel;
using ChannelsService.Application.RemoveChannel;
using ChannelsService.Application.TransferOwnership;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelsService.Application;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCommandHandlers()
        {
            return services
                .AddScoped<ICommandHandler<CreateChannelCommand>, CreateChannelCommandHandler>()
                .AddScoped<ICommandHandler<JoinToChannelCommand>, JoinToChannelCommandHandler>()
                .AddScoped<ICommandHandler<LeftFromChannelCommand>, LeftFromChannelCommandHandler>()
                .AddScoped<ICommandHandler<RemoveChannelCommand>, RemoveChannelCommandHandler>()
                .AddScoped<ICommandHandler<TransferOwnershipCommand>, TransferOwnershipCommandHandler>();
        }

        public IServiceCollection AddQueryHandlers()
        {
            return services
                .AddScoped<IQueryHandler<GetChannelQuery, ChannelDto?>, GetChannelQueryHandler>()
                .AddScoped<IQueryHandler<GetUserChannelsQuery, IEnumerable<UserChannelDto>>, GetUserChannelsQueryHandler>();
        }
    }
}