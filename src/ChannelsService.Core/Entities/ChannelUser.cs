namespace ChannelsService.Core.Entities;

public sealed class ChannelUser
{
    public Guid Id { get; private set; }
    public bool IsOwner { get; private set; }

    #region EF
    #pragma warning disable
    private ChannelUser() {}
    #pragma warning restore
    #endregion
    
    public ChannelUser(Guid id, bool isOwner)
    {
        Id = id;
        IsOwner = isOwner;
    }

    public void MakeOwner() => IsOwner = true;
    
    public void DeleteOwnership() => IsOwner = false;
}