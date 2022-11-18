using StackExchange.Redis;

namespace Dnw.Chat.Api.Services.Redis;

public class RedisChatPublisher : IChatPublisher
{
    private readonly IDatabase _db;

    public RedisChatPublisher(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }
    
    public async Task Publish(string msg)
    {
        await _db.PublishAsync(new RedisChannel(RedisChannels.ChatMessages, RedisChannel.PatternMode.Literal),msg);
    }
}