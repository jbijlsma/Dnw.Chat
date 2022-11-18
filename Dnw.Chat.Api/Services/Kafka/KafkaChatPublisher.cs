using Confluent.Kafka;

namespace Dnw.Chat.Api.Services.Kafka;

public class KafkaChatPublisher : IChatPublisher
{
    private readonly IKafkaProducerFactory _producerFactory;

    public KafkaChatPublisher(IKafkaProducerFactory producerFactory)
    {
        _producerFactory = producerFactory;
    }
    
    public async Task Publish(string msg)
    {
        using var producer = _producerFactory.CreateProducer<Null, string>();
        
        await producer.ProduceAsync(KafkaTopics.ChatMessages, new Message<Null, string> { Value = msg });
    }
}