using System.Net;
using System.Net.Http.Headers;
using ChannelsService.Bootstrap.Endpoints;
using ChannelsService.Core.Entities;
using ChannelsService.Core.Outbox;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.IntegrationTests.Tests;

public sealed class ChannelsTests : IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory = new();

    [Fact]
    public async Task CreateChannel_RequiresAuthorization()
    {
        using var client = _factory.CreateClient();
        
        HttpResponseMessage response = await client.PostAsJsonAsync("api/channels",
            new CreateChannelRequest("test", "test"));
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateChannel_WithAuthorizedRequest_ShouldCreateChannelWithCorrectOwnerId()
    {
        using var client = CreateClientWithAuthorization();
        await using var context = _factory.CreateDbContext();
        
        HttpResponseMessage response = await client.PostAsJsonAsync("api/channels",
            new CreateChannelRequest("newChannel", "testDescription"));
        
        
        bool isExists = await context
            .Channels
            .Include(x => x.Users)
            .AnyAsync(c => c.Users.Any(u => u.Id == Defaults.UserId && u.IsOwner)
                           && c.Name == "newChannel"
                           && c.Description == "testDescription");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(isExists);
    }

    [Fact]
    public async Task JoinToChannel_RequiresAuthorization()
    {
        using var client = _factory.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/channels/join",
            new JoinToChannelRequest(Guid.CreateVersion7()));
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task JoinToChannel_WithAuthorizedRequest_ShouldPlaceUserIntoChannelAndPublishMessage()
    {
        using var client = CreateClientWithAuthorization();
        await using var context = _factory.CreateDbContext();

        var seedingScheme = new Channel("testChannel", null, Guid.NewGuid());

        await _factory.SeedDatabaseAsync(seedingContext =>
        {
            seedingContext.Channels.Add(seedingScheme);
        });
        
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/channels/join",
            new JoinToChannelRequest(seedingScheme.Id));
        
        bool isInChannel = await context
            .Channels
            .Include(x => x.Users)
            .AnyAsync(x => x.Id == seedingScheme.Id 
                           && x.Users.Any(user => user.Id == Defaults.UserId));

        OutboxMessage? singleMessage = await context
            .OutboxMessages
            .SingleOrDefaultAsync();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(isInChannel);
        Assert.NotNull(singleMessage);
        Assert.True(singleMessage.Discriminator == OutboxMessage.Discriminators.UserJoined);
    }

    [Fact]
    public async Task JoinToChannel_ShouldReturnClientErrorStatusAndDoNotCompleteTransact_IfProblemAppeared()
    {
        using var client = CreateClientWithAuthorization();
        await using var context = _factory.CreateDbContext();

        Channel seedingScheme = new Channel("test", "test", Guid.NewGuid());
        
        seedingScheme.Join(Defaults.UserId);

        await _factory.SeedDatabaseAsync(seedingContext =>
        {
            seedingContext.Channels.Add(seedingScheme);
        });
        
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/channels/join",
            new JoinToChannelRequest(seedingScheme.Id));
        
        bool isInChannel = await context
            .Channels
            .AnyAsync(x => x.Id == seedingScheme.Id 
                           && x.Users.Any(user => user.Id == Defaults.UserId));
        
        OutboxMessage? singleMessage = await context
            .OutboxMessages
            .SingleOrDefaultAsync();
        
        AssertThatStatusCodeIsClientError(response.StatusCode);
        Assert.True(isInChannel);
        Assert.Null(singleMessage);
    }
    
    [Fact]
    public async Task LeftFromChannel_RequiresAuthorization()
    {
        using var client = _factory.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/channels/left",
            new LeftFromChannelRequest(Guid.CreateVersion7()));
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task LeftFromChannel_WithAuthorizedRequest_ShouldRemoveUserFromChannelAndPublishMessage()
    {
        using var client = CreateClientWithAuthorization();
        await using var context = _factory.CreateDbContext();
        
        Channel seedingScheme = new Channel("test", "test", Guid.NewGuid());
        
        seedingScheme.Join(Defaults.UserId);

        await _factory.SeedDatabaseAsync(seedingContext =>
        {
            seedingContext.Channels.Add(seedingScheme);
        });
        
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/channels/left",
            new LeftFromChannelRequest(seedingScheme.Id));
        
        bool isInChannel = await context
            .Channels
            .AnyAsync(x => x.Id == seedingScheme.Id 
                           && x.Users.Any(user => user.Id == Defaults.UserId));
        
        OutboxMessage? singleMessage = await context
            .OutboxMessages
            .SingleOrDefaultAsync();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(isInChannel);
        Assert.NotNull(singleMessage);
        Assert.True(singleMessage.Discriminator == OutboxMessage.Discriminators.UserLeft);
    }

    [Fact]
    public async Task LeftFromChannel_ShouldReturnClientErrorStatusAndDoNotCompleteTransact_IfProblemAppeared()
    {
        using var client = CreateClientWithAuthorization();
        await using var context = _factory.CreateDbContext();
        
        Channel seedingScheme = new Channel("test", "test", Guid.NewGuid());

        await _factory.SeedDatabaseAsync(seedingContext =>
        {
            seedingContext.Channels.Add(seedingScheme);
        });
        
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/channels/left",
            new LeftFromChannelRequest(seedingScheme.Id));
        
        bool isInChannel = await context
            .Channels
            .Include(x => x.Users)
            .AnyAsync(x => x.Id == seedingScheme.Id 
                           && x.Users.All(user => user.Id != Defaults.UserId));
        
        OutboxMessage? singleMessage = await context
            .OutboxMessages
            .SingleOrDefaultAsync();
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(isInChannel);
        Assert.Null(singleMessage);
    }

    private HttpClient CreateClientWithAuthorization()
    {
        var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = "test";
                            options.DefaultChallengeScheme = "test";
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                            "test", _ => { });
                });
            })
            .CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "test");

        return client;
    }

    public Task InitializeAsync() => _factory.InitializeAsync();
    public Task DisposeAsync() => _factory.DisposeAsync();

    private void AssertThatStatusCodeIsClientError(HttpStatusCode statusCode)
    {
        Assert.True((int)statusCode is >= 400 and < 500);
    }
}