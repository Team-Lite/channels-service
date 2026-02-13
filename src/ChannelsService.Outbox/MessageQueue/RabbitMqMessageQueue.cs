using System.Text;
using ChannelsService.Core.Outbox;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ChannelsService.Outbox.MessageQueue;

internal sealed class RabbitMqMessageQueue : IMessageQueue
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    
    public RabbitMqMessageQueue(IConfiguration configuration)
    {
        //TODO: Maybe I should to rewrite it and not to call async method in ctor...
        var channelFactory = new ConnectionFactory
        {
            HostName = configuration.GetConnectionString("rabbitMq") 
                       ?? throw new ArgumentException("rabbitMq connection string is not configured")
        };

        _connection = channelFactory
            .CreateConnectionAsync()
            .GetAwaiter()
            .GetResult();

        _channel = _connection
            .CreateChannelAsync()
            .GetAwaiter()
            .GetResult();

        _channel.QueueDeclareAsync(
            queue: "messaging",
            durable: false,
            exclusive: false,
            autoDelete: false)
            .GetAwaiter();
    }
    
    public async Task PushMessageAsync(OutboxMessage message)
    {
        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "messaging",
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}