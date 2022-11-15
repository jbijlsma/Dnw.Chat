using Dnw.Chat.Services;
using StackExchange.Redis;

namespace Dnw.Chat.ServiceExtensions;

public static class ChatServiceExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, ConfigurationManager config)
    {
        var connectionString = config.GetValue<string>("RedisConnectionString");
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddSingleton<IChatPublisher, RedisChatPublisher>();
        services.AddSingleton<IChatConsumer, RedisChatConsumer>();

        return services;
    }
}