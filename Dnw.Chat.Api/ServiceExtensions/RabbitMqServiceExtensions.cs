using Dnw.Chat.Api.Services;
using RabbitMQ.Client;

namespace Dnw.Chat.Api.ServiceExtensions;

public static class RabbitMqServiceExtensions
{
    public static void AddRabbitMq(this IServiceCollection services, string hostName)
    {
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory { HostName = hostName });
        services.AddSingleton<IChatPublisher, RabbitMqChatPublisher>();
        services.AddSingleton<IChatConsumer, RabbitMqChatConsumer>();
    }
}