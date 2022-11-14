using Lib.AspNetCore.ServerSentEvents;
using StackExchange.Redis;

namespace Dnw.Chat;

public class ChatMessageConsumer : BackgroundService
{
    private readonly IConnectionMultiplexer _mux;
    private readonly IServerSentEventsService _sseService;
    private readonly ILogger<ChatMessageConsumer> _logger;

    public ChatMessageConsumer(
        IConnectionMultiplexer mux,
        IServerSentEventsService sseService,
        ILogger<ChatMessageConsumer> logger)
    {
        _mux = mux;
        _sseService = sseService;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ChatMessageConsumer.ExecuteAsync");

        await _mux.GetSubscriber().SubscribeAsync(RedisChannels.ChatMessages, (_, msg) =>
        {
            _logger.LogInformation("{MachineName} received {message}", Environment.MachineName, msg);

            var clients = _sseService.GetClients();
            foreach (var client in clients)
            {
                client.SendEventAsync(msg, cancellationToken).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }
}