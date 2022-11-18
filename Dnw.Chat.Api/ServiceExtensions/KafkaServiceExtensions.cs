using Confluent.Kafka;
using Dnw.Chat.Api.Services;
using Dnw.Chat.Api.Services.Kafka;

namespace Dnw.Chat.Api.ServiceExtensions;

public static class KafkaServiceExtensions
{
    public static void AddKafka(this IServiceCollection services, string bootstrapServers)
    {
        services.AddSingleton<IChatMessageBusInitializer, KafkaChatMessageBusInitializer>();
        
        services.AddSingleton<IKafkaProducerFactory>(_ => new KafkaProducerFactory(new ProducerConfig { BootstrapServers = bootstrapServers }));
        services.AddSingleton<IChatPublisher, KafkaChatPublisher>();
        services.AddSingleton<IKafkaAdminClientBuilderFactory>(_ => new KafkaAdminClientBuilderFactory(bootstrapServers));

        var groupId = Guid.NewGuid().ToString();
        services.AddSingleton<IKafkaConsumerFactory>(_ => new KafkaConsumerFactory(new ConsumerConfig { BootstrapServers = bootstrapServers, GroupId = groupId, AutoOffsetReset = AutoOffsetReset.Earliest }));
        services.AddSingleton<IChatConsumer, KafkaChatConsumer>();
    }
}