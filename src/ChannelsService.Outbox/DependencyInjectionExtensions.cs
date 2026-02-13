using ChannelsService.Outbox.MessageQueue;
using ChannelsService.Outbox.Service;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelsService.Outbox;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOutboxProcessor()
        {
            services.AddHostedService<OutboxWorker>();

            services.AddScoped<IOutboxService, OutboxService>();

            services.AddSingleton<IMessageQueue, RabbitMqMessageQueue>();

        }
    }
}