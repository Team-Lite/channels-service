using ChannelsService.Outbox.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChannelsService.Outbox;

internal sealed class OutboxWorker : BackgroundService
{
    private const int MillisecondsDelay = 5000;
    
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxWorker> _logger;
    
    public OutboxWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            try
            {
                IOutboxService outboxService = scope
                    .ServiceProvider
                    .GetRequiredService<IOutboxService>();

                await outboxService.PushMessagesAsync(stoppingToken);

                await Task.Delay(MillisecondsDelay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while pushing messages");
            }
        }
    }
}