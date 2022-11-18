using Confluent.Kafka;

namespace Dnw.Chat.Api.Services.Kafka;

public interface IKafkaProducerFactory
{
    IProducer<TKey, TValue> CreateProducer<TKey, TValue>();
}

public class KafkaProducerFactory : IKafkaProducerFactory
{
    private readonly ProducerConfig _config;

    public KafkaProducerFactory(ProducerConfig config)
    {
        _config = config;
    }
    
    public IProducer<TKey, TValue> CreateProducer<TKey, TValue>()
    {
        return new ProducerBuilder<TKey, TValue>(_config).Build();
    }
}