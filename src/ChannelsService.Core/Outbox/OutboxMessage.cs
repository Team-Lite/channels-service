using System.Text.Json;

namespace ChannelsService.Core.Outbox;

public sealed partial class OutboxMessage
{
    public Guid Id { get; private init; }
    
    public string Discriminator { get; private init; }
    
    public string Payload { get; private init; }
    
    public bool IsCompleted { get; private set; }

    #region EF
    #pragma warning disable
    private OutboxMessage() { }
    #pragma warning disable
    #endregion

    private OutboxMessage(string discriminator, string payload)
    {
        Id = Guid.CreateVersion7();
        Discriminator = discriminator;
        Payload = payload;
    }

    public void MarkAsCompleted() => IsCompleted = true;

    private static OutboxMessage Create(string discriminator, object payload)
    {
        string payloadJson = JsonSerializer.Serialize(payload);
        
        return new OutboxMessage(discriminator, payloadJson);
    }
}