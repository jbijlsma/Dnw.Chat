using Dnw.Chat.Api.Services;
using Dnw.Chat.Api.Services.Redis;
using StackExchange.Redis;

namespace Dnw.Chat.Api.ServiceExtensions;

public static class ChatServiceExtensions
{
    public static void AddRedis(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IChatMessageBusInitializer, DefaultChatMessageBusInitializer>();
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddSingleton<IChatPublisher, RedisChatPublisher>();
        services.AddSingleton<IChatConsumer, RedisChatConsumer>();
    }
}