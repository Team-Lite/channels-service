using ChannelsService.Core.Outbox.Payloads;

namespace ChannelsService.Core.Outbox;

public sealed partial class OutboxMessage
{
    public static class Discriminators
    {
        public const string UserJoined = "UserJoined";
        public const string UserLeft = "UserLeft";
        public const string ChannelWasRemoved = "ChannelWasRemoved";
        public const string OwnershipWasTransferred = "OwnershipWasTransferred";
    }

}