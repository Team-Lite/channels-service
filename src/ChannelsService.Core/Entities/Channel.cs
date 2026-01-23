using ChannelsService.Core.Errors;
using FluentResults;

namespace ChannelsService.Core.Entities;

public sealed class Channel
{
    private const int MaximumCapacity = 10; 
    
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string? Description { get; private set; }

    private readonly List<ChannelUser> _users = [];

    public IReadOnlyList<ChannelUser> Users => _users;

    #region [EF]
    #pragma warning disable
    private Channel() {}
    #pragma warning restore
    #endregion
    
    public Channel(string name, string? description, Guid creatorId)
    {
        Id = Guid.CreateVersion7();
        
        Name = name;
        Description = description;
        
        _users.Add(new ChannelUser(creatorId, true));
    }

    public Result TransferOwnership(Guid initiatorId, Guid newOwnerId)
    {
        ChannelUser? initiator = GetUser(initiatorId);
        ChannelUser? successor = GetUser(newOwnerId);

        if (initiator is null) return Result.Fail(new UserNotFoundError(initiatorId));
        
        if (successor is null) return Result.Fail(new UserNotFoundError(newOwnerId));
        
        if (!CheckOwnership(initiator)) return Result.Fail(new OwnershipError());
        
        successor.MakeOwner();
        initiator.DeleteOwnership();
        
        return Result.Ok();
    }

    public Result Join(Guid userId)
    {
        if (GetUser(userId) is not null) return Result.Fail(new DuplicateError());
        
        _users.Add(new ChannelUser(userId, false));

        return _users.Count > MaximumCapacity 
            ? Result.Fail(new CapacityError())
            : Result.Ok();
    } 

    public Result Left(Guid userId)
    {
        var user = GetUser(userId);
        
        if (user is null) return Result.Fail(new UserNotFoundError(userId));
        
        if (CheckOwnership(user)) return Result.Fail(new OwnershipError());
        
        _users.Remove(user);
        
        return Result.Ok();
    }

    public bool CheckOwnership(Guid userId)
    {
        var user = GetUser(userId);
        
        if (user is null) return false;
        
        return CheckOwnership(user);
    }

    private bool CheckOwnership(ChannelUser user) => user.IsOwner;

    private ChannelUser? GetUser(Guid userId) => _users.FirstOrDefault(x => x.Id == userId);
}