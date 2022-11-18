using Confluent.Kafka;

namespace Dnw.Chat.Api.Services.Kafka;

public interface IKafkaConsumerFactory
{
    IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>();
}

public class KafkaConsumerFactory : IKafkaConsumerFactory
{
    private readonly ConsumerConfig _config;

    public KafkaConsumerFactory(ConsumerConfig config)
    {
        _config = config;
    }
    
    public IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>()
    {
        return new ConsumerBuilder<TKey, TValue>(_config).Build();
    }
}