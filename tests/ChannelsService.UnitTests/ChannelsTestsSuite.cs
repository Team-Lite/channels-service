using ChannelsService.Core.Entities;
using ChannelsService.Core.Errors;
using FluentResults;

namespace ChannelsService.UnitTests;

public class ChannelsTestsSuite
{
    [Fact]
    public void Channel_ShouldAddNewUserAsOwner_OnConstruction()
    {
        (Channel channel, Guid userId) = CreateChannel();
        
        Assert.Single(channel.Users);
        Assert.Contains(channel.Users, u => u.Id == userId && u.IsOwner);
    }

    [Fact]
    public void Join_ShouldReturnDuplicateError_IfUserWithSameIdExists()
    {
        (Channel channel, Guid userId) = CreateChannel();

        Result result = channel.Join(userId);
        
        Assert.True(result.IsFailed);
        Assert.True(result.HasError<DuplicateError>());
    }

    [Fact]
    public void Join_ShouldReturnCapacityError_IfChannelWithNextUserCapacity_WillBeMoreThan10()
    {
        (Channel channel, _) = CreateChannel();

        //Add 9 users
        for (int i = 0; i < 9; i++) 
        {
            channel.Join(Guid.NewGuid());
        }

        Result result = channel.Join(Guid.NewGuid());
        
        Assert.True(result.IsFailed);
        Assert.True(result.HasError<CapacityError>());
    }

    [Fact]
    public void Join_ShouldAddNewUser_NotAsCreator_AndReturnSuccess()
    {
        Guid newUserId = Guid.NewGuid();
        
        (Channel channel, _) = CreateChannel();

        Result result = channel.Join(newUserId);
        
        Assert.True(result.IsSuccess);
        Assert.Contains(channel.Users, u => u.Id == newUserId && !u.IsOwner);
    }

    [Fact]
    public void Left_ShouldReturnUserNotFoundErrorWithUserId_IfUserDoesNotExistsOnServer()
    {
        (Channel channel, _) = CreateChannel();

        var userId = Guid.NewGuid();
        
        Result result = channel.Left(userId);
        
        AssertUserNotFoundErrorAndIdCorrectness(result, userId);
    }
    
    [Fact]
    public void Left_ShouldReturnOwnershipError_IfUserIsOwner()
    {
        (Channel channel, Guid ownerId) = CreateChannel();
        
        Result result = channel.Left(ownerId);
        
        Assert.True(result.IsFailed);
        Assert.True(result.HasError<OwnershipError>());
    }

    [Fact]
    public void Left_ShouldRemoveUserFromChannel_AndReturnSuccess()
    {
        (Channel channel, _) = CreateChannel();
        Guid userId = Guid.NewGuid();

        channel.Join(userId);
        
        Result result = channel.Left(userId);
        
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(channel.Users, u => u.Id == userId);
    }

    [Fact]
    public void TransferOwnership_ShouldReturnUserNotFoundErrorWithInitiatorId_IfUserNotFound()
    {
        (Channel channel, _) = CreateChannel();
        
        Guid initiatorId = Guid.NewGuid();
        
        Result result = channel.TransferOwnership(initiatorId, Guid.NewGuid());
        
        AssertUserNotFoundErrorAndIdCorrectness(result, initiatorId);
    }

    [Fact]
    public void TransferOwnership_ShouldReturnUserNotFoundErrorWithSuccessorId_IfUserNotFound()
    {
        (Channel channel, Guid ownerId) = CreateChannel();

        Guid successorId = Guid.NewGuid();

        Result result = channel.TransferOwnership(ownerId, successorId);

        AssertUserNotFoundErrorAndIdCorrectness(result, successorId);
    }

    [Fact]
    public void TransferOwnership_ShouldReturnOwnershipError_IfInitiatorIsNotOwner()
    {
        (Channel channel, Guid ownerId) = CreateChannel();
        Guid initiatorId = Guid.NewGuid();
        channel.Join(initiatorId);
        
        Result result = channel.TransferOwnership(initiatorId, ownerId);
        
        Assert.True(result.IsFailed);
        Assert.True(result.HasError<OwnershipError>());
    }
    
    [Fact]
    public void TransferOwnership_ShouldReturnSuccess_AndSwapOwnership_IfInitiatorIsOwner()
    {
        (Channel channel, Guid ownerId) = CreateChannel();
        Guid successorId = Guid.NewGuid();

        channel.Join(successorId);
        
        Result result = channel.TransferOwnership(ownerId, successorId);
        Assert.True(result.IsSuccess);
        Assert.Contains(channel.Users, u => u.Id == successorId && u.IsOwner);
        Assert.Contains(channel.Users, u => u.Id == ownerId && !u.IsOwner);
    }

    private (Channel channel, Guid ownerId) CreateChannel()
    {
        Guid userId = Guid.NewGuid();
        
        return (new Channel("testName", "testDescription", userId), userId);
    }

    private void AssertUserNotFoundErrorAndIdCorrectness(Result result, Guid userId)
    {
        Assert.True(result.IsFailed);
        Assert.True(result.HasError(x => x is UserNotFoundError userNotFoundError
                                         && userNotFoundError.Metadata.TryGetValue("UserId", out object? id)
                                         && id is Guid typedId
                                         && typedId == userId));
    }
}