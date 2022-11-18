using Confluent.Kafka;

namespace Dnw.Chat.Api.Services.Kafka;

public class KafkaChatConsumer : IChatConsumer
{
    private readonly IKafkaConsumerFactory _consumerFactory;
    private readonly ILogger<KafkaChatConsumer> _logger;
    private bool _cancelled;

    public KafkaChatConsumer(
        IKafkaConsumerFactory consumerFactory, 
        ILogger<KafkaChatConsumer> logger)
    {
        _consumerFactory = consumerFactory;
        _logger = logger;
    }
    
    public async Task Start(Action<string> onMessage)
    {
        await Task.Yield();
        
        using var consumer = _consumerFactory.CreateConsumer<Ignore, string>();
        consumer.Subscribe(new [] { KafkaTopics.ChatMessages });

        while (!_cancelled)
        {
            var consumeResult = consumer.Consume();
            
            _logger.LogInformation("Received message with value: {message}", consumeResult.Message.Value);

            onMessage(consumeResult.Message.Value);
        }
    }
    
    public Task Stop()
    {
        _cancelled = true;
        return Task.CompletedTask;
    }
}