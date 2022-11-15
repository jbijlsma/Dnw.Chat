using Dnw.Chat.Services;
using RabbitMQ.Client;

namespace Dnw.Chat.ServiceExtensions;

public static class RabbitMqServiceExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, ConfigurationManager config)
    {
        var hostName = config.GetValue<string>("RabbitMqHostName");
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory { HostName = hostName });
        services.AddSingleton<IChatPublisher, RabbitMqChatPublisher>();
        services.AddSingleton<IChatConsumer, RabbitMqChatConsumer>();

        return services;
    }
}