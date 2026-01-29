using System.Text.Json;

namespace ChannelsService.Core.Outbox;

public sealed partial class OutboxMessage
{
    public string Discriminator { get; private init; }
    
    public string? Payload { get; private init; }
    
    public bool IsCompleted { get; private set; }
    
    private OutboxMessage(string discriminator, string payload)
    {
        Discriminator = discriminator;
        Payload = payload;
    }

    public void MarkAsCompleted() => IsCompleted = true;
    
    public T GetPayload<T>() where T : class
    {
        if (Payload is null) throw new InvalidOperationException("Message payload is null");
        
        T? deserializedPayload = JsonSerializer.Deserialize<T>(Payload);

        if (deserializedPayload is null) throw new ArgumentException("Message has another type");
        
        return deserializedPayload;
    }

    private static OutboxMessage Create(string discriminator, object payload)
    {
        string payloadJson = JsonSerializer.Serialize(payload);
        
        return new OutboxMessage(discriminator, payloadJson);
    }
}