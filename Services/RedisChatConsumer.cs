using StackExchange.Redis;

namespace Dnw.Chat.Services;

public class RedisChatConsumer : IChatConsumer
{
    private readonly IConnectionMultiplexer _mux;
    private readonly ILogger<RedisChatConsumer> _logger;

    public RedisChatConsumer(IConnectionMultiplexer mux, ILogger<RedisChatConsumer> logger)
    {
        _mux = mux;
        _logger = logger;
    }
    
    public async Task Start(Action<string> onMessage)
    {
        await _mux.GetSubscriber().SubscribeAsync(RedisChannels.ChatMessages, (_, msg) =>
        {
            _logger.LogInformation("{MachineName} received {message}", Environment.MachineName, msg);
            onMessage(msg!);
        }).ConfigureAwait(false);
    }
}